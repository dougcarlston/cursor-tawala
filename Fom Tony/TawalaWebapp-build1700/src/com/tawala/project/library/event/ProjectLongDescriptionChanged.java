package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;
import javax.persistence.Lob;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project long desc changed")
public class ProjectLongDescriptionChanged extends ProjectChangeEventBase {
    @Column(name = "prev_desc")
    @Lob
    private String previousDescription;

    @Column(name = "new_desc")
    @Lob
    private String newDescription;

    private ProjectLongDescriptionChanged() {
        // For Hibernate's use
    }

    public ProjectLongDescriptionChanged(String userId, long projectId,
            String previousDescription, String newDescription) {
        super(userId, projectId, new Date());
        this.previousDescription = previousDescription;
        this.newDescription = newDescription;
    }

    public Message getDescription() {
        return new Message("projectevent.long.description.changed",
                previousDescription, newDescription);
    }

    public boolean isCapableOfReverting() {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        return project != null
                && !project.getLongDescription().equals(previousDescription);
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        project.setLongDescription(previousDescription);

        ProjectLibraryService.onProjectUpdate(project, user);
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.long.description.changed.reverted",
                previousDescription);
    }
}
