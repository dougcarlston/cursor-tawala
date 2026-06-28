package com.tawala.jbpm;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;

import org.jbpm.JbpmConfiguration;
import org.jbpm.JbpmContext;
import org.jbpm.file.def.FileDefinition;
import org.jbpm.graph.def.ProcessDefinition;
import org.jbpm.graph.exe.ProcessInstance;
import org.jbpm.graph.exe.Token;
import org.jbpm.job.executor.JobExecutor;
import org.jbpm.taskmgmt.exe.TaskInstance;
import org.jbpm.util.ClassLoaderUtil;

import com.scissor.Log;
import com.tawala.jbpm.sportsdashboards.Constants;
import com.tawala.jbpm.sportsdashboards.UserProjectProcess;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.PredeterminedObjectInitializer;

public class JbpmService {
	private static JbpmConfiguration jbpmConfiguration;

	public JbpmService(String config) {
		jbpmConfiguration = JbpmConfiguration.parseXmlString(config);
	}

	public static JbpmContext createContext() {
		if (jbpmConfiguration == null) {
			throw new IllegalStateException(
					"JBPM configuration is not initialized.");
		}
		return jbpmConfiguration.createJbpmContext();
	}

	public static ProcessDefinition deployProcess(JbpmContext jbpmContext,
			String processPath) {
		ProcessDefinition processDefinition = getDefinitionFromFile(processPath);
		jbpmContext.deployProcessDefinition(processDefinition);
		return processDefinition;
	}

	public static void deployTheLatestDefinitionIfNeeded(String processPath) {
		ProcessDefinition latestFromFile = getDefinitionFromFile(processPath);
		String currentVersion = getSpecifiedVersion(latestFromFile);
		JbpmContext context = createContext();
		try {
			ProcessDefinition latestDeployed = context.getGraphSession()
					.findLatestProcessDefinition(latestFromFile.getName());
			String deployedVersion = latestDeployed == null ? "None"
					: getSpecifiedVersion(latestDeployed);

			if (!currentVersion.equals(deployedVersion)) {
				context.getGraphSession().deployProcessDefinition(
						latestFromFile);
				latestFromFile.setVersion(Integer.parseInt(currentVersion));
				context.getGraphSession().saveProcessDefinition(latestFromFile);
				Log.info(PredeterminedObjectInitializer.class,
						"Deploying Sports Dashboard Workflow version #"
								+ currentVersion);
			}
		} finally {
			context.close();
		}
	}

	private static String getSpecifiedVersion(ProcessDefinition latestDeployed) {
		FileDefinition fileDefinition = latestDeployed.getFileDefinition();
		if (fileDefinition.hasFile("version.txt")) {
			byte[] versionBytes = fileDefinition.getBytes("version.txt");
			return new String(versionBytes);
		} else {
			return "Unknown";
		}
	}

	public static ProcessDefinition getDefinitionFromFile(String processPath) {
		ProcessDefinition processDefinition;

		processDefinition = ProcessDefinition.parseXmlResource(processPath
				+ "/processdefinition.xml");

		addFileToProcessDefinition(processDefinition, processPath, "gpd.xml");
		addFileToProcessDefinition(processDefinition, processPath,
				"processimage.jpg");
		addFileToProcessDefinition(processDefinition, processPath,
				"version.txt");
		return processDefinition;
	}

	@SuppressWarnings("unchecked")
	public static AggregateProcessStatistics getOpenProcessStatistics(
			String processName) {
		AggregateProcessStatistics result = new AggregateProcessStatistics();

		JbpmContext jbpmContext = createContext();
		try {
			List<ProcessDefinition> processVersions = jbpmContext
					.getGraphSession().findAllProcessDefinitionVersions(
							processName);
			for (ProcessDefinition processDefinition : processVersions) {
				List<ProcessInstance> processInstances = jbpmContext
						.getGraphSession().findProcessInstances(
								processDefinition.getId());
				for (ProcessInstance processInstance : processInstances) {
					if (!processInstance.hasEnded()) {
						result.gatherInfo(processInstance);
					}
				}
			}

		} finally {
			jbpmContext.close();
		}

		return result;
	}

