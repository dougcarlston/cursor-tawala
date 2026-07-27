package com.tawala.web.projectmanager;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashSet;
import java.util.Set;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.email.DefaultCustomizationMailToURLBuilder;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.Process;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.theme.CommonTheme;
import com.tawala.web.controller.WellKnown;

public abstract class CustomizationController implements
		Controller {
	public final ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		CustomizationContext customizationContext = getCustomizationContext(request);
		if(customizationContext == null) {
			//--- Something is not right. Overriding classes will report the problem.
			response.sendRedirect(WellKnown.urls.getHome());
			return null;
		}
		UserProject userProject = customizationContext.getUserProject();

		ModelAndView result = new ModelAndView("projectmanager.customization");
		String projectName = customizationContext.getProjectName();
		result.addObject("projectName", projectName);
		result.addObject("userProject", userProject);
		result.addObject("projectIsSaved", customizationContext.isProjectSaved());
		result.addObject("invitationMessage",
				new DefaultCustomizationMailToURLBuilder(userProject,
						projectName));

		result.addObject("availableThemes", CommonTheme.ALL_THEMES);
		result.addObject("customizationMetaData",
				createCustomizationMetaData(userProject.getProject()));

		return result;
	}

	protected abstract CustomizationContext getCustomizationContext(HttpServletRequest request);

	@SuppressWarnings("unchecked")
	private CustomizationMetaData createCustomizationMetaData(Project project) {
		CustomizationMetaData result = new CustomizationMetaData();

		Form customizationForm = project.getCustomizerForm();
		if (customizationForm == null) {
			return result;
		}

		// Collect all form names (main customization and the ones
		// referenced in pre and post processes.
		Set<String> formNames = new HashSet<String>();
		formNames.add(customizationForm.getName());
		formNames.addAll(getReferencedFormsInProcess(project, customizationForm
				.getPreProcessName()));
		formNames.addAll(getReferencedFormsInProcess(project, customizationForm
				.getPostProcessName()));

		addFieldsToTheMetaData(project, result, formNames);

		return result;
	}

	private Set getReferencedFormsInProcess(Project project, String processName) {
		if (processName == null) {
			return Collections.EMPTY_SET;
		}
		Process process = project.getProcess(processName);
		if (processName == null) {
			return Collections.EMPTY_SET;
		}

		return process.getFormNamesCanNavigateTo();
	}

	private void addFieldsToTheMetaData(Project project,
			CustomizationMetaData result, Set<String> formNames) {
		for (String formName : formNames) {
			Form form = project.getForm(formName);
			if (form == null) {
				continue;
			}

			for (Field field : form.getAllFields()) {
				if (field.canBeUsedToReplaceText()) {
					result.addTextFieldToEnhance(field.getHtmlId());
				}
			}
		}
	}

	public static class CustomizationMetaData {
		private Collection<String> textFieldsToEnhance = new ArrayList<String>();

		public Collection<String> getTextFieldsToEnhance() {
			return textFieldsToEnhance;
		}

		public void addTextFieldToEnhance(String textFieldToEnhance) {
			this.textFieldsToEnhance.add(textFieldToEnhance);
		}
	}
}
