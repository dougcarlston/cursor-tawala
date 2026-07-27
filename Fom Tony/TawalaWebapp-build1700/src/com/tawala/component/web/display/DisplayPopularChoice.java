package com.tawala.component.web.display;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.CommonParameter;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.repository.Repository;
import com.tawala.component.runtime.PopularChoiceAlgorithm;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.message.Message;
import com.tawala.project.Checkbox;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.MultipleChoice;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlString;

public class DisplayPopularChoice extends WebComponentMetadataSupport {
	private static final Option[] RANK_OPTIONS = new Option[] {
			new Option("1", "option.first"), new Option("2", "option.second"),
			new Option("3", "option.third") };
	private static final String COMPONENT_ID = "popular-choice-display";
	public static final String RANK = "rank";
	public static final String CHOICE_AVAILABLE_FIELD_NAME = CommonParameter.POPULAR_CHOICE_QUESTION;
	public static final String CONDITIONS_FIELD_NAME = "conditions";

	public static final Map<Integer, Option> RANK_TO_OPTION_MAP = new HashMap<Integer, Option>();
	static {
		for (int i = 0; i < RANK_OPTIONS.length; i++) {
			RANK_TO_OPTION_MAP.put(
					Integer.parseInt(RANK_OPTIONS[i].getValue()),
					RANK_OPTIONS[i]);
		}
	}

	public DisplayPopularChoice() {
		super(COMPONENT_ID, 1);
		addParameters(new Parameter[] {
				new Parameter(RANK, RANK, ParameterType.ENUMERATION, true)
						.addOptions(RANK_OPTIONS),
				new Parameter(CHOICE_AVAILABLE_FIELD_NAME,
						CHOICE_AVAILABLE_FIELD_NAME,
						ParameterType.MCQ_FIELD_NAME, true),
				new Parameter(CONDITIONS_FIELD_NAME, COMPONENT_ID + "_"
						+ CONDITIONS_FIELD_NAME,
						ParameterType.RECORD_SELECTOR, true) });

	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends FormRenderableNotHoldingActiveComponents {
		private static final String NO_CHOICE_MESSAGE_ID = "web."
				+ COMPONENT_ID + ".no-ranked-choices.message";
		private final String availableFieldName;
		private final int rank;
		private final RecordSelector recordSelector;

		public RuntimeProcessor(ConfigElement configElement) {
			availableFieldName = configElement.child(
					CHOICE_AVAILABLE_FIELD_NAME).text();
			rank = Integer.parseInt(configElement.child(RANK).text());
			recordSelector = RecordSelector.instantiateFrom(configElement
					.child(CONDITIONS_FIELD_NAME));
		}

		public Html toHtml(ExecutionContext context) {
			HtmlItems result = new HtmlItems();

			List<PopularChoiceAlgorithm.RankedChoice> choices = new PopularChoiceAlgorithm(
					availableFieldName, recordSelector).calculate(context);
			if (choices == null || choices.size() < rank) {
				displayNoChoicesRanked(result, context, rank);
			} else {
				displayResults(context, result, choices);
			}
			return result;
		}

		private void displayResults(ExecutionContext context, HtmlItems result,
				List<PopularChoiceAlgorithm.RankedChoice> choices) {
			Value choice = new Value(choices.get(rank - 1).value);

			Reference availableFieldReference = new Reference(
					availableFieldName, true);
			Form form = context.getProject().getForm(
					availableFieldReference.getFormName());
			if (form == null) {
				Log.error(this, "Project " + context.getProject().getId()
						+ " doesn't have form '"
						+ availableFieldReference.getFormName() + "'");
				return;
			}

			Field field = form.getFieldById(availableFieldReference
					.getFieldName());
			if (field == null) {
				Log.error(this, "Project " + context.getProject().getId()
						+ " doesn't have field '" + availableFieldReference
						+ "'");
				return;
			}

			if (!field.getClass().equals(MultipleChoice.class)) {
				Log.error(this, "Project " + context.getProject().getId()
						+ " has field '" + availableFieldReference
						+ "' which is not a multiple choice.");
				return;
			}

			MultipleChoice multipleChoice = (MultipleChoice) field;
			for (Checkbox checkbox : multipleChoice.getItems(context)) {
				if (checkbox.getId().equals(choice.toString())) {
					checkbox.appendDescription(context, result);
					return;
				}
			}
			Log.error(this, "None of the choices matched the value '"
					+ choice.toString() + "'.");
		}

		private void displayNoChoicesRanked(HtmlItems result,
				ExecutionContext executionContext, int rank) {
			String rankDescription = "";
			Option selectedOption = DisplayPopularChoice.RANK_TO_OPTION_MAP
					.get(rank);
			if (selectedOption == null) {
				Log.error(this, "Unable to find option for rank " + rank);
			} else {
				rankDescription = selectedOption
						.getDescription(executionContext.getLocale());
			}

			result.add(new HtmlString(Repository.getMessageSource().getMessage(
					new Message(NO_CHOICE_MESSAGE_ID, rankDescription),
					executionContext.getLocale())));
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}
	}
}
