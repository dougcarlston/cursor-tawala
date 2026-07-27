package com.tawala.web.projectmanager;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.domain.User;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class UpgradeWithNewerVersionController extends SimpleFormController {
	public final static String PARAMETER_PROJECT_ID = "project_id";

	public static class UpgradeForm {
		private Integer newLibraryVersionNumber;
		private String versionDescription;

		public Integer getNewLibraryVersionNumber() {
			return newLibraryVersionNumber;
		}

		public void setNewLibraryVersionNumber(Integer newLibraryVersionNumber) {
			this.newLibraryVersionNumber = newLibraryVersionNumber;
		}

		public String getVersionDescription() {
			return versionDescription;
		}

		public void setVersionDescription(String versionDescription) {
			this.versionDescription = versionDescription;
		}
	}

	public static class UpgradeFormValidator implements Validator {

		public boolean supports(Class clazz) {
			return clazz.equals(UpgradeForm.class);
		}

		public void validate(Object target, Errors errors) {
			ValidationUtils.rejectIfEmpty(errors, "newLibraryVersionNumber",
					"upgrade.library.version.not.chosen");
		}
	}

	public UpgradeWithNewerVersionController() {
		setFormView("projectmanager.upgrade.to.later.version");
		setCommandClass(UpgradeForm.class);
		setValidator(new UpgradeFormValidator());
		setCommandName("upgradeForm");
	}

	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();

		UserProject userProject = getUserProject(request);

		result.put("project", userProject);
		result.put("newerVersions", getNewerVersions(userProject));

		return result;
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		UserProject userProject = getUserProject(request);
		UpgradeForm form = (UpgradeForm) command;

		LibraryProjectVersion libraryProjectVersion = ProjectLibraryService
				.findProjectVersionById(userProject
						.getOriginalLibraryProjectVersionId());

		LibraryProjectVersion libraryVersion = libraryProjectVersion
				.getLibraryProject().getVersionByNumber(
						form.getNewLibraryVersionNumber());

		ProjectsHibernateImpl.upgradeWithLibraryProjectVersion(userProject,
				libraryVersion, form.getVersionDescription());

		
		response.sendRedirect(WellKnown.urls
				.getProjectManagerProjectDetailView()
				+ '?'
				+ ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME
				+ '=' + userProject.getName());

		return null;
	}

	private static List<LibraryProjectVersion> getNewerVersions(
			UserProject userProject) {
		LibraryProjectVersion libraryProjectVersion = ProjectLibraryService
				.findProjectVersionById(userProject
						.getOriginalLibraryProjectVersionId());

		List<LibraryProjectVersion> result = new ArrayList<LibraryProjectVersion>();
		for (LibraryProjectVersion version : libraryProjectVersion
				.getLibraryProject().getVersions()) {
			if (version.getVersionNumber() <= libraryProjectVersion
					.getVersionNumber()) {
				break;
			}
			result.add(version);
		}
		return result;
	}

	private static UserProject getUserProject(HttpServletRequest request) {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		String projectId = request.getParameter(PARAMETER_PROJECT_ID);

		return ProjectsHibernateImpl.getUserProjectById(user, Long
				.parseLong(projectId));
	}
}
