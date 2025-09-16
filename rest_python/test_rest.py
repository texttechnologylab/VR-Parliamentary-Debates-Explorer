import json
import requests
with open("data/player.json", "r", encoding="UTF-8") as json_file:
    json_data = json.load(json_file)
    print(json_data[0])
with open("data/objects.json", "r", encoding="UTF-8") as json_file:
    object_data = json.load(json_file)
    print(json_data[0])
headers = {
    'Content-Type': 'application/json',
}

data = json_data[0]
print(f"curl -i http://127.0.0.1:5000/player -X POST -H 'Content-Type: application/json' -d '{json.dumps(json_data[0])}'")
# Player test
for i in json_data:
    response = requests.post('http://127.0.0.1:5000/player', headers=headers, json=i)
# Object test
for i in object_data:
    response = requests.post('http://127.0.0.1:5000/object', headers=headers, json=i)