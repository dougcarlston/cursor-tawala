package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;

import com.tawala.project.library.ProjectChangeEvent;

@Entity
abstract public class ProjectChangeEventBase extends LibraryChangeEventBase implements ProjectChangeEvent {
    @Column(name = "lib_project_id")
    private long projectId;
    
    protected ProjectChangeEventBase() {
        // For Hibernate's use
    }

    public ProjectChangeEventBase(String userId, long projectId, Date date) {
        super(userId, date);
        this.projectId = projectId;
    }

    public final long getProjectId() {
        return projectId;
    }
    
    public boolean isProjectRelated() {
        return true;
    }
}
