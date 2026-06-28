package com.tawala.web.project;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import com.tawala.World;
import com.tawala.project.UserProject.EntryPointType;

public class ProjectTestDriveController extends DataCollectingProjectController {
    public ProjectTestDriveController(ServletContext servletContext) {
		super(servletContext);
	}

	public static final String TESTWORLD_ATTRIBUTE_NAME = "testworld";

	@Override
    protected World getWorld(HttpServletRequest request) {
        HttpSession session = request.getSession(false);
        if(session == null)
            return null;
        return (World)session.getAttribute(TESTWORLD_ATTRIBUTE_NAME);
    }

	@Override
	protected EntryPointType getEntryPointType() {
		return EntryPointType.TEST_DRIVE;
	}

	@Override
	protected boolean getAllowAdsToAppear() {
		return false;
	}
}
