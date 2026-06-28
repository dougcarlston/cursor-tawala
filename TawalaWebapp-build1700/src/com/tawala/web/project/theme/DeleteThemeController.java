package com.tawala.web.project.theme;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.UsersHibernateImpl;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class DeleteThemeController implements Controller {
	public static final String THEME_ID = "theme_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		UsersHibernateImpl.deleteTheme(UserInfoPreparationInterceptor
				.getSessionUser(request), Long.parseLong(request
				.getParameter(THEME_ID)));
		return new ModelAndView("redirect:"
				+ WellKnown.urls.getProjectManagerView());
	}

}
