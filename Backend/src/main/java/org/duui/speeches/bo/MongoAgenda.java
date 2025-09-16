package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoAgenda {
    private String titleRede;
    private String titleTop;

    @Override
    public String toString() {
        String spaces = "        ";
        return "MongoAgenda{" +
                spaces + "titleRede='" + titleRede + '\'' + "\n" +
                spaces + ", titleTop='" + titleTop + '\'' + "\n" +
                spaces + '}';
    }
}
