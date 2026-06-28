package com.tawala.web.project;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

public class KeepHttpSessionAliveController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		HttpSession session = request.getSession(false);
		
		JSONObject responseObject = new JSONObject();
		responseObject.put("expired", session == null);
		response.getOutputStream().print(responseObject.toString());
		return null;
	}
}
