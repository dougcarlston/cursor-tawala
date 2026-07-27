package com.tawala.web.controller;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.core.Ordered;
import org.springframework.web.servlet.HandlerExecutionChain;
import org.springframework.web.servlet.HandlerInterceptor;

import com.tawala.web.project.FormPreviewController;
import com.tawala.web.project.ProjectTestDriveController;
import com.tawala.web.project.RealProjectController;
import com.tawala.web.project.SampleDataCollectingController;
import com.tawala.web.project.theme.DisplayUserUploadedFileController;

public class ProjectRuntimeControllerMapping extends HandlerMappingByURLPrefix
		implements Ordered, InitializingBean {

	public void afterPropertiesSet() throws Exception {
		HandlerInterceptor[] handlerInterceptors = new HandlerInterceptor[] {
				new UserAccessTicketInterceptor(), new NDCSetupInterceptor(),
				new VisitorTrackerInterceptor() };
		addMapping(WellKnown.urls.getProjectRunUrlPrefix() + "/",
				new HandlerExecutionChain(new RealProjectController(getServletContext()),
						handlerInterceptors));
		addMapping(WellKnown.urls.getFormPreviewUrlPrefix() + "/",
				new HandlerExecutionChain(new FormPreviewController(getServletContext()),
						handlerInterceptors));
		addMapping(WellKnown.urls.getLibraryTestDriveProjectUrlPrefix() + "/",
				new HandlerExecutionChain(new ProjectTestDriveController(getServletContext()),
						handlerInterceptors));
		addMapping(WellKnown.urls.getLibraryRecordSampleDataUrlPrefix() + "/",
				new HandlerExecutionChain(new SampleDataCollectingController(getServletContext()),
						handlerInterceptors));
		addMapping(WellKnown.urls.getUserFileDownloadPrefix() + "/",
				new HandlerExecutionChain(new DisplayUserUploadedFileController(),
						handlerInterceptors));
	}

	public int getOrder() {
		return -100;
	}
}
