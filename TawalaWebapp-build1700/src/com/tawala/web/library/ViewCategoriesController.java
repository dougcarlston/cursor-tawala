package com.tawala.web.library;

import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;

public class ViewCategoriesController implements Controller {
	public static final String PARAMETER_DISPLAY_CATEGORY_ID = "id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		Category currentCategory = null;
		String id = null;
		if ((id = request.getParameter(PARAMETER_DISPLAY_CATEGORY_ID)) != null) {
			currentCategory = ProjectLibraryService.findCategoryById(Long
					.parseLong(id));
		}

		ModelAndView result = new ModelAndView("library.manage.categories");

		ProjectLibrary library = ProjectLibrary
				.getLibraryById(Integer
						.parseInt(request
								.getParameter(EditCategoryController.PARAMETER_LIBRARY_ID)));
		List<Category> allCategories = ProjectLibraryService
				.getAllCategoriesFor(library);

		if (currentCategory == null) {
			if (allCategories.size() > 0) {
				currentCategory = allCategories.get(0);
			} else {
				// TODO: This is a hack. We create a new category on the fly
				// if none exists. In practice we don't need to do that because
				// the libraries are usually initialized. But for a brand new
				// library this provides a starting point.
				currentCategory = ProjectLibraryService
						.getOrCreateDefaultCategory(library);
				allCategories.add(currentCategory);
			}
		}

		result.addObject("currentCategory", currentCategory);
		result.addObject("categories", allCategories);
		result.addObject("library", library);

		return EditCategoryController.displayCategory(request, response,
				result, currentCategory.getId());
	}
}
