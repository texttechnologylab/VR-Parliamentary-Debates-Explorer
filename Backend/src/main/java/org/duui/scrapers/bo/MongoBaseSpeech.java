package org.duui.scrapers.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.duui.speeches.bo.MongoAgenda;
import org.duui.speeches.bo.MongoSpeechSection;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoBaseSpeech {
    @Id
    private String id;
    private Integer begin;
    private Integer end;
    private String sofaString;
    private MongoAgenda agenda;
    private Integer wahlperiode;
    private Integer sitzungsNr;
    private Double datum;
    private List<MongoSpeechSection> speechSections;
    private List<Integer> entschuldigteAbgeordnete;

    @Override
    public String toString() {
        return "MongoSpeech{" + "\n" +
                "id=" + id + "\n" +
                ", begin=" + begin + "\n" +
                ", end=" + end + "\n" +
                ", sofaString=" + sofaString + "\n" +
                ", agenda=" + agenda + "\n" +
                ", wahlperiode=" + wahlperiode + "\n" +
                ", sitzungsNr=" + sitzungsNr + "\n" +
                ", datum=" + datum + "\n" +
                ", speechSection=" + speechSections + "\n" +
                ", entschuldigteAbgeordnete=" + entschuldigteAbgeordnete + "\n" +
                '}';
    }

}
