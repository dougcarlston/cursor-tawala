package com.tawala.project.library;

import java.beans.PropertyEditorSupport;

public class LibraryProjectEditor extends PropertyEditorSupport {
	@Override
	public String getAsText() {
		LibraryProject libraryProject = (LibraryProject)getValue();
		return libraryProject == null ? "" : String.valueOf(libraryProject.getId());
	}
	
	@Override
	public void setAsText(String text) throws IllegalArgumentException {
		if(text.length() == 0) {
			setValue(null);
			return;
		}
		
		setValue(ProjectLibraryService.findProjectById(Long.parseLong(text)));
	}
}
