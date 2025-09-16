package org.duui.scrapers;

import org.apache.uima.jcas.JCas;
import org.apache.uima.jcas.tcas.Annotation;
import org.duui.scrapers.bo.MongoBaseSpeech;
import org.duui.speeches.bo.MongoAgenda;
import org.duui.speeches.bo.MongoSpeaker;
import org.duui.speeches.bo.MongoSpeech;
import org.duui.speeches.bo.MongoSpeechSection;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.nodes.Node;
import org.texttechnologylab.annotation.bundestag.*;

import java.time.LocalDate;
import java.time.ZoneId;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.DoubleStream;

public class ProcessProtokoll {
    private static MongoSpeaker copyRedner(MongoSpeaker speaker){
        MongoSpeaker mongoSpeaker = new MongoSpeaker();
        mongoSpeaker.setId(speaker.getId());
        mongoSpeaker.setFristName(speaker.getFristName());
        mongoSpeaker.setLastName(speaker.getLastName());
        mongoSpeaker.setParty(speaker.getParty());
        return mongoSpeaker;
    }

    public static <A> A[] addToArray(A[] array, A a) {
        A[] newArray = Arrays.copyOf(array, array.length + 1);
        newArray[array.length] = a;
        return newArray;
    }

    static List<MongoBaseSpeech> getSpeeches(Document doc, List<String> entschuldigteAbgeordnete, HashMap<String, String> hashmapOfTops) throws Exception {

        List<MongoBaseSpeech> returnList = new ArrayList<>();

        String sitzungsNr = Objects.requireNonNull(doc.selectFirst("sitzungsnr")).text();
        String wahlperiode = Objects.requireNonNull(doc.selectFirst("wahlperiode")).text();
        String datumString = Objects.requireNonNull(doc.selectFirst("datum")).attr("date");
        double datum = convertDate(datumString);

        Element sitzungsverlauf = doc.selectFirst("sitzungsverlauf");
        sitzungsverlauf.selectFirst("sitzungsbeginn");

        for (Element top : sitzungsverlauf.select("tagesordnungspunkt")) {

            String topKey = top.attr("top-id");
            topKey = topKey.replaceAll(" ", " ");

            List<Node> nodeList = top.childNodes();

            for (Node node : nodeList) {

                if (node.nodeName().equals("rede")) {

                    MongoBaseSpeech mongoBaseSpeech = new MongoBaseSpeech();

                    mongoBaseSpeech.setDatum(datum);
                    mongoBaseSpeech.setWahlperiode(Integer.parseInt(wahlperiode));
                    mongoBaseSpeech.setSitzungsNr(Integer.parseInt(sitzungsNr));

                    StringBuilder gesamterText = new StringBuilder();
                    StringBuilder speakerText = new StringBuilder();

                    MongoSpeaker aktuellerRedner = new MongoSpeaker();

                    MongoSpeechSection[] sectionArray = new MongoSpeechSection[0];

                    for (Node redeNode : node.childNodes()) {
                        if (redeNode.nodeName().equals("p")) {
                            if (redeNode.attr("klasse").equals("redner")){
                                if (!speakerText.isEmpty()) {
                                    sectionArray = saveSpeechSection(aktuellerRedner, speakerText, gesamterText, sectionArray);
                                }

                                speakerText = new StringBuilder();

                                aktuellerRedner = extractRednerFromRednerNode(redeNode);
                            } else {
                                if (redeNode instanceof Element element) {
                                    speakerText.append(element.text());
                                }
                            }
                        }
                        else if (redeNode.nodeName().equals("kommentar")) {
                            if (!speakerText.isEmpty()) {
                                sectionArray = saveSpeechSection(aktuellerRedner, speakerText, gesamterText, sectionArray);
                            }

                            speakerText = new StringBuilder();

                            StringBuilder commentText = new StringBuilder();
                            if (redeNode instanceof Element element) {
                                commentText.append(element.text());
                            }

                            MongoSpeechSection mongoSpeechSection = new MongoSpeechSection();

                            mongoSpeechSection.setText(commentText.toString());
                            mongoSpeechSection.setType("comment");
                            mongoSpeechSection.setBegin(gesamterText.length());
                            gesamterText.append(commentText.toString());
                            mongoSpeechSection.setEnd(gesamterText.length());

                            sectionArray = addToArray(sectionArray, mongoSpeechSection);
                        }
                        else if (redeNode.nodeName().equals("name")) {
                            if (!speakerText.isEmpty()) {
                                sectionArray = saveSpeechSection(aktuellerRedner, speakerText, gesamterText, sectionArray);
                            }

                            speakerText = new StringBuilder();

                            aktuellerRedner = extractRednerFromRednerNode(redeNode);
                        }
                    }

                    mongoBaseSpeech.setId(node.attr("id"));
                    mongoBaseSpeech.setBegin(0);
                    mongoBaseSpeech.setEnd(gesamterText.length());
                    mongoBaseSpeech.setSpeechSections(Arrays.stream(sectionArray).toList());

                    MongoAgenda mongoAgenda = new MongoAgenda();
                    mongoAgenda.setTitleTop(hashmapOfTops.get(topKey));
                    mongoBaseSpeech.setAgenda(mongoAgenda);

                    returnList.add(mongoBaseSpeech);
                }
            }
        }
        return returnList;
    }

