package com.tawala.web.library;

import java.util.List;
import javax.servlet.http.HttpServletRequest;

import org.springframework.util.StringUtils;

import com.tawala.project.library.ProjectLibrary;
import com.tawala.web.controller.WellKnown;

public class LibrarySearchController extends LibrarySearchBaseController {

	@Override
	protected String getViewName() {
		return "library.search";
	}

	@Override
	protected ProjectLibrary getCurrentLibrary(HttpServletRequest request, List<ProjectLibrary> availableLibraries) {
		// --- Figure out what the current library is supposed to be.
		String projectLibraryId = request.getParameter(LIBRARY_ID_PARAMETER);
		ProjectLibrary projectLibrary = StringUtils.hasLength(projectLibraryId) ? ProjectLibrary
				.getLibraryById(Integer.parseInt(projectLibraryId))
				: ProjectLibrary.SYSTEM_LIBRARY;

		// --- A bit of sanity and safety check.
		if (!availableLibraries.contains(projectLibrary)) {
			projectLibrary = availableLibraries.get(0);
		}
		return projectLibrary;
	}

	@Override
	protected boolean isDisplayAvailableLibraries() {
		return true;
	}

	@Override
	protected String getSearchURL() {
		return WellKnown.urls.getLibrarySearch();
	}
}
