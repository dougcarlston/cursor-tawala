package com.tawala.web.admin.sportsdashboard;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.jbpm.JbpmService;
import com.tawala.web.controller.WellKnown;

public class PerformTaskController implements Controller {
	public static final String TASK_ID_PARAMETER = "task-id";
	public static final String TRANSITION_NAME_PARAMETER = "transition-name";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		long taskId = Long.parseLong(request.getParameter(TASK_ID_PARAMETER));
		String transitionName = request.getParameter(TRANSITION_NAME_PARAMETER);

		long processInstanceId = JbpmService.endTask(taskId, transitionName);
		response
				.sendRedirect(WellKnown.urls
						.getAdminViewProjectWorkflowDetails()
						+ '?'
						+ ViewProjectWorkflowDetailsController.PROCESS_INSTANCE_ID_PARAMETER
						+ '=' + processInstanceId);
		return null;
	}
}
