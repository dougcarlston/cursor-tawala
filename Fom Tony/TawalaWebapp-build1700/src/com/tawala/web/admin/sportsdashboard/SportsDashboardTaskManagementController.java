package com.tawala.web.admin.sportsdashboard;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.jbpm.JbpmContext;
import org.jbpm.taskmgmt.exe.TaskInstance;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.Role;
import com.tawala.domain.User;
import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.Constants;
import com.tawala.jbpm.sportsdashboards.UserProjectProcessTask;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class SportsDashboardTaskManagementController implements Controller {

	@SuppressWarnings("unchecked")
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		ModelAndView result = new ModelAndView("admin.sports-dashboard.task-management");

		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		JbpmContext jbpmContext = JbpmService.createContext();
		try {
			List<UserProjectProcessTask> currentUserTasks = retrieveCorrespondingUserProjects(jbpmContext
					.getTaskList(user.getId()));
			
			Comparator<UserProjectProcessTask> byCreatedDateComparator = new Comparator<UserProjectProcessTask>() {

				public int compare(UserProjectProcessTask o1,
						UserProjectProcessTask o2) {
					return o1.getTaskInstance().getCreate().compareTo(o2.getTaskInstance().getCreate());
				}};
			Collections.sort(currentUserTasks, byCreatedDateComparator);
			
			result.addObject("currentUserTasks",
					currentUserTasks);

			List<UserProjectProcessTask> unassignedTasks = new ArrayList<UserProjectProcessTask>();
			for (Role role : user.getRoles()) {
				unassignedTasks
						.addAll(retrieveCorrespondingUserProjects(jbpmContext
								.getGroupTaskList(Collections
										.singletonList(role.getRoleId()))));
			}
			Collections.sort(unassignedTasks, byCreatedDateComparator);
			result.addObject("unassignedTasks", unassignedTasks);
		} finally {
			jbpmContext.close();
		}

		return result;
	}

	private List<UserProjectProcessTask> retrieveCorrespondingUserProjects(
			List<TaskInstance> tasks) {
		if (tasks == null) {
			return null;
		}

		List<UserProjectProcessTask> result = new ArrayList<UserProjectProcessTask>(
				tasks.size());
		for (TaskInstance taskInstance : tasks) {
			long userProjectId = (Long) taskInstance
					.getProcessInstance().getContextInstance().getVariable(
							Constants.PROJECT_ID);
			UserProject project = ProjectsHibernateImpl
					.getUserProjectById(userProjectId);
			if (project == null) {
				Log.error(this,
						"Unable to find user project for an instance of workflow. User project id = "
								+ userProjectId);
			}
			result.add(new UserProjectProcessTask(project, taskInstance));
		}
		return result;
	}
}
