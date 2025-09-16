package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoSpeechSection {
    private Integer begin;
    private Integer end;
    private String type;
    private String text;
    private MongoSpeaker speaker;

    @Override
    public String toString() {
        String spaces = "        ";
        return "MongoSpeechSection{" + "\n" +
                spaces + "begin=" + begin + "\n" +
                spaces + ", end=" + end + "\n" +
                spaces + ", type='" + type + '\'' + "\n" +
                spaces + ", text='" + text + '\'' + "\n" +
                spaces + ", speaker=" + speaker + "\n" +
                '}';
    }
}
