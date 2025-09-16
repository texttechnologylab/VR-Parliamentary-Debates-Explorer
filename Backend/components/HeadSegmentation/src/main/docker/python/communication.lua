-- Bind static classes from java
StandardCharsets = luajava.bindClass("java.nio.charset.StandardCharsets")
util = luajava.bindClass("org.apache.uima.fit.util.JCasUtil")
FSArray = luajava.bindClass("org.apache.uima.jcas.cas.FSArray")

-- This "serialize" function is called to transform the CAS object into an stream that is sent to the annotator
-- Inputs:
--  - inputCas: The actual CAS object to serialize
--  - outputStream: Stream that is sent to the annotator, can be e.g. a string, JSON payload, ...
function serialize(inputCas, outputStream, params)
    -- Get data from CAS

    local imageBase64 = inputCas:getSofaDataString()

    -- Encode data as JSON object and write to stream
    outputStream:write(json.encode({
        image = imageBase64,
    }))
end

-- This "deserialize" function is called on receiving the results from the annotator that have to be transformed into a CAS object
-- Inputs:
--  - inputCas: The actual CAS object to deserialize into
--  - inputStream: Stream that is received from to the annotator, can be e.g. a string, JSON payload, ...
function deserialize(inputCas, inputStream)
    -- Get string from stream, assume UTF-8 encoding
    local inputString = luajava.newInstance("java.lang.String", inputStream:readAllBytes(), StandardCharsets.UTF_8)

    -- Parse JSON data from string into object
    local results = json.decode(inputString)


    -- Add tokens to jcas
    if results["image"] ~= nil then

        local image = luajava.newInstance("org.texttechnologylab.annotation.type.Image", inputCas)
        image:setSrc(results["image"]["src"])
        image:setWidth(results["image"]["width"])
        --image:setHeight(sent["image"]["height"]) not working as of now

        if #results["sub_image"]["coordinates"] > 1 then
            local subImage = luajava.newInstance("org.texttechnologylab.annotation.type.SubImage", inputCas)
            subImage:setParent(image)
        
            local coords = {}
            for k, coordinate in pairs(results["sub_image"]["coordinates"]) do
                local coord = luajava.newInstance("org.texttechnologylab.annotation.type.Coordinate", inputCas)
                coord:setX(coordinate[1])
                coord:setY(coordinate[2])
                table.insert(coords, coord)
            end

            local array = FSArray:create(inputCas, coords)
            subImage:setCoordinates(array)

            subImage:addToIndexes()
        end
    end
end
