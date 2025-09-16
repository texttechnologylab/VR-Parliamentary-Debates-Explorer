# app.py
import copy

from flask import Flask, request, jsonify
from pymongo import MongoClient
# from pymongo import database
import gridfs
import datetime
import pymongo.errors
import json
import numpy as np
from urllib.parse import quote_plus
from ConvertAudio import RestApiAudioConverter
#from whisper2text import Whisper2Text
#from Text2Speech import TextToSpeech
#from chatgpt import ChatGpt
#from LLaMaChat import AlpacaBot
#from robert_lora import robert
#from pathlib import Path
#from LongForm import LongForm
import torch

app = Flask(__name__)

required_data_audio = {"roomId", "messageType", "clientId", "timestamp", "position", "rotation", "cameraRotation",
                       "audioData", "peers"}
required_data_player = {"playerId", "audioData", "localTime", "messageId", "body", "leftHand", "rightHand"}
required_data_logIn = {"playerId", "roomId", "sceneName", "clientId", "localTime", "messageId"}
required_data_roleIn = {"playerId", "role", "localTime", "messageId"}
required_data_object = {"playerId", "referenceMessage", "localTime", "objectId", "objectName", "hand", "interaction"}
required_data_special = {"localTime", "mode"}
required_data_log = {"playerId", "localTime", "referenceMessage", "logMessage", "logType", "stacktrace", "localTime"}
required_data_levelChange = {"playerId", "roomId", "sceneName", "localTime", "levelID", "levelStatus"}
required_data_misc = {"playerId", "jsonData"}

def build_pymongo_connection(access_dir: str):

    with open(access_dir, "r", encoding="UTF-8") as json_file:
        access_data = json.load(json_file)
    db_name = access_data["Datenbank"]
    user_name = access_data["User"]
    pw = access_data["Passwort"]
    host = f"{access_data['Server']}:{access_data['Port']}"
    uri = "mongodb://%s:%s@%s" % (
        quote_plus(user_name), quote_plus(pw), host)

    try:
        print(f'mongodb://{user_name}:{pw}@{host}/{db_name}')
        client = MongoClient(f'mongodb://{quote_plus(user_name)}:{quote_plus(pw)}@{host}')
        # client = MongoClient(quote_plus('mongodb://{user_name}:{pw}@{host}'), 27023)

        #db_name = "PraktikumExperiment"
        #client = MongoClient("mongodb://localhost:27017/PraktikumExperiment")

        mydb = client.get_database(db_name)
        fs = gridfs.GridFS(mydb, "Audio")
    except pymongo.errors as ex:
        print(ex)
        mydb = None
        fs = None
    return mydb, fs


# databasedb, audiofs = build_pymongo_connection(f"config.json")
# mycol = databasedb["testing"]
# db_player = databasedb["player"]
# db_object = databasedb["object"]
# body_fs = gridfs.GridFS(databasedb, "body")
# right_hand_fs = gridfs.GridFS(databasedb, "rightHand")
# left_hand_fs = gridfs.GridFS(databasedb, "leftHand")
# db_special = databasedb["special"]
# db_log = databasedb["log"]

databasedb, audiofs = build_pymongo_connection(f"config.json")
mycol = databasedb["testing"]

db_audio = databasedb["Audio"]
db_Body = databasedb["Body"]
db_Hand = databasedb["Hand"]
db_finger = databasedb["Finger"]
db_Head = databasedb["Head"]
db_log = databasedb["Log"]
db_object = databasedb["Object"]
# db_player = databasedb["Player"]
db_special = databasedb["Special"]
db_logIn = databasedb["LogIn"]
db_face = databasedb["Facial"]
db_eye = databasedb["Eye"]
db_role = databasedb["Role"]
db_level = databasedb["Level"]
db_misc = databasedb["Misc"]

