package com.tawala.web.projectmanager;

import java.util.Arrays;
import java.util.Collections;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.FormSubmissionsHibernateImpl;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class ChangeSubmissionFieldValueController implements Controller {
	public static final String USER_PROJECT_ID_PARAMETER = "user_project_id";
	public static final String SHARED_DATA_PARAMETER = "shared_data";
	public static final String SUBMISSION_ID_PARAMETER = "record_id";
	public static final String FIELD_NAME_PARAMETER = "field";
	public static final String VALUES_PARAMETER = "values";

	@SuppressWarnings("unchecked")
	public ModelAndView handleRequest(final HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		boolean sharedData = Boolean.parseBoolean(request
				.getParameter(SHARED_DATA_PARAMETER));

		String[] values = request.getParameterValues(VALUES_PARAMETER);
		long formSubmissionId = Long.parseLong(request
				.getParameter(SUBMISSION_ID_PARAMETER));
		String fieldName = request.getParameter(FIELD_NAME_PARAMETER);
		List data = values == null ? Collections.EMPTY_LIST : Arrays
				.asList(values);
		
		boolean result = false;
		if(sharedData) {
			result = FormSubmissionsHibernateImpl
			.changeFormSubmissionValueForSharedData(UserInfoPreparationInterceptor
					.getSessionUser(request),
					formSubmissionId, fieldName, data);
		} else {
			long userProjectId = Long.parseLong(request
					.getParameter(USER_PROJECT_ID_PARAMETER));

			result = FormSubmissionsHibernateImpl
					.changeFormSubmissionValueForRegularProject(UserInfoPreparationInterceptor
							.getSessionUser(request), userProjectId,
							formSubmissionId, fieldName, data);
		}

		JSONObject responseObject = new JSONObject();
		responseObject.put("success", result);

		response.setContentType("text/plain");
		response.getWriter().write(responseObject.toString());

		return null;
	}
}
