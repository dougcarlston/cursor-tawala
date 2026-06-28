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
import org.springframework.web.servlet.mvc.SimpleFormController;
import org.springframework.web.servlet.support.RequestContext;

import com.scissor.Log;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.web.projectmanager.LastClonedLibraryProjectCustomizationController;
import com.tawala.web.projectmanager.ClonedLibraryProjectCustomizationContext;

public class LoginDuringCustomizationController extends SimpleFormController {
	public LoginDuringCustomizationController() {
		setValidator(new LoginValidator());
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		return new LoginForm();
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		LoginForm form = (LoginForm) command;

		//--- We need to copy the customization context in case a refresh happens.
		ClonedLibraryProjectCustomizationContext customizationContext = (ClonedLibraryProjectCustomizationContext) request
				.getSession()
				.getAttribute(
						LastClonedLibraryProjectCustomizationController.ATTRIBUTE_CUSTOMIZATION_CONTEXT);
		LoginController.onUserLogin(request, response, form.getUser(), form
				.isKeepSignedIn());

		request
				.getSession()
				.setAttribute(
						LastClonedLibraryProjectCustomizationController.ATTRIBUTE_CUSTOMIZATION_CONTEXT,
						customizationContext);

		Log.info(this, "successful login during customization for '"
				+ form.getUserName() + "' from " + request.getRemoteAddr());
		Log.info(this, "Using browser: " + request.getHeader("User-Agent"));

		EventService.createEvent(new Event("LoginDuringCustomization", request,
				form.getUser().getId()));

		response.getOutputStream().print(prepareSuccessResponse());

		return null;
	}

	/*
	 * This method gets invoked in case there are errors. We output the response
	 * directly since there is no formatting, just data.
	 * 
	 * @see org.springframework.web.servlet.mvc.SimpleFormController#showForm(javax.servlet.http.HttpServletRequest,
	 *      javax.servlet.http.HttpServletResponse,
	 *      org.springframework.validation.BindException)
	 */
	@SuppressWarnings("unchecked")
	@Override
	protected ModelAndView showForm(HttpServletRequest request,
			HttpServletResponse response, BindException errors)
			throws Exception {
		Log.info(this, "Login failure from " + request.getRemoteAddr());
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
		return "serverResponse = " + result.toString() + ";";
	}
}
