package com.tawala.web.admin.sportsdashboard;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.jbpm.JbpmContext;
import org.jbpm.taskmgmt.exe.TaskInstance;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.jbpm.JbpmService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class AssignTaskController implements Controller {
	public static final String TASK_ID_PARAMETER = "task-id";
	public static final String USER_ID_PARAMETER = "user-id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		JbpmContext jbpmContext = JbpmService.createContext();
		try {
			long taskId = Long.parseLong(request
					.getParameter(TASK_ID_PARAMETER));
			TaskInstance taskInstance = jbpmContext
					.getTaskInstanceForUpdate(taskId);
			if (taskInstance == null) {
				throw new IllegalStateException(
						"Unable to find task instance with id " + taskId);
			}

			String userId = request.getParameter(USER_ID_PARAMETER);
			if (userId == null) {
				userId = UserInfoPreparationInterceptor.getSessionUser(request)
						.getId();
			}
			taskInstance.setActorId(userId);
		} finally {
			jbpmContext.close();
		}

		response.sendRedirect(WellKnown.urls
				.getAdminSportsDashboardProjectTaskManagement());

		return null;
	}

}
