package com.tawala.web.controller;

import org.springframework.web.servlet.HandlerExecutionChain;
import org.springframework.web.servlet.HandlerInterceptor;

import com.tawala.web.general.LandingPageController;

public class LandingPageMapping extends HandlerMappingByURLPrefix {
	public LandingPageMapping() {
		addMapping(WellKnown.urls.getLandingPagePrefix() + "/",

		new HandlerExecutionChain(new LandingPageController(),
				new HandlerInterceptor[] { new UserAccessTicketInterceptor(),
						new NDCSetupInterceptor(),
						new VisitorTrackerInterceptor() }));
	}
}
