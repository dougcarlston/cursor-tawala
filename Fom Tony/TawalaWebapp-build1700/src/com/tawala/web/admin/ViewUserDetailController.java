package com.tawala.web.admin;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.ProjectFilter;
import com.tawala.project.ProjectSortOrder;
import com.tawala.project.ProjectStatistics;
import com.tawala.project.library.LibraryChangeEvent;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.util.DateUtil;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ViewRecentLibraryChangesController;
import com.tawala.web.projectmanager.ProjectPagingInfo;

public class ViewUserDetailController implements Controller {
	private static enum ExecutionDirection {
		DISPLAY_CURRENT_USER_PAGE, RETURN_TO_MANAGE_USERS
	}

	public static final String USER_ID_PARAMETER = "user_id";
	public static final String LIBRARY_CHANGES_SINCE_PARAMETER = "days";
	public static final int DEFAULT_DAYS_TO_DISPLAY = 7;

	public final static String PARAMETER_ACTION = "actionSelected";
	public final static String PARAMETER_ACTION_APPROVE = "approve";
	public final static String PARAMETER_ACTION_DELETE = "delete";
	public final static String PARAMETER_ACTION_SUSPEND = "suspend";
	public final static String PARAMETER_ACTION_RELEASE = "release";
	public final static String PARAMETER_ACTION_SEARCH = "search";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		long id = Long.parseLong(request.getParameter(USER_ID_PARAMETER));
		User user = WorldInitializer.getDefaultWorld().domain().users().get(id);
		if (user == null) {
			response.sendRedirect(WellKnown.urls.getAdminManageUsers());
			return null;
		}

		List<Message> messages = new ArrayList<Message>();

		ExecutionDirection executionDirection = performAction(request, user,
				messages);
		switch (executionDirection) {
		case DISPLAY_CURRENT_USER_PAGE:
			return displayUserDetails(request, user);

		case RETURN_TO_MANAGE_USERS:
			response.sendRedirect(WellKnown.urls.getAdminManageUsers());
			return null;

		default:
			throw new IllegalStateException("Unhandled case");
		}
	}

	private ModelAndView displayUserDetails(HttpServletRequest request,
			User user) {
		int daysOfChangesToDisplay = DEFAULT_DAYS_TO_DISPLAY;
		String daysOfChangesToDisplayParameter = request
				.getParameter(LIBRARY_CHANGES_SINCE_PARAMETER);
		if (daysOfChangesToDisplayParameter != null) {
			daysOfChangesToDisplay = Integer
					.parseInt(daysOfChangesToDisplayParameter);
		}

		Date startFrom = DateUtil
				.dateEarlierStartingAt12am(daysOfChangesToDisplay);

		ModelAndView result = new ModelAndView("view.user.detail");
		result.addObject("currentUser", user);

		Collection<LibraryChangeEvent> libraryChangeEvents = ProjectLibraryService
				.getChangesByUserSince(user, startFrom);

		ViewRecentLibraryChangesController.populateModelBeanWithEvents(result,
				libraryChangeEvents, daysOfChangesToDisplay);

		ProjectPagingInfo projectPagingInfo = new ProjectPagingInfo();
		projectPagingInfo.setMax(20);
		projectPagingInfo
				.setSortOrder(ProjectSortOrder.lastUpdatedDateDescending);
		projectPagingInfo.setStart(0);
		projectPagingInfo.setFilter(ProjectFilter.all);

		List<ProjectStatistics> projectStats = WorldInitializer
				.getDefaultWorld().domain().projects().getProjectStatistics(
						user, projectPagingInfo.getFilter(),
						projectPagingInfo.getSortOrder(),
						projectPagingInfo.getStart(),
						projectPagingInfo.getMax());

		result.addObject("otherUserProjectStats", projectStats);
		result.addObject("otherUserProjectPagingInfo", projectPagingInfo);
		result.addObject("otherUserProjectCount", WorldInitializer
				.getDefaultWorld().domain().projects().projectCountFor(user));

		return result;
	}

	private ExecutionDirection performAction(HttpServletRequest request,
			User user, List<Message> messages) throws Exception {
		String actionParameter = request.getParameter(PARAMETER_ACTION);
		if (actionParameter == null)
			return ExecutionDirection.DISPLAY_CURRENT_USER_PAGE;

		if (PARAMETER_ACTION_APPROVE.equals(actionParameter)) {
			return doApprove(request, user, messages);
		} else if (PARAMETER_ACTION_DELETE.equals(actionParameter)) {
			return doDelete(request, user, messages);
		} else if (PARAMETER_ACTION_RELEASE.equals(actionParameter)) {
			return doRelease(request, user, messages);
		} else if (PARAMETER_ACTION_SUSPEND.equals(actionParameter)) {
			return doSuspend(request, user, messages);
		} else {
			throw new IllegalStateException("Unhandled action: "
					+ actionParameter);
		}
	}

	private ExecutionDirection doSuspend(HttpServletRequest request, User user,
			List<Message> messages) {
		WorldInitializer.getDefaultWorld().domain().users().onUserSuspension(
				user);
		Log.info(this, "User " + user.getId() + " has been suspended.");

		messages.add(new Message("admin.user.suspended", user.getId(), user
				.getEmail()));

		return ExecutionDirection.DISPLAY_CURRENT_USER_PAGE;
	}

	private ExecutionDirection doRelease(HttpServletRequest request, User user,
			List<Message> messages) {
		WorldInitializer.getDefaultWorld().domain().users()
				.onUserBeingReleasedFromSuspension(user);
		Log.info(this, "User " + user.getId()
				+ " has been released from suspension.");

		messages.add(new Message("admin.user.released.from.suspension", user
				.getId(), user.getEmail()));

		return ExecutionDirection.DISPLAY_CURRENT_USER_PAGE;
	}

	private ExecutionDirection doDelete(HttpServletRequest request, User user,
			List<Message> messages) throws Exception {
		WorldInitializer.getDefaultWorld().domain().users().delete(user);
		Log.info(this, "User " + user.getId() + " has been deleted.");

		messages.add(new Message("admin.user.deleted", user.getId(), user
				.getEmail()));

		return ExecutionDirection.RETURN_TO_MANAGE_USERS;
	}

	private ExecutionDirection doApprove(HttpServletRequest request, User user,
			List<Message> messages) {
		WorldInitializer.getDefaultWorld().domain().users()
				.onUserApproval(user);
		messages.add(new Message("admin.user.approved", new Object[] {
				user.getId(), user.getEmail() }));

		Log.info(this, "User " + user.getId() + " has been approved.");

		return ExecutionDirection.DISPLAY_CURRENT_USER_PAGE;
	}

}
