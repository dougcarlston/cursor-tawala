package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.message.Message;
import com.tawala.project.library.LibraryProjectVersion;

@Entity
@DiscriminatorValue("project version added")
public class ProjectVersionAdded extends ProjectChangeEventBase {
    @Column(name = "version_number")
    private int versionNumber;

    private ProjectVersionAdded() {
        // For Hibernate's use
    }

    public ProjectVersionAdded(long projectId, LibraryProjectVersion version) {
        super(version.getUserId(), projectId, new Date());
        this.versionNumber = version.getVersionNumber();
    }

    public Message getDescription() {
        return new Message("projectevent.version.added", versionNumber);
    }

    public boolean isCapableOfReverting() {
        return false;
    }
}
