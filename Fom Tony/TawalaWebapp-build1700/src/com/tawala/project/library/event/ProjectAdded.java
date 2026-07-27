package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.message.Message;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project added")
public class ProjectAdded extends ProjectChangeEventBase {
    private static Message message = new Message("projectevent.added");
    
    private ProjectAdded() {
        // For Hibernate's use
    }
    
    public ProjectAdded(LibraryProject project) {
        super(project.getAuthorId(), project.getId(), new Date());
    }
    
    public Message getDescription() {
        return message;
    }

    public boolean isCapableOfReverting() {
        return false;
    }
}
