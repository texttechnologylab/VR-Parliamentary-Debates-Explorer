FROM python:3.8

WORKDIR /usr/src/app

EXPOSE 9714

COPY ./src/main/docker/python/communication.lua ./communication.lua
COPY ./src/main/docker/python/duui_head-segmentation.py ./duui_head-segmentation.py
COPY ./src/main/docker/python/typesystem.xml ./typesystem.xml
COPY ./requirements.txt ./requirements.txt

RUN apt-get update
RUN apt-get install ffmpeg -y

RUN python -m pip install git+https://github.com/wiktorlazarski/head-segmentation.git
RUN pip install -r requirements.txt

ENTRYPOINT ["uvicorn", "duui_head-segmentation:app", "--host", "0.0.0.0", "--port" ,"9714"]
CMD ["--workers", "1"]