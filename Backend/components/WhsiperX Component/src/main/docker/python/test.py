import whisperx

whisper = whisperx.load_model("large-v2", "cuda", compute_type="float16", language="de")
audio = whisperx.load_audio("tempAudio")
result = whisper.transcribe(audio, batch_size=16)

print(result)