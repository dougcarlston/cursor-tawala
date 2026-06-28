package com.tawala.web.admin.sportsdashboard;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.jbpm.JbpmContext;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.Constants;

public class ListProjectWorkflowsInAParticularStateController implements
		Controller {
	public static final String STATE_NAME_PARAMETER = "state";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		JbpmContext jbpmContext = JbpmService.createContext();
		try {
			String stateName = request.getParameter(STATE_NAME_PARAMETER);

			ModelAndView result = new ModelAndView(
					"admin.sports-dashboard.list.processes.in.particular.state");
			result.addObject("processes", JbpmService.getProcessesInParticularState(Constants.PROCESS_NAME, stateName));
			result.addObject("processStateName", stateName);
			return result;
		} finally {
			jbpmContext.close();
		}
	}
}
