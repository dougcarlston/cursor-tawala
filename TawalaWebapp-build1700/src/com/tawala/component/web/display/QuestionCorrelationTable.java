package com.tawala.component.web.display;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.springframework.web.util.HtmlUtils;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.repository.Repository;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.message.Message;
import com.tawala.project.Checkbox;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.FormSubmission;
import com.tawala.project.MultipleChoice;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.RenderingContext;

public class QuestionCorrelationTable extends WebComponentMetadataSupport {
	private static final String COMPONENT_ID = "question-correlation-table";
	public static final String QUESTION_FIELD_NAME = "question-field-name";
	public static final String DISPLAY_FIELD_NAME = "display-field-name";
	public static final String PREFERRED_CHOICE_FIELD_NAME = "preferred-choice-field-name";
	public static final String CONDITIONS_FIELD_NAME = "conditions";

	public QuestionCorrelationTable() {
		super(COMPONENT_ID, 1);
		addParameters(new Parameter[] {
				new Parameter(QUESTION_FIELD_NAME, COMPONENT_ID + '_'
						+ QUESTION_FIELD_NAME, ParameterType.MCQ_FIELD_NAME,
						true),
				new Parameter(DISPLAY_FIELD_NAME, COMPONENT_ID + '_'
						+ DISPLAY_FIELD_NAME, ParameterType.BLANK_FIELD_NAME,
						true),
				new Parameter(PREFERRED_CHOICE_FIELD_NAME, COMPONENT_ID + '_'
						+ PREFERRED_CHOICE_FIELD_NAME,
						ParameterType.MCQ_FIELD_NAME, false),
				new Parameter(CONDITIONS_FIELD_NAME, COMPONENT_ID + "_"
						+ CONDITIONS_FIELD_NAME,
						ParameterType.RECORD_SELECTOR, true) });
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends
			FormRenderableNotHoldingActiveComponents {
		private static final String NO_DATA_MESSAGE_ID = "web." + COMPONENT_ID
				+ ".no-data.message";
		private final String displayFieldName;
		private final String preferredChoiceFieldName;
		private final String questionFieldName;
		private final RecordSelector recordSelector;

		public RuntimeProcessor(ConfigElement configElement) {
			displayFieldName = configElement.child(DISPLAY_FIELD_NAME).text();
			ConfigElement preferredChoiceElement = configElement
					.child(PREFERRED_CHOICE_FIELD_NAME);
			preferredChoiceFieldName = preferredChoiceElement == null ? null
					: preferredChoiceElement.text();
			questionFieldName = configElement.child(QUESTION_FIELD_NAME).text();
			recordSelector = RecordSelector.instantiateFrom(configElement
					.child(CONDITIONS_FIELD_NAME));

		}

		public Html toHtml(ExecutionContext context) {
			List<CompositeFormSubmission> rows = recordSelector
					.getRecords(context);

			try {
				Reference questionFieldReference = new Reference(
						questionFieldName, true);

				Form form = context.getProject().getForm(
						questionFieldReference.getFormName());
				if (form == null) {
					throw new IllegalStateException("Unable to find form "
							+ questionFieldReference.getFormName());
				}

				Field field = form.getFieldById(questionFieldReference
						.getFieldName());
				if (field == null) {
					throw new IllegalStateException("Unable to find field "
							+ questionFieldReference.getFieldName());
				}

				if (!MultipleChoice.class.isAssignableFrom(field.getClass())) {
					throw new IllegalStateException("Field "
							+ questionFieldReference.getFieldName()
							+ " is not a multiple choice field: "
							+ field.getClass());
				}

				List<Checkbox> options = ((MultipleChoice) field)
						.getDataProvider().getChoices(context);

				if (options.size() == 0 || rows == null || rows.size() == 0) {
					return displayNoData(context);
				} else {
					StringBuilder result = new StringBuilder();
					result
							.append("<table class=\"component outline sortable stripe\">\n");
					displayResults(context, result, rows,
							questionFieldReference, options);
					result.append("</table>\n");
					return new HtmlReadyString(result.toString());
				}

			} finally {
				context
						.removeRecordList(RecordSelector.DEFAULT_RECORD_LIST_NAME);
			}
		}

		private void displayResults(ExecutionContext context,
				StringBuilder result, List<CompositeFormSubmission> rows,
				Reference questionFieldReference, List<Checkbox> columns) {

			buildTableHeader(context, result, columns);

			Map<String, int[]> counts = buildTableBody(context, result, rows, questionFieldReference, columns);

			buildTableFooter(result, columns, includePreferredOptions(), counts, findBestCount(counts));
		}

		private void buildTableHeader(ExecutionContext context, StringBuilder result, List<Checkbox> columns) {

			result.append("<thead>\n");
			result.append("<tr>\n");
			result.append("<th>&nbsp;</th>");
			
			for (Checkbox checkbox : columns) {
				HtmlItems description = new HtmlItems();
				checkbox.appendDescription(context, description);
				StringWriter descriptionOutput = new StringWriter();
				description.render(new PrintWriter(descriptionOutput), new RenderingContext());

				result.append("<th>").append(descriptionOutput.toString()).append("</th>");
			}
			result.append("</tr>\n");
			result.append("</thead>\n");
		}

		private Map<String, int[]> buildTableBody(ExecutionContext context, StringBuilder result, List<CompositeFormSubmission> rows, Reference questionFieldReference, List<Checkbox> columns) {

			result.append("<tbody>\n");

			Reference displayFieldReference = new Reference(displayFieldName, true);

			Reference preferredChoiceFieldReference = includePreferredOptions() ?
					new Reference(preferredChoiceFieldName, true): null;

			Map<String, int[]> counts = new HashMap<String, int[]>();
			boolean evenRow = false;
			for (CompositeFormSubmission row : rows) {
				if (evenRow) {
					result.append("<tr class=\"even\">");
					evenRow = false;
				} else {
					result.append("<tr class=\"odd\">");
					evenRow = true;
				}

				FormSubmission formSubmission = row
						.getFormSubmission(displayFieldReference);
				result.append("<th> ").append(
						HtmlUtils.htmlEscape(formSubmission.getValue(
								displayFieldReference).toString())).append(
						" </th>");

				for (Checkbox column : columns) {
					result.append("<td class=\"center\">");

					boolean preferred = includePreferredOptions() && formSubmission.contains(preferredChoiceFieldReference, column.getId());
					String iconName = preferred ? "star.png" : "tick.gif";

					boolean checked = formSubmission.contains(questionFieldReference, column.getId());
					
					if (checked) {
						int[] count = counts.get(column.getId());
						if (count == null) {
							count = new int[2];
							count[0] = 0;
							count[1] = 0;
							counts.put(column.getId(), count);
						}
						count[0]++;
						if (preferred) {
							count[1]++;
						}
					}
					String altText;
					if(preferred) {
						altText = "1";
					}else{
						altText = "2";
					}
					result.append(checked ? "<img src=\"/images/silk/" + iconName + "\" alt=\"" + altText + "\" />" : "&nbsp;");
					result.append("</td>");
				}
				result.append("</tr>\n");
			}

			result.append("</tbody>\n");

			return counts;
		}

		private boolean includePreferredOptions() {
			boolean includePreferredOptions = preferredChoiceFieldName != null && preferredChoiceFieldName.length() != 0;
			return includePreferredOptions;
		}

		private void buildTableFooter(StringBuilder result, List<Checkbox> columns, boolean includePreferredOptions, Map<String, int[]> counts, int[] bestOption) {

			result.append("<tfoot>\n");
			result.append("<tr>\n");
			result.append("<th>");
			result.append("<div align=\"right\"><b>Totals:</b>");
			result.append("</div>");
			result.append("</th>");

			for (Checkbox checkbox : columns) {
				int[] count = counts.get(checkbox.getId());
				int total = count == null ? 0 : count[0];
				int preferredCount = count == null ? 0 : count[1];

				boolean thisIsTheBestOption =
					total == bestOption[0] &&
					preferredCount == bestOption[1] &&
					includePreferredOptions;

				result.append("<td>");
				
				if (thisIsTheBestOption) {
					result.append(" <b>");
				}
				
				result.append(total);
				
				if (includePreferredOptions) {
					result.append("&nbsp;(").append(preferredCount).append(')');
				}
				
				if (thisIsTheBestOption) {
					result.append(" </b>&nbsp;");
				}
				
				if (thisIsTheBestOption && includePreferredOptions) {
					result.append(" <img src=\"/images/silk/star.png\" alt=\"x\" />");
				}
				
				result.append("</td>");
			}

			result.append("</tr>\n");

			result.append("<tr class=\"legend\">\n");
			result.append("<td colspan=\"" + (columns.size() + 1) + "\" align=\"right\">");
			
			if (includePreferredOptions) {
				result.append("preferred count in brackets");
				result.append("<br />\n");
				result.append("best option in <b>bold</b>");
				result.append("<br />\n");
				result.append("<img src=\"/images/silk/tick.gif\" alt=\"X\" /> - selected");
				result.append("&nbsp; &nbsp;");
			}

			if (includePreferredOptions) {
				result
						.append("<img src=\"/images/silk/star.png\" alt=\"X\" /> - preferred");
			}
			
			result.append("</td>");

			result.append("</tfoot>\n");
		}

		private int[] findBestCount(Map<String, int[]> counts) {
			int[] result = new int[2];
			result[0] = 0;
			result[1] = 0;
			for (int[] count : counts.values()) {
				if (count[0] >= result[0] && count[1] >= result[1]) {
					result = count;
				}
			}
			return result;
		}

		private Html displayNoData(ExecutionContext executionContext) {
			return new HtmlString(Repository.getMessageSource().getMessage(
					new Message(NO_DATA_MESSAGE_ID),
					executionContext.getLocale()));
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}
	}
}
