package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.message.Message;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project restored")
public class ProjectRestored extends ProjectChangeEventBase {

    private ProjectRestored() {
        // For Hibernate's use
    }

    public ProjectRestored(LibraryProject project, String userId) {
        super(userId, project.getId(), new Date());
    }

    public Message getDescription() {
        return new Message("projectevent.project.restored");
    }

    public boolean isCapableOfReverting() {
        return false;
    }
}
