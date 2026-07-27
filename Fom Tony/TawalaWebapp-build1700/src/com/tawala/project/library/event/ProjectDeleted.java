package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project deleted")
public class ProjectDeleted extends ProjectChangeEventBase {

    private ProjectDeleted() {
        // For Hibernate's use
    }

    public ProjectDeleted(LibraryProject project, String userId) {
        super(userId, project.getId(), new Date());
    }

    public Message getDescription() {
        return new Message("projectevent.project.deleted");
    }

    public boolean isCapableOfReverting() {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        return project != null && project.isDeleted();
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());

        if (project.isDeleted()) {
            ProjectLibraryService.onRestoreProject(project.getId(), user);
            Log.info(this, "Restored project " + project.getId());
        }
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.project.deleted.reverted");
    }
}
