from typing import List, Optional
import uvicorn
from cassis import *
from fastapi import FastAPI, Response
from fastapi.encoders import jsonable_encoder
from fastapi.responses import PlainTextResponse
from pydantic import BaseModel
from pydantic_settings import BaseSettings
from starlette.responses import JSONResponse
from functools import lru_cache
import base64
import torch
from PIL import Image
import head_segmentation.segmentation_pipeline as seg_pipeline
from numpy import asarray
import cv2

# Token
class rImage(BaseModel):
    """
    org.texttechnologylab.annotation.type.Image
    """
    src: str
    width: int
    height: int
    
class SubImage(BaseModel):
    """
    org.texttechnologylab.annotation.type.SubImage
    """
    coordinates: List[List[int]]

# Request sent by DUUI
# Note, this is transformed by the Lua script
class DUUIRequest(BaseModel):
    # image in base64
        image: str


# Response of this annotator
# Note, this is transformed by the Lua script
class DUUIResponse(BaseModel):
    # List of annotated:
    # - audiotoken
    image: rImage
    sub_image: SubImage

# Documentation response
class DUUIDocumentation(BaseModel):
    # Name of this annotator
    annotator_name: str
    # Version of this annotator
    version: str
    # Annotator implementation language (Python, Java, ...)
    implementation_lang: str


class Settings(BaseSettings):
    # Name of the Model
    model_name: Optional[str] = "base"


# settings + cache
settings = Settings()
lru_cache_with_size = lru_cache(maxsize=3)

#config = {"name": settings.model_name}
config = {"name": "base"}


# Start fastapi
app = FastAPI(
    docs_url="/api",
    redoc_url=None,
    title="WhisperX audio transcription",
    description="Audio transcription for TTLab DUUI",
    version="0.1",
    terms_of_service="https://www.texttechnologylab.org/legal_notice/",
    contact={
        "name": "Daniel Bundan",
        "url": "https://texttechnologylab.org",
        "email": "danielbundan60@gmail.com",
    },
    license_info={
        "name": "AGPL",
        "url": "http://www.gnu.org/licenses/agpl-3.0.en.html",
    },
)

# Load the Lua communication script
communication = "src/main/docker/python/communication.lua"
with open(communication, 'rb') as f:
    communication = f.read().decode("utf-8")


# Load the predefined typesystem that is needed for this annotator to work
typesystem_filename = 'src/main/docker/python/typesystem.xml'
with open(typesystem_filename, 'rb') as f:
    typesystem = load_typesystem(f)


# Get input / output of the annotator
@app.get("/v1/details/input_output")
def get_input_output() -> JSONResponse:
    json_item = {
        "inputs": [],
        "outputs": ["org.texttechnologylab.annotation.type.AudioToken"]
    }

    json_compatible_item_data = jsonable_encoder(json_item)
    return JSONResponse(content=json_compatible_item_data)


# Get typesystem of this annotator
@app.get("/v1/typesystem")
def get_typesystem() -> Response:
    # TODO remove cassis dependency, as only needed for typesystem at the moment?
    xml = typesystem.to_xml()
    xml_content = xml.encode("utf-8")

    return Response(
        content=xml_content,
        media_type="application/xml"
    )


# Return Lua communication script
@app.get("/v1/communication_layer", response_class=PlainTextResponse)
def get_communication_layer() -> str:
    return communication


# Return documentation info
@app.get("/v1/documentation")
def get_documentation() -> DUUIDocumentation:

    documentation = DUUIDocumentation(
        annotator_name=settings.duui_tool_name,
        version=settings.duui_tool_version,
        implementation_lang="Python",
    )
    return documentation



# Process request from DUUI
@app.post("/v1/process")
def post_process(request: DUUIRequest) -> DUUIResponse:
    
    # Create pipeline
    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    segmentation_pipeline = seg_pipeline.HumanHeadSegmentationPipeline(device=device)
    
    # Load image
    try:
        with open("tempImage.jpg", "wb") as f:
            f.write(base64.b64decode(request.image))
    except Exception as e:
        print(str(e))
    
    img = Image.open("tempImage.jpg")
    width, height = img.size
    imgArray = asarray(img)
    
    # Run pipeline
    segmentation_map = segmentation_pipeline.predict(imgArray)

    # From mask to polygon
    contours, _ = cv2.findContours(segmentation_map, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)  # external: only outer contours
    
    polygon = []

    if len(contours) > 0:
        for point in contours[0]:
            polygon.append([int(point[0][0]), int(point[0][1])])

    segmented_region = imgArray * cv2.cvtColor(segmentation_map, cv2.COLOR_GRAY2RGB)

    pil_image = Image.fromarray(segmented_region)
    pil_image.save("tempImage.jpg")
    
    with open("tempImage.jpg", "rb") as image_file:
        encoded_string = base64.b64encode(image_file.read())

    
    return DUUIResponse(
        image=rImage(
            src=encoded_string,
            width=width,
            height=height
        ),
        sub_image=SubImage(
            coordinates=polygon
        )
    )


if __name__ == "__main__":
   uvicorn.run("duui_head-segmentation:app", host="0.0.0.0", port=9714, workers=1)
