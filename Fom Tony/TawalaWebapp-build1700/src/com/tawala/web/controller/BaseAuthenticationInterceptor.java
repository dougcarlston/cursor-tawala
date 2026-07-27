package com.tawala.web.controller;

import java.io.IOException;

import javax.servlet.http.HttpServletResponse;

import com.tawala.domain.User;
import com.tawala.web.user.ChangePasswordController;

public class BaseAuthenticationInterceptor {
	public static final String USER_ACCESS_TOKEN_COOKIE_NAME = "x";

    protected boolean checkPasswordResetRequirementAndRedirectIfNeeded(HttpServletResponse response, Object handler, User user) throws IOException {
        if (user.isRequirePasswordReset()
                && !handler.getClass().equals(ChangePasswordController.class)) {
            response.sendRedirect(WellKnown.urls.getUserPasswordChange());
            return false;
        }
    
        return true;
    }
}
