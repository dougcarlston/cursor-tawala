package com.tawala.web.projectmanager;

import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;

public class ClonedLibraryProjectCustomizationContext {
	private final String uniqueProjectId;
	private final String libraryProjectName;
	
	public ClonedLibraryProjectCustomizationContext(UserProject clonedProject, LibraryProject libraryProject) {
		this.uniqueProjectId = clonedProject.getUniqueRandomId();
		this.libraryProjectName = libraryProject.getName();
	}

	public String getLibraryProjectName() {
		return libraryProjectName;
	}

	public String getUniqueProjectId() {
		return uniqueProjectId;
	}
}
