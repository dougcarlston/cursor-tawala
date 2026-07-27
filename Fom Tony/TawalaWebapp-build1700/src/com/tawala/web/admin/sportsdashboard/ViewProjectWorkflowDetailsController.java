package com.tawala.web.admin.sportsdashboard;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.jbpm.JbpmContext;
import org.jbpm.graph.exe.ProcessInstance;
import org.jbpm.graph.exe.Token;
import org.jbpm.logging.log.ProcessLog;
import org.jbpm.taskmgmt.exe.TaskInstance;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.Constants;
import com.tawala.jbpm.sportsdashboards.UserProjectProcess;
import com.tawala.jbpm.sportsdashboards.UserProjectProcessTask;
import com.tawala.project.Form;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.web.WorldInitializer;

public class ViewProjectWorkflowDetailsController implements Controller {
	public static final String PROCESS_INSTANCE_ID_PARAMETER = "process-id";
	public static final String PROJECT_ID_PARAMETER = "project_id";

	@SuppressWarnings("unchecked")
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		JbpmContext jbpmContext = JbpmService.createContext();
		try {
			ProcessInstance processInstance = null;
			UserProject userProject = null;
			String processIdParameter = request
					.getParameter(PROCESS_INSTANCE_ID_PARAMETER);
			Long processInstanceId = null;
			if (processIdParameter == null) {
				String projectIdParameter = request
						.getParameter(PROJECT_ID_PARAMETER);
				long userProjectId = Long.parseLong(projectIdParameter);
				processInstance = JbpmService.getProcessInstanceByProjectId(
						jbpmContext, userProjectId);
				if (processInstance == null) {
					userProject = ProjectsHibernateImpl
							.getUserProjectWithRuntimeById(userProjectId);
				}
			} else {
				processInstanceId = Long.parseLong(processIdParameter);
				processInstance = jbpmContext
						.getProcessInstance(processInstanceId);
				if (processInstance == null) {
					throw new IllegalStateException(
							"Unable to find process instance with id "
									+ processInstanceId);
				}
			}

			ModelAndView result = new ModelAndView(
					"admin.sports-dashboard.view.process");

			if (processInstance != null) {
				UserProjectProcess userProjectProcess = JbpmService
						.getFullyInstantiatedUserProjectProcess(processInstance);
				userProject = userProjectProcess.getUserProject();

				result.addObject("projectProcessInstance", userProjectProcess);

				List<Token> allTokens = processInstance.findAllTokens();
				for (Token token : allTokens) {
					token.getNode().getName();
				}
				result.addObject("tokens", allTokens);
				List<TaskInstance> tasks = jbpmContext.getTaskMgmtSession()
						.findTaskInstancesByProcessInstance(processInstance);

				List<UserProjectProcessTask> processTasks = new ArrayList<UserProjectProcessTask>(tasks.size());
				for (TaskInstance taskInstance : tasks) {
					// --- Forces instantiation of pooled actors sets.
					taskInstance.getPooledActors().isEmpty();
					processTasks.add(new UserProjectProcessTask(userProject, taskInstance));
				}

				result.addObject("tasks", processTasks);

				Set<ProcessLog> sortedLogEntries = new TreeSet<ProcessLog>(
						new Comparator<ProcessLog>() {

							public int compare(ProcessLog o1, ProcessLog o2) {
								return o1.getDate().compareTo(o2.getDate());
							}
						});

				Map<Token, List<ProcessLog>> logs = jbpmContext
						.getLoggingSession().findLogsByProcessInstance(
								processInstance.getId());
				for (Map.Entry<Token, List<ProcessLog>> logMapEntry : logs
						.entrySet()) {
					for (ProcessLog logEntry : logMapEntry.getValue()) {
						logEntry.toString();
						sortedLogEntries.add(logEntry);
					}
				}
				result.addObject("logs", sortedLogEntries);

				result.addObject("creator", processInstance
						.getContextInstance().getVariable(
								Constants.ORIGINAL_CREATOR));
			}

			result.addObject("userProject", userProject);
			result.addObject("lastDataRecordedDate", ProjectsHibernateImpl
					.getLastDataRecordedDate(userProject));


			result.addObject("registrationTrend", ProjectsHibernateImpl
					.getFormSubmissionTrendForProject(userProject.getId(),
							Constants.REGISTRATION_FORM_NAME, 30));

			result.addObject("emailTrend", ProjectsHibernateImpl
					.getEmailTrendForProject(userProject.getId(), 30));

			result.addObject("registrationCount", WorldInitializer
					.getDefaultWorld().domain().storedData().responseCount(
							userProject.getProject(),
							Constants.REGISTRATION_FORM_NAME));

			Form adminMenuForm = userProject.getProject().getForm(
					Constants.ADMIN_MENU_FORM_NAME);
			if (adminMenuForm == null) {
				Log.error(this,
						"Unable to find admin menu form in user project #"
								+ userProject.getId());
			} else {
				result.addObject("linkToAdminMenu", userProject.getUrlToForm(
						EntryPointType.REAL_PROJECT, adminMenuForm));
			}
			return result;
		} finally {
			jbpmContext.close();
		}
	}
}
