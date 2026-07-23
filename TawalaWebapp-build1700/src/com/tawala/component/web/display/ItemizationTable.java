package com.tawala.component.web.display;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.CollectionParameter;
import com.tawala.component.ComponentRuntimeSupport;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.repository.Repository;
import com.tawala.component.web.ResponseCreator;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.message.Message;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Field;
import com.tawala.project.FieldReference;
import com.tawala.project.Form;
import com.tawala.project.FormRenderable;
import com.tawala.project.FormSubmission;
import com.tawala.project.MultipleChoice;
import com.tawala.project.TextBlock;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.RenderingContext;
import com.tawala.web.project.ExcelDataExportTemplate;

public class ItemizationTable extends WebComponentMetadataSupport {
	public static final String HEADER_PARAMETER_ID = "header";
	public static final String CONTENTS_PARAMETER_ID = "contents";
	public static final String CELL_STYLE_PARAMETER_ID = "cell-style";
	public static final String HEADER_STYLE_PARAMETER_ID = "header-style";
	public static final String SHOW_PRINT_CONTROL_OPTION_PARAMETER_ID = "show-print-control";
	public static final String SHOW_EXPORT_OPTION_PARAMETER_ID = "show-export-control";
	public static final String EXCEL_TEMPLATE_PARAMETER_ID = "template-id";
	public static final String DISPLAY_CONDITION_PARAMETER_ID = "display-conditions";
	public static final String COLUMN_PARAMETER_ID = "column";
	public static final String NUMBER_OF_COLUMNS_FIELD_ID = "number-of-columns";
	private static final String COMPONENT_ID = "itemization-table";
	public static final String CONDITIONS_PARAMETER_ID = "conditions";

