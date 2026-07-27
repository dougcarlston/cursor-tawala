package com.tawala.web.projectmanager.projectgroup;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.UsersHibernateImpl;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class DeleteProjectGroupController implements Controller {
	public static final String GROUP_ID = "group_id";
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		UsersHibernateImpl.deleteProjectGroup(UserInfoPreparationInterceptor.getSessionUser(request), Long.parseLong(request.getParameter(GROUP_ID)));
		response.sendRedirect(WellKnown.urls.getProjectManagerView());
		return null;
	}
}
