package com.tawala.web.user;

import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.springframework.validation.BindException;
import org.springframework.validation.ObjectError;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.support.RequestContext;

import com.scissor.Log;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.web.projectmanager.LastClonedLibraryProjectCustomizationController;
import com.tawala.web.projectmanager.ClonedLibraryProjectCustomizationContext;

public class RegistrationDuringCustomizationController extends
		UserRegistrationController {

	@SuppressWarnings("unchecked")
	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		UserForm form = (UserForm) command;
		checkForDuplicateIds(errors, form);
		if (errors.hasErrors()) {
			response.getOutputStream().print(
					prepareResponse(false, request, errors.getAllErrors()));
			return null;
		}

		registerUser(errors, form);

		if (errors.hasErrors()) {
			response.getOutputStream().print(
					prepareResponse(false, request, errors.getAllErrors()));
			return null;
		}

		// --- We need to copy the customization context in case a refresh
		// happens.
		ClonedLibraryProjectCustomizationContext customizationContext = (ClonedLibraryProjectCustomizationContext) request
				.getSession()
				.getAttribute(
						LastClonedLibraryProjectCustomizationController.ATTRIBUTE_CUSTOMIZATION_CONTEXT);

		LoginController.onUserLogin(request, response, form.getUser(), false);

		request
				.getSession()
				.setAttribute(
						LastClonedLibraryProjectCustomizationController.ATTRIBUTE_CUSTOMIZATION_CONTEXT,
						customizationContext);

		EventService.createEvent(new Event("RegistrationDuringCustomization",
				request, form.getUser().getId()));

		response.getOutputStream().print(prepareSuccessResponse());

		return null;
	}

	@SuppressWarnings("unchecked")
	@Override
	protected ModelAndView showForm(HttpServletRequest request,
			HttpServletResponse response, BindException errors)
			throws Exception {
		Log.info(this, "Signup failure from " + request.getRemoteAddr());
		response.getOutputStream().print(
				prepareResponse(false, request, errors.getAllErrors()));
		return null;
	}

	private static String prepareSuccessResponse() throws JSONException {
		return prepareResponse(true, null, null);
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
		return "signupResponse = " + result.toString() + ";";
	}
}
