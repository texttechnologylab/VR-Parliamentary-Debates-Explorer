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
from tqdm import tqdm

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

databasedb, audiofs = build_pymongo_connection(f"config.json")

collections = databasedb.list_collection_names()

# Iterate through each collection and export as JSON
for collection_name in tqdm(collections):
    print(f"Save {collection_name}")
    collection = databasedb[collection_name]
    documents = list(collection.find())

    # Create a JSON file for each collection
    json_filename = f"exports/2024_02_05/{collection_name}.json"
    with open(json_filename, "w") as json_file:
        json.dump(documents, json_file, default=str, indent=2)

    print(f"Exported {len(documents)} documents from {collection_name} to {json_filename}")

# Close the MongoDB connection
#databasedb.close()