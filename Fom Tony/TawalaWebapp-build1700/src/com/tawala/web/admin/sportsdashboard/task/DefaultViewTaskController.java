package com.tawala.web.admin.sportsdashboard.task;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.jbpm.JbpmContext;
import org.jbpm.taskmgmt.exe.TaskInstance;
import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.Constants;
import com.tawala.jbpm.sportsdashboards.UserProjectProcessTask;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.admin.sportsdashboard.ViewProjectWorkflowDetailsController;
import com.tawala.web.controller.WellKnown;

public class DefaultViewTaskController extends SimpleFormController {
	public static final String TASK_ID_PARAMETER = "task-id";

	public DefaultViewTaskController() {
		// --- Calling setFormView() in extending classes will override it.
		setFormView("admin.sports-dashboard.view.task");
	}

	@Override
	protected final ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		ViewTaskForm form = (ViewTaskForm) command;

		performTaskSpecificWork(form);

		String transitionName = form.getTransitionName();
		long processInstanceId = JbpmService.endTask(form.getProcessTask()
				.getTaskInstance().getId(), transitionName);

		response
				.sendRedirect(WellKnown.urls
						.getAdminViewProjectWorkflowDetails()
						+ '?'
						+ ViewProjectWorkflowDetailsController.PROCESS_INSTANCE_ID_PARAMETER
						+ '=' + processInstanceId);

		return null;
	}

	/**
	 * To be used by the extending classes
	 * 
	 * @param form
	 */
	protected void performTaskSpecificWork(ViewTaskForm form) {
		// --- Do nothing
	}

	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		List availableTransitions = ((ViewTaskForm) command).getProcessTask()
				.getTaskInstance().getAvailableTransitions();
		result.put("transitions", availableTransitions);
		return result;
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		return new ViewTaskForm(getRequestedTask(request));

	}

	protected UserProjectProcessTask getRequestedTask(HttpServletRequest request) {
		UserProjectProcessTask userProjectTask = null;
		JbpmContext jbpmContext = JbpmService.createContext();
		try {
			long taskId = Long.parseLong(request
					.getParameter(TASK_ID_PARAMETER));
			TaskInstance taskInstance = jbpmContext
					.getTaskInstanceForUpdate(taskId);
			if (taskInstance == null) {
				throw new IllegalStateException(
						"Unable to find task instance with id " + taskId);
			}

			long userProjectId = (Long) taskInstance.getProcessInstance()
					.getContextInstance().getVariable(Constants.PROJECT_ID);
			UserProject project = ProjectsHibernateImpl
					.getUserProjectById(userProjectId);

			// --- Needed to fetch the transaction.
			taskInstance.getAvailableTransitions();

			userProjectTask = new UserProjectProcessTask(project, taskInstance);
		} finally {
			jbpmContext.close();
		}
		return userProjectTask;
	}

	public static class ViewTaskForm {
		private UserProjectProcessTask processTask;
		private String transitionName;

		protected ViewTaskForm(UserProjectProcessTask taskInstance) {
			this.processTask = taskInstance;
		}

		public String getTransitionName() {
			return transitionName;
		}

		public void setTransitionName(String transitionName) {
			this.transitionName = transitionName;
		}

		public UserProjectProcessTask getProcessTask() {
			return processTask;
		}
	}
}
