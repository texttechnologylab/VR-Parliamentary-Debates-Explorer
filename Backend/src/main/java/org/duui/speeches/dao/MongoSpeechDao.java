package org.duui.speeches.dao;

import org.duui.speeches.bo.*;
import org.springframework.data.mongodb.repository.Aggregation;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.data.mongodb.repository.Update;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface MongoSpeechDao extends MongoRepository<MongoSpeech, String> {

    @Aggregation(pipeline = {
            "{ $unwind: '$speechSections' }",
            "{ $match: { 'speechSections.speaker.id': ?0 } }",
            "{ $project: { _id: 0, speechSections: 1 } }"
    })
    List<MongoSpeechSection> findSpeechSectionsBySpeakerId(String speakerId);

    @Query(value = "{}", fields = "{ '_id' : 1 }")
    List<String> findAllIds();

    boolean existsByWahlperiodeAndSitzungsNr(String wahlperiode, String sitzungsNr);

    @Query("{ '_id': ?0 }")
    @Update("{ '$set' : { 'embeddings' : ?1, 'Sentiments' : ?2 } }")
    void updateById(String id, List<MongoSentenceEmbedding> embeddings, List<MongoSentiment> sentiments);

    @Query("{ '_id': ?0 }")
    MongoSpeech getMongoSpeechById(String id);

    @Aggregation(pipeline = {
            "{ $group: { _id: { sitzungsNr: '$sitzungsNr', title: '$agenda.titleTop' }, ids: { $push: '$_id' } } }",
            "{ $sort: { '_id.sitzungsNr': 1, '_id.title': 1 } }",
            "{ $group: { _id: '$_id.sitzungsNr', titles: { $push: { title: '$_id.title', ids: '$ids' } } } }",
            "{ $set: { titles: { $map: { " +
                    "input: '$titles', " +
                    "as: 'titleEntry', " +
                    "in: { " +
                    "title: '$$titleEntry.title', " +
                    "ids: { $sortArray: { input: '$$titleEntry.ids', sortBy: 1 } }, " +
                    "id: { $arrayElemAt: [{ $sortArray: { input: '$$titleEntry.ids', sortBy: -1 } }, 0] } " +
                    "} " +
                    "} } } }",
            "{ $sort: { 'titles.id': 1 } }",
            "{ $set: { titles : { $sortArray : { input : $titles , sortBy : { id: 1 } } } } }",
            "{ $project: { 'titles.id': 0 } }",
            "{ $sort: { '_id': 1 } }"
    })
    List<MongoNavResult> getAllNavigationObjekts();


    @Aggregation(pipeline = {
            "{ '$match': { 'sitzungsNr' : { '$lte': ?3} }}",
            "{ '$project': { '_id': 1, 'sofaString': 1, 'embeddings': 1, 'speechSections': 1 } }",
            "{ '$unwind': '$speechSections' }",
            "{ '$match': { 'speechSections.speaker.fristName': ?0 } }",
            "{ '$match': { 'speechSections.speaker.lastName': ?1 } }",
            "{ '$unwind': '$embeddings' }",
            "{ '$match': { '$expr': { '$and': [ { '$gte': [ '$embeddings.begin', '$speechSections.begin' ] }, { '$lte': [ '$embeddings.end', '$speechSections.end' ] } ] } } }",
            "{ '$addFields': { 'dotProduct': { '$sum': { '$map': { 'input': { '$zip': { 'inputs': [ '$embeddings.floats', ?2 ] } }, 'as': 'pair', 'in': { '$multiply': [{ '$arrayElemAt': ['$$pair', 0] }, { '$arrayElemAt': ['$$pair', 1] }] } } } } } }",
            "{ '$project': { '_id': 1, 'sofaString': 1, 'dotProduct': 1, 'embeddings.begin': 1, 'embeddings.end': 1, 'sofaSubstring': { '$substrCP': ['$sofaString', '$embeddings.begin', { '$subtract': [ '$embeddings.end', '$embeddings.begin' ] }]}} }",
            "{ '$sort': { 'dotProduct': -1 } }",
            "{ '$limit': 20 }"
    })
    List<MongoDotProduktResult> findBestAnswersByRednerId(String vorname, String nachname, List<Float> vector, Integer sitzungsNrMaximum);

}
