package com.tawala.web.controller;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;

import com.tawala.domain.User;
import com.tawala.web.tags.LoginLinkTag;

public class OnlyAuthenticatedUserAccessInterceptor extends
        BaseAuthenticationInterceptor implements HandlerInterceptor {
    public boolean preHandle(HttpServletRequest request,
            HttpServletResponse response, Object handler) throws Exception {

        User user = UserInfoPreparationInterceptor.getSessionUser(request);
        if (user == null) {
            response.sendRedirect(LoginLinkTag.constructLoginURL(request));
            return false;
        }

        return checkPasswordResetRequirementAndRedirectIfNeeded(response,
                handler, user);
    }

    public void postHandle(HttpServletRequest request,
            HttpServletResponse response, Object handler,
            ModelAndView modelAndView) throws Exception {
        // --- Do nothing
    }

    public void afterCompletion(HttpServletRequest request,
            HttpServletResponse response, Object handler, Exception ex)
            throws Exception {
        // --- Do nothing
    }
}
