package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoSentiment {
    private Integer begin;
    private Integer end;
    private Double sentiment;

    @Override
    public String toString() {
        String spaces = "        ";
        return "MongoSentiment{" + "\n" +
                spaces + "begin=" + begin + "\n" +
                spaces + ", end=" + end + "\n" +
                spaces + ", sentiment=" + sentiment + "\n" +
                '}';
    }
}
