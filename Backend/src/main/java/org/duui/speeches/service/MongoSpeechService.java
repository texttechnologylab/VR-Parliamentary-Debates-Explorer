package org.duui.speeches.service;

import org.apache.commons.compress.compressors.CompressorException;
import org.apache.uima.fit.factory.JCasFactory;
import org.apache.uima.fit.util.JCasUtil;
import org.apache.uima.jcas.JCas;
import org.apache.uima.util.InvalidXMLException;
import org.duui.speeches.bo.MongoDotProduktResult;
import org.duui.speeches.bo.MongoNavResult;
import org.duui.speeches.bo.MongoSpeech;
import org.duui.speeches.dao.MongoSpeechDao;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.texttechnologylab.DockerUnifiedUIMAInterface.DUUIComposer;
import org.texttechnologylab.DockerUnifiedUIMAInterface.driver.DUUIRemoteDriver;
import org.texttechnologylab.DockerUnifiedUIMAInterface.lua.DUUILuaContext;
import org.texttechnologylab.uima.type.Embedding;
import org.xml.sax.SAXException;

import java.io.IOException;
import java.net.URISyntaxException;
import java.util.List;
import java.util.concurrent.atomic.AtomicReference;
import java.util.stream.Collectors;
import java.util.stream.DoubleStream;

@Service
public class MongoSpeechService {

    private final MongoSpeechDao mongoSpeechDao;
    private final DUUIComposer pComposer;
    private final DUUIRemoteDriver remoteDriver;
    private final DUUILuaContext ctx;

    @Autowired
    public MongoSpeechService(MongoSpeechDao mongoSpeechDao) throws IOException, URISyntaxException, CompressorException, InvalidXMLException, SAXException {
        this.mongoSpeechDao = mongoSpeechDao;
        ctx = new DUUILuaContext().withJsonLibrary();
        pComposer = new DUUIComposer()
                .withSkipVerification(true)
                .withLuaContext(ctx)
                .withWorkers(1);
        remoteDriver = new DUUIRemoteDriver();
        pComposer.addDriver(remoteDriver);
        pComposer.add(new DUUIRemoteDriver.Component("http://spacy.lehre.texttechnologylab.org")
                .withScale(1)
                .build());
        pComposer.add(new DUUIRemoteDriver.Component("http://jina.duui.lehre.texttechnologylab.org")
                .withScale(1)
                .build());
    }

    public boolean isAlreadyInDatabase(String wahlperiode, String sitzungsNr){
        return mongoSpeechDao.existsByWahlperiodeAndSitzungsNr(wahlperiode, sitzungsNr);
    }

    public List<String> getSpeechIds(){
        return mongoSpeechDao.findAllIds();
    }

    public MongoSpeech getSpeech(String id){
        return mongoSpeechDao.getMongoSpeechById(id);
    }

    public List<MongoNavResult> getNavigation(){
        return mongoSpeechDao.getAllNavigationObjekts();
    }

    public List<MongoDotProduktResult> getTopSpeeches(String vorname, String nachname, Integer maxSitzungsNr, String text) throws Exception {
        JCas cas = JCasFactory.createJCas();
        cas.setSofaDataString(text, "text/plain");
        cas.setDocumentLanguage("de");
        List<Float> vector = scanCas(cas);

        return mongoSpeechDao.findBestAnswersByRednerId(vorname, nachname, vector, maxSitzungsNr);
    }

    private List<Float> scanCas(JCas cas) throws Exception {
        pComposer.run(cas);

        AtomicReference<List<Float>> floats = new AtomicReference<>();
        JCasUtil.select(cas, Embedding.class).forEach(embedding -> {
            floats.set(convertToList(embedding.getEmbedding().toArray()));
        });
        if (floats == null){
            return null;
        }
        return floats.get();
    }

    private static List<Float> convertToList(float[] floatArray) {
        return DoubleStream.of(toDoubleArray(floatArray))
                .boxed()
                .map(Double::floatValue)
                .collect(Collectors.toList());
    }

    private static double[] toDoubleArray(float[] floatArray) {
        double[] doubleArray = new double[floatArray.length];
        for (int i = 0; i < floatArray.length; i++) {
            doubleArray[i] = floatArray[i];
        }
        return doubleArray;
    }

}
