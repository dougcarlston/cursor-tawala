package com.tawala.web.library;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.util.StringUtils;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class EditCategoryController implements Controller {
	public static final String PARAMETER_PARENT_ID = "parent";
	public static final String PARAMETER_DESCRIPTION = "description";
	public static final String PARAMETER_NAME = "name";

	public static final String PARAMETER_CATEGORY_ID = "id";
	public static final String PARAMETER_LIBRARY_ID = "library";

	public static final String PARAMETER_NEW = "create";
	public static final String PARAMETER_DELETE = "delete";
	public static final String PARAMETER_UPDATE = "update";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		if (request.getParameter(PARAMETER_NEW) != null) {
			Category category = createNew(request);
			return navigateToDisplayCategoriesScreen(response, category,
					category.getLibrary());
		} else if (request.getParameter(PARAMETER_DELETE) != null) {
			ProjectLibrary library = deleteCategory(request);
			return navigateToDisplayCategoriesScreen(response, null, library);
		} else if (request.getParameter(PARAMETER_UPDATE) != null) {
			Category category = updateCategory(request);
			return navigateToDisplayCategoriesScreen(response, category,
					category.getLibrary());
		} else {
			return displayCategory(request, response, null, 0);
		}
	}

	private static ModelAndView navigateToDisplayCategoriesScreen(
			HttpServletResponse response, Category category,
			ProjectLibrary library) throws IOException {
		StringBuilder url = new StringBuilder(WellKnown.urls
				.getLibraryCategories());

		url.append("?").append(EditCategoryController.PARAMETER_LIBRARY_ID)
				.append("=").append(library.getId());
		if (category != null) {
			url.append("&").append(
					ViewCategoriesController.PARAMETER_DISPLAY_CATEGORY_ID)
					.append("=").append(category.getId());
		}
		response.sendRedirect(url.toString());

		return null;
	}

	private Category updateCategory(HttpServletRequest request) {
		String id = request.getParameter(PARAMETER_CATEGORY_ID);
		long categoryId = Long.parseLong(id);

		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		String name = request.getParameter(PARAMETER_NAME);
		String description = request.getParameter(PARAMETER_DESCRIPTION);
		String parentId = request.getParameter(PARAMETER_PARENT_ID);

		if (!StringUtils.hasText(name)) {
			return ProjectLibraryService.findCategoryById(categoryId);
		}

		Long parentCategoryId = null;
		if (!"".equals(parentId)) {
			parentCategoryId = Long.parseLong(parentId);
		}

		Category category = ProjectLibraryService.updateCategory(user,
				categoryId, name, description, parentCategoryId);

		return category;
	}

	private ProjectLibrary deleteCategory(HttpServletRequest request)
			throws Exception {
		String id = request.getParameter(PARAMETER_CATEGORY_ID);

		Category category = ProjectLibraryService.findCategoryById(Long
				.parseLong(id));
		if (category == null)
			return ProjectLibrary.COMMUNITY_LIBRARY;

		if (category.isReadOnly())
			return category.getLibrary();

		if (UserInfoPreparationInterceptor.getSessionUser(request) == null)
			return category.getLibrary();

		ProjectLibraryService.deleteCategory(category,
				UserInfoPreparationInterceptor.getSessionUser(request));
		return category.getLibrary();
	}

	private Category createNew(HttpServletRequest request) {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if (user == null)
			return null;

		String name = request.getParameter(PARAMETER_NAME);
		String description = request.getParameter(PARAMETER_DESCRIPTION);
		String parentId = request.getParameter(PARAMETER_PARENT_ID);

		// --- Validation of sorts
		if (!StringUtils.hasText(name)) {
			return null;
		}

		Category category = null;
		if ("".equals(parentId)) {
			ProjectLibrary library = user.isAdministrator() ? ProjectLibrary
					.getLibraryById(Integer.parseInt(request
							.getParameter(PARAMETER_LIBRARY_ID)))
					: ProjectLibrary.COMMUNITY_LIBRARY;
			category = new Category(library, name, description);
		} else {
			category = new Category(ProjectLibraryService.findCategoryById(Long
					.parseLong(parentId)), name, description);

		}

		ProjectLibraryService.createCategory(category, user);

		return category;
	}

	@SuppressWarnings("unchecked")
	public static ModelAndView displayCategory(HttpServletRequest request,
			HttpServletResponse response, ModelAndView largerPart,
			long categoryId) throws Exception {
		long id = categoryId;
		if (id == 0)
			id = Long.parseLong(request.getParameter(PARAMETER_CATEGORY_ID));

		Category category = ProjectLibraryService.findCategoryById(id);
		if (category == null)
			throw new IllegalStateException("Unable to find category with id '"
					+ id + "'");

		ModelAndView result = largerPart == null ? new ModelAndView(
				"library.edit.category") : largerPart;

		result.addObject("category", category);
		ProjectLibrary library = ProjectLibrary.getLibraryById(Integer
				.parseInt(request.getParameter(PARAMETER_LIBRARY_ID)));
		result.addObject("library", library);

		if (!result.getModel().containsKey("categories")) {
			result.addObject("categories", ProjectLibraryService
					.getAllCategoriesFor(library));
		}

		if (UserInfoPreparationInterceptor.getSessionUser(request) != null) {
			// --- It's relatively expensive to do this, let's skip it if not
			// needed inside JSP.

			result.addObject("categoriesWithoutCurrentSubtree",
					getCategoriesWithoutCurrentSubtree(
							(Collection<Category>) result.getModel().get(
									"categories"), category));
		}
		result.addObject("projects", ProjectLibraryService
				.findUndeletedProjectsByCategory(category));

		return result;
	}

	private static Collection<Category> getCategoriesWithoutCurrentSubtree(
			Collection<Category> categories, Category category) {
		Collection<Category> result = new ArrayList<Category>(categories.size());

		for (Category nextCategory : categories) {
			if (!nextCategory.equals(category)
					&& !nextCategory.isChildOf(category)) {
				result.add(nextCategory);
			}
		}
		return result;
	}
}
