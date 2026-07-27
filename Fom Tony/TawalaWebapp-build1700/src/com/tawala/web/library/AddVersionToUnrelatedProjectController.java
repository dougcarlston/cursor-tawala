package com.tawala.web.library;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;

import com.tawala.domain.User;
import com.tawala.project.UserProject;
import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class AddVersionToUnrelatedProjectController extends
		AddVersionController {

	@Override
	protected AddVersionForm instantiateForm(User user, UserProject userProject) {
		return new AddVersionForUnrelatedProjectForm(user.getId(), userProject);
	}

	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		List<ProjectLibrary> availableLibraries = ProjectLibraryService
				.getLibrariesUpdatableByUser(UserInfoPreparationInterceptor
						.getSessionUser(request));
		result.put("availableLibraries", availableLibraries);

		Map<ProjectLibrary, List<Category>> libraryCategories = new HashMap<ProjectLibrary, List<Category>>();

		for (ProjectLibrary library : availableLibraries) {
			libraryCategories.put(library, ProjectLibraryService
					.getCategoriesWithProjectsFrom(library));
		}
		
		result.put("libraryCategories", libraryCategories);
		result.put("showCategories", Boolean.TRUE);
	
		return result;
	}
}
