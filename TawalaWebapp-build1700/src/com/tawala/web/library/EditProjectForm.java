package com.tawala.web.library;

import org.springframework.validation.Errors;

import com.tawala.project.UserProject;
import com.tawala.project.library.Category;
import com.tawala.project.library.CategoryValidator;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectValidator;

public class EditProjectForm {
    private LibraryProject project;
    private Category category;
    private long savedCategoryId;
    private String categoryCreationAction;
    private UserProject userProject;

    /**
     * @return Returns the originalProject.
     */
    public UserProject getUserProject() {
        return userProject;
    }

    /**
     * @param originalProject The originalProject to set.
     */
    public void setUserProject(UserProject originalProject) {
        this.userProject = originalProject;
    }

    /**
     * @return Returns the categoryCreationAction.
     */
    public String getCategoryCreationAction() {
        return categoryCreationAction;
    }

    /**
     * @param categoryCreationAction The categoryCreationAction to set.
     */
    public void setCategoryCreationAction(String categoryCreationAction) {
        this.categoryCreationAction = categoryCreationAction;
    }

    /**
     * @return Returns the savedCategoryId.
     */
    public long getSavedCategoryId() {
        return savedCategoryId;
    }

    /**
     * @param savedCategoryId The savedCategoryId to set.
     */
    public void setSavedCategoryId(long savedCategoryId) {
        this.savedCategoryId = savedCategoryId;
    }

    /**
     * @return Returns the category.
     */
    public Category getCategory() {
        return category;
    }

    /**
     * @param category
     *            The category to set.
     */
    public void setCategory(Category category) {
        this.category = category;
    }

    /**
     * @return Returns the project.
     */
    public LibraryProject getProject() {
        return project;
    }

    /**
     * @param project
     *            The project to set.
     */
    public void setProject(LibraryProject project) {
        this.project = project;
    }

    public static class Validator implements
            org.springframework.validation.Validator {

        public boolean supports(Class clazz) {
            return EditProjectForm.class.equals(clazz);
        }

        public void validate(Object obj, Errors errors) {
            EditProjectForm form = (EditProjectForm) obj;

            if (form.getCategoryCreationAction() != null) {
                errors.setNestedPath("category");
                try {
                    new CategoryValidator().validate(form.getCategory(),
                            errors);
                } finally {
                    errors.setNestedPath("");
                }
            } else {
                errors.setNestedPath("project");
                try {
                    new LibraryProjectValidator().validate(form.getProject(),
                            errors);
                } finally {
                    errors.setNestedPath("");
                }
            }
        }
    }
}
