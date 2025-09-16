package org.duui.scrapers.dao;

import org.duui.scrapers.bo.MongoBaseSpeech;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface MongoBaseSpeechDAO extends MongoRepository<MongoBaseSpeech, String> {

    boolean existsByWahlperiodeAndSitzungsNr(Integer wahlperiode, Integer sitzungsNr);

}
