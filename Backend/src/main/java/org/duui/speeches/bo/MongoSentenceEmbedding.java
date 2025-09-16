package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.Arrays;
import java.util.List;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoSentenceEmbedding {

    private Integer begin;
    private Integer end;
    private List<Float> floats;

    @Override
    public String toString() {
        String spaces = "        ";
        return "MongoSentenceEmbedding{" + "\n" +
                spaces + "begin=" + begin + "\n" +
                spaces + ", end=" + end + "\n" +
//                spaces + ", floats=" + floats.toString() + "\n" +
                '}';
    }
}
