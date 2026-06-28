package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project version deleted")
public class ProjectVersionDeleted extends ProjectChangeEventBase {
    @Column(name = "lib_version_id")
    private long versionId;

    @Column(name = "version_user_id", length = 20)
    private String versionUserId;

    @Column(name = "version_number")
    private int versionNumber;

    private ProjectVersionDeleted() {
        // For Hibernate's use
    }

    public ProjectVersionDeleted(LibraryProject project, String userId,
            LibraryProjectVersion version) {
        super(userId, project.getId(), new Date());
        this.versionNumber = version.getVersionNumber();
        this.versionUserId = version.getUserId();
        this.versionId = version.getId();
    }

    public Message getDescription() {
        return new Message("projectevent.version.deleted", versionNumber,
                versionUserId);
    }

    public boolean isCapableOfReverting() {
        LibraryProjectVersion version = ProjectLibraryService.findProjectVersionById(versionId);
        return version != null && version.isDeleted();
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        if (project == null)
            return;

        LibraryProjectVersion version = ProjectLibraryService.findProjectVersionById(versionId);
        if (version == null || !version.isDeleted())
            return;

        ProjectLibraryService.onVersionRestoration(project, version, user);
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.version.deleted.reverted",
                versionNumber, versionUserId);
    }
}