	private static void addFileToProcessDefinition(
			ProcessDefinition processDefinition, String processPath,
			String fileName) {
		InputStream stream = ClassLoaderUtil.getClassLoader()
				.getResourceAsStream(processPath + "/" + fileName);
		if (stream == null) {
			throw new IllegalStateException("resource not found: " + fileName);
		}

		FileDefinition fileDefinition = processDefinition.getFileDefinition();
		if (fileDefinition == null) {
			fileDefinition = new FileDefinition();
			processDefinition.addDefinition(fileDefinition);
		}
		try {
			fileDefinition.addFile(fileName, stream);
		} finally {
			try {
				stream.close();
			} catch (IOException e) {
				throw new IllegalStateException("Error closing stream: ", e);
			}
		}
	}

	public static long endTask(long taskId, String transitionName) {
		long processInstanceId;
		JbpmContext jbpmContext = createContext();
		try {
			TaskInstance taskInstance = jbpmContext
					.getTaskInstanceForUpdate(taskId);
			if (taskInstance == null) {
				throw new IllegalStateException(
						"Unable to find task instance with id " + taskId);
			}

			if (transitionName == null || transitionName.length() == 0) {
				taskInstance.end();
			} else {
				taskInstance.end(transitionName);
			}
			processInstanceId = taskInstance.getProcessInstance().getId();

		} finally {
			jbpmContext.close();
		}
		return processInstanceId;
	}

	@SuppressWarnings("unchecked")
	public static List<UserProjectProcess> getProcessesInParticularState(
			String processName, String stateName) {

		List<UserProjectProcess> result = new ArrayList<UserProjectProcess>();
		JbpmContext jbpmContext = createContext();
		try {
			List<ProcessDefinition> processVersions = jbpmContext
					.getGraphSession().findAllProcessDefinitionVersions(
							processName);
			for (ProcessDefinition processDefinition : processVersions) {
				List<ProcessInstance> processInstances = jbpmContext
						.getGraphSession().findProcessInstances(
								processDefinition.getId());
				for (ProcessInstance processInstance : processInstances) {
					if (processInstance.hasEnded()) {
						continue;
					}
					if (isProcessInState(processInstance, stateName)) {
						result.add(getUserProjectProcess(processInstance));
					}
				}
			}

		} finally {
			jbpmContext.close();
		}

		return result;
	}

	@SuppressWarnings("unchecked")
	private static boolean isProcessInState(ProcessInstance processInstance,
			String stateName) {
		List<Token> allTokens = processInstance.findAllTokens();
		for (Token token : allTokens) {
			if (stateName.equals(token.getNode().getDescription())) {
				return true;
			}
		}
		return false;
	}

	public static UserProjectProcess getUserProjectProcess(
			ProcessInstance processInstance) {
		long userProjectId = getUserProjectId(processInstance);
		UserProject project = ProjectsHibernateImpl
				.getUserProjectById(userProjectId);

		return new UserProjectProcess(project, processInstance);
	}

	public static UserProjectProcess getFullyInstantiatedUserProjectProcess(
			ProcessInstance processInstance) {
		long userProjectId = getUserProjectId(processInstance);
		UserProject project = ProjectsHibernateImpl
				.getUserProjectWithRuntimeById(userProjectId);

		return new UserProjectProcess(project, processInstance);
	}

	private static long getUserProjectId(ProcessInstance processInstance) {
		long userProjectId = (Long) processInstance.getContextInstance()
				.getVariable(Constants.PROJECT_ID);
		return userProjectId;
	}

	public static ProcessInstance getProcessInstanceByProjectId(
			JbpmContext jbpmContext, long projectId) {
		for (Object object : jbpmContext.getGraphSession()
				.findAllProcessDefinitionVersions(Constants.PROCESS_NAME)) {
			ProcessDefinition processDefinition = (ProcessDefinition) object;
			ProcessInstance processInstance = jbpmContext.getProcessInstance(
					processDefinition, String.valueOf(projectId));
			if (processInstance != null) {
				return processInstance;
			}
		}
		return null;
	}

	public static JobExecutor getJobExecutor() {
		return jbpmConfiguration.getJobExecutor();
	}
}
