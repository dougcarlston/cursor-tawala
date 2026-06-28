package com.tawala.project.data;

import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;

public class RegularProjectFormSubmissionCreator implements
		FormSubmissionCreator {
	private final UserProject userProject;
	private final Form form;
	
	public RegularProjectFormSubmissionCreator(UserProject userProject, Form form) {
		this.userProject = userProject;
		this.form = form;
	}

	public FormSubmission create() {
		return new FormSubmission(userProject, form);
	}

	public FieldSetter getFieldSetter(String fieldId) {
		Field field = form.getFieldById(fieldId);
		if(field == null) {
			throw new IllegalStateException("Unable to find field by id '" + fieldId + "'");
		}
		return field.getFieldSetter();
	}
}
