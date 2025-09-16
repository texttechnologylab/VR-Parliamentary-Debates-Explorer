package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoSpeech {
    @Id
    private String id;
    private Integer begin;
    private Integer end;
    private String sofaString;
    private MongoAgenda agenda;
    private Integer wahlperiode;
    private Integer sitzungsNr;
    private Double datum;
    private List<MongoSentiment> sentiments;
    private List<MongoSpeechSection> speechSections;
    private List<MongoSentenceEmbedding> embeddings;
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
                ", sentiments=" + sentiments + "\n" +
                ", embeddings=" + embeddings + "\n" +
                ", entschuldigteAbgeordnete=" + entschuldigteAbgeordnete + "\n" +
                '}';
    }
}
