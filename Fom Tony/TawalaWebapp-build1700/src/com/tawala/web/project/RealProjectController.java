package com.tawala.web.project;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;

import com.tawala.World;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.web.WorldInitializer;

public class RealProjectController extends DataCollectingProjectController {

    public RealProjectController(ServletContext servletContext) {
		super(servletContext);
	}

	@Override
    protected World getWorld(HttpServletRequest request) {
        return WorldInitializer.getDefaultWorld();
    }

	@Override
	protected EntryPointType getEntryPointType() {
		return EntryPointType.REAL_PROJECT;
	}

	@Override
	protected boolean getAllowAdsToAppear() {
		return true;
	}
}
