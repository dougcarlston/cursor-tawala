package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;

import com.tawala.project.Form;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;

public class OneClickTestDriveController extends TestDrivePreparationController {
	public static final String FORM_NAME_PARAMETER = "form";

	@Override
	protected ModelAndView handlePreparedProject(UserProject runnableProject,
			LibraryProject project, LibraryProjectVersion version,
			HttpServletRequest request, HttpServletResponse response)
			throws IOException {
		String formName = request.getParameter(FORM_NAME_PARAMETER);
		if(formName == null) {
			throw new IllegalStateException("Unable to find parameter '" + FORM_NAME_PARAMETER + "'");
		}
		Form form = runnableProject.getProject().getForm(formName);
		if(form == null) {
			throw new IllegalStateException("Unable to find form '" + formName + "'");
		}
		
		String url = runnableProject.getEntryPointURLs(UserProject.EntryPointType.TEST_DRIVE).get(form);
		response.sendRedirect(url);
		
		return null;
	}
}
