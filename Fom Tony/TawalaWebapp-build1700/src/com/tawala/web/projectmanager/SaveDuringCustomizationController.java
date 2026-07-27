package com.tawala.web.projectmanager;

import java.util.Collections;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.springframework.validation.ObjectError;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;
import org.springframework.web.servlet.support.RequestContext;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class SaveDuringCustomizationController implements Controller {
	public static final String PROJECT_ID_PARAMETER = "project_id";
	public static final String PROJECT_NAME_PARAMETER = "name";

	private static String prepareSuccessResponse(HttpServletRequest request)
			throws JSONException {
		return prepareResponse(true, request, null);
	}

	private static String prepareResponse(boolean success,
			HttpServletRequest request, List<ObjectError> errors)
			throws JSONException {
		JSONObject result = new JSONObject();
		result.put("success", success);

		if (success) {
			User user = UserInfoPreparationInterceptor.getSessionUser(request);
			result.put("userName", user.getId());

			EventService.createEvent(new Event(
					"SaveToMyTawalaDuringCustomization", request));
		}

		if (errors != null) {
			RequestContext requestContext = new RequestContext(request);
			JSONArray messages = new JSONArray();
			for (ObjectError error : errors) {
				messages.put(requestContext.getMessage(error, false));
			}
			result.put("messages", messages);
		}
		return "saveResponse = " + result.toString() + ";";
	}

	// --- TODO: ability to handle expired sessions.
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		long projectId = Long.parseLong(request
				.getParameter(PROJECT_ID_PARAMETER));

		String name = request.getParameter(PROJECT_NAME_PARAMETER).trim();
		if (name.length() == 0) {
			response
					.getOutputStream()
					.print(
							prepareResponse(
									false,
									request,
									Collections
											.singletonList(new ObjectError(
													"projectName",
													new String[] { "submitted.project.name.empty" },
													new String[] {},
													"Name is required."))));
			return null;

		}
		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().get(user.getId(), name);
		if (userProject != null) {
			response
					.getOutputStream()
					.print(
							prepareResponse(
									false,
									request,
									Collections
											.singletonList(new ObjectError(
													"projectName",
													new String[] { "submitted.project.duplicate.keys" },
													new String[] { name },
													"Project with this name already exists"))));
			return null;
		}

		try {
			WorldInitializer.getDefaultWorld().domain().projects()
					.changeProjectNameAndOwnership(projectId, name, user);
		} catch (Throwable e) {
			Log.error(this, "Failed to save change project ownership:", e);
			response
					.getOutputStream()
					.print(
							prepareResponse(
									false,
									request,
									Collections
											.singletonList(new ObjectError(
													"projectName",
													new String[] {},
													new String[] {},
													"Error saving project. Please try again"))));
		}

		response.getOutputStream().print(prepareSuccessResponse(request));

		return null;
	}
}
