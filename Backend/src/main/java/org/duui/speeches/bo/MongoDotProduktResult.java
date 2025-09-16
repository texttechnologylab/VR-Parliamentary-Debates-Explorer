package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoDotProduktResult {
    private String id;
    private String sofaString;
    private String sofaSubstring;
    private MongoSentenceEmbedding embeddings;
    private Double dotProduct;

    @Override
    public String toString() {
        return "MongoDotProduktResult [ \n " +
                "id=" + id + "\n" +
                "sofaString=" + sofaString + "\n" +
                "sofaSubstring=" + sofaSubstring + "\n"
                + "embedding=" + embeddings + "\n"
                + "dotProdukt=" + dotProduct + "\n";
    }
}
