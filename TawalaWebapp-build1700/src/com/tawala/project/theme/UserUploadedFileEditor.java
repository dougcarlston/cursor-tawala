package com.tawala.project.theme;

import java.beans.PropertyEditorSupport;

import com.tawala.UsersHibernateImpl;

public class UserUploadedFileEditor extends PropertyEditorSupport {
	@Override
	public String getAsText() {
		UserUploadedFile file = (UserUploadedFile)getValue();
		return file == null ? "" : String.valueOf(file.getId());
	}
	
	@Override
	public void setAsText(String text) throws IllegalArgumentException {
		if(text.length() == 0) {
			setValue(null);
			return;
		}
		
		UserUploadedFile file = UsersHibernateImpl.getUserUploadedFileById(text);
		setValue(file);
	}
}
