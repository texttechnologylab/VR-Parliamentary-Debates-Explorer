FROM nvidia/cuda:11.7.0-devel-ubuntu20.04

RUN apt update && \
    DEBIAN_FRONTEND=noninteractive \
    apt install --no-install-recommends -y build-essential software-properties-common && \
    add-apt-repository -y ppa:deadsnakes/ppa && \
    apt install --no-install-recommends -y python3.8 python3-pip python3-setuptools python3-distutils && \
    apt clean && rm -rf /var/lib/apt/lists/*

RUN ln -s /usr/bin/python3 /usr/bin/python
RUN python -m pip install --upgrade pip
#FROM python:3.8

ADD rest-datasaver.py /
ADD ConvertAudio.py /
ADD LLaMaChat.py /
ADD chatgpt.py /
ADD Text2Speech.py /
ADD whisper2text.py /
ADD config.json /
ADD config_chatgpt.json /
ADD requirements.txt /
ADD LongForm.py /

ADD adapter.py /
ADD generate.py /
ADD lora.py /
ADD model.py /
ADD prepare_alpaca.py /
ADD robert_lora.py /
ADD tokenizer.py /
ADD utils.py /
ADD tokenizer.model /
ADD lit-llama.pth /
ADD lit-llama-lora-finetuned.pth /

RUN apt-get -y update && apt-get -y upgrade && apt-get install -y ffmpeg && apt-get -y install git
RUN pip install torch==2.0.0+cu117 torchvision==0.15.1+cu117 torchaudio==2.0.1 --index-url https://download.pytorch.org/whl/cu117
RUN pip install -r requirements.txt


#RUN python -c "from transformers import pipeline; pipeline('text-generation', model='akoksal/LongForm-OPT-2.7B')"
RUN python -c "import whisper; whisper.load_model('small')"
RUN python -c "from transformers import pipeline; pipeline('text-generation', model='declare-lab/flan-alpaca-base')"
RUN python -c "from transformers import pipeline; pipeline('text-generation', model='akoksal/LongForm-OPT-2.7B')"

CMD [ "python", "./rest-datasaver.py" ]