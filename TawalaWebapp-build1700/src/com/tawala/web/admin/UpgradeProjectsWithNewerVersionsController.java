package com.tawala.web.admin;

import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;

public class UpgradeProjectsWithNewerVersionsController extends
		SimpleFormController {
	public final static String PARAMETER_PROJECT_ID = "project_id";

	public static class UpgradeForm {
		private Long[] projectIds;

		public Long[] getProjectIds() {
			return projectIds;
		}

		public void setProjectIds(Long[] projectIds) {
			this.projectIds = projectIds;
		}
	}

	public UpgradeProjectsWithNewerVersionsController() {
		setFormView("admin.upgrade.projects.with.newer.version");
		setCommandClass(UpgradeForm.class);
		setCommandName("upgradeForm");
	}

	@Override
	protected Map<String, Object> referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();

		result.put("projects", ProjectsHibernateImpl
				.findAllObsoleteProjects(ProjectLibrary.PAID_PROJECT_LIBRARY));

		return result;
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		UpgradeForm form = (UpgradeForm) command;

		if (form.getProjectIds() != null) {
			for (Long projectId : form.getProjectIds()) {
				upgradeProject(projectId);
			}
		}

		response.sendRedirect(WellKnown.urls
				.getAdminUpgradeProjectsWithNewerVersion());

		return null;
	}

	private void upgradeProject(Long projectId) {
		UserProject userProject = ProjectsHibernateImpl
				.getUserProjectById(projectId);

		LibraryProjectVersion libraryProjectVersion = ProjectLibraryService
				.findProjectVersionById(userProject
						.getOriginalLibraryProjectVersionId());

		LibraryProjectVersion libraryVersion = libraryProjectVersion
				.getLibraryProject().getLatestVersion();

		ProjectsHibernateImpl.upgradeWithLibraryProjectVersion(userProject,
				libraryVersion, "Upgraded to version "
						+ libraryVersion.getVersionNumber() + ".");
	}
}
