package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibraryService;

@Entity
@DiscriminatorValue("category parent changed")
public class CategoryParentChangedEvent extends CategoryChangeEventBase {
    @Column(name = "prev_category_id")
    private long previousParentId;

    @Column(name = "new_category_id")
    private long newParentId;

    @Column(name = "prev_string_value")
    private String previousParentName;

    @Column(name = "new_string_value")
    private String newParentName;

    private CategoryParentChangedEvent() {
        // For Hibernate's use
    }

    public CategoryParentChangedEvent(User user, Category category,
            Category previousParent) {
        super(user.getId(), category, new Date());
        this.previousParentId = previousParent == null ? 0 : previousParent
                .getId();
        this.previousParentName = previousParent == null ? null
                : previousParent.getFullName();
        this.newParentName = category.getParent() == null ? null : category
                .getParent().getFullName();
        this.newParentId = category.getParent() == null ? 0 : category
                .getParent().getId();
    }

    public Message getDescription() {
        return new Message("projectevent.category.parentChanged",
                getCategoryName(),
                getCategoryNameConvertingNulls(previousParentName),
                getCategoryNameConvertingNulls(newParentName));
    }

    // TODO: this is a hack to avoid multiple messages based on whether a
    // category name is empty.
    private static String getCategoryNameConvertingNulls(String name) {
        return name == null ? "top level category" : name;
    }

    public boolean isCapableOfReverting() {
        Category category = getCategory();
        return isCategoryParentChangeReversable(category);
    }

    @Override
    public void revertChanges(User user) throws Exception {
        Category category = getCategory();
        if (!isCategoryParentChangeReversable(category))
            return;
        
		ProjectLibraryService.updateCategory(user, category.getId(), category.getName(),
				category.getDescription(), previousParentId);
    }

    private boolean isCategoryParentChangeReversable(Category category) {
        return category != null
                && (previousParentId == 0 || ProjectLibraryService
                        .findCategoryById(previousParentId) != null)
                && (category.getParent() == null ? newParentId == 0 : category
                        .getParent().getId() == newParentId);
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.category.parentChanged.reverted",
                getCategoryName(),
                getCategoryNameConvertingNulls(previousParentName),
                getCategoryNameConvertingNulls(newParentName));
    }
}
