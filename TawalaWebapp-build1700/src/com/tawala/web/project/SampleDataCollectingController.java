package com.tawala.web.project;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import com.tawala.World;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.web.Request;
import com.tawala.web.WorldInitializer;
import com.tawala.web.library.ViewProjectVersionSampleData;

public class SampleDataCollectingController extends
        DataCollectingProjectController {

    public SampleDataCollectingController(ServletContext servletContext) {
		super(servletContext);
	}

	@Override
    protected World getWorld(HttpServletRequest request) {
        //--- We want to record to the real "world".
        return WorldInitializer.getDefaultWorld();
    }

    @Override
    protected LinkToUserProject fetchProject(World world, Request request) throws NotFoundException {
        HttpSession session = request.getHttpRequest().getSession(false);
        if(session == null)
            throw new NotFoundException("Session is closed");
        
        LinkToUserProject result = (LinkToUserProject)session.getAttribute(ViewProjectVersionSampleData.SAMPLE_COLLECTING_PROJECT_ATTRIBUTE);
        if(result == null)
            throw new NotFoundException("Project not found in the session");
        
        return result;
    }

	@Override
	protected EntryPointType getEntryPointType() {
		return EntryPointType.SAMPLE_COLLECTION;
	}

	@Override
	protected boolean getAllowAdsToAppear() {
		return false;
	}
}
