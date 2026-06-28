package com.tawala.component.web.display;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.springframework.web.util.HtmlUtils;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.CollectionParameter;
import com.tawala.component.ComponentRuntimeSupport;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.web.ResponseCreator;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Form;
import com.tawala.project.FormRenderable;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.TextBlock;
import com.tawala.project.Value;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.project.commands.RecordSelector.CurrentProjectFormDataProvider;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.web.RedirectResponse;
import com.tawala.web.Response;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.RenderingContext;

public class Categorizer extends WebComponentMetadataSupport {
	public static final String CATEGORY_NAME_PARAMETER_ID = "category-names";
	public static final String CATEGORY_VALUE_PARAMETER_ID = "category-ids";
	public static final String CATEGORY_STORAGE_PARAMETER_ID = "category-storage-field";
	public static final String FORM_TO_NAVIGATE_PARAMETER_ID = "navigate-to";
	public static final String HEADER_PARAMETER_ID = "header";
	public static final String CONTENTS_PARAMETER_ID = "contents";
	public static final String DISPLAY_CONDITION_PARAMETER_ID = "display-conditions";
	public static final String COLUMN_PARAMETER_ID = "column";
	public static final String NUMBER_OF_COLUMNS_FIELD_ID = "number-of-columns";
	private static final String COMPONENT_ID = "categorizer";
	public static final String CONDITIONS_PARAMETER_ID = "conditions";

