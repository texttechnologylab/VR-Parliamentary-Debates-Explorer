package org.duui.scrapers.controller;

import org.duui.scrapers.PlenarprotokollScraper;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping(path = "/BaseSpeeches")
public class MongoBaseSpeechController {

    private PlenarprotokollScraper plenarprotokollScraper;

    public MongoBaseSpeechController(PlenarprotokollScraper plenarprotokollScraper) {
        this.plenarprotokollScraper = plenarprotokollScraper;
    }

    @GetMapping("/scrape")
    public String scrape(@RequestParam("wp") Integer wp) throws Exception {
        plenarprotokollScraper.startScraping(wp);
        return "Scraping";
    }
}