    public static MongoSpeech convertJcasToMongoDoc(JCas jcas, List<Integer> entschuldigteAbgeordnete) {
        MongoSpeech mongoSpeech = new MongoSpeech();
        List<MongoSpeechSection> mongoSpeechSections = new ArrayList<>();
        mongoSpeech.setEntschuldigteAbgeordnete(entschuldigteAbgeordnete);

        for (Annotation annotation : jcas.getAnnotationIndex()) {
            if (annotation.getType().getShortName().equals("Speech")){
                mongoSpeech.setId(((Speech) annotation).getId());
                mongoSpeech.setBegin(annotation.getBegin());
                mongoSpeech.setEnd(annotation.getEnd());
                mongoSpeech.setSofaString(annotation.getSofa().getLocalStringData());

            } else if (annotation.getType().getShortName().equals("Agenda")) {
                MongoAgenda mongoAgenda = new MongoAgenda();
                mongoAgenda.setTitleTop(((Agenda) annotation).getTitle());
                mongoSpeech.setAgenda(mongoAgenda);

                mongoSpeech.setDatum(((Agenda) annotation).getProtocol().getDate());
                mongoSpeech.setWahlperiode(((Agenda) annotation).getProtocol().getElectionPeriod());
                mongoSpeech.setSitzungsNr(((Agenda) annotation).getProtocol().getSessionNumber());

            } else if (annotation.getType().getShortName().equals("SpeechSection")) {
                MongoSpeechSection mongoSpeechSection = new MongoSpeechSection();
                mongoSpeechSection.setType(((SpeechSection) annotation).getTextType());
                mongoSpeechSection.setBegin(annotation.getBegin());
                mongoSpeechSection.setEnd(annotation.getEnd());

                MongoSpeaker mongoSpeaker = new MongoSpeaker();

                if (((SpeechSection) annotation).getTextType().equals("text")) {
                    Speaker speaker = ((SpeechSection) annotation).getSpeaker();

                    mongoSpeaker.setFristName(speaker.getFirstName());
                    mongoSpeaker.setLastName(speaker.getLastName());
                    mongoSpeaker.setId(speaker.getId());
                    mongoSpeaker.setParty(speaker.getParty());
                }
                mongoSpeechSection.setSpeaker(mongoSpeaker);
                mongoSpeechSections.add(mongoSpeechSection);
            }
        }
        mongoSpeech.setSpeechSections(mongoSpeechSections);
        return mongoSpeech;
    }

    private static MongoSpeechSection[] saveSpeechSection(MongoSpeaker aktuellerRedner,
                                          StringBuilder speakerText,
                                          StringBuilder gesamterText,
                                          MongoSpeechSection[] sectionArray) {

        MongoSpeaker speaker = copyRedner(aktuellerRedner);

        MongoSpeechSection mongoSpeechSection = new MongoSpeechSection();

        mongoSpeechSection.setSpeaker(speaker);
        mongoSpeechSection.setText(speakerText.toString());
        mongoSpeechSection.setType("text");
        mongoSpeechSection.setBegin(gesamterText.length());
        gesamterText.append(speakerText.toString());
        mongoSpeechSection.setEnd(gesamterText.length());

        sectionArray = addToArray(sectionArray, mongoSpeechSection);

        return sectionArray;
    }

    public static double convertDate(String dateString) {
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");

        LocalDate localDate = LocalDate.parse(dateString, formatter);

        long unixTimestamp = localDate
                .atStartOfDay(ZoneId.of("UTC"))
                .toEpochSecond();

        return (double) unixTimestamp;
    }

    private static MongoSpeaker extractRednerFromRednerNode(Node node) {
        MongoSpeaker mongoSpeaker = new MongoSpeaker();

        if (node.attr("klasse").equals("redner")){
            for (Node subnode : node.childNodes()) {
                if (subnode.nodeName().equals("redner")){
                    mongoSpeaker.setId(subnode.attr("id"));
                    for (Node subsubnode : subnode.childNodes()) {
                        if (subsubnode.nodeName().equals("name")) {
                            for (Node daten : subsubnode.childNodes()) {
                                if (daten.nodeName().equals("vorname")) {
                                    if (daten instanceof Element element) {
                                        mongoSpeaker.setFristName(element.text());
                                    }
                                }
                                if (daten.nodeName().equals("nachname")) {
                                    if (daten instanceof Element element) {
                                        mongoSpeaker.setLastName(element.text());
                                    }
                                }
                                if (daten.nodeName().equals("fraktion")) {
                                    if (daten instanceof Element element) {
                                        mongoSpeaker.setParty(element.text());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (node.nodeName().equals("name")) {
            if (node instanceof Element element) {
                String[] s = element.text().split(" ");
                String vorname = s[1].trim();
                String nachname = s[2].trim();
                mongoSpeaker.setFristName(vorname);
                mongoSpeaker.setLastName(nachname);
            }
        }
        return mongoSpeaker;
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
