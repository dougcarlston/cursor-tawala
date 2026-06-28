package com.tawala.web.library;

import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;

import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibraryService;

public abstract class AddVersionForm {
	private String projectName;
	private String versionDescription;
	private LibraryProjectVersion projectVersion;
	private UserProject userProject;
	private LibraryProject libraryProject;
	private String userId;

	/**
	 * @return Returns the libraryProject.
	 */
	public LibraryProject getLibraryProject() {
		return libraryProject;
	}

	/**
	 * @param libraryProject
	 *            The libraryProject to set.
	 */
	public void setLibraryProject(LibraryProject libraryProject) {
		this.libraryProject = libraryProject;
	}

	public AddVersionForm(String userId, UserProject userProject) {
		this.userProject = userProject;
		this.userId = userId;
	}

	public LibraryProjectVersion getProjectVersion() {
		if (projectVersion == null) {
			projectVersion = new LibraryProjectVersion(getLibraryProject(),
					userId, userProject.getProject().makeCopy());
			projectVersion.setText(getVersionDescription());
		}
		return projectVersion;
	}

	abstract long getLibraryProjectId();

	/**
	 * @return Returns the projectName.
	 */
	public String getProjectName() {
		return projectName;
	}

	/**
	 * @param projectName
	 *            The projectName to set.
	 */
	public void setProjectName(String projectName) {
		this.projectName = projectName;
	}

	/**
	 * @return Returns the deployedProject.
	 */
	public UserProject getUserProject() {
		return userProject;
	}

	public static class Validator implements
			org.springframework.validation.Validator {
		public boolean supports(Class clazz) {
			return AddVersionForm.class.isAssignableFrom(clazz);
		}

		public void validate(Object obj, Errors errors) {
			AddVersionForm form = (AddVersionForm) obj;
			if (form.getLibraryProjectId() == 0) {
				errors.rejectValue("libraryProjectId",
						"submitted.project.version.project.not.selected");
			} else {
				LibraryProject project = ProjectLibraryService
						.findProjectById(form.getLibraryProjectId());
				if (project == null)
					throw new IllegalStateException(
							"Unable to find library project by id '"
									+ form.getLibraryProjectId() + "'.");
				form.setLibraryProject(project);
			}

			if (!errors.hasErrors()) {
				if (form.getLibraryProject().isVetted()
						&& !form.getUserProject().getUser().isAdministrator()) {
					errors
							.reject("submitted.project.version.not.allowed.to.update.vetted");
				}
			}
			ValidationUtils.rejectIfEmpty(errors, "versionDescription",
					"submitted.project.version.text.empty");

		}
	}

	public String getVersionDescription() {
		return versionDescription;
	}

	public void setVersionDescription(String versionDescription) {
		this.versionDescription = versionDescription;
	}
}
