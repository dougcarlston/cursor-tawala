package com.tawala.web.controller;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;

import com.tawala.web.WorldInitializer;

public class AdminStatisticsPreparationInterceptor implements
        HandlerInterceptor {

    public boolean preHandle(HttpServletRequest request,
            HttpServletResponse response, Object handler) throws Exception {
        return true;
    }

    //--- TODO: separate method in Users to do just counts.
    public void postHandle(HttpServletRequest request,
            HttpServletResponse response, Object handler,
            ModelAndView modelAndView) throws Exception {

    	if(modelAndView != null) {
    		modelAndView.addObject("totalUsersWaitingApproval", WorldInitializer
    			.getDefaultWorld().domain().users().findUsersWithStatus(null)
                .size());
    	}
    }

    public void afterCompletion(HttpServletRequest request,
            HttpServletResponse response, Object handler, Exception ex)
            throws Exception {
        // --- Do nothing
    }
}
