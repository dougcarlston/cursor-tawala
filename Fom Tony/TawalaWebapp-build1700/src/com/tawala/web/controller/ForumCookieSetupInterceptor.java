package com.tawala.web.controller;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;

import com.tawala.web.jforum.ForumIntegrationToken;

public class ForumCookieSetupInterceptor implements HandlerInterceptor {
    public boolean preHandle(HttpServletRequest request,
            HttpServletResponse response, Object handler) throws Exception {
        return true;
    }

    public void postHandle(HttpServletRequest request,
            HttpServletResponse response, Object handler,
            ModelAndView modelAndView) throws Exception {

        boolean needToSetupCookie = true;
        Cookie[] cookies = request.getCookies();
        if (cookies != null) {
            for (int i = 0; i < cookies.length; i++) {
                Cookie cookie = cookies[i];
                if (cookie.getName().equals(ForumIntegrationToken.COOKIE_ID)) {
                    needToSetupCookie = false;
                }
            }
        }
        
        if (needToSetupCookie) {
            response.addCookie(new ForumIntegrationToken(ForumIntegrationToken.ANONYMOUS_FORUM_USER_ID)
                    .asCookie());
        }
    }

    public void afterCompletion(HttpServletRequest request,
            HttpServletResponse response, Object handler, Exception ex)
            throws Exception {
        // Do nothing
    }

}
