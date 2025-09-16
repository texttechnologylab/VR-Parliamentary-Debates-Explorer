//package org.duui.scrapers;
//
//import org.duui.abgeordnete.bo.Abgeordneter;
//import org.duui.abgeordnete.service.AbgeordneterService;
//import org.jsoup.Jsoup;
//import org.jsoup.nodes.Document;
//import org.jsoup.nodes.Element;
//import org.jsoup.select.Elements;
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.stereotype.Component;
//
//import java.io.*;
//import java.net.HttpURLConnection;
//import java.net.URL;
//import java.util.HashSet;
//
//@Component
//public class AbgeordneteScraper {
//
//    private final AbgeordneterService abgeordnetenService;
//    private static final String ROOT_URL = "https://www.bundestag.de";
//
//    @Autowired
//    public AbgeordneteScraper(AbgeordneterService abgeordnetenService) throws Exception {
//        this.abgeordnetenService = abgeordnetenService;
//        startScraping();
//    }
//
//    public void startScraping() throws Exception {
//        startScraping(false);
//    }
//
//    public void startScraping(boolean segmentFace) throws Exception {
//        HashSet<String> allAbgeordnetenIds = abgeordnetenService.getAllAbgeordnetenIds();
//
//        String abgeordneteListeWP21 = "/ajax/filterlist/de/abgeordnete/1040594-1040594?limit=9999&view=BTBiographyList";
//
//        Document abgordnetenHtml = Jsoup.connect(ROOT_URL+abgeordneteListeWP21)
//                .userAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36")
//                .header("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8")
//                .header("Accept-Encoding", "gzip, deflate, br")
//                .header("Accept-Language", "en-US,en;q=0.9,de;q=0.8")
//                .header("Connection", "keep-alive")
//                .header("Upgrade-Insecure-Requests", "1")
//                .get();
//
//        Elements listItems = abgordnetenHtml.select("li");
//        for (Element li : listItems) {
//            Element a = li.selectFirst("a");
//            if (a != null) {
//                String name = a.attr("title");
//                String href = a.attr("href");
//                String dataId = a.attr("data-id");
//
//                Element p = a.selectFirst("p");
//                String partei = (p != null) ? p.text().replace("\n", "") : "";
//
//                if (!allAbgeordnetenIds.contains(dataId)) {
//
//                    if(!href.contains(ROOT_URL)){
//                        href = ROOT_URL + href;
//                    }
//
//                    Document resp = Jsoup.connect(href)
//                            .userAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36")
//                            .header("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8")
//                            .header("Accept-Encoding", "gzip, deflate, br")
//                            .header("Accept-Language", "en-US,en;q=0.9,de;q=0.8")
//                            .header("Connection", "keep-alive")
//                            .header("Upgrade-Insecure-Requests", "1")
//                            .get();
//
//                    byte[] abgeordnetenBild = getAbgeordnetenBild(resp);
//
//                    Abgeordneter abgeordneter = new Abgeordneter(
//                            dataId,
//                            name,
//                            getAbgeordneterBeruf(resp),
//                            partei,
//                            getAbgeordnetenBiografie(resp),
//                            abgeordnetenBild
//                    );
//
//                    System.out.printf("Neuer Abgeordneter mit ID %s hinzugefügt\n", dataId);
//
//                    this.abgeordnetenService.uploadAbgeordneter(abgeordneter);
//
//                } else {
//                    System.out.printf("Abgeordneter mit id %s bereits in der Datenbank enthalten\n", dataId);
//                }
//            }
//        }
//    }
//
//    private static String getAbgeordneterBeruf(Document doc){
//        Element berufDiv = doc.selectFirst("div.bt-biografie-beruf");
//
//        if (berufDiv != null) {
//            return berufDiv.text();
//        } else {
//            return "";
//        }
//    }
//
//    private static String getAbgeordnetenBiografie(Document doc) {
//        Element bioDiv = doc.selectFirst("div.m-biography__biography");
//
//        if (bioDiv != null) {
//            return bioDiv.text();
//        }
//        return "";
//    }
//
//    private static byte[] getAbgeordnetenBild(Document resp) throws IOException {
//        Element imgDiv = resp.selectFirst("div.e-image__wrapper.--rounded");
//
//        if (imgDiv != null) {
//            Element img = imgDiv.selectFirst("img");
//            if (img != null) {
//                String imgUrl = img.attr("data-img-md-normal");
//                String keyword = "/resource";
//                int index = imgUrl.indexOf(keyword);
//                if (index != -1) {
//                    imgUrl = imgUrl.substring(index);
//                } else {
//                    imgUrl = "";
//                    System.out.println("Keyword nicht gefunden.");
//                }
//
//                if (!imgUrl.isEmpty()) {
//                    URL imageUrl = new URL(ROOT_URL + imgUrl);
//                    HttpURLConnection connection = (HttpURLConnection) imageUrl.openConnection();
//                    connection.setRequestMethod("GET");
//
//                    if (connection.getResponseCode() == HttpURLConnection.HTTP_OK) {
//                        try (InputStream in = connection.getInputStream();
//                             ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
//                            byte[] imageFile = new byte[1024];
//                            int bytesRead;
//                            while ((bytesRead = in.read(imageFile)) != -1) {
//                                baos.write(imageFile, 0, bytesRead);
//                            }
//                            System.out.println("baos.size() = " + baos.size());
//                            System.out.println("Bild erfolgreich gespeichert");
//                            return baos.toByteArray();
//                        }
//                    } else {
//                        System.err.println("Fehler beim Abrufen des Bildes: " + connection.getResponseCode());
//                    }
//                }
//            }
//        }
//        System.out.println("return null bild");
//        return null;
//    }
//
//}
