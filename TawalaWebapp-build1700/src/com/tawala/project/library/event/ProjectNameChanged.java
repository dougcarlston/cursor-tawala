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
@DiscriminatorValue("project name changed")
public class ProjectNameChanged extends ProjectChangeEventBase {
    @Column(name = "prev_string_value", length = 255)
    private String previousName;

    @Column(name = "new_string_value", length = 255)
    private String newName;

    private ProjectNameChanged() {
        // For Hibernate's use
    }

    public ProjectNameChanged(String userId, long projectId,
            String previousName, String newName) {
        super(userId, projectId, new Date());
        this.previousName = previousName;
        this.newName = newName;
    }

    public Message getDescription() {
        return new Message("projectevent.name.changed", previousName, newName);
    }

    public boolean isCapableOfReverting() {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        return project != null && !project.getName().equals(previousName);
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        project.setName(previousName);
        ProjectLibraryService.onProjectUpdate(project, user);
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.name.changed.reverted", previousName);
    }
}
