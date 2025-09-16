package org.duui.abgeordnete.controller;

import org.duui.abgeordnete.bo.Abgeordneter;
import org.duui.abgeordnete.bo.SubAbgeordneter;
import org.duui.abgeordnete.service.AbgeordneterService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.HashSet;

@RestController
@RequestMapping(path = "/abgeordnete")
public class AbgeordnetenController {

    private static final Logger logger = LoggerFactory.getLogger(AbgeordnetenController.class);

    private final AbgeordneterService abgeordneterService;

    @Autowired
    public AbgeordnetenController(AbgeordneterService abgeordneterService) {
        this.abgeordneterService = abgeordneterService;
    }

    @GetMapping("/getAbgeordneterById/{id}")
    public ResponseEntity<?> getAbgeordneterById(@PathVariable("id") String id) {
        logger.info("getAbgeordneterById {}", id);
        Abgeordneter abgeordneter = abgeordneterService.getAbgeordneter(id);

        if (abgeordneter != null) {
            return new ResponseEntity<>(
                    abgeordneter, new HttpHeaders(), HttpStatus.OK);
        } else {
            return new ResponseEntity<>(
                    null, new HttpHeaders(), HttpStatus.NOT_FOUND);
        }
    }

    @GetMapping("/getAbgeordneter")
    public ResponseEntity<?> getAbgeordneterById(@RequestParam("firstName") String firstName,
                                                 @RequestParam("lastName") String lastName,
                                                 @RequestParam("party") String party) {

        String name = "";

        if (firstName == null && lastName == null) {
            return new ResponseEntity<>(
                    "Enter firstName or lastName", new HttpHeaders(), HttpStatus.NOT_FOUND);
        }

        if (firstName == null && lastName != null){
            name = lastName;
        } else if (firstName != null && lastName == null){
            name = firstName;
        } else {
            name = lastName.strip() + ", " + firstName;
        }

        SubAbgeordneter abgeordneter = abgeordneterService.getAbgeordneterByNameAndParty(name, party);

        if (abgeordneter != null) {
            return new ResponseEntity<>(
                    abgeordneter, new HttpHeaders(), HttpStatus.OK);
        } else {
            return new ResponseEntity<>(
                    null, new HttpHeaders(), HttpStatus.NOT_FOUND);
        }
    }

    @GetMapping("/getAll")
    public ResponseEntity<?> getAll() {
        return new ResponseEntity<>(
                abgeordneterService.getAllAbgeordnete(),
                new HttpHeaders(), HttpStatus.OK);
    }

}
