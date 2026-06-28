package com.tawala.web.library;

import java.util.List;

import javax.servlet.http.HttpServletRequest;

import com.tawala.project.library.ProjectLibrary;
import com.tawala.web.controller.WellKnown;

public class CommunityLibraryController extends LibrarySearchBaseController {

	@Override
	protected ProjectLibrary getCurrentLibrary(HttpServletRequest request, List<ProjectLibrary> availableLibraries) {
		return ProjectLibrary.COMMUNITY_LIBRARY;
	}

	@Override
	protected String getSearchURL() {
		return WellKnown.urls.getCommunityLibrary();
	}

	@Override
	protected String getViewName() {
		return "community.library";
	}

	@Override
	protected boolean isDisplayAvailableLibraries() {
		return false;
	}
}
