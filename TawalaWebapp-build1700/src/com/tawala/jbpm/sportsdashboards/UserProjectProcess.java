package com.tawala.jbpm.sportsdashboards;

import org.jbpm.graph.exe.ProcessInstance;

import com.tawala.project.UserProject;

public class UserProjectProcess {
	private final UserProject userProject;
	private final ProcessInstance processInstance;
	
	public UserProjectProcess(UserProject userProject, ProcessInstance processInstance) {
		this.processInstance = processInstance;
		this.userProject = userProject;
	}

	public UserProject getUserProject() {
		return userProject;
	}

	public ProcessInstance getProcessInstance() {
		return processInstance;
	}
}
