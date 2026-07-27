package com.tawala.web.admin.sportsdashboard.task;

import java.util.Map;

import javax.servlet.http.HttpServletRequest;

import org.springframework.validation.Errors;

import com.tawala.project.Form;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;

public class PrepareProjectForReleaseTaskController extends
		DefaultViewTaskController {

	public PrepareProjectForReleaseTaskController() {
		setFormView("admin.sports-dashboard.task.prepare.for.release");
	}

	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors) throws Exception {
		DefaultViewTaskController.ViewTaskForm form = (ViewTaskForm) command;
		Map<String, Object> result = super.referenceData(request, command,
				errors);

		UserProject userProject = form.getProcessTask().getUserProject();
		userProject = ProjectsHibernateImpl.getUserProjectWithRuntimeById(userProject.getId());
		Map<Form, String> urls = userProject
				.getEntryPointURLs();

		for (Map.Entry<Form, String> mapEntry : urls.entrySet()) {
			if (mapEntry.getKey().getName().equals("PrepareForRelease")) {
				result.put("prepareForReleaseURL", mapEntry.getValue());
			}
		}

		return result;
	}

}
