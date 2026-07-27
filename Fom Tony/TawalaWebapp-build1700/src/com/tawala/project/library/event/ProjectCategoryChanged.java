package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("project category changed")
public class ProjectCategoryChanged extends ProjectChangeEventBase {
    @Column(name = "prev_category_id")
    private long previousCategoryId;

    @Column(name = "prev_string_value", length = 255)
    private String previousName;

    @Column(name = "new_string_value", length = 255)
    private String newName;

    private ProjectCategoryChanged() {
        // For Hibernate's use
    }

    public ProjectCategoryChanged(String userId, long projectId,
            Category previousCategory, Category newCategory) {
        super(userId, projectId, new Date());
        this.previousName = convertCategoryToString(previousCategory);
        this.previousCategoryId = previousCategory.getId();
        this.newName = convertCategoryToString(newCategory);
    }

    private static String convertCategoryToString(Category category) {
        StringBuffer buffer = new StringBuffer();
        for (Category parent : category.getAllParents()) {
            buffer.append(parent.getName()).append(" -> ");
        }
        buffer.append(category.getName());
        return buffer.toString();
    }

    public Message getDescription() {
        return new Message("projectevent.category.changed", previousName,
                newName);
    }

    public boolean isCapableOfReverting() {
        Category category = ProjectLibraryService.findCategoryById(previousCategoryId);
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        return category != null && project != null && !category.equals(project.getCategory());
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        Category category = ProjectLibraryService.findCategoryById(previousCategoryId);
        project.setCategory(category);

        ProjectLibraryService.onProjectUpdate(project, user);
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.category.change.reverted",
                previousName);
    }
}
