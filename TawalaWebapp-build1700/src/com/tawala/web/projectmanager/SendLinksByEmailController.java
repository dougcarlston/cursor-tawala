package com.tawala.web.projectmanager;

import java.util.Collections;
import java.util.List;

import javax.mail.internet.InternetAddress;
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
import com.tawala.domain.notification.ProjectLinksMessage;
import com.tawala.email.Emailer;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.LinkToUserProject;
import com.tawala.web.WorldInitializer;

public class SendLinksByEmailController implements Controller {
	public static final String PROJECT_ID_PARAMETER = "project_id";
	public static final String EMAIL_PARAMETER = "email";
	public static final String ORIGINAL_PROJECT_NAME_PARAMETER = "project_name";

	private static String prepareSuccessResponse(HttpServletRequest request)
			throws JSONException {
		return prepareResponse(true, request, null);
	}

	private static String prepareResponse(boolean success,
			HttpServletRequest request, List<ObjectError> errors)
			throws JSONException {
		JSONObject result = new JSONObject();
		result.put("success", success);

		if (errors != null) {
			RequestContext requestContext = new RequestContext(request);
			JSONArray messages = new JSONArray();
			for (ObjectError error : errors) {
				messages.put(requestContext.getMessage(error, false));
			}
			result.put("messages", messages);
		}
		return "var sendResponse = " + result.toString() + ";";
	}

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String projectId = request.getParameter(PROJECT_ID_PARAMETER);
		String email = request.getParameter(EMAIL_PARAMETER);
		String projectName = request.getParameter(ORIGINAL_PROJECT_NAME_PARAMETER);

		LinkToUserProject link = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(projectId);
		if (link == null) {
			Log.error(this, "Unable to find project under id: " + projectId);
			response
					.getOutputStream()
					.print(
							prepareResponse(
									false,
									request,
									Collections
											.singletonList(new ObjectError(
													"error", new String[] {},
													new String[] {},
													"We are sorry, this operation is not currently available."))));
			return null;
		}

		try {
			new InternetAddress(email);
			if (email.indexOf('@') < 0) {
				throw new IllegalArgumentException("No @ sign.");
			}
			if (email.indexOf('.') < 0) {
				throw new IllegalArgumentException("Not a single period.");
			}

			ProjectLinksMessage message = new ProjectLinksMessage(email, link.getProject(), projectName);

			Emailer.getSender().send(message);
		} catch (Throwable e) {
			Log.warn(this, "Email address '" + email + "' is invalid:", e);
			response
					.getOutputStream()
					.print(
							prepareResponse(
									false,
									request,
									Collections
											.singletonList(new ObjectError(
													"emailIncorrect",
													new String[] {},
													new String[] {},
													"Email address is incorrect. Please try again"))));
			return null;
		}

		EventService.createEvent(new Event("sendEmailWithProjectLinks", request, email));
		Log.info(this, "Email with notification links is sent to '" + email + "'");
		response.getOutputStream().print(prepareSuccessResponse(request));

		return null;
	}
}
