package org.duui.abgeordnete.dao;

import org.duui.abgeordnete.bo.Abgeordneter;
import org.duui.abgeordnete.bo.SubAbgeordneter;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface AbgeordneterDAO extends MongoRepository<Abgeordneter, Integer> {

    Optional<Abgeordneter> findAbgeordneterById(String id);

//    Optional<Abgeordneter> getAbgeordneterByNameContainingAndParteiContaining(String name, String partei);

//    Boolean existsById(String id);

    Optional<SubAbgeordneter> getAbgeordneterByNameContainingAndParteiContaining(String name, String partei);

    @Query(value = "{}", fields = "{ '_id': 1 }")
    List<String> findAllIds();

}