audio_converter = RestApiAudioConverter(np.int64, 16000, 2, 1)
"""
whisper_model = Whisper2Text("small")
with open("config_chatgpt.json", "r", encoding="UTF-8") as json_file:
    key_chatgpt = json.load(json_file)

chatgpt_class = ChatGpt(key_chatgpt["key"], "gpt-3.5-turbo-16k")
text_2_speech = TextToSpeech(False)
alpaca_bot = AlpacaBot("declare-lab/flan-alpaca-base")
# longform_bot = AlpacaBot("declare-lab/flan-alpaca-base")
longform_bot = LongForm("akoksal/LongForm-OPT-2.7B", "cuda:0", 1024)
my_robert = robert(Path("lit-llama-lora-finetuned.pth"), Path("lit-llama.pth"), Path("tokenizer.model"), None, "float32", 100, 200, 0.8)
print("ChatGPT")
print(chatgpt_class.get_response("What is a Chatbot?"))
print("Alpaca")
print(alpaca_bot.get_response("What is a Chatbot?"))
print("Longform")
print(longform_bot.get_response("What is a Chatbot?"))
print("Robert")
print(my_robert.get_response("Hi, how are you?"))
print(my_robert.get_response("Where are we?"))
print(my_robert.get_response("Tell me something about this place."))
"""
# @app.get("/Unload")
# def unload_models():
#     try:
#         del alpaca_bot
#         del longform_bot
#         del my_robert
#         torch.cuda.empty_cache()
#     except Exception as ex:
#         return {"error": f"{ex}"}, 412
#     return {"status": "Models Unloaded"}, 202

# @app.get("/Load")
# def load_models():
#     try:
#         global alpaca_bot
#         alpaca_bot = AlpacaBot("declare-lab/flan-alpaca-base")
#         # longform_bot = AlpacaBot("declare-lab/flan-alpaca-base")
#         global longform_bot
#         longform_bot = LongForm("akoksal/LongForm-OPT-2.7B", "cuda:0", 1024)
#         global my_robert
#         my_robert = robert(Path("lit-llama-lora-finetuned.pth"), Path("lit-llama.pth"), Path("tokenizer.model"), None,
#                            "float32", 100, 200, 0.8)
#     except Exception as ex:
#         return {"error": f"{ex}"}, 412
#     return {"status": "Loaded Models"}, 202

"""
@app.post("/ChatGPT")
def chat_gpt_response():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        data_request = request.get_json()
        try:
            audio_bytes = data_request["audioData"]["base64"]
            audio_seg = audio_converter.convert_bytes_to_audio_segment(audio_bytes)
            print("Convert Audio")
            whisper_text = whisper_model.get_text(audio_seg, True)
            print("Get Text")
            response = chatgpt_class.get_response(whisper_text["text"])
            print("Text2Speech")
            audio_response = text_2_speech.text_audio(response, whisper_text["language"])
            print("Completed")
            return {
                "status": "success",
                "audio": {
                    "base64": audio_response
                },
                "text_in": whisper_text["text"],
                "text_out": response,
                "lang": whisper_text["language"]
            }, 202
        except Exception as ex:
            return {"error": f"{ex}"}, 412
    return {"error": "Request must be JSON"}, 415


@app.post("/Longform")
def longform_chat_response():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        data_request = request.get_json()
        try:
            audio_bytes = data_request["audioData"]["base64"]
            audio_seg = audio_converter.convert_bytes_to_audio_segment(audio_bytes)
            print("Convert Audio")
            whisper_text = whisper_model.get_text(audio_seg, True)
            print("Get Text")
            response = longform_bot.get_response(whisper_text["text"])
            print("Text2Speech")
            audio_response = text_2_speech.text_audio(response, whisper_text["language"])
            print("Completed")
            return {
                "status": "success",
                "audio": {
                    "base64": audio_response
                },
                "text_in": whisper_text["text"],
                "text_out": response,
                "lang": whisper_text["language"]
            }, 202
        except Exception as ex:
            return {"error": f"{ex}"}, 412
    return {"error": "Request must be JSON"}, 415


@app.post("/Alpaca")
def Alpaca_chat_response():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        data_request = request.get_json()
        try:
            audio_bytes = data_request["audioData"]["base64"]
            audio_seg = audio_converter.convert_bytes_to_audio_segment(audio_bytes)
            print("Convert Audio")
            whisper_text = whisper_model.get_text(audio_seg, True)
            print("Get Text")
            response = alpaca_bot.get_response(whisper_text["text"])
            print("Text2Speech")
            audio_response = text_2_speech.text_audio(response, whisper_text["language"])
            print("Completed")
            return {
                "status": "success",
                "audio": {
                    "base64": audio_response
                },
                "text_in": whisper_text["text"],
                "text_out": response,
                "lang": whisper_text["language"]
            }, 202
        except Exception as ex:
            return {"error": f"{ex}"}, 412
    return {"error": "Request must be JSON"}, 415
#
#
@app.post("/Robert")
def Robert_chat_response():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        data_request = request.get_json()
        try:
            audio_bytes = data_request["audioData"]["base64"]
            audio_seg = audio_converter.convert_bytes_to_audio_segment(audio_bytes)
            print("Convert Audio")
            whisper_text = whisper_model.get_text(audio_seg, True)
            print("Get Text")
            response = my_robert.get_response(whisper_text["text"])
            print("Text2Speech")
            audio_response = text_2_speech.text_audio(response, whisper_text["language"])
            print("Completed")
            return {
                "status": "success",
                "audio": {
                    "base64": audio_response
                },
                "text_in": whisper_text["text"],
                "text_out": response,
                "lang": whisper_text["language"]
            }, 202
        except Exception as ex:
            return {"error": f"{ex}"}, 412
    return {"error": "Request must be JSON"}, 415


@app.post("/whisper")
def speech_to_text():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        data_request = request.get_json()
        try:
            audio_bytes = data_request["audioData"]["base64"]
            audio_seg = audio_converter.convert_bytes_to_audio_segment(audio_bytes)
            print("Convert Audio")
            whisper_text = whisper_model.get_text(audio_seg, True)
            print("Completed")
            return {
                "status": "success",
                "text_in": whisper_text["text"],
                "lang": whisper_text["language"]
            }, 202
        except Exception as ex:
            return {"error": f"{ex}"}, 412
    return {"error": "Request must be JSON"}, 415

@app.post("/TTS")
def text_to_speech():
    if request.is_json:
        data_request = request.get_json()
        try:
            audio_response = text_2_speech.text_audio(data_request["text"], data_request["lang"])
            return {
                "status": "success",
                "audio": {
                    "base64": audio_response
                }
            }
        except Exception as ex:
            return {"error": f"{ex}"}, 412
    return {"error": "Request must be JSON"}, 415
"""