	public ItemizationTable() {
		super(COMPONENT_ID, 2);
		addParameter(new Parameter(SHOW_PRINT_CONTROL_OPTION_PARAMETER_ID,
				COMPONENT_ID + '_' + SHOW_PRINT_CONTROL_OPTION_PARAMETER_ID,
				ParameterType.ENUMERATION, true).addOptions(new Option[] {
				new Option(Boolean.FALSE.toString(), "option.no"),
				new Option(Boolean.TRUE.toString(), "option.yes") }));
		addParameter(new Parameter(SHOW_EXPORT_OPTION_PARAMETER_ID,
				COMPONENT_ID + '_' + SHOW_EXPORT_OPTION_PARAMETER_ID,
				ParameterType.ENUMERATION, true).addOptions(new Option[] {
				new Option(Boolean.FALSE.toString(), "option.no"),
				new Option(Boolean.TRUE.toString(), "option.yes") }));

		Collection<String> templateIds = ExcelDataExportTemplate
				.getTemplateIds();
		if (templateIds != null && templateIds.size() > 0) {
			Option[] templateOptions = new Option[templateIds.size() + 1];
			int i = 0;
			templateOptions[i++] = new Option("",
					"excel.template.no-template.description");

			for (String templateId : templateIds) {
				templateOptions[i++] = new Option(templateId, "excel.template."
						+ templateId + ".description");
			}
			addParameter(new Parameter(EXCEL_TEMPLATE_PARAMETER_ID,
					COMPONENT_ID + '_' + EXCEL_TEMPLATE_PARAMETER_ID,
					ParameterType.ENUMERATION, true)
					.addOptions(templateOptions));
		}

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
								ParameterType.BOOLEAN_EXPRESSION, false)
				/*
				 * , new Parameter(HEADER_STYLE_PARAMETER_ID, COMPONENT_ID + "_"
				 * + HEADER_STYLE_PARAMETER_ID, ParameterType.TEXT, false), new
				 * Parameter(CELL_STYLE_PARAMETER_ID, COMPONENT_ID + "_" +
				 * CELL_STYLE_PARAMETER_ID, ParameterType.TEXT, false)
				 */});

		addParameter(columnsParameter);
		addParameter(new Parameter(CONDITIONS_PARAMETER_ID, COMPONENT_ID + "_"
				+ CONDITIONS_PARAMETER_ID, ParameterType.RECORD_SELECTOR, true));
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	private static class ColumnInfo {
		FormRenderable headerVersionOne;
		StringConcatenationExpression headerVersionTwo;
		TextBlock cellContent;
		String headerStyle;
		String cellStyle;
		BooleanExpression displayCondition;
	}

	public static class RuntimeProcessor extends ComponentRuntimeSupport
			implements FormRenderable {

		private static final String NO_DATA_MESSAGE_ID = "web." + COMPONENT_ID
				+ ".no-data.message";
		private final RecordSelector recordSelector;
		private final List<ColumnInfo> columnInfos;
		private final boolean enablePrint;
		private final boolean enableExport;
		private final String templateId;
		private Reference lastFieldReference;
		private boolean includeTimestamps;
		private final SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");

		public RuntimeProcessor(ConfigElement configElement) {
			super(configElement);
			enablePrint = configElement
					.hasChild(SHOW_PRINT_CONTROL_OPTION_PARAMETER_ID)
					&& Boolean.parseBoolean(configElement.child(
							SHOW_PRINT_CONTROL_OPTION_PARAMETER_ID).text());
			enableExport = configElement
					.hasChild(SHOW_EXPORT_OPTION_PARAMETER_ID)
					&& Boolean.parseBoolean(configElement.child(
							SHOW_EXPORT_OPTION_PARAMETER_ID).text());
			templateId = configElement.hasChild(EXCEL_TEMPLATE_PARAMETER_ID) ? configElement
					.child(EXCEL_TEMPLATE_PARAMETER_ID).text()
					: "";
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

				columnInfo.headerStyle = extractInlineStyle(columnConfig
						.child(HEADER_STYLE_PARAMETER_ID));

				columnInfo.cellStyle = extractInlineStyle(columnConfig
						.child(CELL_STYLE_PARAMETER_ID));

				ConfigElement displayConditionElement = columnConfig
						.child(DISPLAY_CONDITION_PARAMETER_ID);
				if (displayConditionElement != null) {
					columnInfo.displayCondition = BooleanExpression
							.load(displayConditionElement.childElement(0));
				}

				columnInfos.add(columnInfo);
			}
		}

		private String extractInlineStyle(ConfigElement styleConfig) {
			return styleConfig == null ? null : styleConfig.text().replace('"',
					'\'');
		}

		private TextBlock makeContentTextBlock(ConfigElement contentConfig,
				RecordSelector recordSelector) {
			return new FieldTextBlock(contentConfig, recordSelector);
		}

		public Html toHtml(ExecutionContext context) {
			List<CompositeFormSubmission> records = recordSelector
					.getRecords(context);
			
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
			
			StringBuilder result = new StringBuilder();
			boolean needsPaginations = records != null && records.size() > 100;
			boolean fixTableHeight = records != null && records.size() > 16;
			// Content-sized by default (no empty full-width frame). Authors still
			// widen columns via YUI "Drag column heading border to resize column".
			// Legacy used displayableColumns.size() > 3 → dtFixTableWidth → 97%.
			boolean fixTableWidth = false;
			boolean presetColumnWidth = records != null && records.size() > 150;

			boolean useSimpleTableFormatting = context
					.isGeneratingEmailContent()
					|| records == null || records.size() == 0;
			String tableClassAttribute = useSimpleTableFormatting ? " class=\"component outline sortable stripe\""
					: " class=\"component\"";
			String containingDivClassAttribute = useSimpleTableFormatting ? ""
					: " class=\""
							+ buildContainerClass(needsPaginations,
									fixTableWidth, fixTableHeight,
									presetColumnWidth) + "\"";
			result.append("<div").append(containingDivClassAttribute);
			if(enableExport && templateId.length() > 0) {
				result.append(" exportTemplateId=\"").append(templateId).append("\"");
			}
			result.append(">\n");
			result.append("<table").append(tableClassAttribute).append(">\n");
			result.append("<thead>\n");
			result.append("<tr>\n");
			
			lastFieldReference = null; 
			int numberOfFormsReferenced = 0;
			for (ColumnInfo columnInfo : displayableColumns) {
				Html html = columnInfo.headerVersionOne == null ? new HtmlString(
						columnInfo.headerVersionTwo.evaluate(context))
						: columnInfo.headerVersionOne.toHtml(context);
				StringWriter stringWriter = new StringWriter();
				html.render(new PrintWriter(stringWriter),
						new RenderingContext());

				result.append("<th");
				if (columnInfo.headerStyle != null) {
					result.append(" style=\"").append(columnInfo.headerStyle)
							.append('"');
				}
				result.append(">").append(stringWriter.getBuffer()).append(
						"</th>");
				
				List<FormRenderable> cellContents = columnInfo.cellContent.getContents();
				if (cellContents != null) {
					FieldReference cellFieldReference = (FieldReference)cellContents.get(0);
					if (cellFieldReference != null) {
						String previousFormName = lastFieldReference == null ? "" : lastFieldReference.getFormName();
						lastFieldReference = new Reference(cellFieldReference.getName(), true);
						if (lastFieldReference != null && !lastFieldReference.getFormName().equals(previousFormName)) {
							numberOfFormsReferenced++;
						}
					}
				}
			}
			
			includeTimestamps = numberOfFormsReferenced == 1 && records != null && records.size() > 0;
			if (includeTimestamps) {
				result.append("<th>__Created__</th>");
				result.append("<th>__Updated__</th>");
			}

			result.append("</tr>\n");
			result.append("</thead>\n");
			result.append("<tbody>\n");

			if (records == null || records.size() == 0) {
				displayNoData(context, displayableColumns, result);
			} else {
				displayResults(context, displayableColumns, records, result);
			}

			result.append("</tbody>\n");
			result.append("</table>\n");
			result.append("</div>\n");
			return new HtmlReadyString(result.toString());
		}

		private String buildContainerClass(boolean needsPaginations,
				boolean fixTableWidth, boolean fixTableHeight,
				boolean presetColumnWidth) {
			StringBuilder result = new StringBuilder("tawalaDataTable");
			if (needsPaginations) {
				result.append(" paginate");
			}
			if (fixTableWidth) {
				result.append(" dtFixTableWidth");
			}
			if (fixTableHeight) {
				result.append(" dtFixTableHeight");
			}
			if (presetColumnWidth) {
				result.append(" presetColumnWidth");
			}
			if (enablePrint) {
				result.append(" enablePrint");
			}
			if (enableExport) {
				result.append(" enableExport");
			}
			return result.toString();
		}
		
		private void displayResults(ExecutionContext context,
				List<ColumnInfo> displayableColumns,
				List<CompositeFormSubmission> records, StringBuilder result) {

			boolean evenRow = false;
			for (CompositeFormSubmission submission : records) {
				context.mapRecord(RecordSelector.DEFAULT_RECORD_LIST_NAME,
						submission);

				result.append(evenRow ? "<tr class=\"even\">"
						: "<tr class=\"odd\">");
				evenRow = !evenRow;
				
				RenderingContext renderingContext = new RenderingContext();
				for (ColumnInfo columnInfo : displayableColumns) {
					
					Html html = columnInfo.cellContent.toHtml(context);
					StringWriter stringWriter = new StringWriter();
					html
							.render(new PrintWriter(stringWriter),
									renderingContext);
					StringBuffer value = stringWriter.getBuffer();

					result.append("<td");
					if (columnInfo.cellStyle != null) {
						result.append(" style=\"").append(columnInfo.cellStyle)
								.append('"');
					}
					result.append('>');
					result.append(value).append("</td>");
				}
				
				if (includeTimestamps) {
					result.append("<td>");
					FormSubmission sub = submission.getFormSubmission(lastFieldReference);
					result.append(dateFormat.format(sub.getCreationDate()));
					result.append("</td>");

					result.append("<td>");
					result.append(sub.getUpdatedDate() == null ? "" : dateFormat.format(sub.getUpdatedDate()));
					result.append("</td>");
				}

				result.append("</tr>\n");
			}
		}

		private void displayNoData(ExecutionContext executionContext,
				List<ColumnInfo> displayableColumns, StringBuilder result) {
			result.append("<tr class=\"even\">");
			result.append("<td colspan=\"").append(displayableColumns.size())
					.append("\">");
			result.append(Repository.getMessageSource().getMessage(
					new Message(NO_DATA_MESSAGE_ID),
					executionContext.getLocale()));
			result.append("</td>");
			result.append("</tr>\n");
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public ResponseCreator getResponseCreatorForComponentId(
				String componentId) {
			return null;
		}
	}

	// Class designed primarily to provide choice text (rather than choice
	// label) HTML for multiple choice questions
	public static class FieldTextBlock extends TextBlock {
		private final String fieldName;
		private final RecordSelector recordSelector;

		public FieldTextBlock(ConfigElement config,
				RecordSelector recordSelector) {
			super(config);
			this.fieldName = config.child("field").attribute("name")
					.stringValue();
			this.recordSelector = recordSelector;
		}

		@Override
		protected boolean useEnclosingDiv() {
			return false;
		}

		@Override
		public Html produceHtml(ExecutionContext context) {
			Reference fieldReference = new Reference(fieldName, context);
			Field field = null;

			Form form = recordSelector.getForm(context, fieldReference
					.getFormName());
			if (form != null) {
				field = form.getFieldById(fieldReference.getFieldName());
			}

			if (isMultipleChoice(field)) {
				return getChoiceTextHtml(context, fieldReference,
						(MultipleChoice) field);
			} else {
				return super.produceHtml(context);
			}
		}

		private boolean isMultipleChoice(Field field) {
			return (field != null && field.getClass().equals(
					MultipleChoice.class));
		}

		private Html getChoiceTextHtml(ExecutionContext context,
				Reference fieldReference, MultipleChoice multipleChoice) {
			HtmlItems htmlResult = new HtmlItems();

			DisplayMultipleChoiceLabel.Runtime.addMultipleChoiceLabels(context,
					fieldReference, multipleChoice,
					DisplayMultipleChoiceLabel.DisplayType.label_only,
					htmlResult);

			return htmlResult;
		}
	}
}
