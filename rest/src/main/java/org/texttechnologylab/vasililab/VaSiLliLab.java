package org.texttechnologylab.vasililab;

import com.mongodb.Mongo;
import org.texttechnologylab.vasililab.database.MongoDBConfig;
import org.texttechnologylab.vasililab.database.MongoDBConnectionHandler;
import org.texttechnologylab.vasililab.rest.Methods;
import spark.Spark;
import spark.servlet.SparkApplication;

import java.io.IOException;

public class VaSiLliLab implements SparkApplication {

    public static void main(String[] args){

        VaSiLliLab lab = new VaSiLliLab();
        lab.init();

    }

    public VaSiLliLab(){


    }

    @Override
    public void init() {

        Spark.port(8081);
        Spark.staticFileLocation("html");

        String sDBConfig = VaSiLliLab.class.getClassLoader().getResource("dbconnection.txt").getPath();

        try {
            MongoDBConfig dbConfig = new MongoDBConfig(sDBConfig);
            MongoDBConnectionHandler pHandler = new MongoDBConnectionHandler(dbConfig);

            Methods m = new Methods(pHandler);
            m.init();

        } catch (IOException e) {
            throw new RuntimeException(e);
        }



    }

    @Override
    public void destroy() {
        SparkApplication.super.destroy();
    }
}
