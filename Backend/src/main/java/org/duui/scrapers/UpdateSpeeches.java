//package org.duui.scrapers;
//
//import org.duui.bo.MongoSentenceEmbedding;
//import org.duui.bo.MongoSentiment;
//import org.duui.service.MongoSpeechService;
//import org.jsoup.Jsoup;
//import org.jsoup.nodes.Document;
//import org.jsoup.nodes.Element;
//import org.jsoup.select.Elements;
//import org.springframework.stereotype.Component;
//
//import java.io.*;
//import java.nio.file.*;
//import java.nio.file.attribute.BasicFileAttributes;
//import java.util.ArrayList;
//import java.util.List;
//import java.util.zip.GZIPInputStream;
//
//@Component
//public class UpdateSpeeches {
//
//    MongoSpeechService mongoSpeechService;
//
//    public UpdateSpeeches(MongoSpeechService mongoSpeechService) {
//        this.mongoSpeechService = mongoSpeechService;
//        main();
//    }
//
//    public void main() {
//        String directoryPath = "/Users/wmano/Downloads/praktikum/embeddings/Bundestag/20"; // Passe den Pfad an
//        try {
//            Files.walkFileTree(Paths.get(directoryPath), new SimpleFileVisitor<Path>() {
//                @Override
//                public FileVisitResult visitFile(Path file, BasicFileAttributes attrs) throws IOException {
//                    if (file.toString().endsWith(".gz")) {
//                        processGzFile(file);
//                    }
//                    return FileVisitResult.CONTINUE;
//                }
//            });
//        } catch (IOException e) {
//            e.printStackTrace();
//        }
//    }
//
//    private void processGzFile(Path gzFilePath) {
//        try (GZIPInputStream gzipInputStream = new GZIPInputStream(new FileInputStream(gzFilePath.toFile()));
//             BufferedReader reader = new BufferedReader(new InputStreamReader(gzipInputStream))) {
//
//            StringBuilder content = new StringBuilder();
//            String line;
//            while ((line = reader.readLine()) != null) {
//                content.append(line).append("\n");
//            }
//
//            Document doc = Jsoup.parse(content.toString(), "", org.jsoup.parser.Parser.xmlParser());
//
//            Elements idElement = doc.select("bundestag|Speech");
//            String id = idElement.attr("id");
//            System.out.println("ID: " + id);
//
//            List<MongoSentenceEmbedding> mongoSentenceEmbeddingList = new ArrayList<>();
//            Elements embedding = doc.select("type20|Embedding");
//            for (Element umlClass : embedding) {
//                String embedding1 = umlClass.attr("embedding");
//                List<Float> list = convertToFloatArray(embedding1, 512);
//
//                String begin = umlClass.attr("begin");
//                String end = umlClass.attr("end");
//
//                MongoSentenceEmbedding mongoSentenceEmbedding = new MongoSentenceEmbedding();
//                mongoSentenceEmbedding.setBegin(Integer.valueOf(begin));
//                mongoSentenceEmbedding.setEnd(Integer.valueOf(end));
//                mongoSentenceEmbedding.setFloats(list);
//
//                mongoSentenceEmbeddingList.add(mongoSentenceEmbedding);
//
//            }
//
//            List<MongoSentiment> mongoSentimentList = new ArrayList<>();
//            Elements sentiment = doc.select("type18|GerVaderSentiment");
//            for (Element umlClass : sentiment) {
//                MongoSentiment mongoSentiment = new MongoSentiment();
//                mongoSentiment.setBegin(Integer.valueOf(umlClass.attr("begin")));
//                mongoSentiment.setEnd(Integer.valueOf(umlClass.attr("end")));
//                mongoSentiment.setSentiment(Double.valueOf(umlClass.attr("sentiment")));
//                mongoSentimentList.add(mongoSentiment);
//            }
//
//            mongoSpeechService.updateObject(id, mongoSentenceEmbeddingList, mongoSentimentList);
//
//        } catch (IOException e) {
//            System.err.println("Fehler beim Verarbeiten der Datei: " + gzFilePath);
//            e.printStackTrace();
//        }
//    }
//
//    public static List<Float> convertToFloatArray(String input, int arraySize) {
//        String[] parts = input.trim().split("\\s+"); // Auf Leerzeichen basierend splitten
//        float[] result = new float[arraySize];
//        List<Float> list = new ArrayList<>();
//
//        for (int i = 0; i < arraySize; i++) {
//            if (i < parts.length) {
//                try {
//                    result[i] = Float.parseFloat(parts[i]); // String zu Float umwandeln
//                    list.add(result[i]);
//                } catch (NumberFormatException e) {
//                    result[i] = 0.0f; // Falls ein Wert ungültig ist, mit 0.0f auffüllen
//                    list.add(result[i]);
//                }
//            } else {
//                result[i] = 0.0f; // Falls zu wenige Werte vorhanden sind, mit 0.0f auffüllen
//                list.add(result[i]);
//            }
//        }
//        return list;
//    }
//
//}
