package com.tawala.web.projectmanager.integration;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.integration.ysl.LeagueDataExchange;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class UpdateYourSportsLeagueDataController implements Controller {
	public static final String PROJECT_ID_PARAMETER = "project_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		ModelAndView result = new ModelAndView(
				"projectmanager.update.yoursportsleague.data");

		String projectIdParameter = request.getParameter(PROJECT_ID_PARAMETER);
		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		UserProject userProject = ProjectsHibernateImpl.getUserProjectWithRuntimeById(user, Long.parseLong(projectIdParameter));
		if (userProject == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		result.addObject("messages", LeagueDataExchange.updateYSL(userProject));

		EventService.createEvent(new Event("SynchronizationWithYSL", request,
				userProject.getName()));

		return result;
	}
}
