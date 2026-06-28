package com.tawala.component.web.display;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.springframework.web.util.HtmlUtils;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.repository.Repository;
import com.tawala.component.runtime.PopularChoiceAlgorithm;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.message.Message;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;

public class PopularChoiceTable extends WebComponentMetadataSupport {
	private static final Option[] RANK_OPTIONS = new Option[] {
			new Option("1", "option.first"), new Option("2", "option.second"),
			new Option("3", "option.third") };
	private static final String COMPONENT_ID = "popular-choice-correlation-table";
	public static final String RANK = "rank";
	public static final String POPULAR_CHOICE_DISPLAY_FIELD_NAME = "popular-choice-display-field-name";
	public static final String CHOICE_PREFERRED_FIELD_NAME = "choice-preferred-field-name";
	public static final String CHOICE_AVAILABLE_FIELD_NAME = "choice-available-field-name";
	public static final String CONDITIONS_FIELD_NAME = "conditions";

	public static final Map<Integer, Option> RANK_TO_OPTION_MAP = new HashMap<Integer, Option>();
	static {
		for (int i = 0; i < RANK_OPTIONS.length; i++) {
			RANK_TO_OPTION_MAP.put(
					Integer.parseInt(RANK_OPTIONS[i].getValue()),
					RANK_OPTIONS[i]);
		}
	}

	public PopularChoiceTable() {
		super(COMPONENT_ID, 1);
		addParameters(new Parameter[] {
				new Parameter(RANK, RANK, ParameterType.ENUMERATION, true)
						.addOptions(RANK_OPTIONS),
				new Parameter(CHOICE_AVAILABLE_FIELD_NAME,
						CHOICE_AVAILABLE_FIELD_NAME,
						ParameterType.MCQ_FIELD_NAME, true),
				new Parameter(CHOICE_PREFERRED_FIELD_NAME,
						CHOICE_PREFERRED_FIELD_NAME,
						ParameterType.MCQ_FIELD_NAME, true),
				new Parameter(POPULAR_CHOICE_DISPLAY_FIELD_NAME,
						POPULAR_CHOICE_DISPLAY_FIELD_NAME,
						ParameterType.BLANK_FIELD_NAME, true),
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
		private static final String NO_CHOICE_MESSAGE_ID = "web."
				+ COMPONENT_ID + ".no-ranked-choices.message";
		private final String displayFieldName;
		private final String preferredFieldName;
		private final String availableFieldName;
		private final int rank;
		private final RecordSelector recordSelector;

		public RuntimeProcessor(ConfigElement configElement) {
			displayFieldName = configElement.child(
					POPULAR_CHOICE_DISPLAY_FIELD_NAME).text();
			preferredFieldName = configElement.child(
					CHOICE_PREFERRED_FIELD_NAME).text();
			availableFieldName = configElement.child(
					CHOICE_AVAILABLE_FIELD_NAME).text();
			rank = Integer.parseInt(configElement.child(RANK).text());
			recordSelector = RecordSelector.instantiateFrom(configElement
					.child(CONDITIONS_FIELD_NAME));
		}

		public Html toHtml(ExecutionContext context) {
			Reference displayFieldReference = new Reference(displayFieldName,
					true);

			Reference preferredFieldReference = new Reference(
					preferredFieldName, true);

			StringBuilder result = new StringBuilder();
			result
					.append("<table class=\"component outline popularChoice sortable stripe\">\n");
			result.append("<thead>\n");
			result.append("<tr>\n");
			result.append("<th>").append(
					HtmlUtils.htmlEscape(displayFieldReference.getFieldName()))
					.append("</th>");
			result.append("<th>")
					.append(
							HtmlUtils.htmlEscape(preferredFieldReference
									.getFieldName())).append("</th>");
			result.append("</tr>\n");
			result.append("</thead>\n");
			result.append("<tbody>\n");

			List<PopularChoiceAlgorithm.RankedChoice> choices = new PopularChoiceAlgorithm(
					availableFieldName, recordSelector).calculate(context);
			if (choices == null || choices.size() < rank) {
				displayNoChoicesRanked(result, context, rank);
			} else {
				displayResults(context, result, choices, displayFieldReference,
						preferredFieldReference);
			}

			result.append("</tbody>\n");
			result.append("</table>\n");
			return new HtmlReadyString(result.toString());
		}

		private void displayResults(ExecutionContext context,
				StringBuilder result,
				List<PopularChoiceAlgorithm.RankedChoice> choices,
				Reference displayFieldReference,
				Reference preferredFieldReference) {
			Value choice = new Value(choices.get(rank - 1).value);

			Reference availableFieldReference = new Reference(
					availableFieldName, true);

			List<CompositeFormSubmission> records = recordSelector
					.getRecords(context);
			boolean evenRow = false;
			for (CompositeFormSubmission record : records) {
				FormSubmission submission = record
						.getFormSubmission(displayFieldReference);
				if (submission == null) {
					continue;
				}
				if (submission.getValues(availableFieldReference).contains(
						choice)) {
					if (evenRow) {
						result.append("<tr class=\"even\">");
						evenRow = false;
					} else {
						result.append("<tr class=\"odd\">");
						evenRow = true;
					}
					result.append("<td>");
					result.append(HtmlUtils.htmlEscape(submission.getValue(
							displayFieldReference).toString()));
					result.append("</td>");

					result.append("<td>");
					result
							.append(submission.contains(
									preferredFieldReference, choice.toString()) ? "<img src=\"/images/silk/tick.gif\" alt=\"X\" />"
									: "&nbsp;");
					result.append("</td>");

					result.append("</tr>\n");
				}
			}
		}

		private void displayNoChoicesRanked(StringBuilder result,
				ExecutionContext executionContext, int rank) {
			String rankDescription = "";
			Option selectedOption = PopularChoiceTable.RANK_TO_OPTION_MAP
					.get(rank);
			if (selectedOption == null) {
				Log.error(this, "Unable to find option for rank " + rank);
			} else {
				rankDescription = selectedOption
						.getDescription(executionContext.getLocale());
			}

			result.append("<tr class=\"even\">");
			result.append("<td>");
			result.append(Repository.getMessageSource().getMessage(
					new Message(NO_CHOICE_MESSAGE_ID, rankDescription),
					executionContext.getLocale()));
			result.append("</td>");
			result.append("</tr>\n");
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}
	}
}
