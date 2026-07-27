package com.tawala.web.projectmanager;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.project.Field;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.StoredField;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

abstract public class FormDataExportController implements Controller {
	public static final String PARAMETER_PROJECT_NAME = "project";
	public static final String PARAMETER_FORM_NAME = "form";
	public static final String PARAMETER_SHARED_DATA = "sharedData";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		Project project = null;
		String projectName = null;

		String formName = request.getParameter(PARAMETER_FORM_NAME);
		Collection<Reference> fieldReferences = new ArrayList<Reference>();

		boolean extractSharedData = extractSharedData(request);
		if (extractSharedData) {
			project = WorldInitializer.getDefaultWorld().domain().users()
					.getSharedStorageForUser(user);
			if(project == null) {
				response.sendRedirect(WellKnown.urls.getProjectManagerView());
				return null;
			}
			for (StoredField field : project.getDataSourceNamed(formName)
					.getFields()) {
				fieldReferences.add(new Reference(field.getName()));
			}
		} else {
			projectName = request.getParameter(PARAMETER_PROJECT_NAME);
			UserProject userProject = WorldInitializer.getDefaultWorld()
					.domain().projects().getWithProjectRuntime(user.getId(),
							projectName);
			if(userProject == null) {
				response.sendRedirect(WellKnown.urls.getProjectManagerView());
				return null;
			}
			project = userProject.getProject();
			for (Field field : project.getForm(formName).getAllFields()) {
				fieldReferences.add(new Reference(field.getHtmlId()));
			}
		}
		List<FormSubmission> formResponses = WorldInitializer.getDefaultWorld()
				.domain().storedData().responsesFor(project, formName);

		if (formResponses.size() > 0 && fieldReferences.size() > 0) {
			response.setContentType(getContentType());
			response.setHeader("Content-Disposition", "attachment; filename=\""
					+ getFileName(formName) + "\";");
			sendData(formName, formResponses, fieldReferences, response);
		} else {
			response.sendRedirect(extractSharedData ? WellKnown.urls
					.getViewSharedDatasources() : WellKnown.urls
					.getProjectManagerProjectDetailView()
					+ "?projectName=" + projectName);
		}
		return null;
	}

	abstract protected void sendData(String formName,
			List<FormSubmission> formResponses, Collection<Reference> fields,
			HttpServletResponse response) throws IOException;

	abstract protected String getFileName(String formName);

	abstract protected String getContentType();

	private boolean extractSharedData(HttpServletRequest request) {
		return request.getParameter(PARAMETER_SHARED_DATA) != null;
	}
}