@app.post("/playerLogIn")
def player_log_in():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    player_data = request.get_json()
    z = set(player_data.keys()).intersection(required_data_logIn)
    if request.is_json:
        if len(z) == len(required_data_logIn):
            try:
                player_input = {
                    "playerId": player_data["playerId"],
                    "clientId": player_data["clientId"],
                    "roomId": player_data["roomId"],
                    "sceneName": player_data["sceneName"],
                    "localTime": player_data["localTime"],
                    "serverTime": server_time,
                    "messageId": player_data["messageId"]
                }
                db_logIn.insert_one(player_input)
                return {"status": "success"}, 202
            except Exception as ex:
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for player LogIn: {required_data_logIn}"}, 415
    return {"error": "Request must be JSON"}, 415


@app.post("/playerRoleLogIn")
def role_log_in():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    player_data = request.get_json()
    z = set(player_data.keys()).intersection(required_data_roleIn)
    if request.is_json:
        if len(z) == len(required_data_roleIn):
            try:
                player_input = {
                    "playerId": player_data["playerId"],
                    "localTime": player_data["localTime"],
                    "serverTime": server_time,
                    "role": player_data["role"],
                    "messageId": player_data["messageId"]
                }
                db_role.insert_one(player_input)
                return {"status": "success"}, 202
            except Exception as ex:
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for player LogIn: {required_data_roleIn}"}, 415
    return {"error": "Request must be JSON"}, 415

