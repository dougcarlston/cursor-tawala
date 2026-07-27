package com.tawala.web.projectmanager;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.DataSource;
import com.tawala.project.data.StoredField;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ViewProjectManagerDataController implements Controller {
	public static final String FORM_NAME = "formName";
	public static final String PROJECT_NAME = "projectName";
	public static final String SHARED_DATA = "sharedData";

	public static final String SUBMISSSION_ID = "submission_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		boolean sharedData = request.getParameter(SHARED_DATA) != null;

		String[] recordIds = request.getParameterValues(SUBMISSSION_ID);
		if (recordIds != null) {
			for (String recordId : recordIds) {
				if (sharedData) {
					WorldInitializer.getDefaultWorld().domain().storedData()
							.deleteSharedDataSourceSubmission(
									Long.parseLong(recordId),
									UserInfoPreparationInterceptor
											.getSessionUser(request));
				} else {
					WorldInitializer.getDefaultWorld().domain().storedData()
							.verifyThatBelongsToUserAndDelete(
									Long.parseLong(recordId),
									UserInfoPreparationInterceptor
											.getSessionUser(request));
				}
			}
			// --- TODO: send redirect.
			// response.sendRedirect();
			// return null;
		}
		String formName = request.getParameter(FORM_NAME);

		ModelAndView result = new ModelAndView("projectmanager.dataview");
		User user = UserInfoPreparationInterceptor.getSessionUser(request);

		Project project = null;
		String projectName = null;
		List<FormSubmission> formSubmissions = null;
		DataPresentationBuilder presentationBuilder = null;
		if (sharedData) {
			project = WorldInitializer.getDefaultWorld().domain().users()
					.getSharedStorageForUser(user);
			formSubmissions = WorldInitializer.getDefaultWorld().domain()
					.storedData().responsesFor(project, formName);
			DataSource dataSource = project.getDataSourceNamed(formName);
			if (dataSource == null) {
				response.sendRedirect(WellKnown.urls.getProjectManagerView());
				return null;
			}
			presentationBuilder = new SharedDataPresentationBuilder(dataSource);
		} else {
			projectName = request.getParameter(PROJECT_NAME);
			UserProject userProject = WorldInitializer.getDefaultWorld()
					.domain().projects().getWithProjectRuntime(user.getId(),
							projectName);
			if (userProject == null) {
				response.sendRedirect(WellKnown.urls.getProjectManagerView());
				return null;
			}
			project = userProject.getProject();
			result.addObject("userProject", userProject);

			formSubmissions = WorldInitializer.getDefaultWorld().domain()
					.storedData().responsesFor(project, formName);
			presentationBuilder = new RegularProjectDataPresentationBuilder(
					userProject, userProject.getProject().getForm(formName));
		}

		result.addObject("dataPresentation", presentationBuilder
				.buildDataPresentation(formSubmissions));

		if (project == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		Collection<Reference> fieldReferences = getAllSubmissionFieldIds(formSubmissions);

		List<Message> messages = new ArrayList<Message>();

		result.addObject(PROJECT_NAME, projectName);
		result.addObject(FORM_NAME, formName);
		result.addObject("formSubmissions", formSubmissions);
		result.addObject("fieldReferences", fieldReferences);
		result.addObject("messages", messages);
		result.addObject("sharedData", sharedData);

		return result;
	}

	public static Set<Reference> getAllSubmissionFieldIds(
			List<FormSubmission> formSubmissions) {
		Set<Reference> result = new LinkedHashSet<Reference>();

		for (FormSubmission submission : formSubmissions) {
			for (String fieldId : submission.getFieldIds()) {
				result.add(new Reference(fieldId));
			}
		}

		return result;
	}

	public static class DataPresentationInfo {
		private JSONArray data;
		private JSONObject responseSchema;
		private JSONArray columnDefinitions;

		public DataPresentationInfo(JSONArray data, JSONObject responseSchema,
				JSONArray columnDefinitions) {
			this.data = data;
			this.responseSchema = responseSchema;
			this.columnDefinitions = columnDefinitions;
		}

		public JSONArray getColumnDefinitions() {
			return columnDefinitions;
		}

		public JSONArray getData() {
			return data;
		}

		public JSONObject getResponseSchema() {
			return responseSchema;
		}
	}

	private static abstract class DataPresentationBuilder {
		@SuppressWarnings("unchecked")
		DataPresentationInfo buildDataPresentation(
				List<FormSubmission> submissions) throws JSONException {
			SimpleDateFormat dateFormat = new SimpleDateFormat(
					"yyyy/MM/dd HH:mm:ss");

			Set<Reference> fieldReferences = new LinkedHashSet<Reference>();
			JSONObject responseSchema = new JSONObject();
			JSONArray columnDefinitions = new JSONArray();
			JSONArray fields = new JSONArray();

			// --- Create field and column information
			// --- Primary key
			JSONObject primaryKeyField = new JSONObject();
			primaryKeyField.put("key", "_primaryKey");
			fields.put(primaryKeyField);

			JSONObject deleteRecordColumn = new JSONObject();
			deleteRecordColumn.put("key", "_primaryKey");
			deleteRecordColumn.put("label", "");
			deleteRecordColumn.put("formatter", "formatDeleteRecordCheckbox");
			deleteRecordColumn.put("sortable", false);
			deleteRecordColumn.put("width", 10);
			deleteRecordColumn.put("resizeable", false);
			columnDefinitions.put(deleteRecordColumn);

			// --- Get information about all fields
			Map<String, Integer> maxFieldLengths = new HashMap<String, Integer>();

			for (String fieldId : getAllFieldIds()) {
				maxFieldLengths.put(fieldId, fieldId.length());

				// -- Create references for all fields to be used to extract
				// data.
				fieldReferences.add(new Reference(fieldId));

				// -- Column definition
				JSONObject currentColumnDefinition = new JSONObject();
				currentColumnDefinition.put("key", fieldId);
				currentColumnDefinition.put("label", fieldId);
				currentColumnDefinition.put("sortable", true);
				currentColumnDefinition.put("resizeable", true);
				currentColumnDefinition.put("checked", true);
				columnDefinitions.put(currentColumnDefinition);

				JSONObject fieldSpecificProperties = getDataTableEditorInfo(fieldId);
				Iterator<String> keyIterator = fieldSpecificProperties.keys();
				while (keyIterator.hasNext()) {
					String key = keyIterator.next();
					currentColumnDefinition.put(key, fieldSpecificProperties
							.get(key));
				}

				// -- Field definition
				JSONObject currentFieldDefinition = new JSONObject();
				currentFieldDefinition.put("key", fieldId);
				fields.put(currentFieldDefinition);
			}

			// --- Date created column
			JSONObject createdDateColumn = new JSONObject();
			createdDateColumn.put("key", "_dateCreated");
			createdDateColumn.put("label", "Created");
			createdDateColumn.put("sortable", true);
			createdDateColumn.put("resizeable", true);
			createdDateColumn.put("width", 112);
			columnDefinitions.put(createdDateColumn);

			JSONObject createdDateField = new JSONObject();
			createdDateField.put("key", "_dateCreated");
			fields.put(createdDateField);

			// --- Date updated column
			JSONObject updatedDateColumn = new JSONObject();
			updatedDateColumn.put("key", "_dateUpdated");
			updatedDateColumn.put("label", "Updated");
			updatedDateColumn.put("sortable", true);
			updatedDateColumn.put("resizeable", true);
			updatedDateColumn.put("width", 112);
			columnDefinitions.put(updatedDateColumn);

			JSONObject updatedDateField = new JSONObject();
			updatedDateField.put("key", "_dateUpdated");
			fields.put(updatedDateField);

			// --- Create data
			JSONArray data = new JSONArray();
			for (FormSubmission submission : submissions) {
				JSONObject row = new JSONObject();
				for (Reference fieldReference : fieldReferences) {
					int displayableFieldLength = 0;
					List<Value> values = submission.getValues(fieldReference);
					String fieldId = fieldReference.getFieldName();
					switch (values.size()) {
					case 0:
						break;

					case 1:
						String stringValue = values.get(0).toString();
						row.put(fieldId, stringValue);
						displayableFieldLength = stringValue.length();
						break;

					default:
						List<String> stringValues = new ArrayList<String>(
								values.size());
						for (Value nextValue : values) {
							stringValue = nextValue.toString();
							stringValues.add(stringValue);
							displayableFieldLength += stringValue.length() + 1;
						}
						row.put(fieldId, stringValues);
						break;
					}
					int previousMaxLength = maxFieldLengths.get(fieldId);
					if (displayableFieldLength > previousMaxLength) {
						maxFieldLengths.put(fieldId, displayableFieldLength);
					}
				}
				row.put("_dateCreated", dateFormat.format(submission
						.getCreationDate()));
				row.put("_dateUpdated",
						submission.getUpdatedDate() == null ? "" : dateFormat
								.format(submission.getUpdatedDate()));
				row.put("_primaryKey", submission.getDatabaseId());
				data.put(row);
			}

			// --- Set column widths based on the lengths of displayed data.
			// --- Let the library decide the widths on its own if the number of
			// columns is small.
			if (columnDefinitions.length() > 6) {
				for (int i = 0; i < columnDefinitions.length(); i++) {
					JSONObject columnDefinition = columnDefinitions
							.getJSONObject(i);
					Integer maxDisplayableLength = maxFieldLengths
							.get(columnDefinition.getString("key"));
					if (maxDisplayableLength == null) {
						continue;
					}
					int width = Math.min(maxDisplayableLength, 40) * 7;
					columnDefinition.put("width", width);
				}
			}

			responseSchema.put("fields", fields);

			DataPresentationInfo result = new DataPresentationInfo(data,
					responseSchema, columnDefinitions);
			return result;
		}

		abstract JSONObject getDataTableEditorInfo(String fieldId)
				throws JSONException;

		abstract List<String> getAllFieldIds();
	}

	private static class RegularProjectDataPresentationBuilder extends
			DataPresentationBuilder {
		private UserProject userProject;
		private Form form;

		public RegularProjectDataPresentationBuilder(UserProject userProject,
				Form form) {
			this.userProject = userProject;
			this.form = form;
		}

		@Override
		List<String> getAllFieldIds() {
			List<String> result = new ArrayList<String>();
			for (Field field : form.getAllFields()) {
				result.add(field.getHtmlId());
			}
			return result;
		}

		@Override
		JSONObject getDataTableEditorInfo(String fieldId) throws JSONException {
			return form.getFieldById(fieldId).getDataTableEditorInfo(
					userProject);
		}
	}

	private static class SharedDataPresentationBuilder extends
			DataPresentationBuilder {
		private DataSource dataSource;

		public SharedDataPresentationBuilder(DataSource dataSource) {
			this.dataSource = dataSource;
		}

		@Override
		List<String> getAllFieldIds() {
			List<String> result = new ArrayList<String>();
			for (StoredField field : dataSource.getFields()) {
				result.add(field.getName());
			}
			return result;
		}

		@Override
		JSONObject getDataTableEditorInfo(String fieldId) throws JSONException {
			return dataSource.getFieldById(fieldId).getDataTableEditorInfo();
		}

	}
}
