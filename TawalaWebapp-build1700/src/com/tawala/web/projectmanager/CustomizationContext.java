package com.tawala.web.projectmanager;

import com.tawala.project.UserProject;

public class CustomizationContext {
	private final UserProject userProject;
	private final String projectName;
	private final boolean projectSaved;
	
	public CustomizationContext(UserProject userProject, String projectName, boolean projectSaved) {
		this.userProject = userProject;
		this.projectName = projectName;
		this.projectSaved = projectSaved;
	}

	
	public UserProject getUserProject() {
		return userProject;
	}

	public String getProjectName() {
		return projectName;
	}

	public boolean isProjectSaved() {
		return projectSaved;
	}
}
