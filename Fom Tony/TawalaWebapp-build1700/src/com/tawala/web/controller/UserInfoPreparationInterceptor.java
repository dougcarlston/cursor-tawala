package com.tawala.web.controller;

import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;

import com.tawala.domain.User;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.projectmanager.ProjectInfo;

public class UserInfoPreparationInterceptor implements HandlerInterceptor {

	public static final String PROJECT_COUNT_ATTRIBUTE = "projectCount";

	public boolean preHandle(HttpServletRequest request,
			HttpServletResponse response, Object handler) throws Exception {
		return true;
	}

	public void postHandle(HttpServletRequest request,
			HttpServletResponse response, Object handler,
			ModelAndView modelAndView) throws Exception {
		if (modelAndView == null)
			return;

		User user = getSessionUser(request);

		if (user != null) {
			List<UserProject> userProjects = ProjectsHibernateImpl.getFirstXUserProjects(user);
			List<ProjectInfo> projectsInfo = new ArrayList<ProjectInfo>();

			for (UserProject proj : userProjects) {
				ProjectInfo info = new ProjectInfo(WorldInitializer
						.getDefaultWorld(), proj, user);
				projectsInfo.add(info);
			}
			modelAndView.addObject("projectsInfo", projectsInfo);
			modelAndView.addObject(PROJECT_COUNT_ATTRIBUTE, WorldInitializer
					.getDefaultWorld().domain().projects()
					.projectCountFor(user));
		}
	}

	/**
	 * TODO: find a better home for this method.
	 * 
	 * @param request
	 * @return
	 */
	public static User getSessionUser(HttpServletRequest request) {
		HttpSession session = request.getSession(false);
		if (session == null) {
			return null;
		}
		return (User) session.getAttribute("user");
	}

	public void afterCompletion(HttpServletRequest request,
			HttpServletResponse response, Object handler, Exception ex)
			throws Exception {
		// --- Do nothing
	}
}
