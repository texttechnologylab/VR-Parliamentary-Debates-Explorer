<h1>Virtual-Parliament-Explorer</h1>
<h2></h2>

## /speeches
### GET
### /getSpeeches/{id} 

**Beschreibung:** Gibt das Redeobjekt einer Rede zurück. Enthalten ist die Rede als Text, Informationen über alle beteiligten Redner, alle Kommentare, die Sentiments sowie die Embeddings.

**Bsp.: Antwort:**
```json

        {
            "id": "ID20400100",
            "begin": 0,
            "end": 9510,
            "sofaString": "Sehr geehrte Frau Präsidentin! Meine Damen..."
            "agenda": {
                "titleRede": null,
                "titleTop": "a) Erste Beratung des ..."
            },
                "wahlperiode": 20,
            "sitzungsNr": 4,
            "datum": 1.6388352E9,
            "sentiments": [
                {
                    "begin": 0,
                    "end": 9510,
                    "sentiment": 0.3853833333333333
                },...
            ],
            "speechSections": [
                {
                    "begin": 0,
                    "end": 319,
                    "type": "text",
                    "speaker": {
                        "firstName": null,
                        "lastName": "Dittmar",
                        "id": "11004261",
                        "party": "SPD"
                    }
                },...
            ],
            "embeddings": [
                {
                    "begin": 0,
                    "end": 260,
                    "floats": [...]
                }, ...
            ]
        }
```
### GET
### /getNav

**Beschreibung:** Gibt eine Liste mit allen Tagesordnungspunkte mit den Titeln und den dazugehörigen Reden.

**Bsp.: Antwort:**
```json
[
    {
            "id": 4,
            "titles": [
                {
                    "title": "a) Erste Beratung des ...",
                    "ids": [
                        "ID20400100",
                        "ID20400200",
                        "ID20400300",
                        "ID20400400",
                        "ID20400500",
                        "ID20400600",
                        "ID20400700",
                        "ID20400800",
                        "ID20400900",
                        "ID20401000",
                        "ID20401100",
                        "ID20401200",
                        "ID20401300",
                        "ID20401400",
                        "ID20401500",
                        "ID20401600",
                        "ID20401700"
                    ]
                }
            ]
        }
    },
    ...
]
```
### POST
### /getTopSpeeches

**Beschreibung:** Gibt eine Liste mit den 10 passendsten Redeabschnitten und der ganzen Rede sowie der ID.

**Bsp.: Antwort:**
```json
[
    {
        "id": "ID204603900",
        "sofaString": "Schönen Dank für Ihre Frage. ...",
        "sofaSubstring": "Ich glaube auch, dass sich die Lage ...",
        "embeddings": {
            "begin": 247,
            "end": 556,
            "floats": ...
        },
        "dotProduct": 0.4984118354062165
    },
    ...
]
```

## /abgeordnete
### GET
### /getAbgeordneterById/{id}

**Beschreibung:** Gibt das Rednerobjekt eines Abgeordneten.

**Bsp.: Antwort:**
```json
{
  "id": "...",
  "name": "...",
  "beruf": "...",
  "partei": "...",
  "biografie": "...",
  "image": "..."
}
```

### GET
### /getAbgeordneter?firstName={firstName}}&lastName={lastName}&party={partei}

**Beschreibung:** Gibt die wichtigsten Informationen eines Abgeordneten.

**Bsp.: Antwort:**
```json
{
  "id": "1043330",
  "name": "Abdi, Sanae",
  "beruf": "",
  "partei": "SPD"
}
```

### GET
### /getAll

**Beschreibung:** Gibt alle Abgeordneten mit Bildern. Wird hauptsächlich beim Initialisieren des Bundestags verwendet.

**Bsp.: Antwort:**
```json
[
  {
    "id": "...",
    "name": "...",
    "beruf": "...",
    "partei": "...",
    "biografie": "...",
    "image": "..."
  },
  ...
]
```
