package com.tawala.jbpm.sportsdashboards.action;

import org.jbpm.graph.def.ActionHandler;
import org.jbpm.graph.exe.ExecutionContext;
import org.jbpm.taskmgmt.exe.TaskInstance;

import com.scissor.Log;
import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.UserProjectProcess;

public class HandlePOTimeout implements ActionHandler {
	private static final long serialVersionUID = 1L;

	public void execute(ExecutionContext executionContext) throws Exception {
		UserProjectProcess userProjectProcess = JbpmService
				.getUserProjectProcess(executionContext.getProcessInstance());
		Log
				.error(
						this,
						"Project '"
								+ userProjectProcess.getUserProject().getName()
								+ "' for "
								+ userProjectProcess.getUserProject().getUser()
								+ " has been moved into the exception handling state because the PO hasn't been received.");
		TaskInstance taskInstance = executionContext.getTaskInstance();
		if(taskInstance != null) {
			taskInstance.end();
		}
	}
}
