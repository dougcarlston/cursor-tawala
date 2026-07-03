package com.tawala.web.projectmanager.projectgroup;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.UsersHibernateImpl;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class DeleteProjectFromGroupController implements Controller {
	public static final String GROUP_ID = "group_id";
	public static final String PROJECT_ID = "project_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String groupId = request.getParameter(GROUP_ID);
		UsersHibernateImpl.deleteProjectFromGroup(
				UserInfoPreparationInterceptor.getSessionUser(request), Long
						.parseLong(groupId), Long.parseLong(request
						.getParameter(PROJECT_ID)));
		response.sendRedirect(WellKnown.urls.getManageProjectGroup() + "?"
				+ ManageProjectGroupController.GROUP_ID_PARAMETER + "="
				+ groupId);
		return null;
	}
}
