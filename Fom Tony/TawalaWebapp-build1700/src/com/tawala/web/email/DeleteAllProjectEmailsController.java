package com.tawala.web.email;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.email.EmailService;

public class DeleteAllProjectEmailsController implements Controller {
	public static final String USER_PROJECT_ID_PARAMETER = "user_project_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		
		int deletedEmails = EmailService.deleteAllEmailsForProject(Long.parseLong(request.getParameter(USER_PROJECT_ID_PARAMETER)));

		JSONObject result = new JSONObject();
		result.put("deleteCount", deletedEmails);

		response.getOutputStream().print(result.toString());
		
		return null;
	}
}
