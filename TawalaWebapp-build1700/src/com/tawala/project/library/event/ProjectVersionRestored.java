package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.message.Message;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project version restored")
public class ProjectVersionRestored extends ProjectChangeEventBase {
    @Column(name = "version_user_id", length = 20)
    private String versionUserId;

    @Column(name = "version_number")
    private int versionNumber;

    private ProjectVersionRestored() {
        // For Hibernate's use
    }

    public ProjectVersionRestored(LibraryProject project, String userId,
            LibraryProjectVersion version) {
        super(userId, project.getId(), new Date());
        this.versionNumber = version.getVersionNumber();
        this.versionUserId = version.getUserId();
    }

    public Message getDescription() {
        return new Message("projectevent.version.restored", versionNumber,
                versionUserId);
    }

    public boolean isCapableOfReverting() {
        return false;
    }
}
