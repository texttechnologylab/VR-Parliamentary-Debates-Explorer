package org.duui.abgeordnete.service;

import org.duui.abgeordnete.bo.Abgeordneter;
import org.duui.abgeordnete.bo.SubAbgeordneter;
import org.duui.abgeordnete.dao.AbgeordneterDAO;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.HashSet;
import java.util.List;
import java.util.Optional;

@Service
public class AbgeordneterService {
    private final AbgeordneterDAO abgeordneterDAO;

    @Autowired
    public AbgeordneterService(AbgeordneterDAO abgeordneterDAO) {
        this.abgeordneterDAO = abgeordneterDAO;
    }

    public void uploadAbgeordneter(Abgeordneter abgeordneter) {
        abgeordneterDAO.save(abgeordneter);
    }

    public HashSet<String> getAllAbgeordnetenIds() {
        List<String> ids = abgeordneterDAO.findAllIds();
        return new HashSet<>(ids);
    }

    public SubAbgeordneter getIdOfAbgeordneterByNameAndParty(String abgeordneterName, String party) {
        Optional<SubAbgeordneter> abgeordnetenId = abgeordneterDAO.getAbgeordneterByNameContainingAndParteiContaining(abgeordneterName, party);
        return abgeordnetenId.orElse(null);
    }

    public SubAbgeordneter getAbgeordneterByNameAndParty(String abgeordneterName, String party) {
        Optional<SubAbgeordneter> result = abgeordneterDAO.getAbgeordneterByNameContainingAndParteiContaining(abgeordneterName, party);
        return result.orElse(null);
    }

    public Abgeordneter getAbgeordneter(String id) {
        Optional<Abgeordneter> byId = abgeordneterDAO.findAbgeordneterById(id);
        return byId.orElse(null);
    }

    public List<Abgeordneter> getAllAbgeordnete() {
        return abgeordneterDAO.findAll();
    }

}
