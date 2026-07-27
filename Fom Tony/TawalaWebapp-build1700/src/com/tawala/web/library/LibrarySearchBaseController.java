package com.tawala.web.library;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.util.StringUtils;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.message.Message;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.admin.SwitchUserController;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public abstract class LibrarySearchBaseController implements Controller {

	private static final String PROJECTS_KEY = "projects";
	public final static String PARAMETER_QUERY = "query";
	public final static String CATEGORY_ID_PARAMETER = "category";
	public final static String LIBRARY_ID_PARAMETER = "library";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		User originalUser = (User) request.getSession().getAttribute(
				SwitchUserController.ORIGINAL_USER_ATTRIBUTE);

		ModelAndView result = new ModelAndView(getViewName());
		result.addObject("searchURL", getSearchURL());

		// --- Get available libraries to this user
		List<ProjectLibrary> availableLibraries = ProjectLibraryService
				.getLibrariesReadableByUser(originalUser == null ? user
						: originalUser);
		result.addObject("availableLibraries", availableLibraries);
		result.addObject("displayAvailableLibraries",
				isDisplayAvailableLibraries());

		ProjectLibrary projectLibrary = getCurrentLibrary(request,
				availableLibraries);
		result.addObject("currentLibrary", projectLibrary);

		String categoryIdParameter = request
				.getParameter(CATEGORY_ID_PARAMETER);
		if (categoryIdParameter != null) {
			long categoryId = Long.parseLong(categoryIdParameter);
			Category category = ProjectLibraryService
					.findCategoryById(categoryId);

			Map<Long, Boolean> showAsExpandedNodes = new HashMap<Long, Boolean>();
			showAsExpandedNodes.put(category.getId(), true);
			for (Category parent : category.getAllParents()) {
				showAsExpandedNodes.put(parent.getId(), true);
			}

			result.addObject(PROJECTS_KEY, ProjectLibraryService
					.getAllProjectsWithinCategory(categoryId));
			result.addObject("selectedCategory", category);
			result.addObject("showAsExpandedNodes", showAsExpandedNodes);
		} else {
			String query = request.getParameter(PARAMETER_QUERY);
			if (StringUtils.hasText(query)) {
				result.addObject(PARAMETER_QUERY, query);

				EventService.createEvent(new Event("LibarySearch", request,
						query));

				try {
					Collection<LibraryProject> foundProjects = ProjectLibraryService
							.search(projectLibrary, query);
					result.addObject(PROJECTS_KEY, foundProjects);
				} catch (Exception e) {
					Log.info(this, "Failed to execute the query '" + query
							+ "':", e);

					result.addObject("messages", Collections
							.singletonList(new Message(
									"library.error.searching")));
					result.addObject(PROJECTS_KEY, Collections.EMPTY_LIST);
				}
			} else {
				// --- Display all projects in the library
				result.addObject(PROJECTS_KEY, ProjectLibraryService
						.getAllProjectsFrom(projectLibrary));
			}
		}

		sortProjects(result);

		Collection<Category> categories = ProjectLibraryService
				.getCategoriesWithProjectsFrom(projectLibrary);
		result.addObject("categories", categories);

		int totalProjects = 0;
		for (Category category : categories) {
			if (category.isTopLevelCategory()) {
				totalProjects += category.getProjectCount();
			}
		}
		result.addObject("totalProjects", totalProjects);

		return result;
	}

	protected abstract String getSearchURL();

	protected abstract boolean isDisplayAvailableLibraries();

	protected abstract ProjectLibrary getCurrentLibrary(
			HttpServletRequest request, List<ProjectLibrary> availableLibraries);

	protected abstract String getViewName();

	@SuppressWarnings("unchecked")
	private static void sortProjects(ModelAndView result) {
		List<LibraryProject> projects = (List<LibraryProject>) result
				.getModel().get(PROJECTS_KEY);
		if (projects == null) {
			throw new IllegalStateException("Didn't find projects under '"
					+ PROJECTS_KEY + "' key");
		}

		Collections.sort(projects, new Comparator<LibraryProject>() {

			public int compare(LibraryProject o1, LibraryProject o2) {
				if (o1.getRating() != o2.getRating())
					return o2.getRating() - o1.getRating();

				if (o1.getDownloadCount() != o2.getDownloadCount())
					return o2.getDownloadCount() - o1.getDownloadCount();

				if (o1.getCommentCount() != o2.getCommentCount()) {
					return o2.getCommentCount() - o1.getCommentCount();
				}

				return o1.getName().compareToIgnoreCase(o2.getName());
			}
		});
	}
}
