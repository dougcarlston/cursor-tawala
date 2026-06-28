package com.tawala.web.library;

import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.theme.CommonTheme;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ViewProjectManagerDetailsController;

public class DeployToMyTawalaController extends SimpleFormController {
	public static final String PARAMETER_PROJECT_ID = "app_id";

	public static class DeployForm {
		private final User user;
		private final LibraryProject project;
		private String projectName;
		private String versionDescription;
		private String themePath;

		public DeployForm(User user, LibraryProject project) {
			this.user = user;
			this.project = project;
		}

		public LibraryProject getProject() {
			return project;
		}

		public String getProjectName() {
			return projectName;
		}

		public void setProjectName(String projectName) {
			this.projectName = projectName;
		}

		public String getThemePath() {
			return themePath;
		}

		public void setThemePath(String themePath) {
			this.themePath = themePath;
		}

		public User getUser() {
			return user;
		}

		public String getVersionDescription() {
			return versionDescription;
		}

		public void setVersionDescription(String versionDescription) {
			this.versionDescription = versionDescription;
		}
	}

	private static class FormValidator implements Validator {
		@SuppressWarnings("unchecked")
		public boolean supports(Class clazz) {
			return clazz.equals(DeployForm.class);
		}

		public void validate(Object target, Errors errors) {
			ValidationUtils.rejectIfEmptyOrWhitespace(errors, "projectName",
					"deploy.project-name.empty");
			DeployForm form = (DeployForm) target;

			UserProject anotherProject = WorldInitializer.getDefaultWorld()
					.domain().projects().get(form.getUser().getId(),
							form.getProjectName());
			if (anotherProject != null) {
				errors.rejectValue("projectName",
						"deploy.project-name.is-not-unique",
						new Object[] { form.getProjectName() },
						"Project already exists");
			}
		}
	}

	public DeployToMyTawalaController() {
		setCommandName("form");
		setFormView("library.deploy.to.mytawala");
		setValidator(new FormValidator());
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		LibraryProject libraryProject = ProjectLibraryService
				.findProjectById(Long.parseLong(request
						.getParameter(PARAMETER_PROJECT_ID)));
		DeployForm form = new DeployForm(user, libraryProject);
		form.setVersionDescription("Deployed version " + libraryProject.getLatestVersionNumber() + " of \"" + libraryProject.getName() + "\".");
		return form;
	}

	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		result.put("commonThemes", CommonTheme.ALL_THEMES);
		result.put("userDefinedThemes", UsersHibernateImpl
				.getAllUserThemes(UserInfoPreparationInterceptor
						.getSessionUser(request)));
		// --- this is needed to display the left panel details.
		result.put("project", ProjectLibraryService.findProjectById(Long
				.parseLong(request.getParameter(PARAMETER_PROJECT_ID))));

		return result;
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		DeployForm form = (DeployForm) command;

		ProjectLibraryService.cloneProjectToUserAccount(form.getProject(),
				UserInfoPreparationInterceptor.getSessionUser(request), false,
				form.getProjectName(), form.getThemePath(), form.getVersionDescription());

		EventService.createEvent(new Event("LibraryProjectCloning", request,
				form.getProject().getName()));

		response.sendRedirect(WellKnown.urls
				.getProjectManagerProjectDetailView()
				+ '?'
				+ ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME
				+ '=' + form.getProjectName());

		return null;
	}
}
