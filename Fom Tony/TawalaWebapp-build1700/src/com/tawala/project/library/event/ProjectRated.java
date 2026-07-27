package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.message.Message;
import com.tawala.project.library.Rating;

// TODO: remove it once it becomes really clear that it's not needed. 
// All the records in the database will have to be purged; otherwise Hibernate will throw an exception 

@Entity
@DiscriminatorValue("project rated")
public class ProjectRated extends ProjectChangeEventBase {
    private ProjectRated() {
        // For Hibernate's use
    }

    public ProjectRated(long projectId, Rating rating) {
        super(rating.getUserId(), projectId, new Date());
    }

    public Message getDescription() {
        return new Message("projectevent.project.rated");
    }

    public boolean isCapableOfReverting() {
        return false;
    }
}
