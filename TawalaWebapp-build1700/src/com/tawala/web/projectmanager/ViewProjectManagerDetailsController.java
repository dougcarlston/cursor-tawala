package com.tawala.web.projectmanager;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.ImpossibleException;
import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.World;
import com.tawala.domain.User;
import com.tawala.email.EmailService;
import com.tawala.message.Message;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.backup.DailyBackup;
import com.tawala.project.backup.UserProjectBackupService;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.theme.CommonTheme;
import com.tawala.sportsdashboards.data.TeamRosterTemplate;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ViewProjectManagerDetailsController implements Controller {
	public static final String PARAMETER_FORM_NAME = "form";

	// CAREFUL: these names are used by JavaScript on the confirmation dialogs.
	// See popup.js for details. SL.
	public static final String PARAMETER_ACTION_ERASE = "Erase";
	public static final String PARAMETER_ACTION_DELETE = "Delete";
	public static final String PARAMETER_ACTION_PURGE = "Purge";
	public static final String PARAMETER_ACTION_DEPLOY = "Deploy";
	public static final String PARAMETER_ACTION_DELETE_ALL_EMAILS = "DeleteAllEmails";
	public static final String PARAMETER_ACTION_DELETE_VERSION = "deleteversion";
	public static final String PARAMETER_ACTION_COPY = "Copy";
	public static final String PARAMETER_NEW_NAME = "newName";
	public static final String PARAMETER_COPY_RESPONSES = "copyResponses";

	public static final String PARAMETER_PROJECT_NAME = "projectName";
	public static final String PROJECT_VERSION_PARAMETER = "project_version";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		String projectName = request.getParameter(PARAMETER_PROJECT_NAME);
		World world = WorldInitializer.getDefaultWorld();

		if (projectName == null) {
			Log.warn(this, "Unable to get name of the project.");
			return navigateOnFailureToLoadProject(request, response);
		}

		UserProject project = world.domain().projects().getWithVersions(
				user.getId(), projectName);
		if (project == null) {
			Log.warn(this, "Can't find a project named \"" + projectName
					+ "\".");
			return navigateOnFailureToLoadProject(request, response);
		}

		List<Message> messages = new ArrayList<Message>();
		String action = request.getParameter("action");
		if (action != null && action.equalsIgnoreCase(PARAMETER_ACTION_COPY)) {
			String destination = copyOwnedProject(request, response, user, project);
			if (destination != null) {
				redirectToProjectDetails(response, destination);
			}
			return null;
		}
		if (action != null) {
			doAction(request, response, messages, action);
			redirectToProjectDetails(response, projectName);
			return null;
		}

		List<Form> forms = project.getProject().getForms();
		List<FormInfo> formsInfo = new ArrayList<FormInfo>();
		for (Form form : forms) {
			long responses = world.domain().storedData().responseCount(
					project.getProject(), form.getName());
			String size = ProjectInfo.getStoragePercent(ProjectInfo
					.getFormResponsesSize(form, project.getProject(), world));
			FormInfo info = new FormInfo(form, responses, size);
			formsInfo.add(info);
		}

		ModelAndView result = new ModelAndView("projectmanager.detail");
		result.addObject("formsInfo", formsInfo);
		result.addObject("projectInfo", new ProjectInfo(world, project, user));
		result.addObject("commonThemes", CommonTheme.ALL_THEMES);
		result.addObject("userDefinedThemes", UsersHibernateImpl
				.getAllUserThemes(user));
		result.addObject("lastDataRecordedDate", ProjectsHibernateImpl
				.getLastDataRecordedDate(project));

		// --- Backup Schedules
		List backupSchedules = ProjectsHibernateImpl
				.getBackupSchedules(project);
		result.addObject("backupSchedules", backupSchedules);
		if (backupSchedules.size() == 0) {
			result.addObject("dailyBackupSchedule", new DailyBackup(project));
		}
		result.addObject("hours", DailyBackup.HOURS);
		result.addObject("minutes", DailyBackup.MINUTES);

		// --- Online project backups.
		result.addObject("onlineBackups", UserProjectBackupService
				.getBackupsForProject(project));

		result.addObject("showSelectedFormsOnly", project.getProject()
				.isShowSelectedFormsByDefault());
		
		//--- Check library for a newer version.
		if(project.getOriginalLibraryProjectVersionId() != null) {
			LibraryProjectVersion libraryProjectVersion = ProjectLibraryService.findProjectVersionById(project.getOriginalLibraryProjectVersionId());
			if(libraryProjectVersion != null) {
				result.addObject("originalLibraryVersion", libraryProjectVersion);

				long originalVersionNumber = libraryProjectVersion.getVersionNumber();
				long currentVersionNumber = libraryProjectVersion.getLibraryProject().getLatestVersionNumber();
				
				if(originalVersionNumber < currentVersionNumber) {
					result.addObject("thereIsNewerLibraryVersion", true);
				}
			}
		}
		
		//--- Team roster templates
		result.addObject("teamRosterTemplateIds", TeamRosterTemplate.getAllTemplates());
		
		result.addObject("updateProjectDetailsForm", new UpdateProjectDetailsController.Form(project));

		return result;
	}

	public static void redirectToProjectDetails(HttpServletResponse response,
			String projectName) throws IOException,
			UnsupportedEncodingException {
		response.sendRedirect(WellKnown.urls
				.getProjectManagerProjectDetailView()
				+ '?'
				+ PARAMETER_PROJECT_NAME
				+ '='
				+ URLEncoder.encode(projectName, "UTF-8"));
	}

	/**
	 * @param request
	 * @param response
	 * @return
	 * @throws Exception
	 */
	private ModelAndView navigateOnFailureToLoadProject(
			HttpServletRequest request, HttpServletResponse response)
			throws Exception {
		response.sendRedirect(WellKnown.urls.getProjectManagerView());
		return null;
	}

	/**
	 * New project on this account. The name must be free. Responses are copied
	 * only when asked, and only for forms stored on the project. The original
	 * project and its responses stay where they are.
	 */
	private String copyOwnedProject(HttpServletRequest request,
			HttpServletResponse response, User user, UserProject source)
			throws IOException {
		String newName = request.getParameter(PARAMETER_NEW_NAME);
		if (newName != null) {
			newName = newName.trim();
		}
		if (newName == null || newName.length() == 0 || newName.length() > 100) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST);
			return null;
		}
		String folded = newName.toLowerCase();
		List<UserProject> owned = WorldInitializer.getDefaultWorld().domain()
				.projects().getAllForUserId(user.getId());
		for (int i = 0; i < owned.size(); i++) {
			String existing = owned.get(i).getName();
			if (existing != null && existing.toLowerCase().equals(folded)) {
				response.sendError(HttpServletResponse.SC_CONFLICT);
				return null;
			}
		}

		boolean copyResponses = "1".equals(request
				.getParameter(PARAMETER_COPY_RESPONSES));
		Project definition = source.getProject().makeCopy();
		UserProject copy = new UserProject(definition, user, newName);
		copy.getVersions().get(0).setDescription("Copy of " + source.getName());
		UserProject saved = WorldInitializer.getDefaultWorld().domain()
				.projects().put(copy);
		if (saved == null || saved.getId() == 0 || saved.getId() == source.getId()
				|| alreadyOwned(owned, saved)) {
			response.sendError(HttpServletResponse.SC_CONFLICT);
			return null;
		}
		if (copyResponses) {
			copyProjectResponses(source, saved);
		}
		return newName;
	}

	private static boolean sameProject(UserProject left, UserProject right) {
		return left.getId() == right.getId();
	}

	private static boolean alreadyOwned(List<UserProject> owned, UserProject saved) {
		for (int i = 0; i < owned.size(); i++) {
			if (sameProject(owned.get(i), saved)) {
				return true;
			}
		}
		return false;
	}

	private static void copyProjectResponses(UserProject source, UserProject dest) {
		Project sourceDef = source.getProject();
		for (Form form : dest.getProject().getForms()) {
			if (form.isSharedData()) {
				continue;
			}
			Form sourceForm = sourceDef.getForm(form.getName());
			if (sourceForm == null) {
				continue;
			}
			List<FormSubmission> rows = WorldInitializer.getDefaultWorld()
					.domain().storedData()
					.fullyInitializedResponsesFor(sourceDef, sourceForm);
			for (int i = 0; i < rows.size(); i++) {
				FormSubmission row = new FormSubmission(dest, form);
				row.copyFieldsFrom(rows.get(i));
				WorldInitializer.getDefaultWorld().domain().storedData()
						.record(row);
			}
		}
	}

	private void doAction(HttpServletRequest request,
			HttpServletResponse response, List<Message> messages, String action)
			throws Exception {
		String projectName = request.getParameter(PARAMETER_PROJECT_NAME);
		if (projectName == null) {
			Log.warn(this, "Couldn't find parameter '" + projectName + "'.");
			return;
		}

		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		UserProject project = WorldInitializer.getDefaultWorld().domain()
				.projects().get(user.getId(), projectName);

		if (project == null) {
			Log.warn(this, "Couldn't find project '" + projectName + "'.");
			return;
		}

		if (action.equalsIgnoreCase(PARAMETER_ACTION_PURGE)) {
			WorldInitializer.getDefaultWorld().domain().storedData()
					.purgeProjectResponses(project.getProject());
		} else if (action.equalsIgnoreCase(PARAMETER_ACTION_DELETE)) {
			WorldInitializer.getDefaultWorld().domain().projects().delete(
					project, WorldInitializer.getDefaultWorld());
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
		} else if (action.equalsIgnoreCase(PARAMETER_ACTION_ERASE)) {
			String formName = request.getParameter(PARAMETER_FORM_NAME);
			WorldInitializer.getDefaultWorld().domain().storedData()
					.eraseResponsesFor(project.getProject(), formName);
		} else if (action.equalsIgnoreCase(PARAMETER_ACTION_DEPLOY)) {
			long projectVersionId = Long.parseLong(request
					.getParameter(PROJECT_VERSION_PARAMETER));

			UserProject deployedPproject = WorldInitializer.getDefaultWorld()
					.domain().projects().deployProjectVersion(
							UserInfoPreparationInterceptor
									.getSessionUser(request), projectVersionId);

			Log.info(this, "Deployed version "
					+ deployedPproject.getDeployedVersion().getVersionNumber()
					+ " of project '" + deployedPproject.getName() + "'");
		} else if (action.equalsIgnoreCase(PARAMETER_ACTION_DELETE_VERSION)) {
			long projectVersionId = Long.parseLong(request
					.getParameter(PROJECT_VERSION_PARAMETER));

			WorldInitializer.getDefaultWorld().domain().projects()
					.deleteProjectVersion(
							UserInfoPreparationInterceptor
									.getSessionUser(request), projectVersionId);
		} else if (action.equalsIgnoreCase(PARAMETER_ACTION_DELETE_ALL_EMAILS)) {
			EmailService.deleteAllEmailsForProject(project.getId());
		} else {
			Log.error(this, "Unknown action '" + action + "'");
		}
	}

	protected String urlEncode(String userId) {
		try {
			return URLEncoder.encode(userId, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			throw new ImpossibleException(e);
		}
	}
}
