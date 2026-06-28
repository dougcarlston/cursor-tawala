package com.tawala.project.data;

import com.tawala.project.FormSubmission;

public interface FieldSetter {
	void setField(FormSubmission submission, String rawData);
}
