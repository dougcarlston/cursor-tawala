package com.tawala.project.data;

import com.tawala.domain.User;
import com.tawala.project.FormSubmission;

public class SharedDataSourceFormSubmissionCreator implements
		FormSubmissionCreator {
	private final User user;
	private final DataSource dataSource;
	
	public SharedDataSourceFormSubmissionCreator(User user, DataSource dataSource) {
		this.user = user;
		this.dataSource = dataSource;
	}

	public FormSubmission create() {
		return new FormSubmission(user, dataSource);
	}

	public FieldSetter getFieldSetter(String fieldId) {
		StoredField field = dataSource.getFieldById(fieldId);
		if(field == null) {
			throw new IllegalStateException("Unable to find field by id '" + fieldId + "'");
		}
		return field.getFieldSetter();
	}

}
