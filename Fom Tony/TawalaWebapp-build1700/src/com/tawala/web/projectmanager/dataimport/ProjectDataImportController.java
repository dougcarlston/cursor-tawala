package com.tawala.web.projectmanager.dataimport;

import java.util.ArrayList;
import java.util.Collection;

import javax.servlet.http.HttpServletRequest;

import com.tawala.domain.User;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.UserProject;
import com.tawala.project.data.FormSubmissionCreator;
import com.tawala.project.data.RegularProjectFormSubmissionCreator;
import com.tawala.project.data.StoredField;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ViewProjectManagerDetailsController;

public class ProjectDataImportController extends ImportDataController {
	public static final String PARAMETER_FORM_NAME = "form";
	public static final String PARAMETER_PROJECT_NAME = "project";

	public static class ProjectDataFileUploadBean extends FileUploadBean {
		private String projectName;
		private String formName;

		public String getFormName() {
			return formName;
		}

		public void setFormName(String formName) {
			this.formName = formName;
		}

		public String getProjectName() {
			return projectName;
		}

		public void setProjectName(String projectName) {
			this.projectName = projectName;
		}

		@Override
		public String getDataSourceDescription() {
			return "project \"" + projectName + "\", form \"" + formName + "\"";
		}

		@Override
		public String getReturnURL() {
			return WellKnown.urls.getProjectManagerProjectDetailView()
					+ "?"
					+ ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME
					+ "=" + getProjectName();
		}
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		ProjectDataFileUploadBean result = new ProjectDataFileUploadBean();
		result.setFormName(request.getParameter(PARAMETER_FORM_NAME));
		result.setProjectName(request.getParameter(PARAMETER_PROJECT_NAME));
		result.setFields(getFields(UserInfoPreparationInterceptor
				.getSessionUser(request), result.getProjectName(), result
				.getFormName()));

		return result;
	}

	private static Collection<StoredField> getFields(User user,
			String projectName, String formName) {
		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(user.getId(), projectName);
		if (userProject == null) {
			throw new IllegalStateException("unable to find project '"
					+ projectName + "'");
		}

		Form form = userProject.getProject().getForm(formName);
		if (form == null) {
			throw new IllegalStateException("unable to find form '" + formName
					+ "' in project '" + projectName + "'");
		}

		Collection<Field> fields = form.getAllFields();
		Collection<StoredField> result = new ArrayList<StoredField>(fields
				.size());
		for (Field field : fields) {
			result.add(field.getStoredFieldDefinition());
		}
		return result;
	}

	@Override
	protected void deleteOldData(User user, Object command) {
		ProjectDataFileUploadBean uploadBean = (ProjectDataFileUploadBean) command;
		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(user.getId(),
						uploadBean.getProjectName());
		WorldInitializer.getDefaultWorld().domain().storedData()
				.eraseResponsesFor(userProject.getProject(),
						uploadBean.getFormName());
	}

	@Override
	protected FormSubmissionCreator getFormSubmissionCreator(Object command,
			User user) {
		ProjectDataFileUploadBean fileUploadBean = (ProjectDataFileUploadBean) command;
		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(user.getId(),
						fileUploadBean.getProjectName());
		if (userProject == null) {
			throw new IllegalStateException("unable to find project '"
					+ fileUploadBean.getProjectName() + "'");
		}

		Form form = userProject.getProject().getForm(
				fileUploadBean.getFormName());
		if (form == null) {
			throw new IllegalStateException("unable to find form '"
					+ fileUploadBean.getFormName() + "' in project '"
					+ fileUploadBean.getProjectName() + "'");
		}

		return new RegularProjectFormSubmissionCreator(userProject, form);
	}
}
