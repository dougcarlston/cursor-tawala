package com.tawala.web.projectmanager.projectgroup;

import java.util.Collections;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;
import com.tawala.project.commands.Equals;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

//--- TODO: unused for now. 
public abstract class UpdateLateRegistrationRecordController implements Controller {
	public static final String GROUP_ID = "group_id";
	public static final String PROJECT_ID = "project_id";
	public static final String COACH_ID = "coach_id";

	final public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		long groupId = Long.parseLong(request.getParameter(GROUP_ID));
		long projectId = Long.parseLong(request.getParameter(PROJECT_ID));
		UserProject userProject = UsersHibernateImpl.getProjectInGroup(
				UserInfoPreparationInterceptor.getSessionUser(request),
				groupId, projectId);

		JSONObject result = new JSONObject();

		if (userProject == null) {
			Log.error(this, "Unable to find project #" + projectId
					+ " in project group # " + groupId);
			result.put("success", false);
		} else {
			boolean updateResult = updateRecord(userProject, request
					.getParameter(COACH_ID), getFieldReference(), request
					.getParameter(getValueParameterName()));
			result.put("success", updateResult);
		}

		response.setContentType("application/json");
		response.getWriter().write('(' + result.toString() + ')');

		return null;
	}

	protected abstract String getValueParameterName();

	protected abstract Reference getFieldReference();

	private boolean updateRecord(UserProject userProject, String coachId,
			Reference fieldReference, String fieldValue) {
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), userProject);

		RecordSelector setupRecordSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								"Coach")), new Equals(
						"Record:Coach:CoachId", coachId),
				RecordSelector.DEFAULT_RECORD_LIST_NAME);

		List<CompositeFormSubmission> records = setupRecordSelector
				.getRecords(context);
		if (records == null) {
			return false;
		}

		if (records.size() != 1) {
			Log.error(this, "More than one coach for coach id " + coachId
					+ " for user project #" + userProject.getId());
			return false;
		}

		FormSubmission submission = records.get(0).getFormSubmission(
				fieldReference);
		submission.setValue(fieldReference.getFieldName(), fieldValue);

		WorldInitializer.getDefaultWorld().domain().storedData().update(
				submission);
		return true;
	}
}