	public Categorizer() {
		super(COMPONENT_ID, 2);
		addParameter(new Parameter(CATEGORY_NAME_PARAMETER_ID, COMPONENT_ID
				+ "_" + CATEGORY_NAME_PARAMETER_ID,
				ParameterType.BLANK_FIELD_NAME, true));
		addParameter(new Parameter(CATEGORY_VALUE_PARAMETER_ID, COMPONENT_ID
				+ "_" + CATEGORY_VALUE_PARAMETER_ID,
				ParameterType.BLANK_FIELD_NAME, true));
		addParameter(new Parameter(CATEGORY_STORAGE_PARAMETER_ID, COMPONENT_ID
				+ "_" + CATEGORY_STORAGE_PARAMETER_ID,
				ParameterType.BLANK_FIELD_NAME, true));
		addParameter(new Parameter(FORM_TO_NAVIGATE_PARAMETER_ID, COMPONENT_ID
				+ "_" + FORM_TO_NAVIGATE_PARAMETER_ID, ParameterType.FORM_NAME,
				true));
		/*
		addParameter(new NumericListParameter(NUMBER_OF_COLUMNS_FIELD_ID,
				COMPONENT_ID + "_number-of-columns", true, 1, 30));
				*/
		CollectionParameter columnsParameter = new CollectionParameter(
				COLUMN_PARAMETER_ID, COMPONENT_ID + "_column",
				NUMBER_OF_COLUMNS_FIELD_ID, true);
		columnsParameter
				.addNestedParameters(new Parameter[] {
						new Parameter(HEADER_PARAMETER_ID, COMPONENT_ID + "_"
								+ HEADER_PARAMETER_ID,
								ParameterType.EXPRESSION, false),
						new Parameter(CONTENTS_PARAMETER_ID, COMPONENT_ID + "_"
								+ CONTENTS_PARAMETER_ID,
								ParameterType.DISPLAYABLE_CONTENTS, true),
						new Parameter(DISPLAY_CONDITION_PARAMETER_ID,
								COMPONENT_ID + "_"
										+ DISPLAY_CONDITION_PARAMETER_ID,
								ParameterType.BOOLEAN_EXPRESSION, false) });

		addParameter(columnsParameter);
		addParameter(new Parameter(CONDITIONS_PARAMETER_ID, COMPONENT_ID + "_"
				+ CONDITIONS_PARAMETER_ID, ParameterType.RECORD_SELECTOR, true));
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends ComponentRuntimeSupport
			implements FormRenderable, ResponseCreator {
		private final RecordSelector recordSelector;
		private final String categoryNameFieldName;
		private final String categoryValueFieldName;
		private final String categoryStorageFieldName;
		private final String formToNavigateOnSave;
		private final List<ColumnInfo> columnInfos;
		private final Reference categoryStorageFieldReference;

		public RuntimeProcessor(ConfigElement configElement) {
			super(configElement);
			recordSelector = RecordSelector.instantiateFrom(configElement
					.child(CONDITIONS_PARAMETER_ID));
			List<ConfigElement> columnConfigs = configElement
					.children(COLUMN_PARAMETER_ID);
			columnInfos = new ArrayList<ColumnInfo>(columnConfigs.size());

			for (ConfigElement columnConfig : columnConfigs) {
				ConfigElement headerConfig = columnConfig
						.child(HEADER_PARAMETER_ID);
				ColumnInfo columnInfo = new ColumnInfo();

				switch (getVersion()) {
				case 1:
					columnInfo.headerVersionOne = (headerConfig == null ? new HtmlString(
							"")
							: new TextBlock(headerConfig));
					break;

				default:
					columnInfo.headerVersionTwo = new StringConcatenationExpression(
							headerConfig);
					break;
				}
				columnInfo.cellContent = makeContentTextBlock(columnConfig
						.child(CONTENTS_PARAMETER_ID), recordSelector);

				ConfigElement displayConditionElement = columnConfig
						.child(DISPLAY_CONDITION_PARAMETER_ID);
				if (displayConditionElement != null) {
					columnInfo.displayCondition = BooleanExpression
							.load(displayConditionElement.childElement(0));
				}

				columnInfos.add(columnInfo);
			}

			categoryValueFieldName = configElement.child(
					CATEGORY_VALUE_PARAMETER_ID).text();
			categoryNameFieldName = configElement.child(
					CATEGORY_NAME_PARAMETER_ID).text();
			categoryStorageFieldName = configElement.child(
					CATEGORY_STORAGE_PARAMETER_ID).text();
			formToNavigateOnSave = configElement.child(
					FORM_TO_NAVIGATE_PARAMETER_ID).text();

			categoryStorageFieldReference = new Reference(
					categoryStorageFieldName, true);

			// --- TODO: Should be fixed at some point. The method should be
			// removed too. SL.
			recordSelector
					.removeFormDataProvidersOtherThan(categoryStorageFieldReference
							.getFormName());
		}

		private FormRenderable makeContentTextBlock(ConfigElement contentConfig, RecordSelector recordSelector) {
			return new ItemizationTable.FieldTextBlock(contentConfig, recordSelector);
		}

		public Html toHtml(ExecutionContext context) {
			List<ColumnInfo> displayableColumns = new ArrayList<ColumnInfo>(
					columnInfos.size());
			for (ColumnInfo nextColumn : columnInfos) {
				if (nextColumn.displayCondition == null
						|| nextColumn.displayCondition.isTrue(context)) {
					displayableColumns.add(nextColumn);
				}
			}

			if (displayableColumns.size() == 0) {
				return new HtmlReadyString("");
			}

			ClassificationResult classificationResult = classifyFormSubmissions(context);

			StringBuilder result = new StringBuilder();

			String componentId = generateComponentId();
			result.append("<div class=\"categorizer\" id=\"" + componentId
					+ "\">\n");
			result.append("<div class=\"categorizer-left-filter\">");
			result.append("<form class=\"categorizer-filter\">\n"
					+ "<div class=\"buttons inline\">\n"
					+ "<input class=\"text\" type=\"text\" id=\"left-filter" + componentId
					+ "\" class=\"filterInput\" /> "
					+ "<button type=\"button\" onclick=\"Tawala.Categorizers['"
					+ componentId + "'].filterSource(document.getElementById('left-filter"
					+ componentId
					+ "').value); return false;\">FIND</button>\n"
					+ "<button type=\"button\" onclick=\"Tawala.Categorizers['"
					+ componentId
					+ "'].clearSourceFilter(document.getElementById('left-filter"
					+ componentId + "')); return false;\">CLEAR</button>\n"
					+ "</div>\n" + "</form>\n");
			result.append("</div>\n");

			result.append("<div class=\"categorizer-right-filter\">");
			result.append("<form class=\"categorizer-filter\">\n"
					+ "<div class=\"buttons inline\">\n"
					+ "<input class=\"text\" type=\"text\" id=\"filter-right" + componentId
					+ "\" class=\"filterInput\" /> "
					+ "<button type=\"button\" onclick=\"Tawala.Categorizers['"
					+ componentId + "'].filterDestination(document.getElementById('filter-right"
					+ componentId
					+ "').value); return false;\">FIND</button>\n"
					+ "<button type=\"button\" onclick=\"Tawala.Categorizers['"
					+ componentId
					+ "'].clearDestinationFilter(document.getElementById('filter-right"
					+ componentId + "')); return false;\">CLEAR</button>\n"
					+ "</div>\n" + "</form>\n");
			result.append("</div>\n");

			result.append("<div class=\"destinationTable\">\n");
			result
					.append("<table class=\"component outline draggable destinationTable ruler noFixWidth\">\n");

			for (Category category : classificationResult.categories) {
				result.append("<tbody>\n");
				result.append(makeCategoryHeaderRowString(displayableColumns, category));

				for (CompositeFormSubmission submission : category.submissions) {
					result.append(makeDataRowString(context,
							displayableColumns, submission));
				}

				result.append("</tbody>\n");
			}

			result.append("</table>\n");
			result.append("</div>\n");

			result.append("<div class=\"sourceTable\">\n");
			result
					.append("<table class=\"component outline draggable sourceTable sortable saveSortOrder ruler noFixWidth\">\n");

			result.append("<thead>\n");
			result.append(makeSourceTableHeaderRowString(context,
					displayableColumns,
					classificationResult.uncategorizedSubmissions));
			result.append("</thead>\n");

			result.append("<tbody>\n");

			for (CompositeFormSubmission submission : classificationResult.uncategorizedSubmissions) {
				result.append(makeDataRowString(context, displayableColumns,
						submission));
			}

			result.append("</tbody>\n");
			result.append("</table>\n");
			result.append("</div>\n");
			result.append("<div class=\"buttons\">\n");
			result.append("<br />\n");
			result
					.append("<button type=\"submit\" class=\"categorizerSubmit\" formname=\""
							+ context.getUserProject().getProjectComponentUrl(
									context, false, componentId)
							+ "\">SAVE CHANGES</button>\n");
			result.append("<br />\n");
			result.append("</div>");

			result.append("</div>\n");

			return new HtmlReadyString(result.toString());
		}

		private String makeSourceTableHeaderRowString(ExecutionContext context,
				List<ColumnInfo> displayableColumns,
				List<CompositeFormSubmission> submissions) {
			StringBuilder result = new StringBuilder();

			result.append("<tr class=\"heading\"><th class=\"id\"></th>");

			for (ColumnInfo columnInfo : displayableColumns) {
				Html html = columnInfo.headerVersionOne == null ? new HtmlString(
						columnInfo.headerVersionTwo.evaluate(context))
						: columnInfo.headerVersionOne.toHtml(context);
				StringWriter stringWriter = new StringWriter();
				html.render(new PrintWriter(stringWriter),
						new RenderingContext());

				result.append("<th>");
				result.append(stringWriter);
				result.append("</th>");
			}

			result.append("</tr>\n");

			return result.toString();
		}

		private String makeDataRowString(
				ExecutionContext context,
				List<com.tawala.component.web.display.Categorizer.ColumnInfo> displayableColumns,
				CompositeFormSubmission submission) {
			StringBuilder result = new StringBuilder();

			context.mapRecord(RecordSelector.DEFAULT_RECORD_LIST_NAME,
					submission);

			result.append("<tr class=\"datarow\">");
			result.append("<td class=\"id\">").append(
					submission.getFormSubmission(categoryStorageFieldReference)
							.getDatabaseId()).append("</td>");

			for (ColumnInfo columnInfo : displayableColumns) {
				Html html = columnInfo.cellContent.toHtml(context);
				StringWriter stringWriter = new StringWriter();
				html.render(new PrintWriter(stringWriter),
						new RenderingContext());

				result.append("<td>");
				result.append(stringWriter);
				result.append("</td>");
			}

			result.append("</tr>\n");

			return result.toString();
		}

		private String makeCategoryHeaderRowString(List<ColumnInfo> displayableColumns, Category category) {
			return "<tr class=\"heading\">" + "<th class=\"id\">"
					+ HtmlUtils.htmlEscape(category.id) + "</th>"
					+ "<th colspan=\"" + displayableColumns.size() + "\">"
					// + "<span class=\"categoryCount\">000</span>"
					+ HtmlUtils.htmlEscape(category.name) + "</th></tr>\n";
		}

		@SuppressWarnings("unchecked")
		private ClassificationResult classifyFormSubmissions(
				ExecutionContext context) {
			Map<String, Category> categoryByIdMap = new LinkedHashMap<String, Category>();

			Reference categoryValueReference = new Reference(
					categoryValueFieldName, true);

			Reference categoryNameReference = new Reference(
					categoryNameFieldName, true);

			List<FormDataProvider> formDataProviders = Collections
					.singletonList((FormDataProvider) new CurrentProjectFormDataProvider(
							categoryValueReference.getFormName()));

			RecordSelector categoryRecordSelector = new RecordSelector(
					formDataProviders, BooleanExpression.TRUE,
					RecordSelector.DEFAULT_RECORD_LIST_NAME);

			List<CompositeFormSubmission> categoryRecords = categoryRecordSelector
					.getRecords(context);

			if (categoryRecords == null) {
				categoryRecords = Collections.EMPTY_LIST;
			}

			for (CompositeFormSubmission submission : categoryRecords) {
				String categoryId = submission.getFormSubmission(
						categoryValueReference)
						.getValue(categoryValueReference).toString();

				FormSubmission formSubmission = submission
						.getFormSubmission(categoryNameReference);

				if (formSubmission != null) {
					Value value = formSubmission
							.getValue(categoryNameReference);
					String categoryName = value.toString();

					Category category = new Category();
					category.id = categoryId;
					category.name = categoryName;
					categoryByIdMap.put(categoryId, category);
				}
			}

			List<CompositeFormSubmission> uncategorizedSubmissions = new ArrayList<CompositeFormSubmission>();
			Reference categoryStorageReference = new Reference(
					categoryStorageFieldName, true);
			List<CompositeFormSubmission> sourceRecords = recordSelector
					.getRecords(context);

			if (sourceRecords == null) {
				sourceRecords = Collections.EMPTY_LIST;
			}

			for (CompositeFormSubmission submission : sourceRecords) {
				String currentRecordCategoryId = submission.getFormSubmission(
						categoryStorageReference).getValue(
						categoryStorageReference).toString();

				Category category = categoryByIdMap
						.get(currentRecordCategoryId);

				if (category == null) {
					// --- This is uncategorized submission - it goes into the
					// source table eventually.
					uncategorizedSubmissions.add(submission);
				} else {
					category.submissions.add(submission);
				}
			}

			ClassificationResult result = new ClassificationResult();
			result.categories = new ArrayList<Category>(categoryByIdMap.values());
			Collections.sort(result.categories);
			
			result.uncategorizedSubmissions = uncategorizedSubmissions;

			return result;
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public String generateComponentId() {
			return "categorizer"
					+ Math
							.abs((categoryValueFieldName
									+ categoryNameFieldName + categoryStorageFieldName)
									.hashCode()
									+ recordSelector.hashCode());
		}

		public ResponseCreator getResponseCreatorForComponentId(
				String componentId) {
			return componentId.equals(generateComponentId()) ? this : null;
		}

		public Response createResponse(ExecutionContext context) {
			String categoryIdPrefix = "categoryId";
			String sourceIdPrefix = "sourceId";

			Form itemsForm = context.getProject().getForm(
					categoryStorageFieldReference.getFormName());

			Project projectOwningData = null;
			String formName = null;
			if (itemsForm.isSharedData()) {
				projectOwningData = context.getUserProject().getUser()
						.getSharedStorage();
				formName = itemsForm.getDataSourceName();
			} else {
				projectOwningData = context.getProject();
				formName = categoryStorageFieldReference.getFormName();
			}
			// Access to form submissions is currently not optimized.
			// It makes sense to get them all in one go, and do updates.
			List<FormSubmission> submissions = context.getDomain().storedData()
					.responsesFor(projectOwningData, formName);
			Map<Long, FormSubmission> submissionByIdMap = new HashMap<Long, FormSubmission>(
					submissions.size());
			for (FormSubmission submission : submissions) {
				submissionByIdMap.put(submission.getDatabaseId(), submission);
			}

			List<String> data = context.getRequest().getParameterValues("data");
			String currentCategoryId = null;
			for (String parameter : data) {
				if (parameter.startsWith(categoryIdPrefix)) {
					currentCategoryId = parameter.substring(categoryIdPrefix
							.length());
				} else {
					if (parameter.startsWith(sourceIdPrefix)) {
						String currentFormSubmissionId = parameter
								.substring(sourceIdPrefix.length());
						long submissionId = Long
								.parseLong(currentFormSubmissionId);
						FormSubmission submission = submissionByIdMap
								.get(submissionId);
						if (submission == null) {
							Log.warn(this, "Unable to find submission by id: "
									+ submissionId);
						} else {
							Value presentValue = submission
									.getValue(categoryStorageFieldReference);
							if (!presentValue.toString().equals(
									currentCategoryId)) {
								submission.setValue(
										categoryStorageFieldReference
												.getFieldName(),
										currentCategoryId);
								context.getDomain().storedData().update(
										submission);
							}
						}
					} else {
						throw new IllegalStateException(
								"Unexpected parameter: " + parameter);
					}
				}
			}

			Form form = context.getProject().getForm(formToNavigateOnSave);
			if (form == null) {
				throw new IllegalStateException("Unable to find form '"
						+ formToNavigateOnSave + "'");
			}
			return new RedirectResponse(context.getUserProject().getUrlToForm(
					context.getLink(), context.getEntryPointType(), form));
		}
	}

	private static class Category implements Comparable<Category> {
		List<CompositeFormSubmission> submissions = new ArrayList<CompositeFormSubmission>();

		String id;
		String name;
		
		public int compareTo(Category o) {
			return this.name.compareToIgnoreCase(o.name);
		}
	}

	private static class ClassificationResult {
		List<Category> categories;
		List<CompositeFormSubmission> uncategorizedSubmissions;
	}

	private static class ColumnInfo {
		FormRenderable headerVersionOne;
		StringConcatenationExpression headerVersionTwo;
		FormRenderable cellContent;
		BooleanExpression displayCondition;
	}
}
