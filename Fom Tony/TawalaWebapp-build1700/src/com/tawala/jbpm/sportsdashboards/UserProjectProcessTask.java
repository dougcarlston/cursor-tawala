package com.tawala.jbpm.sportsdashboards;

import java.util.HashMap;
import java.util.Map;

import org.jbpm.taskmgmt.exe.TaskInstance;

import com.tawala.project.UserProject;
import com.tawala.web.controller.WellKnown;

public class UserProjectProcessTask {
	public static final Map<String, String> TASK_ID_TO_VIEW_TASK_URL_MAP = new HashMap<String, String>();
	static {
		TASK_ID_TO_VIEW_TASK_URL_MAP.put("prepare project for release",
				WellKnown.urls.getAdminTaskPrepareForRelease());
		TASK_ID_TO_VIEW_TASK_URL_MAP.put("create invoice", WellKnown.urls
				.getAdminTaskCreateInvoice());
		TASK_ID_TO_VIEW_TASK_URL_MAP.put("create purchase order",
				WellKnown.urls.getAdminTaskCreatePurchaseOrder());
	}

	private static String DEFAULT_VIEW_TASK_URL = WellKnown.urls
			.getAdminViewDefaultTask();

	private final UserProject userProject;
	private final TaskInstance taskInstance;

	public UserProjectProcessTask(UserProject userProject,
			TaskInstance taskInstance) {
		this.taskInstance = taskInstance;
		this.userProject = userProject;
	}

	public UserProject getUserProject() {
		return userProject;
	}

	public TaskInstance getTaskInstance() {
		return taskInstance;
	}

	public String getViewTaskURL() {
		String result = TASK_ID_TO_VIEW_TASK_URL_MAP
				.get(taskInstance.getName());
		if (result == null) {
			result = DEFAULT_VIEW_TASK_URL;
		}
		return result;
	}
}
