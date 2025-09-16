from flask import Flask, request
from flask_socketio import SocketIO, call
import whisperx
import torch
import base64
import torch
from TTS.api import TTS

device = "cuda" if torch.cuda.is_available() else "cpu"

app = Flask(__name__)

whisper = None
tts = TTS(model_name="tts_models/de/thorsten/tacotron2-DDC", progress_bar=False).to(device)

@app.post("/unload")
def unload():
    global whisper
    if whisper is not None:
        del whisper
        whisper = None
    return {"result": "success"}, 200

@app.post("/load")
def load():
    global whisper
    if whisper is None:
                
        if torch.cuda.is_available():
            whisper = whisperx.load_model("large-v2", "cuda", compute_type="float16", language="de")
        else:
            whisper = whisperx.load_model("large-v2", "cpu", compute_type="int8", language="de")
            
    return {"result": "success"}, 200

@app.post("/whisperx")
def speech_to_text():    
    if request.is_json:
                
        data_request = request.get_json()
        
        try:
            load()

            audioBase64 = data_request["audioBase64"]
                        
            try:
                with open("tempAudio", "wb") as f:
                    f.write(base64.b64decode(audioBase64))
            except Exception as e:
                print(str(e))
            
            print("Convert audio to text")
            audio = whisperx.load_audio("tempAudio")
            result = whisper.transcribe(audio, batch_size=16)
            
            transcription = ""

            for segment in result["segments"]:
                
                text = segment.get("text").strip()
                
                if transcription:
                    transcription = transcription + " " + text
                else:
                    transcription = text
            
            print("Done")
            
            return {
                "status": "success",
                "transcription": transcription
            }, 202
        except Exception as ex:
            return {
                "status": "error",
                "error": f"{ex}"
            }, 412
    return {
        "status": "error",
        "error": "Request must be JSON"
    }, 415
    
@app.post("/tts")
def text_to_speech():    
    if request.is_json:
        
        speaker = "v2/de_speaker_1"
        
        data_request = request.get_json()
        
        try:
            prompt = data_request["prompt"]
            #sentences = nltk.sent_tokenize(prompt, language="german")

            #silence = np.zeros(int(0.25 * SAMPLE_RATE))  # quarter second of silence

            #pieces = []
            #for sentence in sentences:
            ##    semantic_tokens = generate_text_semantic(
             #       sentence,
              #      history_prompt=speaker,
               #     temp=0.6,
                #    min_eos_p=0.05,  # this controls how likely the generation is to end
                #)

                #audio_array = semantic_to_waveform(semantic_tokens, history_prompt=speaker,)
                #pieces += [audio_array, silence.copy()]
            
            # save audio to disk
            #write_wav("bark_generation.wav", SAMPLE_RATE, np.concatenate(pieces))
            
            global tts
            
            tts.tts_to_file(text=prompt, file_path="generation.wav")

        
            #sound = pydub.AudioSegment.from_wav("generation.wav")
            #sound.export("generation.mp3", format="mp3")
        
            enc = base64.b64encode(open("generation.wav", "rb").read()).decode("utf-8")
            
            print("Done")
            
            return {
                "status": "success",
                "base64": enc
            }, 202
        except Exception as ex:
            return {
                "status": "error",
                "error": f"{ex}"
            }, 412
    return {
        "status": "error",
        "error": "Request must be JSON"
    }, 415

if __name__ == '__main__':
    app.run(threaded=True, host="0.0.0.0", port=5000)