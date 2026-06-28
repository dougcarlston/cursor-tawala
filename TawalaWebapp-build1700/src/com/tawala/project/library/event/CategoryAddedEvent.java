package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibraryService;

@Entity
@DiscriminatorValue("category added")
public class CategoryAddedEvent extends CategoryChangeEventBase {
    private CategoryAddedEvent() {
        // For Hibernate's use
    }

    public CategoryAddedEvent(Category category, User user) {
        super(user.getId(), category, new Date());
    }

    public Message getDescription() {
        return new Message("projectevent.category.added", getCategoryName());
    }

    public boolean isCapableOfReverting() {
        return isCategoryAdditionReversable(getCategory());
    }

    @Override
    public void revertChanges(User user) throws Exception {
        Category category = getCategory();
        if (!isCategoryAdditionReversable(category))
            return;

        ProjectLibraryService.deleteCategory(category, user);
    }

    /**
     * @param category
     * @return
     */
    private static boolean isCategoryAdditionReversable(Category category) {
        return category != null && category.getProjectCount() == 0;
    }

    @Override
    public Message getReversionDescription() {
        return new Message(
                "projectevent.category.added.reverted", getCategoryName());
    }
}
