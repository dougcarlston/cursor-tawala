package com.tawala.web.controller;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.core.Ordered;
import org.springframework.web.servlet.HandlerExecutionChain;
import org.springframework.web.servlet.HandlerInterceptor;

public class SportsDashboardPromoControllerMapping extends HandlerMappingByURLPrefix
		implements Ordered, InitializingBean {

	public void afterPropertiesSet() throws Exception {
		HandlerInterceptor[] handlerInterceptors = new HandlerInterceptor[] {
				new UserAccessTicketInterceptor(), new NDCSetupInterceptor(),
				new VisitorTrackerInterceptor(),
				new UserInfoPreparationInterceptor(),
				new ForumCookieSetupInterceptor() 
				};
		addMapping(WellKnown.urls.getSportsPromoPrefix(),
				new HandlerExecutionChain(new URLToViewNameDisplayController(WellKnown.urls.getSportsHome()),
						handlerInterceptors));
	}

	public int getOrder() {
		return -90;
	}
}
