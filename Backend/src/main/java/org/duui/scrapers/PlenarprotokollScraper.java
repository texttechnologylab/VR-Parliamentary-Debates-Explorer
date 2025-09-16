package org.duui.scrapers;

import org.duui.abgeordnete.bo.SubAbgeordneter;
import org.duui.scrapers.bo.MongoBaseSpeech;
import org.duui.scrapers.service.MongoBaseSpeechService;
import org.duui.abgeordnete.service.AbgeordneterService;
import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.Element;
import org.jsoup.select.Elements;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.util.*;

@Component
public class PlenarprotokollScraper {

    private static final String ROOT_URL = "https://www.bundestag.de";
    private static final String START_URL = "/ajax/filterlist/de/services/opendata/543410-543410?limit=10&noFilterSet=true&offset=";
    private final AbgeordneterService abgeordneterService;
    private final MongoBaseSpeechService mongoBaseSpeechService;

    private Integer wahlperiode = 20;

    @Autowired
    public PlenarprotokollScraper(AbgeordneterService abgeordneterService, MongoBaseSpeechService mongoBaseSpeechService) throws Exception {
        this.abgeordneterService = abgeordneterService;
        this.mongoBaseSpeechService = mongoBaseSpeechService;
    }

    public void startScraping(Integer wahlperiode) throws Exception {
        this.wahlperiode = wahlperiode;
        Document plenarprotokolle = getRequest(START_URL + "0");
        Integer offsetMax = getOffsetMax(plenarprotokolle);
        for (int offset = 10; offset <= offsetMax; offset += 10){
            scrapeProtokolle(plenarprotokolle);
            System.out.println("startScraping Loop");
            plenarprotokolle = getRequest(START_URL + String.valueOf(offset));
        }
    }

    private static Integer getOffsetMax(Document plenarprotokolle){
        Element strong = plenarprotokolle.selectFirst("strong");
        String text = strong.text();
        String sitzungsnummertext = text.split("\\.")[0];
        return Integer.valueOf(sitzungsnummertext.replaceAll("[a-zA-Z\s]", ""));
    }

    private static Document getRequest(String url) throws IOException {

        if(!url.contains(ROOT_URL)){
            url = ROOT_URL + url;
        }

        System.out.println(url);

        return Jsoup.connect(url)
                .userAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36")
                .header("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8")
                .header("Accept-Encoding", "gzip, deflate, br")
                .header("Accept-Language", "en-US,en;q=0.9,de;q=0.8")
                .header("Connection", "keep-alive")
                .header("Upgrade-Insecure-Requests", "1")
                .get();
    }

    private void scrapeProtokolle(Document plenarprotokolle) throws Exception {
        Elements listItems = plenarprotokolle.select("li");
        for (Element li : listItems) {
            Element a = li.selectFirst("a");
            if (a != null) {

                String href = a.attr("href");
                String[] split = href.split("/");
                String s = split[split.length - 1];
                String replace_ = s.replace(".xml", "");
                String replace = replace_.substring(String.valueOf(wahlperiode).length());

                if (mongoBaseSpeechService.isAlreadyInDatabase(String.valueOf(this.wahlperiode), replace)){
                    System.out.println("Skipping Wahlperiode: " + wahlperiode + ", SitzungsNr: " + replace);
                    continue;
                }

                Document sitzung = getRequest(href);

                HashMap<String, String> hashmapOfTops = getHashmapOfTops(sitzung);

                List<String> entschuldigteAbgeordnete = getEntschuldigteAbgeordnete(sitzung);
                List<MongoBaseSpeech> speeches = ProcessProtokoll.getSpeeches(sitzung, entschuldigteAbgeordnete, hashmapOfTops);
                try {
                    mongoBaseSpeechService.insertUnprocessedDocuments(speeches);
                    System.out.println("Ging ohne Probleme");
                } catch (Exception e) {
                    mongoBaseSpeechService.insertSave(speeches);
                    System.out.println("Fehler gehabt");
                }

                System.out.println("Wahlperiode: " + wahlperiode + ", SitzungsNr: " + replace);

            }
        }
    }

    private HashMap<String, String> getHashmapOfTops(Document document) {
        HashMap<String, String> alleTops = new HashMap<>();

        for (Element ivzBlock : document.select("ivz-block")) {
            Element element = ivzBlock.selectFirst("ivz-block-titel");
            if (element != null) {
                String text = element.text();
                text = text.replaceAll(":", "");
                Element eintrag = ivzBlock.selectFirst("ivz-eintrag");
                if (eintrag != null) {
                    Element inhalt = eintrag.selectFirst("ivz-eintrag-inhalt");
                    if (inhalt != null) {
                        String topic = inhalt.text();
                        if (topic != null) {
                            alleTops.put(text, topic);
                        }
                    }
                }
            }
        }

        return alleTops;
    }

    private List<String> getEntschuldigteAbgeordnete(Document doc){
        List<String> entschuldigteAbgeordneteIds = new ArrayList<>();
        Element anlagen = doc.selectFirst("anlagen");
        if (anlagen != null) {
            Element table = anlagen.selectFirst("table");
            if (table != null) {
                Elements tr = table.select("tr");
                if (tr != null) {
                    for (Element trElement : tr) {
                        Elements td = trElement.select("td");
                        if (td.size() == 2) {
                            String name = td.get(0).text();
                            String partei = td.get(1).text();

                            SubAbgeordneter subAbgeordneter = abgeordneterService.getIdOfAbgeordneterByNameAndParty(name, partei);

                            if (subAbgeordneter != null) {
                                String idOfAbgeordneterByNameAndParty = subAbgeordneter.getId();
                                if (idOfAbgeordneterByNameAndParty != null) {
                                    entschuldigteAbgeordneteIds.add(idOfAbgeordneterByNameAndParty);
                                }
                            } else {
                                System.out.println(name + " : " + partei);
                                System.out.println("Abgeordneter nicht gefunden");
                            }
                        }
                    }
                }
            }

        }


        return entschuldigteAbgeordneteIds;
    }

}
