package com.tawala.project.library;

import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;

import com.tawala.domain.DomainMetadata;

public class LibraryProjectValidator implements Validator {

	public boolean supports(Class clazz) {
		return clazz.equals(LibraryProject.class);
	}

	public void validate(Object obj, Errors errors) {
		LibraryProject project = (LibraryProject) obj;

		ValidationUtils.rejectIfEmptyOrWhitespace(errors, "shortDescription",
				"submitted.project.short.description.empty");
		ValidationUtils.rejectIfEmptyOrWhitespace(errors, "category",
				"submitted.project.category.empty");
		ValidationUtils.rejectIfEmptyOrWhitespace(errors, "name",
				"submitted.project.name.empty");

		if (project.getShortDescription().length() > DomainMetadata.instance
				.getLibraryProjectShortDescriptionMaxLength()) {
			errors.rejectValue("shortDescription",
					"submitted.project.short.description.too.long",
					new Object[] { DomainMetadata.instance
							.getLibraryProjectShortDescriptionMaxLength() },
					"Short description is too long");
		}

		if (project.getName().length() > DomainMetadata.instance
				.getLibraryProjectNameMaxLength()) {
			errors.rejectValue("name", "submitted.project.name.too.long",
					new Object[] { DomainMetadata.instance
							.getLibraryProjectNameMaxLength() },
					"Name too long");
		}

		boolean checkForDuplicateName = false;

		if (project.getId() == 0) {
			checkForDuplicateName = true;
		} else {
			LibraryProject unchangedProject = ProjectLibraryService
					.findProjectById(project.getId());
			if (!unchangedProject.getName().equals(project.getName())) {
				checkForDuplicateName = true;
			}
		}
		if (checkForDuplicateName
				&& ProjectLibraryService.projectNameExists(project.getName())) {
			errors
					.rejectValue("name", "submitted.project.duplicate.keys",
							new Object[] { project.getName() },
							"Project with name \"{0}\" already exists.");
		}
	}
}
