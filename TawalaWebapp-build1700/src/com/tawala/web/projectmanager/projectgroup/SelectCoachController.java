package com.tawala.web.projectmanager.projectgroup;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.ProjectGroup;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class SelectCoachController implements Controller {
	public static final String GROUP_ID = "group_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		long groupId = Long.parseLong(request.getParameter(GROUP_ID));

		ProjectGroup projectGroup = UsersHibernateImpl
				.getProjectGroup(UserInfoPreparationInterceptor
						.getSessionUser(request), groupId);
		if (projectGroup == null) {
			Log.error(this, "Unable to find project group by id: " + groupId);
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		ModelAndView result = new ModelAndView(
				"projectmanager.project.group.coach.management");
		result.addObject("group", projectGroup);
		result.addObject("sportsdashboardsGroups", UsersHibernateImpl
				.getAllUserSportsDashboardGroups(UserInfoPreparationInterceptor
						.getSessionUser(request)));

		return result;
	}

}
