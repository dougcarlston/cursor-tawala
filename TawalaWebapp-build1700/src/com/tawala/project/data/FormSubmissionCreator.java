package com.tawala.project.data;

import com.tawala.project.FormSubmission;

public interface FormSubmissionCreator {
	FormSubmission create();
	FieldSetter getFieldSetter(String fieldId);
}
