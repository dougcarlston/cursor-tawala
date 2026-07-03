package com.tawala.web.projectmanager.projectgroup;

import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.ProjectGroup;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Form;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ViewGroupMenuController implements Controller {
	private static final String ADMIN_DASH_FORM_NAME = "AdminDash";
	private static final String SPECIAL_LEAGUE_SETUP_OPTIONS_FORM = "SpecialLeagueSetupOptions";

	public static class AdditionalProjectData {
		private boolean lateAssignmentNotificationFlag;
		private String lateAssignmentNotificationEmail;
		private String adminDashURL;

		public boolean isLateAssignmentNotificationFlag() {
			return lateAssignmentNotificationFlag;
		}

		public void setLateAssignmentNotificationFlag(
				boolean lateAssignmentNotificationFlag) {
			this.lateAssignmentNotificationFlag = lateAssignmentNotificationFlag;
		}

		public String getLateAssignmentNotificationEmail() {
			return lateAssignmentNotificationEmail;
		}

		public void setLateAssignmentNotificationEmail(
				String lateAssignmentNotificationEmail) {
			this.lateAssignmentNotificationEmail = lateAssignmentNotificationEmail;
		}

		public String getAdminDashURL() {
			return adminDashURL;
		}

		public void setAdminDashURL(String adminDashURL) {
			this.adminDashURL = adminDashURL;
		}
	}

	public static final String GROUP_ID = "group_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String groupId = request.getParameter(GROUP_ID);
		ProjectGroup group = UsersHibernateImpl.getProjectGroup(
				UserInfoPreparationInterceptor.getSessionUser(request), Long
						.parseLong(groupId));
		if (group == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		Map<UserProject, AdditionalProjectData> additionalProjectData = new HashMap<UserProject, AdditionalProjectData>();
		for (UserProject userProject : group.getUserProjects()) {
			addAdditionalProjectDataForProject(additionalProjectData,
					userProject);
		}

		ModelAndView result = new ModelAndView(
				"projectmanager.project.group.menu");
		result.addObject("group", group);
		result.addObject("additionalProjectData", additionalProjectData);

		result.addObject("sportsdashboardsGroups", UsersHibernateImpl
				.getAllUserSportsDashboardGroups(UserInfoPreparationInterceptor
						.getSessionUser(request)));

		return result;
	}

	private void addAdditionalProjectDataForProject(
			Map<UserProject, AdditionalProjectData> additionalProjectData,
			UserProject userProject) {
		AdditionalProjectData data = new AdditionalProjectData();
		additionalProjectData.put(userProject, data);

		Form adminDashForm = userProject.getProject().getForm(ADMIN_DASH_FORM_NAME);
		if (adminDashForm == null) {
			Log.error(this, "Form " + ADMIN_DASH_FORM_NAME
					+ " for project \"" + userProject.getName()
					+ "\" for user " + userProject.getUser().getId()
					+ " doesn't exist.");
		} else {
			data.setAdminDashURL(userProject
					.getUrlToForm(EntryPointType.REAL_PROJECT, adminDashForm));
		}

		RecordSelector setupRecordSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								SPECIAL_LEAGUE_SETUP_OPTIONS_FORM)),
				BooleanExpression.TRUE, RecordSelector.DEFAULT_RECORD_LIST_NAME);

		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), userProject);
		List<CompositeFormSubmission> records = setupRecordSelector
				.getRecords(context);
		if (records == null || records.size() == 0) {
			return;
		}

		if (records.size() > 1) {
			Log.error(this, "Form " + SPECIAL_LEAGUE_SETUP_OPTIONS_FORM
					+ " for project \"" + userProject.getName()
					+ "\" for user " + userProject.getUser().getId()
					+ " contain " + records.size() + " records");
			return;
		}

		CompositeFormSubmission submission = records.get(0);

		Reference lateNofificationFlagRef = new Reference(
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":"
						+ SPECIAL_LEAGUE_SETUP_OPTIONS_FORM + ":"
						+ "LateAssignmentNotificationFlag", true);

		Reference lateNofificationEmailRef = new Reference(
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":"
						+ SPECIAL_LEAGUE_SETUP_OPTIONS_FORM + ":"
						+ "LateAssignmentNotificationEmail", true);

		data.setLateAssignmentNotificationFlag(submission.getValue(
				lateNofificationFlagRef).toString().equals("On"));
		data.setLateAssignmentNotificationEmail(submission.getValue(
				lateNofificationEmailRef).toString());
	}
}
