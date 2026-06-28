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
@DiscriminatorValue("category name changed")
public class CategoryNameChangedEvent extends CategoryChangeEventBase {
	@Column(name = "new_string_value")
	private String newParentName;

	@Column(name = "prev_string_value")
	private String previousName;

	private CategoryNameChangedEvent() {
		// For Hibernate's use
	}

	public CategoryNameChangedEvent(User user, Category category,
			String previousName) {
		super(user.getId(), category, new Date());
		this.newParentName = category.getName();
		this.previousName = previousName;
	}

	public Message getDescription() {
		return new Message("projectevent.category.nameChanged", previousName,
				newParentName);
	}

	public boolean isCapableOfReverting() {
		Category category = getCategory();
		return isCategoryNameReversable(category);
	}

	@Override
	public void revertChanges(User user) throws Exception {
		Category category = getCategory();
		if (!isCategoryNameReversable(category))
			return;

		ProjectLibraryService.updateCategory(user, category.getId(), previousName,
				category.getDescription(), category.getParent() == null ? null
						: category.getParent().getId());
	}

	/**
	 * @param category
	 * @return
	 */
	private boolean isCategoryNameReversable(Category category) {
		return category != null && !category.getName().equals(previousName);
	}

	@Override
	public Message getReversionDescription() {
		return new Message("projectevent.category.nameChanged.reverted",
				previousName, newParentName);
	}
}
