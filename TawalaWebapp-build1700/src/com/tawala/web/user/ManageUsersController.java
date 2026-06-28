package com.tawala.web.user;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.lucene.queryParser.ParseException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.util.DateUtil;
import com.tawala.web.WorldInitializer;

public class ManageUsersController implements Controller {
	public final static String PARAMETER_REINDEX_USERS = "reindexUsers";
	public final static String PARAMETER_ACTION_SEARCH_BY_STATUS = "searchByStatus";
	public final static String PARAMETER_ACTION_SEARCH_BY_REGISTRATION_DATE = "searchByRegistration";
	public static final String PARAMETER_REGISTRATION_DAYS_BACK = "registrationDaysBack";

	public final static String PARAMETER_STATUS = "status";
	public final static String PARAMETER_QUERY = "query";
	public final static String PARAMETER_USER_ID = "id";

	public final static Collection<Status> SEARCHABLE_STATUSES = new ArrayList<Status>();
	static {
		SEARCHABLE_STATUSES.add(Status.EMAIL_VALIDATED);
		SEARCHABLE_STATUSES.add(Status.EMAIL_UNVALIDATED);
	}

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		ModelAndView result = new ModelAndView("admin.manage.users");

		List<Message> messages = new ArrayList<Message>();
		if (request.getParameter(PARAMETER_REINDEX_USERS) != null) {
			doReindex(messages);
		}

		String query = request.getParameter(PARAMETER_QUERY);
		if (request.getParameter(PARAMETER_ACTION_SEARCH_BY_STATUS) != null) {
			doSearchByStatus(request, result);
		} else if (request
				.getParameter(PARAMETER_ACTION_SEARCH_BY_REGISTRATION_DATE) != null) {
			doSearchByRegistration(request, result);
		} else if (query != null) {
			try {
				doSearch(request, messages, query, result);
			} catch (Exception e) {
				Log.warn(this, "User search failed: ", e);
				messages.add(new Message("query.error"));
			}
		}

		retrieveProjectCounts(result);
		
		result.addObject("messages", messages);
		result.addObject("searchableStatuses", SEARCHABLE_STATUSES);

		return result;
	}

	@SuppressWarnings("unchecked")
	private void retrieveProjectCounts(ModelAndView result) {
		List<User> users = (List<User>)result.getModel().get("users");
		if(users == null || users.size() == 0) {
			return;
		}
		
		Map<User, Long> projectCounts = new HashMap<User, Long>(users.size());
		for (User user : users) {
			projectCounts.put(user, WorldInitializer.getDefaultWorld().domain().projects().projectCountFor(user));
		}
		
		result.addObject("projectCounts", projectCounts);
	}

	private void doSearchByRegistration(HttpServletRequest request,
			ModelAndView result) {
		String daysBackParameter = request
				.getParameter(PARAMETER_REGISTRATION_DAYS_BACK);
		int daysBack = Integer.parseInt(daysBackParameter.trim());

		result.addObject("registrationDaysBack", daysBackParameter);
		result.addObject("users", WorldInitializer.getDefaultWorld().domain()
				.users().findUsersRegisteredSince(
						DateUtil.dateEarlierStartingAt12am(daysBack)));
	}

	private void doSearchByStatus(HttpServletRequest request,
			ModelAndView result) {
		String statusName = request.getParameter(PARAMETER_STATUS);
		Status status = Status.valueOf(statusName);
		result.addObject("status", status);
		result.addObject("users", WorldInitializer.getDefaultWorld().domain()
				.users().findUsersWithStatus(status));
	}

	private void doReindex(List<Message> messages) {
		WorldInitializer.getDefaultWorld().domain().users().reindexUsers();
		messages.add(new Message("user.reindexing.done"));
	}

	private void doSearch(HttpServletRequest request, List<Message> messages,
			String query, ModelAndView result) throws ParseException,
			IOException {
		result.addObject("query", query);
		result.addObject("users", WorldInitializer.getDefaultWorld().domain()
				.users().search(query));
	}
}