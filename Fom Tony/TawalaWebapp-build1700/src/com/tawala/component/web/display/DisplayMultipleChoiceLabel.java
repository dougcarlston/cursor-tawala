package com.tawala.component.web.display;

import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterRestriction;
import com.tawala.component.ParameterType;
import com.tawala.component.ReturnType;
import com.tawala.component.parameter.WorksWithinRecordIteration;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.project.Checkbox;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.FormSubmission;
import com.tawala.project.MultipleChoice;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlString;

public class DisplayMultipleChoiceLabel extends WebComponentMetadataSupport {
	public static enum DisplayType {
		label_only {
			public void display(HtmlItems result, ExecutionContext context,
					MultipleChoice multipleChoice, Set<String> selectedValues) {
				DisplayMultipleChoiceLabel.Runtime.displayLabelsOnly(result,
						context, multipleChoice, selectedValues);
			}
		},
		all_choices {
			public void display(HtmlItems result, ExecutionContext context,
					MultipleChoice multipleChoice, Set<String> selectedValues) {
				DisplayMultipleChoiceLabel.Runtime.displayAllChoices(result,
						context, multipleChoice, selectedValues);
			}
		};

		abstract public void display(HtmlItems result,
				ExecutionContext context, MultipleChoice multipleChoice,
				Set<String> selectedValues);
	}

	private static final String COMPONENT_ID = "display-mcq-label";
	public static final String FIELD_NAME_PARAMETER = "field-name";
	public static final String DISPLAY_PARAMETER = "display";

	private static final Option[] DISPLAY_OPTIONS = new Option[] {
			new Option(DisplayType.label_only.toString(), "option."
					+ COMPONENT_ID + "_" + DisplayType.label_only.toString()),
			new Option(DisplayType.all_choices.toString(), "option."
					+ COMPONENT_ID + "_" + DisplayType.all_choices.toString()) };

	public DisplayMultipleChoiceLabel() {
		super(COMPONENT_ID, 1);
		addParameters(new Parameter[] {
		// ---
				new Parameter(
						FIELD_NAME_PARAMETER,
						COMPONENT_ID + "_" + FIELD_NAME_PARAMETER,
						ParameterType.MCQ_FIELD_NAME,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				// ---
				new Parameter(DISPLAY_PARAMETER, COMPONENT_ID + "_"
						+ DISPLAY_PARAMETER, ParameterType.ENUMERATION, true)
						.addOptions(DISPLAY_OPTIONS) });
	}

	public ReturnType getReturnType() {
		return ReturnType.STRING;
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return Runtime.class;
	}

	public static class Runtime extends
			FormRenderableNotHoldingActiveComponents {
		private final String fieldName;
		private DisplayType displayType;

		public Runtime(ConfigElement configElement) {
			this.fieldName = configElement.child(FIELD_NAME_PARAMETER).text();
			try {
				this.displayType = DisplayType.valueOf(configElement.child(
						DISPLAY_PARAMETER).text());
			} catch (IllegalArgumentException e) {
				Log.error(this, "Error instantiating display type: ", e);
				this.displayType = DisplayType.label_only;
			}
		}

		public Html toHtml(ExecutionContext context) {
			if(context.isPreviewMode()) {
				return generatePreviewHtml(context);
			} else {
				return generateRealHtml(context);
			}
		}

		private Html generatePreviewHtml(ExecutionContext context) {
			return new HtmlString("<<Responses to " + fieldName + ">>");
		}

		private Html generateRealHtml(ExecutionContext context) {
			HtmlItems result = new HtmlItems();
			Reference fieldReference = new Reference(fieldName, context);

			Form form = context.getProject().getForm(
					fieldReference.getFormName());
			if (form == null) {
				Log.error(this, "Unable to find form '"
						+ fieldReference.getFormName() + "'");
				return result;
			}
			Field field = form.getFieldById(fieldReference.getFieldName());
			if (field == null) {
				Log.error(this, "Unable to find field '"
						+ fieldReference.getFieldName() + "'");
				return result;
			}
			if (field.getClass() != MultipleChoice.class) {
				Log.error(this, "Field '" + fieldReference.getFieldName()
						+ "' is of class " + field.getClass());
				return result;
			}

			MultipleChoice multipleChoice = (MultipleChoice) field;

			addMultipleChoiceLabels(context, fieldReference, multipleChoice,
					displayType, result);

			return result;
		}

		public static void addMultipleChoiceLabels(ExecutionContext context,
				Reference fieldReference, MultipleChoice multipleChoice,
				DisplayType displayType, HtmlItems result) {
			FormSubmission submission = context.getSubmission(fieldReference);
			if (submission == null) {
				return;
			}

			List<Value> selections = submission.getValues(fieldReference);

			Set<String> selectedValues = new HashSet<String>();
			for (Value selection : selections) {
				selectedValues.add(selection.toString());
			}

			displayType
					.display(result, context, multipleChoice, selectedValues);
		}

		static void displayAllChoices(HtmlItems result,
				ExecutionContext context, MultipleChoice multipleChoice,
				Set<String> selectedValues) {
			result.add(multipleChoice.getDisplayComponent().toHtml(context,
					selectedValues));
		}

		static void displayLabelsOnly(HtmlItems result,
				ExecutionContext context, MultipleChoice multipleChoice,
				Set<String> selectedValues) {
			boolean firstOne = true;
			for (Checkbox checkbox : multipleChoice.getItems(context)) {
				if (selectedValues.contains(checkbox.getId())) {
					if (firstOne) {
						firstOne = false;
					} else {
						result.add(new HtmlString(", "));
					}
					checkbox.appendDescription(context, result);
				}
			}
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}
	}
}
