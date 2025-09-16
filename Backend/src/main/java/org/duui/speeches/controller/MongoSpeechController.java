package org.duui.speeches.controller;

import lombok.Getter;
import lombok.Setter;
import org.duui.speeches.bo.MongoDotProduktResult;
import org.duui.speeches.bo.MongoSpeech;
import org.duui.speeches.service.MongoSpeechService;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping(path = "/speeches")
public class MongoSpeechController {

    MongoSpeechService mongoSpeechService;

    public MongoSpeechController(MongoSpeechService mongoSpeechService) {
        this.mongoSpeechService = mongoSpeechService;
    }

    @Getter
    @Setter
    public static class RequestData {
        private Integer maxSitzungsNr;
        private String text;
        private String firstName;
        private String lastName;
    }

    @GetMapping("/getSpeech/{id}")
    public ResponseEntity<?> getSpeech(@PathVariable("id") String id) {
        MongoSpeech speech = mongoSpeechService.getSpeech(id);
        if (speech == null) {
            return new ResponseEntity<>(HttpStatus.NOT_FOUND);
        } else {
            return new ResponseEntity<>(speech, HttpStatus.OK);
        }
    }

    @GetMapping("/getNav")
    public ResponseEntity<?> getNav() {
        return new ResponseEntity<>(mongoSpeechService.getNavigation(), HttpStatus.OK);
    }

    @PostMapping("/getTopSpeeches")
    public ResponseEntity<?> receiveData(@RequestBody RequestData requestData) throws Exception {

        List<MongoDotProduktResult> topSpeeches = mongoSpeechService.getTopSpeeches(requestData.getFirstName(), requestData.getLastName(), requestData.getMaxSitzungsNr(), requestData.getText());

        return new ResponseEntity<>(topSpeeches, HttpStatus.OK);
    }

}
