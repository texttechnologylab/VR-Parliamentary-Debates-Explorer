package org.duui.abgeordnete.bo;

import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.springframework.data.mongodb.core.mapping.Document;

@Document(collection = "abgeordnete")  // Name der MongoDB-Sammlung
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class Abgeordneter {
    @Id
    private String id;
    private String name;
    private String beruf;
    private String partei;
    private String biografie;
    private byte[] image;
}
