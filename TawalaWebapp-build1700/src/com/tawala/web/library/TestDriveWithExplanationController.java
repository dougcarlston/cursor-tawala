package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;

import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;

public class TestDriveWithExplanationController extends
		TestDrivePreparationController {
	@Override
	protected ModelAndView handlePreparedProject(UserProject runnableProject,
			LibraryProject project, LibraryProjectVersion version, HttpServletRequest request, HttpServletResponse response) {
		ModelAndView modelAndView = new ModelAndView("library.test.drive.main");
		modelAndView.addObject("project", project);
		modelAndView.addObject("version", version);
		modelAndView.addObject("runnableProject", runnableProject);

		return modelAndView;
	}
}