@app.post("/player")
def player_db():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    #print(request)
    if request.is_json:
        player_data = request.get_json()
        #print(player_data)
        z = set(player_data.keys()).intersection(required_data_player)
        if len(z) == len(required_data_player):
            try:
                # Audio
                audio_data = {
                    "playerId": player_data["playerId"],
                    "audio": player_data["audioData"]["base64"],
                    "messageId": player_data["messageId"],
                    "localTime": player_data["localTime"],
                    "serverTime": server_time
                }
                db_audio.insert_one(audio_data)
                # Body
                body_infos = []
                left_hand_infos = []
                right_hand_infos = []
                left_hand_finger_infos = []
                right_hand_finger_infos = []
                head_infos = []
                eye_infos = []
                face_infos = []
                for c, i in enumerate(player_data["body"]["positions"]):
                    body_infos.append({
                        "playerId": player_data["playerId"],
                        "position": player_data["body"]["positions"][c],
                        "rotation": player_data["body"]["rotations"][c],
                        "messageId": player_data["messageId"],
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })
                    left_hand_infos.append({
                        "playerId": player_data["playerId"],
                        "position": player_data["leftHand"]["positions"][c],
                        "rotation": player_data["leftHand"]["rotations"][c],
                        "messageId": player_data["messageId"],
                        "identifier": "left",
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })
                    right_hand_infos.append({
                        "playerId": player_data["playerId"],
                        "position": player_data["rightHand"]["positions"][c],
                        "rotation": player_data["rightHand"]["rotations"][c],
                        "messageId": player_data["messageId"],
                        "identifier": "right",
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })
                    head_infos.append({
                        "playerId": player_data["playerId"],
                        "position": player_data["body"]["cameraPositions"][c],
                        "rotation": player_data["body"]["cameraRotations"][c],
                        "messageId": player_data["messageId"],
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })
                    if "metaMessage" not in player_data:
                        continue

                    left_hand_finger_infos.append({
                        "playerId": player_data["playerId"],
                        "identifier": "left",
                        "status": player_data["metaMessage"]["leftHandStates"][c]["Status"],
                        "rootPose": player_data["metaMessage"]["leftHandStates"][c]["RootPose"],
                        "boneRotations": player_data["metaMessage"]["leftHandStates"][c]["BoneRotations"],
                        "pinches": player_data["metaMessage"]["leftHandStates"][c]["Pinches"],
                        "pinchStrength": player_data["metaMessage"]["leftHandStates"][c]["PinchStrength"],
                        "pointerPose": player_data["metaMessage"]["leftHandStates"][c]["PointerPose"],
                        "handScale": player_data["metaMessage"]["leftHandStates"][c]["HandScale"],
                        "handConfidence": player_data["metaMessage"]["leftHandStates"][c]["HandConfidence"],
                        "fingerConfidences": player_data["metaMessage"]["leftHandStates"][c]["FingerConfidences"],
                        "requestedTimeStamp": player_data["metaMessage"]["leftHandStates"][c][
                            "RequestedTimeStamp"],
                        "sampleTimeStamp": player_data["metaMessage"]["leftHandStates"][c]["SampleTimeStamp"],
                        "messageId": player_data["messageId"],
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })

                    right_hand_finger_infos.append({
                        "playerId": player_data["playerId"],
                        "identifier": "right",
                        "status": player_data["metaMessage"]["rightHandStates"][c]["Status"],
                        "rootPose": player_data["metaMessage"]["rightHandStates"][c]["RootPose"],
                        "boneRotations": player_data["metaMessage"]["rightHandStates"][c]["BoneRotations"],
                        "pinches": player_data["metaMessage"]["rightHandStates"][c]["Pinches"],
                        "pinchStrength": player_data["metaMessage"]["rightHandStates"][c]["PinchStrength"],
                        "pointerPose": player_data["metaMessage"]["rightHandStates"][c]["PointerPose"],
                        "handScale": player_data["metaMessage"]["rightHandStates"][c]["HandScale"],
                        "handConfidence": player_data["metaMessage"]["rightHandStates"][c]["HandConfidence"],
                        "fingerConfidences": player_data["metaMessage"]["rightHandStates"][c][
                            "FingerConfidences"],
                        "requestedTimeStamp": player_data["metaMessage"]["rightHandStates"][c][
                            "RequestedTimeStamp"],
                        "sampleTimeStamp": player_data["metaMessage"]["rightHandStates"][c]["SampleTimeStamp"],
                        "messageId": player_data["messageId"],
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })

                    face_infos.append({
                        "playerId": player_data["playerId"],
                        "expressionWeights": player_data["metaMessage"]["faceStates"][c]["ExpressionWeights"],
                        "expressionWeightConfidences": player_data["metaMessage"]["faceStates"][c]["ExpressionWeightConfidences"],
                        "status":
                            player_data["metaMessage"]["faceStates"][c]["Status"],
                        "time":
                            player_data["metaMessage"]["faceStates"][c]["Time"],

                        "messageId": player_data["messageId"],
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })
                    eye_infos.append({
                        "playerId": player_data["playerId"],
                        "eyeGazes": player_data["metaMessage"]["eyeGazesStates"][c]["EyeGazes"],
                        "time": player_data["metaMessage"]["eyeGazesStates"][c]["Time"],
                        "messageId": player_data["messageId"],
                        "localTime": player_data["localTime"],
                        "counter": player_data["count"][c],
                        "serverTime": server_time
                    })
                db_Body.insert_many(body_infos)
                db_Hand.insert_many(left_hand_infos)
                db_Hand.insert_many(right_hand_infos)
                db_Head.insert_many(head_infos)
                db_face.insert_many(face_infos)
                db_eye.insert_many(eye_infos)
                db_finger.insert_many(left_hand_finger_infos)
                db_finger.insert_many(right_hand_finger_infos)
                return {"status": "success"}, 202
            except Exception as ex:
                print(ex)
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for player: {required_data_player}"}, 415
    return {"error": "Request must be JSON"}, 415


