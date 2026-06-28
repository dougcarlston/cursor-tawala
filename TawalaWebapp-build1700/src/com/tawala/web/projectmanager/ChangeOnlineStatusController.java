package com.tawala.web.projectmanager;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class ChangeOnlineStatusController implements Controller {
	public final static String PARAMETER_PROJECT_ID = "id";
	public final static String PARAMETER_TAKE_OFFLINE = "offline";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		long projectId = Long.parseLong(request
				.getParameter(PARAMETER_PROJECT_ID));
		boolean doTakeOffLine = Boolean.parseBoolean(request
				.getParameter(PARAMETER_TAKE_OFFLINE));
		boolean status = WorldInitializer.getDefaultWorld().domain().projects()
				.takeProjectOffline(
						UserInfoPreparationInterceptor.getSessionUser(request),
						projectId, doTakeOffLine);
		response.getOutputStream().print(status);
		return null;
	}
}
