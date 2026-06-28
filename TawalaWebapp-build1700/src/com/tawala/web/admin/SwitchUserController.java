package com.tawala.web.admin;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ViewProjectManagerDetailsController;
import com.tawala.web.user.LoginController;

public class SwitchUserController implements Controller {
	public static final String ORIGINAL_USER_ATTRIBUTE = "originalUser";
	public static final String USER_ID_PARAMETER = "user_id";
	public static final String PROJECT_NAME_PARAMETER = "project_name";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String userIdParameter = request.getParameter(USER_ID_PARAMETER);
		User user = WorldInitializer.getDefaultWorld().domain().users().get(Long.parseLong(userIdParameter));
		if(user == null) {
			Log.warn(this, "User with id '" + userIdParameter + "' is not found." );
			response.sendRedirect(WellKnown.urls.getHomePage());
			return null;
		}

		User currentUser = UserInfoPreparationInterceptor.getSessionUser(request);

		//--- We don't want to have any automatic sign-in, not without some deeper analysis
		boolean keepUserSignedIn = false;
		LoginController.onUserLogin(request, response, user, keepUserSignedIn);

		HttpSession session = request.getSession(false);
		session.setAttribute(ORIGINAL_USER_ATTRIBUTE, currentUser);

		String projectName = request.getParameter(PROJECT_NAME_PARAMETER);
		if(projectName == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
		} else {
			ViewProjectManagerDetailsController.redirectToProjectDetails(response, projectName);
		}
		return null;
	}
}