@app.post("/object")
def object_db():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        object_data = request.get_json()
        z = set(object_data.keys()).intersection(required_data_object)
        if len(z) == len(required_data_object):
            try:
                object_data["server-timestamp"] = dt_string
                id_insert = db_object.insert_one(object_data)
                return {"status": "success"}, 202
            except Exception as ex:
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for Object: {required_data_object}"}, 415
    return {"error": "Request must be JSON"}, 415


@app.get("/status")
def get_status():
    return {"status": "online"}, 202


@app.post("/special")
def add_special():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        special_data = request.get_json()
        z = set(special_data.keys()).intersection(required_data_special)
        if len(z) == len(required_data_special):
            try:
                special_data["server-timestamp"] = dt_string
                id_insert = db_special.insert_one(special_data)
                return {"status": "success"}, 202
            except Exception as ex:
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for Special: {required_data_special}"}, 415
    return {"error": "Request must be JSON"}, 415


@app.post("/levelChange")
def level_change():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    level_data = request.get_json()
    z = set(level_data.keys()).intersection(required_data_levelChange)
    if request.is_json:
        if len(z) == len(required_data_levelChange):
            try:
                level_input = {
                        "playerId": level_data["playerId"],
                        "roomId": level_data["roomId"],
                        "sceneName": level_data["sceneName"],
                        "localTime": level_data["localTime"],
                        "serverTime": server_time,
                        "server-timestamp": dt_string,
                        "levelID": level_data["levelID"],
                        "levelStatus": level_data["levelStatus"]
                }
                db_level.insert_one(level_input)
                return {"status": "success"}, 202
            except Exception as ex:
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for level change: {required_data_levelChange}"}, 415
    return {"error": "Request must be JSON"}, 415

@app.post("/logMisc")
def log_misc():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    misc_data = request.get_json()
    print(request)
    print(misc_data)
    print(type(misc_data))
    z = set(misc_data.keys()).intersection(required_data_misc)
    if request.is_json:
        if len(z) == len(required_data_misc):
            try:
                misc_input = {
                        "playerId": misc_data["playerId"],
                        "localTime": misc_data["localTime"],
                        "serverTime": server_time,
                        "server-timestamp": dt_string,
                        "data": misc_data["jsonData"],
                }
                db_misc.insert_one(misc_input)
                return {"status": "success"}, 202
            except Exception as ex:
                print(ex)
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for misc log: {required_data_misc}"}, 415
    return {"error": "Request must be JSON"}, 415

# @app.post("/audio")
# def add_audio():
#     if request.is_json:
#         audio_data = request.get_json()
#         audio_data_clone = copy.deepcopy(audio_data)
#         audio_data_clone.pop("audioData")
#         z = set(audio_data.keys()).intersection(required_data_audio)
#         if len(z) == len(required_data_audio):
#             server_time = datetime.datetime.now()
#             dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
#
#             if mycol.count_documents({"roomId": audio_data["roomId"], "clientId": audio_data["clientId"],
#                                       "messageType": audio_data["messageType"]}, limit=1):
#                 result = mycol.find_one({"roomId": audio_data["roomId"], "clientId": audio_data["clientId"],
#                                          "messageType": audio_data["messageType"]})
#
#             else:
#                 byte_data = bytes(audio_data["audioData"]["base64"], 'utf-8')
#                 id_fs = audiofs.put(byte_data, filename=f"{audio_data['clientId']}#{audio_data['roomId']}",
#                                     contentType="base64")
#                 audio_data_clone["id_fs"] = id_fs
#                 audio_data_clone["server-time"] = server_time
#                 id_insert = mycol.insert_one(audio_data_clone)
#                 print(mycol.find({"_id": id_insert}))
#             id_database = f"id"
#             return id_database, 202
#         else:
#             return {"error": f"Request Json must content following keys for audio: {required_data_audio}"}, 415
#     return {"error": "Request must be JSON"}, 415


@app.post("/log")
def add_log():
    server_time = datetime.datetime.now()
    dt_string = server_time.strftime("%d/%m/%Y %H:%M:%S")
    if request.is_json:
        log_data = request.get_json()
        z = set(log_data.keys()).intersection(required_data_log)
        if len(z) == len(required_data_special):
            try:
                log_data["server-timestamp"] = dt_string
                id_insert = db_log.insert_one(log_data)
                return {"status": "success"}, 202
            except Exception as ex:
                return {"error": f"{ex}"}, 412
        else:
            return {"error": f"Request Json must content following keys for Special: {required_data_special}"}, 415
    return {"error": "Request must be JSON"}, 415


if __name__ == '__main__':
    app.run(threaded=True, host="0.0.0.0", port=5000)