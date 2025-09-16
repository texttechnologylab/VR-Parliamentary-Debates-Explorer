FROM python:3.10

WORKDIR /usr/src/app

EXPOSE 5000

COPY ./src/main/docker/python/communication.lua ./communication.lua
COPY ./src/main/docker/python/socketio_whsiperx.py ./socketio_whsiperx.py
COPY ./src/main/docker/python/typesystem.xml ./typesystem.xml
COPY ./requirements.txt ./requirements.txt

RUN apt-get update
RUN apt-get install ffmpeg -y

RUN pip install Flask --ignore-installed
RUN pip install Flask-SocketIO --ignore-installed
RUN apt-get install espeak -y

RUN pip install torch==2.5.1 torchvision==0.20.1 torchaudio==2.5.1 --index-url https://download.pytorch.org/whl/cpu 
RUN pip install -r requirements.txt --ignore-installed

ENTRYPOINT [ "python" ]
CMD ["socketio_whsiperx.py"]