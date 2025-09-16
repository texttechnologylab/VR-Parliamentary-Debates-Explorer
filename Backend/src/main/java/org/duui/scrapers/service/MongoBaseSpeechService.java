package org.duui.scrapers.service;

import org.duui.scrapers.PlenarprotokollScraper;
import org.duui.scrapers.bo.MongoBaseSpeech;
import org.duui.scrapers.dao.MongoBaseSpeechDAO;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class MongoBaseSpeechService {

    MongoBaseSpeechDAO mongoBaseSpeechDAO;

    public MongoBaseSpeechService(MongoBaseSpeechDAO mongoBaseSpeechDAO) {
        this.mongoBaseSpeechDAO = mongoBaseSpeechDAO;
    }

    @Transactional
    public void insertUnprocessedDocuments(List<MongoBaseSpeech> speeches) {
        mongoBaseSpeechDAO.insert(speeches);
    }

    @Transactional
    public void insertSave(List<MongoBaseSpeech> speeches){
        for (MongoBaseSpeech speech : speeches) {
            if (!mongoBaseSpeechDAO.existsById(speech.getId())) {
                mongoBaseSpeechDAO.insert(speech);
            }
        }
    }

    public boolean isAlreadyInDatabase(String wahlperiode, String sitzungsNr) {
        return mongoBaseSpeechDAO.existsByWahlperiodeAndSitzungsNr(Integer.valueOf(wahlperiode), Integer.valueOf(sitzungsNr));
    }
}
