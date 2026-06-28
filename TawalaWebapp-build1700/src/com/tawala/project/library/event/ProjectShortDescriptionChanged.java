package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project short desc changed")
public class ProjectShortDescriptionChanged extends ProjectChangeEventBase {
    @Column(name = "prev_string_value", length = 255)
    private String previousDescription;

    @Column(name = "new_string_value", length = 255)
    private String newDescription;

    private ProjectShortDescriptionChanged() {
        // For Hibernate's use
    }

    public ProjectShortDescriptionChanged(String userId, long projectId,
            String previousDescription, String newDescription) {
        super(userId, projectId, new Date());
        this.previousDescription = previousDescription;
        this.newDescription = newDescription;
    }

    public Message getDescription() {
        return new Message("projectevent.short.description.changed",
                previousDescription, newDescription);
    }

    public boolean isCapableOfReverting() {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        return project != null
                && !project.getShortDescription().equals(previousDescription);
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        project.setShortDescription(previousDescription);

        ProjectLibraryService.onProjectUpdate(project, user);
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.short.description.changed.reverted",
                previousDescription);
    }
}
