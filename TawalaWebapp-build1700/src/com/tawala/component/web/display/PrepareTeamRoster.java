package com.tawala.component.web.display;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Iterator;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRichTextString;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.ComponentRuntimeSupport;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterRestriction;
import com.tawala.component.ParameterType;
import com.tawala.component.parameter.WorksWithinRecordIteration;
import com.tawala.component.web.ResponseCreator;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormRenderable;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.And;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.Equals;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.ReferenceOperator;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.sportsdashboards.data.TeamRosterTemplate;
import com.tawala.web.ExcelDownloadResponse;
import com.tawala.web.OldPageResponse;
import com.tawala.web.Response;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.Link;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.project.DataCollectingProjectController;

public class PrepareTeamRoster extends WebComponentMetadataSupport {
	public static final String OPEN_IN_CURRENT_WINDOW_OPTION = "current-window";
	public static final String OPEN_IN_NEW_WINDOW_OPTION = "new-window";

	private static final String COMPONENT_ID = "prepare-team-roster";
	public static final String LINK_DESCRIPTION = "description";
	public static final String TEAM_ID = "team-id";

	public PrepareTeamRoster() {
		super(COMPONENT_ID, 1);

		addParameters(new Parameter[] {
				new Parameter(
						LINK_DESCRIPTION,
						COMPONENT_ID + "_" + LINK_DESCRIPTION,
						ParameterType.EXPRESSION,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				new Parameter(
						TEAM_ID,
						COMPONENT_ID + "_" + TEAM_ID,
						ParameterType.EXPRESSION,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null)))

		});
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends ComponentRuntimeSupport
			implements FormRenderable, ResponseCreator {
		private final StringConcatenationExpression descriptionExpression;
		private final StringConcatenationExpression teamIdExpression;

		public RuntimeProcessor(ConfigElement configElement) {
			descriptionExpression = new StringConcatenationExpression(
					configElement.child(LINK_DESCRIPTION));
			teamIdExpression = new StringConcatenationExpression(configElement
					.child(TEAM_ID));
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public Html toHtml(ExecutionContext context) {
			String templateId = context.getUserProject()
					.getTeamRosterTemplateId();
			if (!TeamRosterTemplate.isTemplateExists(templateId)) {
				return new HtmlString("");
			}

			String teamId = teamIdExpression.evaluate(context);
			String text = descriptionExpression.evaluate(context);

			StringBuilder urlBuilder = new StringBuilder(context
					.getUserProject().getProjectComponentUrl(context, false,
							generateComponentId()));
			urlBuilder.append('?');
			urlBuilder.append("team-id");
			try {
				urlBuilder.append('=').append(
						URLEncoder.encode(teamId, "UTF-8"));
			} catch (UnsupportedEncodingException e) {
				throw new IllegalStateException("Unable to encode team id: ", e);
			}

			Link result = new Link(urlBuilder.toString(), text, false);

			return result;
		}

		private String generateComponentId() {
			return COMPONENT_ID;
		}

		public ResponseCreator getResponseCreatorForComponentId(
				String componentId) {
			return componentId.equals(generateComponentId()) ? this : null;
		}

		public Response createResponse(ExecutionContext context) {
			HSSFWorkbook workbook = generateTeamRoster(context);
			if (workbook == null) {
				OldPage page = new OldPage();
				Div div = new Div("class", "text error");
				div
						.add(new HtmlString(
								"Unable to produce the team roster. Please contact customer service."));
				page.add(div);
				DataCollectingProjectController.addHeaderAndStyleSheets(context
						.getProject(), context, page);
				return new OldPageResponse(page);
			} else {
				Response result = new ExcelDownloadResponse(workbook, "Roster");
				return result;
			}
		}

		private HSSFWorkbook generateTeamRoster(ExecutionContext context) {
			TeamRosterTemplate template = TeamRosterTemplate
					.getTemplateById(context.getUserProject()
							.getTeamRosterTemplateId());
			if (template == null) {
				Log.error(this, "Unable to find team roster template for id="
						+ context.getUserProject().getTeamRosterTemplateId());
			}
			return populateWithData(template, context);
		}

		@SuppressWarnings("unchecked")
		private HSSFWorkbook populateWithData(TeamRosterTemplate template,
				ExecutionContext context) {

			HSSFWorkbook result;
			try {
				result = template.getWorkbook();
			} catch (IOException e) {
				throw new IllegalStateException(
						"Error instantiating workbook for template id = "
								+ context.getUserProject()
										.getTeamRosterTemplateId(), e);
			}

			List<CompositeFormSubmission> coachesData = loadCoaches(context);
			if (coachesData.size() > template.getMaxNumberOfCoaches()) {
				Log.error(this, "Team roster template " + template.getId()
						+ " supports up to " + template.getMaxNumberOfCoaches()
						+ " coaches, but this team has " + coachesData.size());
				return null;
			}
			
			

			List<CompositeFormSubmission> playersData = loadPlayers(context);
			if (playersData.size() > template.getMaxNumberOfPlayers()) {
				Log.error(this, "Team roster template " + template.getId()
						+ " supports up to " + template.getMaxNumberOfPlayers()
						+ " players, but this team has " + playersData.size());
				return null;
			}

			List<CompositeFormSubmission> teamData = loadTeamRecord(context);
			List<CompositeFormSubmission> setupData = loadSetupRecord(context);

			HSSFSheet sheet = result.getSheetAt(0);
			Iterator<HSSFRow> rowIterator = sheet.rowIterator();

			Pattern pattern = Pattern
					.compile("<(([^:.]+):[^:.]+)(\\.(\\d+))?>");
			while (rowIterator.hasNext()) {
				HSSFRow row = rowIterator.next();
				Iterator<HSSFCell> cellIterator = row.cellIterator();
				while (cellIterator.hasNext()) {
					HSSFCell cell = (HSSFCell) cellIterator.next();
					if (cell.getCellType() != HSSFCell.CELL_TYPE_STRING) {
						continue;
					}
					HSSFRichTextString value = cell.getRichStringCellValue();
					String rawValue = value.getString();
					Matcher matcher = pattern.matcher(rawValue);
					if (matcher.matches()) {
						String referenceValue = matcher.group(1);
						String formName = matcher.group(2);

						if (formName.equals("Coach")
								|| formName.equals("TeamCoachAssignments")) {
							replaceValueWithRecordData(cell, referenceValue,
									getRowNumber(rawValue, matcher),
									coachesData);
						} else if (formName.equals("Registration")
								|| formName.equals("Player")) {
							replaceValueWithRecordData(cell, referenceValue,
									getRowNumber(rawValue, matcher),
									playersData);
						} else if (formName.equals("Team")) {
							replaceValueWithRecordData(cell, referenceValue, 0,
									teamData);
						} else if (formName.equals("AdminSetup")) {
							replaceValueWithRecordData(cell, referenceValue, 0,
									setupData);
						} else {
							throw new IllegalStateException(
									"Unknown form name: " + formName);
						}
					}
				}
			}

			return result;
		}

		private int getRowNumber(String rawValue, Matcher matcher) {
			if (matcher.groupCount() != 4) {
				throw new IllegalStateException(
						"Expected field reference in the format <Form:Field.RecordNumber>, but got "
								+ rawValue);
			}
			int rowNumber = Integer.parseInt(matcher.group(4)) - 1;
			return rowNumber;
		}

		private void replaceValueWithRecordData(HSSFCell cell,
				String referenceValue, int rowNumber,
				List<CompositeFormSubmission> records) {
			String newValue = "";
			if (rowNumber < records.size()) {
				CompositeFormSubmission submission = records.get(rowNumber);
				Reference reference = new Reference(referenceValue, true);
				Value formValue = submission.getValue(reference);
				newValue = formValue.toString();
			}
			cell.setCellValue(new HSSFRichTextString(newValue));
		}

		private List<CompositeFormSubmission> loadTeamRecord(
				ExecutionContext context) {
			List<FormDataProvider> forms = new ArrayList<FormDataProvider>(2);
			forms
					.add((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
							"Team"));

			And condition = new And();
			String teamId = context.getRequest().getParameter(TEAM_ID);
			condition.add(new Equals("Record:Team:TeamId", teamId));

			RecordSelector teamRecordsSelector = new RecordSelector(forms,
					condition, RecordSelector.DEFAULT_RECORD_LIST_NAME);

			List<CompositeFormSubmission> teamRecords = teamRecordsSelector
					.getRecords(context);

			if (teamRecords == null) {
				throw new IllegalStateException("Unable to find team with id "
						+ teamId);
			}
			if (teamRecords.size() != 1) {
				throw new IllegalStateException(
						"Expected to find 1 team with id " + teamId
								+ ", but instead found " + teamRecords.size());
			}
			return teamRecords;
		}

		private List<CompositeFormSubmission> loadSetupRecord(
				ExecutionContext context) {
			List<FormDataProvider> forms = new ArrayList<FormDataProvider>(1);
			forms
					.add((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
							"AdminSetup"));

			BooleanExpression condition = BooleanExpression.TRUE;

			RecordSelector setupRecordsSelector = new RecordSelector(forms,
					condition, RecordSelector.DEFAULT_RECORD_LIST_NAME);

			List<CompositeFormSubmission> setupRecords = setupRecordsSelector
					.getRecords(context);

			if (setupRecords == null) {
				throw new IllegalStateException(
						"Unable to find the setup record");
			}
			if (setupRecords.size() != 1) {
				throw new IllegalStateException(
						"Expected to find 1 setup record, but instead found "
								+ setupRecords.size());
			}
			return setupRecords;
		}

		private List<CompositeFormSubmission> loadPlayers(
				ExecutionContext context) {
			List<FormDataProvider> forms = new ArrayList<FormDataProvider>(2);
			forms
					.add((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
							"Registration"));

			And condition = new And();
			condition.add(new Equals("Record:Registration:TeamId", context
					.getRequest().getParameter(TEAM_ID)));

			RecordSelector registrationsRecordSelector = new RecordSelector(
					forms, condition, RecordSelector.DEFAULT_RECORD_LIST_NAME);

			List<CompositeFormSubmission> playerRecords = registrationsRecordSelector
					.getRecords(context);

			if (playerRecords == null) {
				return playerRecords = Collections.emptyList();
			}

			Reference firstNameReference = new Reference(
					"Registration:FirstName", true);
			Reference lastNameReference = new Reference(
					"Registration:LastName", true);
			Reference middleNameReference = new Reference(
					"Registration:MiddleName", true);
			for (CompositeFormSubmission compositeFormSubmission : playerRecords) {
				String firstName = compositeFormSubmission.getValue(
						firstNameReference).toString();
				String lastName = compositeFormSubmission.getValue(
						lastNameReference).toString();
				String middleName = compositeFormSubmission.getValue(
						middleNameReference).toString();

				String fullName = lastName + ", " + firstName + " "
						+ middleName;

				FormSubmission fakeSubmission = new FormSubmission(true);
				fakeSubmission.setFormName("Player");
				fakeSubmission.setValue("FullName", fullName);

				compositeFormSubmission.add(fakeSubmission);
			}

			final Reference fullNameReference = new Reference(
					"Player:FullName", true);

			Collections.sort(playerRecords,
					new Comparator<CompositeFormSubmission>() {
						public int compare(CompositeFormSubmission o1,
								CompositeFormSubmission o2) {
							return o1.getValue(fullNameReference).toString()
									.compareToIgnoreCase(
											o2.getValue(fullNameReference)
													.toString());
						}
					});

			return playerRecords;
		}

		private List<CompositeFormSubmission> loadCoaches(
				ExecutionContext context) {
			List<FormDataProvider> forms = new ArrayList<FormDataProvider>(2);
			forms
					.add((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
							"Coach"));
			forms
					.add((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
							"TeamCoachAssignments"));

			And condition = new And();
			condition.add(new Equals("Record:TeamCoachAssignments:TeamID",
					context.getRequest().getParameter(TEAM_ID)));
			condition.add(new Equals(new ReferenceOperator(
					"Record:Coach:CoachId"), new ReferenceOperator(
					"Record:TeamCoachAssignments:CoachID")));

			RecordSelector coachesRecordSelector = new RecordSelector(forms,
					condition, RecordSelector.DEFAULT_RECORD_LIST_NAME);

			List<CompositeFormSubmission> coachesRecords = coachesRecordSelector
					.getRecords(context);

			if (coachesRecords == null) {
				return coachesRecords = Collections.emptyList();
			}
			
			Collections.sort(coachesRecords, new Comparator<CompositeFormSubmission>() {
				private Reference roleRef = new Reference("Record:TeamCoachAssignments:Role", true);
				private Reference lastNameRef = new Reference("Record:Coach:LastName", true);

				public int compare(CompositeFormSubmission o1,
						CompositeFormSubmission o2) {
					String coach1Role = o1.getValue(roleRef).toString(); 
					String coach2Role = o2.getValue(roleRef).toString();
					
					//-- We want to put Coaches ahead of Assistant Coaches, that's why the reverse order.
					int roleComparison = coach2Role.compareTo(coach1Role);
					return roleComparison != 0 ? roleComparison :
						o1.getValue(lastNameRef).toString().compareTo(o2.getValue(lastNameRef).toString());
				}
				
			});
			
			return coachesRecords;
		}
	}
}
