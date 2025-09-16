package org.texttechnologylab.vasililab.database;

import com.mongodb.MongoClient;
import com.mongodb.*;
import com.mongodb.client.*;
import com.mongodb.client.result.UpdateResult;
import org.apache.uima.UIMAException;
import org.bson.Document;
import org.bson.types.ObjectId;

import java.util.*;
import java.util.concurrent.TimeUnit;

/**
 * Class for interaction with a MongoDB
 * @author Giuseppe Abrami
 */
public class MongoDBConnectionHandler {

    /**
     *  MongoDBConfig Object
     */
    private MongoDBConfig pConfig = null;

    /**
     *  The connection with the MongoDB
     */
    private MongoClient pClient = null;

    /**
     * The object for the selected Database
     */
    private MongoDatabase pDatabase = null;

    /**
     * Amount of all Collections
     */
    private MongoCollection<Document> pCollection = null;

    /**
     * Constructor
     * @param pConfig
     */
    public MongoDBConnectionHandler(MongoDBConfig pConfig){
        this.pConfig = pConfig;
        init();
    }

    /**
     * Internal method for establishing the connection
     */
    private void init(){

       // defind credentials (Username, database, password)
        MongoCredential credential = MongoCredential.createScramSha1Credential(pConfig.getMongoUsername(), pConfig.getMongoDatabase(), pConfig.getMongoPassword().toCharArray());
        // defining Hostname and Port
        ServerAddress seed = new ServerAddress(pConfig.getMongoHostname(), pConfig.getMongoPort());
        List<ServerAddress> seeds = new ArrayList(0);
        seeds.add(seed);
        // defining some Options
        MongoClientOptions options = MongoClientOptions.builder()
                .connectionsPerHost(20)
                .socketTimeout(300000)
                .maxWaitTime(300000)
                .serverSelectionTimeout(300000)
                .connectTimeout(300000)
                .sslEnabled(false)
                .build();

        // connect to MongoDB
        pClient = new MongoClient(seeds, credential, options);

        // select database
        pDatabase = pClient.getDatabase(pConfig.getMongoDatabase());

        // select default connection
        pCollection = pDatabase.getCollection(pConfig.getMongoCollection());

        // some debug information
        System.out.println("Connect to "+pConfig.getMongoDatabase()+" on "+pConfig.getMongoHostname());

    }

    /**
     * Method to return the default Collection
     * @return MongoCollection
     */
    public MongoCollection getCollection(){
        return this.pCollection;
    }

    /**
     * Method to return the default Collection
     * @return MongoCollection
     */
    public MongoCollection getCollection(String sCollection){
        return this.pDatabase.getCollection(sCollection);
    }

    /**
     * Method to return the connected Database-Object
     * @return
     */
    public MongoDatabase getDatabase(){
        return this.pDatabase;
    }

    public Document getObject(String sID, String sCollection){

        BasicDBObject whereQuery = new BasicDBObject();
        whereQuery.put("_id", new ObjectId(sID));

        FindIterable<Document> result = this.getCollection(sCollection).find(whereQuery);

        Document doc = null;

        MongoCursor<Document> it = result.iterator();

        while(it.hasNext()){
            doc = it.next();
        }

        return doc;

    }

    /**
     * Method to update (replace) a comment in the database
     * @param pDocument
     * @param sCollection
     * @return
     * @throws UIMAException
     */
    public boolean update(Document pDocument, String sCollection) {

        BasicDBObject whereQuery = new BasicDBObject();
        whereQuery.put("_id", pDocument.getObjectId("_id"));
        UpdateResult uResult = null;

        try {
            uResult = this.getCollection(sCollection).replaceOne(whereQuery, pDocument);
        }
        catch (Exception e){
            e.printStackTrace();
        }

        if(uResult==null){
            return false;
        }
        else{
            if(uResult.getMatchedCount()==0){
                this.getCollection(sCollection).insertOne(pDocument);
            }
        }
        return uResult.getModifiedCount()>0;

    }

    /**
     * Method to execute a query
     * @param query
     * @return
     */
    public FindIterable doQuery(BasicDBObject query){
        FindIterable result = this.getCollection().find(query);
        return result;
    }

    /**
     * Method to execute a query
     * @param query
     * @return
     */
    public MongoCursor<Document> doQueryIterator(String query, String sCollection){
        return doQueryIterator(query, sCollection, -1, -1);
    }

    /**
     * Method to execute a query
     * @param query
     * @return
     */
    public MongoCursor<Document> doQueryIterator(BasicDBObject query, String sCollection){
        return doQueryIterator(query, sCollection, -1, -1);
    }

    /**
     * Method to execute a query
     * @param query
     * @return
     */

    public MongoCursor<Document> doQueryIterator(BasicDBObject query, String sCollection, int iSkip, int iLimit){
        FindIterable result = this.getCollection(sCollection).find(query);
        if(iLimit>0){
            result = result.limit(iLimit);
        }
        if(iSkip>0){
            result = result.skip(iSkip);
        }
        result = result.maxTime(5, TimeUnit.MINUTES);

        return result.iterator();
    }

    public MongoCursor<Document> doQueryIterator(String query, String sCollection, int iSkip, int iLimit){
        return doQueryIterator(BasicDBObject.parse(query), sCollection, iSkip, iLimit);
    }

    /**
     * Method to execute a query
     * @param sField
     * @param sCollection
     * @param returnType
     * @return
     */
    public MongoCursor doQueryIteratorDistinct(String sField, Class returnType, String sCollection){
        DistinctIterable result = this.getCollection(sCollection).distinct(sField, returnType);
        return result.iterator();
    }

    /**
     * Method to count the results of a given query
     * @param query
     * @return
     */
    public long count(BasicDBObject query){
        return this.getCollection().countDocuments(query);
    }
    /**
     *
     * Method to count the results of a given query
     * @param query
     * @return
     */
    public long count(BasicDBObject query, String sCollection){
        return this.getCollection(sCollection).countDocuments(query);
    }

    /**
     *
     * Method to count the results of a given query
     * @param query
     * @return
     */
    public long count(String query, String sCollection){
        return count(BasicDBObject.parse(query), sCollection);
    }

    /**
     * Method to return all existing collections
     * @return
     */
    public Set<String> listCollections(){
        Set<String> rSet = new HashSet<>(0);
        rSet.addAll((Collection<? extends String>) this.getDatabase().listCollectionNames().spliterator());
        return rSet;
    }


    public void insert(Document pDocument, String sCollection) {

        Document rDocument = null;

        if(pDocument.containsKey("_id")){
            rDocument = getObject(String.valueOf(pDocument.getObjectId("_id")), sCollection);
        }

        if(rDocument==null){

            this.getCollection(sCollection).insertOne(pDocument);

        }

    }

    public String createIndex(String sCollection, String sField){

        BasicDBObject query = new BasicDBObject();
        query.put(sField, "text");
        String rString = this.getCollection(sCollection).createIndex(query);

        return rString;
    }
}
