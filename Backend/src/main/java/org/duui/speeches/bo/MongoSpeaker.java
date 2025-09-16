package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoSpeaker {
    private String fristName;
    private String lastName;
    private String id;
    private String party;

    @Override
    public String toString() {
        String spaces = "                ";
        return "MongoSpeaker{" + "\n" +
                spaces + "fristName='" + fristName + '\'' + "\n" +
                spaces + ", lastName='" + lastName + '\'' + "\n" +
                spaces + ", id=" + id + "\n" +
                spaces + ", party='" + party + '\'' + "\n" +
                spaces + '}';
    }
}
