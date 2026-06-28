package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.UserProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public abstract class AddVersionController extends SimpleFormController {
	public static final String PARAMETER_PROJECT_ID = "project_name";

	public AddVersionController() {
		setCommandName("form");
		setFormView("library.update.project.version");
		setSuccessView("library.update.project.version.confirmation");
		setSessionForm(false);
		setValidator(new AddVersionForm.Validator());
	}

	@Override
	protected final ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		AddVersionForm form = (AddVersionForm) command;

		ProjectLibraryService.onAddingProjectVersion(form.getUserProject(), form
				.getLibraryProject(), form.getProjectVersion());

		EventService.createEvent(new Event("PublishProjectVersionToLibrary",
				request, form.getLibraryProject().getName()));

		return super.onSubmit(request, response, command, errors);
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see org.springframework.web.servlet.mvc.AbstractFormController#formBackingObject(javax.servlet.http.HttpServletRequest)
	 */
	@Override
	protected final Object formBackingObject(HttpServletRequest request)
			throws Exception {
		String projectName = request.getParameter(PARAMETER_PROJECT_ID);

		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(
						UserInfoPreparationInterceptor.getSessionUser(request)
								.getId(), projectName);

		if (userProject == null)
			throw new IllegalStateException(
					"Unable to find deployed project by name '" + projectName
							+ "'.");

		AddVersionForm form = instantiateForm(UserInfoPreparationInterceptor
				.getSessionUser(request), userProject);
		form.setProjectName(projectName);

		return form;
	}

	protected abstract AddVersionForm instantiateForm(User user,
			UserProject userProject);
}
