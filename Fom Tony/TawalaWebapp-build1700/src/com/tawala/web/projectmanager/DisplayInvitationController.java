package com.tawala.web.projectmanager;

import java.util.LinkedHashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.email.InvitationToProjectMailToURLBuilder;
import com.tawala.project.Form;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class DisplayInvitationController implements Controller {
	public static final String PROJECT_NAME_PARAMETER = "project_name";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String projectName = request.getParameter(PROJECT_NAME_PARAMETER);

		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(
						UserInfoPreparationInterceptor.getSessionUser(request)
								.getId(), projectName);
		
		Map<Form, InvitationToProjectMailToURLBuilder> invitations = new LinkedHashMap<Form, InvitationToProjectMailToURLBuilder>();
		for (Form form : userProject.getProject().getForms()) {
			if(form.isStartingPoint()) {
				invitations.put(form, new InvitationToProjectMailToURLBuilder(userProject,form));
			}
		}
		
		ModelAndView result = new ModelAndView(
				"projectmanager.invite.to.project");
		
		result.addObject("project", userProject);
		result.addObject("invitations", invitations);
		
		return result;
	}
}
