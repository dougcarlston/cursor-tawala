package com.tawala.project.library;

import java.beans.PropertyEditorSupport;

public class CategoryEditor extends PropertyEditorSupport {
	public static String TOP_LEVEL_CATEGORY_PREFIX = "toplevelcategory";

	/*
	 * (non-Javadoc)
	 * 
	 * @see java.beans.PropertyEditorSupport#getAsText()
	 */
	@Override
	public String getAsText() {
		Category category = (Category) getValue();
		return String.valueOf(category == null ? "" : category.getId());
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see java.beans.PropertyEditorSupport#setAsText(java.lang.String)
	 */
	@Override
	public void setAsText(String text) throws IllegalArgumentException {
		if (text == null || text.length() == 0)
			return;

		try {
			if (text.startsWith(TOP_LEVEL_CATEGORY_PREFIX)) {
				int libraryId = Integer.parseInt(text
						.substring(TOP_LEVEL_CATEGORY_PREFIX.length()));
				setValue(new Category.TopLevelCategory(ProjectLibrary.getLibraryById(libraryId)));
			} else {
				long id = Long.parseLong(text);
				Category category = ProjectLibraryService.findCategoryById(id);
				if (category == null)
					throw new IllegalArgumentException(text);
				setValue(category);
			}
		} catch (NumberFormatException e) {
			throw new IllegalArgumentException(text);
		}
	}
}
