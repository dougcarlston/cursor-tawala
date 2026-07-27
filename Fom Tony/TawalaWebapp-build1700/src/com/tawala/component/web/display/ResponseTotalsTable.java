package com.tawala.component.web.display;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.text.DecimalFormat;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.repository.Repository;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.component.web.form.MultiChoiceDataProvider;
import com.tawala.message.Message;
import com.tawala.project.Checkbox;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.FormSubmission;
import com.tawala.project.MultipleChoice;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.RenderingContext;

public class ResponseTotalsTable extends WebComponentMetadataSupport {
	private static final String COMPONENT_ID = "response-totals-table";
	public static final String FIELD_ID = "field";
	public static final String CONDITIONS_PARAMETER_ID = "conditions";
	public static final String LAYOUT_TYPE = "layout-type";

	public ResponseTotalsTable() {
		super(COMPONENT_ID, 1);

		addParameters(new Parameter[] {
				new Parameter(LAYOUT_TYPE, COMPONENT_ID + "_" + LAYOUT_TYPE,
						ParameterType.ENUMERATION, true).addOptions(LAYOUT_OPTIONS),
						
				new Parameter(FIELD_ID, COMPONENT_ID + "_" + FIELD_ID,
						ParameterType.MCQ_FIELD_NAME, true),
						
				new Parameter(CONDITIONS_PARAMETER_ID, COMPONENT_ID + "_"
						+ CONDITIONS_PARAMETER_ID,
						ParameterType.RECORD_SELECTOR, true) });
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	private static final Option[] LAYOUT_OPTIONS = new Option[] {
			new Option("vertical", "layout.vertical"),	
			new Option("horizontal", "layout.horizontal")
	};

	public static class RuntimeProcessor extends FormRenderableNotHoldingActiveComponents {
		private static final DecimalFormat COUNT_FORMAT = new DecimalFormat(
				"#,###,###,###");
		private static final int MAX_COLOR_COUNT = 16;

		private static final Message MESSAGE_NO_DATA = new Message(("web."
				+ COMPONENT_ID + ".no-data.message"));
		private static final Message HEADER_CHOICE = new Message("web."
				+ COMPONENT_ID + ".header-choice");
		private static final Message HEADER_COUNT = new Message("web."
				+ COMPONENT_ID + ".header-count");

		private static final String[] ROW_STYLE_CLASS_NAMES = { "odd", "even" };

		private final RecordSelector recordSelector;
		private final String fieldName;
		private final String layoutType;

		public RuntimeProcessor(ConfigElement configElement) {
			this.fieldName = configElement.child(FIELD_ID).text();
			this.layoutType = configElement.child(LAYOUT_TYPE).text();
			recordSelector = RecordSelector.instantiateFrom(configElement.child(CONDITIONS_PARAMETER_ID));
		}

		public Html toHtml(ExecutionContext context) {
			if (layoutType.equals("horizontal")) {
				return toHorizontalLayoutHtml(context);
			}
			return toVerticalLayoutHtml(context);
		}

		private Html toVerticalLayoutHtml(ExecutionContext context) {
			StringBuilder result = new StringBuilder();
			result.append("<table class=\"component outline sortable stripe\">\n");
			result.append("<thead>\n");
			result.append("<tr>\n");
			result.append("<th> ").append(
					Repository.getMessageSource().getMessage(HEADER_CHOICE,
							context.getLocale())).append(" </th>");
			result.append("<th> ").append(
					Repository.getMessageSource().getMessage(HEADER_COUNT,
							context.getLocale())).append(" </th>");
			result.append("</tr>\n");
			result.append("</thead>\n");
			result.append("<tbody>\n");

			List<CompositeFormSubmission> records = recordSelector
					.getRecords(context);

			if (records == null || records.size() == 0) {
				displayNoData(result, context);
			} else {
				Reference fieldReference = new Reference(fieldName, true);
				MultiChoiceDataProvider dataProvider = getMultiChoiceDataProvider(
						context, fieldReference);
				if (dataProvider == null) {
					displayNoData(result, context);
				} else {
					TallyResults tallyResults = tally(context, dataProvider,
							records, fieldReference);
					if (tallyResults.totalResponses == 0) {
						displayNoData(result, context);
					} else {
						displayResults(context, tallyResults, result);
					}
				}
				context.removeRecordList(RecordSelector.DEFAULT_RECORD_LIST_NAME);
			}

			result.append("</tbody>\n");
			result.append("</table>\n");
			return new HtmlReadyString(result.toString());
		}

		private Html toHorizontalLayoutHtml(ExecutionContext context) {
			StringBuilder result = new StringBuilder();
			result.append("<table class=\"component outline sortable stripe\">\n");
			result.append("<tbody>\n");

			List<CompositeFormSubmission> records = recordSelector
					.getRecords(context);

			if (records == null || records.size() == 0) {
				displayNoData(result, context);
			} else {
				Reference fieldReference = new Reference(fieldName, true);
				MultiChoiceDataProvider dataProvider = getMultiChoiceDataProvider(context, fieldReference);
				if (dataProvider == null) {
					displayNoData(result, context);
				} else {
					TallyResults tallyResults = tally(context, dataProvider, records, fieldReference);
					if (tallyResults.totalResponses == 0) {
						displayNoData(result, context);
					} else {
						displayHorizontalResults(context, tallyResults, result);
					}
				}
				context.removeRecordList(RecordSelector.DEFAULT_RECORD_LIST_NAME);
			}

			result.append("</tbody>\n");
			result.append("</table>\n");
			return new HtmlReadyString(result.toString());
		}

		private MultiChoiceDataProvider getMultiChoiceDataProvider(
				ExecutionContext context, Reference fieldReference) {
			Form form = context.getProject().getForm(
					fieldReference.getFormName());
			if (form == null) {
				return null;
			} else {
				Field question = form.getFieldById(fieldReference
						.getFieldName());
				if (MultipleChoice.class.isAssignableFrom(question.getClass())) {
					MultipleChoice mcq = (MultipleChoice) question;
					return mcq.getDataProvider();
				} else {
					return null;
				}
			}
		}

		private void displayResults(ExecutionContext context,
				TallyResults tallyResults, StringBuilder result) {

			int currentColorCount = 1;
			for (Checkbox checkbox : tallyResults.choices) {
				HtmlItems description = new HtmlItems();
				checkbox.appendDescription(context, description);
				StringWriter descriptionOutput = new StringWriter();
				description.render(new PrintWriter(descriptionOutput), new RenderingContext());

				result.append("<tr class=\"").append(
						ROW_STYLE_CLASS_NAMES[currentColorCount % 2]).append(
						"\">");
				// --- Description
				result.append("<td>").append(descriptionOutput.getBuffer())
						.append("</td>");

				// --- Count
				result.append("<td>").append(
						COUNT_FORMAT.format(tallyResults.counts.get(checkbox
								.getId())[0])).append("</td>");

				result.append("</tr>\n");

				if (++currentColorCount > MAX_COLOR_COUNT) {
					currentColorCount = 1;
				}
			}
		}

		private void displayHorizontalResults(ExecutionContext context,
				TallyResults tallyResults, StringBuilder result) {

			int currentColorCount = 1;

			result.append("<tr class=\"").append(ROW_STYLE_CLASS_NAMES[currentColorCount % 2]).append("\">");

			// --- Description heading
			result.append("<th class=\"leftHeading\"> ").append(
					Repository.getMessageSource().getMessage(HEADER_CHOICE,
							context.getLocale())).append(" </th>");

			for (Checkbox checkbox : tallyResults.choices) {
				HtmlItems description = new HtmlItems();
				checkbox.appendDescription(context, description);
				StringWriter descriptionOutput = new StringWriter();
				description.render(new PrintWriter(descriptionOutput), new RenderingContext());

				// --- Description
				result.append("<td>").append(descriptionOutput.getBuffer()).append("</td>");
			}

			result.append("</tr>\n");

			if (++currentColorCount > MAX_COLOR_COUNT) {
				currentColorCount = 1;
			}

			result.append("<tr class=\"").append(ROW_STYLE_CLASS_NAMES[currentColorCount % 2]).append("\">");

			// --- Count heading
			result.append("<th class=\"leftHeading\"> ").append(
					Repository.getMessageSource().getMessage(HEADER_COUNT,
							context.getLocale())).append(" </th>");

			for (Checkbox checkbox : tallyResults.choices) {
				HtmlItems description = new HtmlItems();
				checkbox.appendDescription(context, description);
				StringWriter descriptionOutput = new StringWriter();
				description.render(new PrintWriter(descriptionOutput), new RenderingContext());

				// --- Count
				result.append("<td>").append(COUNT_FORMAT.format(tallyResults.counts.get(checkbox.getId())[0])).append("</td>");
			}

			result.append("</tr>\n");
		}

		private void displayNoData(StringBuilder result,
				ExecutionContext executionContext) {
			result.append("<tr class=\"even\">");
			result.append("<td colspan=\"3\">");
			result.append(Repository.getMessageSource().getMessage(
					MESSAGE_NO_DATA, executionContext.getLocale()));
			result.append("</td>");
			result.append("</tr>\n");
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		private TallyResults tally(ExecutionContext context,
				MultiChoiceDataProvider dataProvider,
				List<CompositeFormSubmission> records, Reference reference) {

			TallyResults result = new TallyResults();
			result.choices = dataProvider.getChoices(context);
			result.counts = new LinkedHashMap<String, int[]>(result.choices
					.size());
			result.totalResponses = 0;

			for (Checkbox checkbox : result.choices) {
				result.counts.put(checkbox.getId(), new int[] { 0 });
			}

			for (CompositeFormSubmission record : records) {
				context.mapRecord(RecordSelector.DEFAULT_RECORD_LIST_NAME,
						record);
				FormSubmission submission = record.getFormSubmission(reference);
				if (submission == null)
					continue;

				for (Value value : submission.getValues(reference)) {
					int[] count = result.counts.get(value.toString());
					if (count == null) {
						// --- Unknown value. We just ignore it.
						continue;
					}
					count[0]++;
					result.totalResponses++;
				}
			}

			return result;
		}
	}

	public static class TallyResults {
		int totalResponses;
		List<Checkbox> choices;
		Map<String, int[]> counts;
	}
}
