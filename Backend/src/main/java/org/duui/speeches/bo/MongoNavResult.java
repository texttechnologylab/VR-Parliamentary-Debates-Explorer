package org.duui.speeches.bo;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.List;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MongoNavResult {
    private Integer id;
    private List<Titles> titles;

    @Getter
    @Setter
    private class Titles {
        private String title;
        private List<String> ids;
    }
}
