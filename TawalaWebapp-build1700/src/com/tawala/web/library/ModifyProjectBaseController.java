package com.tawala.web.library;

import java.io.IOException;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

abstract public class ModifyProjectBaseController extends SimpleFormController {
	public static final String PARAMETER_PROJECT_ID = "project_id";

	public ModifyProjectBaseController() {
		setSessionForm(false);
		setValidateOnBinding(true);
		setCommandName("form");
		setFormView(getFormViewName());
		setSuccessView(getSuccessViewName());
		setValidator(new EditProjectForm.Validator());
	}

	protected abstract String getFormViewName();

	protected abstract String getSuccessViewName();

	protected abstract void performModification(EditProjectForm form, User user)
			throws IOException;

	/*
	 * (non-Javadoc)
	 * 
	 * @see org.springframework.web.servlet.mvc.SimpleFormController#onSubmit(javax.servlet.http.HttpServletRequest,
	 *      javax.servlet.http.HttpServletResponse, java.lang.Object,
	 *      org.springframework.validation.BindException)
	 */
	@Override
	final protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		EditProjectForm form = (EditProjectForm) command;

		Category category = form.getCategory();
		if (category.getName().length() > 0) {
			ProjectLibraryService.createCategory(category,
					UserInfoPreparationInterceptor.getSessionUser(request));
			Log.info(this, "Successfully created category '"
					+ category.getName() + "'.");

			// --- This needs to be done to clear the fields on successful
			// category creation.
			form.setSavedCategoryId(category.getId());
			form.setCategory(new Category(ProjectLibrary.COMMUNITY_LIBRARY, "",
					""));

			ModelAndView result = showForm(request, errors, getFormViewName());
			result.addObject(getFormViewName(), result);

			result.addObject("messages", Collections.singletonList(new Message(
					"category.created", category.getName())));

			return result;
		} else {
			performModification(form, UserInfoPreparationInterceptor
					.getSessionUser(request));

			ModelAndView result = super.onSubmit(request, response, command,
					errors);

			result = postProcessResult(result, form.getProject(), request,
					response);

			return result;
		}
	}

	abstract protected ModelAndView postProcessResult(ModelAndView result,
			LibraryProject project, HttpServletRequest request,
			HttpServletResponse response) throws Exception;

	/*
	 * (non-Javadoc)
	 * 
	 * @see org.springframework.web.servlet.mvc.SimpleFormController#referenceData(javax.servlet.http.HttpServletRequest)
	 */
	@Override
	final protected Map referenceData(HttpServletRequest request)
			throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		List<ProjectLibrary> availableLibraries = ProjectLibraryService
				.getLibrariesUpdatableByUser(UserInfoPreparationInterceptor
						.getSessionUser(request));
		result.put("availableLibraries", availableLibraries);

		Map<ProjectLibrary, List<Category>> libraryCategories = new HashMap<ProjectLibrary, List<Category>>();

		for (ProjectLibrary library : availableLibraries) {
			libraryCategories.put(library, ProjectLibraryService
					.getAllCategoriesFor(library));
		}
		
		result.put("libraryCategories", libraryCategories);

		return result;
	}
}
