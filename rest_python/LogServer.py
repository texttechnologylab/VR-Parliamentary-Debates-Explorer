# app.py
import copy

from flask import Flask, request, jsonify
from pymongo import MongoClient
import gridfs
import datetime
import pymongo.errors
import json
from urllib.parse import quote_plus

app = Flask(__name__)

required_data_audio = {"roomId", "messageType", "clientId", "timestamp", "position", "rotation", "cameraRotation",
                       "audioData", "peers"}
required_data_player = {"playerId", "audioData", "localTime", "messageId", "body", "leftHand", "rightHand"}
required_data_logIn = {"playerId", "roomId", "sceneName", "clientId", "localTime", "messageId"}
required_data_roleIn = {"playerId", "role", "localTime", "messageId"}
required_data_object = {"playerId", "referenceMessage", "localTime", "objectId", "objectName", "hand", "interaction"}
required_data_special = {"localTime", "mode"}
required_data_log = {"playerId", "localTime", "referenceMessage", "logMessage", "logType", "stacktrace", "localTime"}


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
        mydb = client.get_database(db_name)
        fs = gridfs.GridFS(mydb, "Audio")
    except pymongo.errors as ex:
        print(ex)
        mydb = None
        fs = None
    return mydb, fs


databasedb, audiofs = build_pymongo_connection(f"config.json")
mycol = databasedb["testing"]

# db_audio = databasedb["AudioTest"]
# db_Body = databasedb["BodyTest"]
# db_Hand = databasedb["handTest"]
# db_Head = databasedb["headTest"]
# db_log = databasedb["logTest"]
# db_object = databasedb["objectTest"]
# db_player = databasedb["playerTest"]
# db_special = databasedb["specialTest"]
# db_logIn = databasedb["logInTest"]
# db_face = databasedb["facialTest"]
# db_eye = databasedb["eyeTest"]
# db_role = databasedb["roleTest"]

db_audio = databasedb["Audio"]
db_Body = databasedb["Body"]
db_Hand = databasedb["Hand"]
db_Head = databasedb["Head"]
db_log = databasedb["Log"]
db_object = databasedb["Object"]
# db_player = databasedb["Player"]
db_special = databasedb["Special"]
db_logIn = databasedb["LogIn"]
db_face = databasedb["Facial"]
db_eye = databasedb["Eye"]
db_role = databasedb["Role"]


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
    if request.is_json:
        player_data = request.get_json()
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
                return {"status": "success"}, 202
            except Exception as ex:
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
    app.run(threaded=True, host="0.0.0.0", port=5000, debug=True)
