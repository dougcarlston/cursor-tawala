package com.tawala.jbpm.sportsdashboards.handler;

import org.jbpm.graph.def.ActionHandler;
import org.jbpm.graph.exe.ExecutionContext;

import com.scissor.Log;
import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.UserProjectProcess;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.backup.DailyBackup;

public class EnableProjectBackupHandler implements ActionHandler {
	private static final long serialVersionUID = 1L;

	public void execute(ExecutionContext executionContext) throws Exception {
		UserProjectProcess userProjectProcess = JbpmService
				.getUserProjectProcess(executionContext.getProcessInstance());

		DailyBackup backupSchedule = new DailyBackup(userProjectProcess
				.getUserProject());

		backupSchedule.setHour(0);
		backupSchedule.updateNextBackupDate();

		ProjectsHibernateImpl.saveBackupSchedule(backupSchedule);

		Log.info(this, "Enabled project backups for user '"
				+ userProjectProcess.getUserProject().getUser().getId()
				+ "', project '"
				+ userProjectProcess.getUserProject().getName() + "'");
		executionContext.getToken().signal();
	}

}
