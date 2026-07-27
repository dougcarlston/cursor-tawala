package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;

public class ProjectDescriptionController implements Controller {
	public static final String PARAMETER_PROJECT_ID = "app_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String projectIdParameter = request.getParameter(PARAMETER_PROJECT_ID);
		long projectId = Long.parseLong(projectIdParameter);

		LibraryProject libraryProject = ProjectLibraryService
				.findProjectById(projectId);
		if (libraryProject == null) {
			throw new IllegalStateException("Unable to find project by id="
					+ projectId);
		}

		ModelAndView result = new ModelAndView("library.describe.project");
		result.addObject("libraryProject", libraryProject);

		return result;
	}
}
