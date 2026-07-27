package com.tawala.web.email;

import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.email.EmailService;
import com.tawala.email.UserProjectEmail;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class RetrieveNextProjectEmailPageController implements Controller {
	public static final String USER_PROJECT_ID_PARAMETER = "user_project_id";
	public static final String START_INDEX_PARAMETER = "startIndex";
	public static final String ROWS_PER_PAGE_PARAMETER = "results";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		long userProjectId = Long.parseLong(request.getParameter(USER_PROJECT_ID_PARAMETER));
		int startIndex = Integer.parseInt(request.getParameter(START_INDEX_PARAMETER));
		int rowsPerPage = Integer.parseInt(request.getParameter(ROWS_PER_PAGE_PARAMETER));
		List<UserProjectEmail> nextRows = EmailService.getNextPageOfEmailsForProject(user, userProjectId,
			startIndex, rowsPerPage);
		
		long totalEmails = EmailService.getProjectEmailCount(user, userProjectId);

		JSONArray records = new JSONArray();
		for (UserProjectEmail email : nextRows) {
			JSONObject record = new JSONObject();
	        record.put("status", email.getState().getShortDescription());
	        record.put("to", email.getTo());
	        record.put("cc", email.getCc());
	        record.put("from", email.getFrom());
	        record.put("subject", email.getSubject());
	        record.put("date_created", email.getCreatedDate());
	        record.put("id", email.getId());
			
			records.put(record);
		}

		JSONObject result = new JSONObject();
		result.put("recordsReturned", nextRows.size());
		result.put("totalRecords", totalEmails);
		result.put("startIndex", startIndex);
		result.put("dir", "asc");
		result.put("sort", JSONObject.NULL);
		result.put("records", records);

		response.getOutputStream().print(result.toString());
		
		return null;
	}
}
