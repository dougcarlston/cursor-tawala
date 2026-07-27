package com.tawala.project.data;

import com.tawala.project.FormSubmission;

public class SingleValueFieldSetter extends BaseFieldSetter {

	public SingleValueFieldSetter(String fieldName) {
		super(fieldName);
	}

	public void setField(FormSubmission submission, String rawData) {
		submission.setValue(getFieldName(), rawData);
	}
}
