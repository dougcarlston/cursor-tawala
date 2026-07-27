package com.tawala.web.projectmanager.projectgroup;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.sportsdashboards.model.Coach;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class LoadProjectCoachesController implements Controller {
	private static final String ADMIN_DASHBOARD_FORM_NAME = "AdminDash";
	public static final String GROUP_ID = "group_id";
	public static final String PROJECT_ID = "project_id";

	public ModelAndView handleRequest(HttpServletRequest request,
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
			result.put("success", true);
			result.put("coaches", createListOfCoaches(userProject));
			result.put("statusMap", createStatusMap(userProject));
			Form adminDashForm = userProject.getProject().getForm(
					ADMIN_DASHBOARD_FORM_NAME);
			if (adminDashForm == null) {
				Log.error(this, "Unable to find form '"
						+ ADMIN_DASHBOARD_FORM_NAME + "' in user project #"
						+ userProject.getId());
			} else {
				result.put("adminDashboardURL", userProject.getUrlToForm(
						EntryPointType.REAL_PROJECT, adminDashForm));
			}
		}

		response.setContentType("application/json");
		response.getWriter().write('(' + result.toString() + ')');

		return null;
	}

	private JSONArray createListOfCoaches(UserProject userProject) {
		JSONArray result = new JSONArray();
		Reference coachRecordReference = new Reference("Record:Coach:CoachId",
				true);
		List<CompositeFormSubmission> coachesData = CoachReportGenerator
				.retrieveCoachesFrom(userProject);
		if (coachesData != null) {
			List<Coach> coaches = new ArrayList<Coach>();
			for (CompositeFormSubmission compositeFormSubmission : coachesData) {
				FormSubmission submission = compositeFormSubmission
						.getFormSubmission(coachRecordReference);
				Coach coach = new Coach(submission);
				coaches.add(coach);
			}

			Collections.sort(coaches, new Comparator<Coach>() {
				public int compare(Coach o1, Coach o2) {
					int comparisonResult = o1.getLastName()
							.compareToIgnoreCase(o2.getLastName());
					return comparisonResult == 0 ? 
							o1.getFirstName().compareToIgnoreCase(o2.getFirstName())
							: comparisonResult;
				}
			});

			for (Coach coach : coaches) {
				result.put(coach.toJSON());
			}
		}
		return result;
	}

	private JSONObject createStatusMap(UserProject userProject) {
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), userProject);

		RecordSelector recordSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								"CoachStatus")), BooleanExpression.TRUE,
				RecordSelector.DEFAULT_RECORD_LIST_NAME);

		List<CompositeFormSubmission> records = recordSelector
				.getRecords(context);

		JSONObject result = new JSONObject();
		if (records == null) {
			return result;
		}

		Reference statusIdField = new Reference("Record:CoachStatus:ID", true);
		for (CompositeFormSubmission compositeFormSubmission : records) {
			FormSubmission submission = compositeFormSubmission
					.getFormSubmission(statusIdField);

			try {
				JSONObject statusObject = new JSONObject();
				statusObject
						.put("name", submission.getValue("Name").toString());

				result.put(submission.getValue(statusIdField).toString(),
						statusObject);
			} catch (JSONException e) {
				throw new IllegalStateException(
						"Unable to create status object: ", e);
			}

		}

		return result;
	}
}
