package com.tawala.web.projectmanager;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.World;
import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.Project;
import com.tawala.project.data.DataSource;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ViewSharedDataSourcesController implements Controller {
	public static final String PARAMETER_FORM_NAME = "form";

	// CAREFUL: these names are used by JavaScript on the confirmation dialogs.
	// See popup.js for details. SL.
	public static final String PARAMETER_ACTION_ERASE = "Erase";

	public static final String PARAMETER_ACTION_DELETE = "Delete";

	public static final String PARAMETER_ACTION_PURGE = "Purge";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		World world = WorldInitializer.getDefaultWorld();

		List<Message> messages = new ArrayList<Message>();
		String action = request.getParameter("action");
		if (action != null) {
			doAction(request, response, messages, action);
			response.sendRedirect(WellKnown.urls.getViewSharedDatasources());
			return null;
		} else {
			Project project = world.domain().users().getSharedStorageForUser(
					user);
			Map<DataSource, Long> responseCounts = new HashMap<DataSource, Long>();
			for (DataSource dataSource : project.getDataSources()) {
				responseCounts.put(dataSource, world.domain().storedData()
						.responseCount(project, dataSource.getName()));
			}

			ModelAndView result = new ModelAndView(
					"projectmanager.listDataSources");
			result.addObject("sharedData", project);
			result.addObject("responseCounts", responseCounts);

			return result;
		}
	}

	private void doAction(HttpServletRequest request,
			HttpServletResponse response, List<Message> messages, String action)
			throws Exception {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		Project sharedData = WorldInitializer.getDefaultWorld().domain()
				.users().getSharedStorageForUser(user);

		if (action.equalsIgnoreCase(PARAMETER_ACTION_DELETE)) {
			String dataSourceName = request.getParameter(PARAMETER_FORM_NAME);
			WorldInitializer.getDefaultWorld().domain().projects()
					.removeDataSource(WorldInitializer.getDefaultWorld(), sharedData, dataSourceName);
		} else if (action.equalsIgnoreCase(PARAMETER_ACTION_ERASE)) {
			String formName = request.getParameter(PARAMETER_FORM_NAME);
			WorldInitializer.getDefaultWorld().domain().storedData()
					.eraseResponsesFor(sharedData, formName);
		} else {
			Log.error(this, "Unknown action '" + action + "'");
		}
	}
}
