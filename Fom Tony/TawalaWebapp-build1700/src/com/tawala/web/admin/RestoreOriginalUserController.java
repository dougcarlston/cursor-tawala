package com.tawala.web.admin;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.user.LoginController;

public class RestoreOriginalUserController implements Controller {
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		User currentUser = UserInfoPreparationInterceptor
				.getSessionUser(request);

		HttpSession session = request.getSession(false);
		User originalUser = (User) session
				.getAttribute(SwitchUserController.ORIGINAL_USER_ATTRIBUTE);
		if(originalUser == null) {
			Log.error(this, "Unable to find the original user in the session.");
			response.sendRedirect(WellKnown.urls.getHomePage());
		}

		//--- We don't want to have any automatic sign-in, not without some deeper analysis
		boolean keepUserSignedIn = false;
		LoginController.onUserLogin(request, response, originalUser, keepUserSignedIn);

		response.sendRedirect(WellKnown.urls.getAdminViewUserInfo() + "?"
				+ ViewUserDetailController.USER_ID_PARAMETER + "="
				+ currentUser.getDatabaseId());

		return null;
	}
}
