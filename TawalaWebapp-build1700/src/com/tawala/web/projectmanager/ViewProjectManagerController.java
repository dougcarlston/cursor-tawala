package com.tawala.web.projectmanager;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.ProjectFilter;
import com.tawala.project.ProjectSortOrder;
import com.tawala.project.ProjectStatistics;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ViewProjectManagerController implements Controller {
	private static final int DEFAULT_NUMBER_OF_PROJECTS_PER_PAGE = 30;
	public static final String PROJECT_PAGING_INFO_ATTRIBUTE = "projectPagingInfo";
	public static final String PROJECT_STATISTICS_ATTRIBUTE = "projectStatistics";

	public static final String START_PARAMETER = "start";
	public static final String MAX_PROJECTS_PER_PAGE_PARAMETER = "max";
	public static final String SORT_PARAMETER = "sort";
	public static final String FILTER_PARAMETER = "filter";

	private static final String PAGING_ATTRIBUTE_NAME = "tawala.project.paging";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		ModelAndView result = new ModelAndView("projectmanager.view");

		List<Message> messages = new ArrayList<Message>();
		String action = request.getParameter("action");
		if (action != null) {
			if(doAction(request, response, messages, action)) {
				return null;
			}
		}

		ProjectPagingInfo projectPagingInfo = extractPagingInfo(request);

		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		int totalProjectCount = (int) WorldInitializer.getDefaultWorld()
				.domain().projects().projectCountFor(user);
		if (projectPagingInfo.getStart() >= totalProjectCount) {
			projectPagingInfo.setStart(Math.max(0, totalProjectCount
					- projectPagingInfo.getMax()));
		}

		List<ProjectStatistics> projectStatistics = WorldInitializer
				.getDefaultWorld().domain().projects().getProjectStatistics(
						user, projectPagingInfo.getFilter(),
						projectPagingInfo.getSortOrder(),
						projectPagingInfo.getStart(),
						projectPagingInfo.getMax());

		result.addObject(PROJECT_STATISTICS_ATTRIBUTE, projectStatistics);
		result.addObject(PROJECT_PAGING_INFO_ATTRIBUTE, projectPagingInfo);
		result.addObject("availableSortOrders", ProjectSortOrder.values());
		result.addObject("availableFilters", ProjectFilter.values());
		result.addObject("sharedStorage", WorldInitializer.getDefaultWorld()
				.domain().users().getSharedStorageForUser(user));
		result.addObject("inactiveProjectCount", WorldInitializer
				.getDefaultWorld().domain().projects().inactiveProjectCountFor(
						user));

		result.addObject("messages", messages);
		result.addObject("userThemes", UsersHibernateImpl.getAllUserThemes(user));
		result.addObject("sportsdashboardsGroups", UsersHibernateImpl.getAllUserSportsDashboardGroups(user));
		return result;
	}

	private ProjectPagingInfo extractPagingInfo(HttpServletRequest request) {
		HttpSession session = request.getSession();
		ProjectPagingInfo result = (ProjectPagingInfo) session
				.getAttribute(PAGING_ATTRIBUTE_NAME);
		if (result == null) {
			result = new ProjectPagingInfo();
			result.setMax(DEFAULT_NUMBER_OF_PROJECTS_PER_PAGE);
			result.setStart(0);
			result.setSortOrder(ProjectSortOrder.nameAscending);
			result.setFilter(ProjectFilter.all);

			session.setAttribute(PAGING_ATTRIBUTE_NAME, result);
		}

		String sortOrder = request.getParameter(SORT_PARAMETER);
		if (sortOrder != null) {
			result.setSortOrder(ProjectSortOrder.valueOf(sortOrder));
		}

		String filter = request.getParameter(FILTER_PARAMETER);
		if (filter != null) {
			result.setFilter(ProjectFilter.valueOf(filter));
		}

		String start = request.getParameter(START_PARAMETER);
		if (start != null) {
			result.setStart(Integer.parseInt(start));
		}

		String maxProjectsPerPage = request
				.getParameter(MAX_PROJECTS_PER_PAGE_PARAMETER);
		if (maxProjectsPerPage != null) {
			result.setMax(Integer.parseInt(maxProjectsPerPage));
		}

		return result;
	}

	/**
	 * @return true if the action resulted in a redirect. Means that no further processing is needed.
	 * @throws IOException 
	 */
	private boolean doAction(HttpServletRequest request, HttpServletResponse response, List<Message> messages,
			String action) throws IOException {
		String projectName = request.getParameter("project");
		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		if (projectName == null) {
			Log.warn(this, "Couldn't find parameter '" + projectName + "'.");
			return false;
		}

		UserProject project = WorldInitializer.getDefaultWorld().domain()
				.projects().get(user.getId(), projectName);

		if (project == null) {
			Log.warn(this, "Couldn't find project '" + projectName + "'.");
			return false;
		}

		if (action.equalsIgnoreCase("Purge")) {
			WorldInitializer.getDefaultWorld().domain().storedData()
					.purgeProjectResponses(project.getProject());
		} else if (action.equalsIgnoreCase("Delete")) {
			WorldInitializer.getDefaultWorld().domain().projects().delete(
					project, WorldInitializer.getDefaultWorld());
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return true;
		}
		return false;
	}
}
