package com.tawala.component.web.display;

import java.util.List;

import org.springframework.web.util.HtmlUtils;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.ComponentRuntimeSupport;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.repository.Repository;
import com.tawala.component.web.ResponseCreator;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.message.Message;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormRenderable;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;

public class SimpleList extends WebComponentMetadataSupport {
	public static final String FIELD_NAME = "simple-list-field";
	public static final String CONDITIONS_NAME = "conditions";
	private static final String COMPONENT_ID = "simple-list";

	public SimpleList() {
		super(COMPONENT_ID, 2);
		addParameters(new Parameter[] {
				new Parameter(FIELD_NAME, FIELD_NAME,
						ParameterType.BLANK_FIELD_NAME, true),
				new Parameter(CONDITIONS_NAME, COMPONENT_ID + "_"
						+ CONDITIONS_NAME, ParameterType.RECORD_SELECTOR,
						true) });
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends ComponentRuntimeSupport implements FormRenderable {
		private static final int MINIMUM_REQUIRED_VERSION = 2;
		private static final String NO_DATA_MESSAGE_ID = "web." + COMPONENT_ID
				+ ".no-data.message";
		private final String fieldName;
		private final RecordSelector recordSelector;

		public RuntimeProcessor(ConfigElement configElement) {
			super(configElement);
			if(getVersion() != MINIMUM_REQUIRED_VERSION) {
				throw new IllegalStateException("This component is not supporting versions earlier than " + MINIMUM_REQUIRED_VERSION);
			}
			this.fieldName = configElement.child(FIELD_NAME).text();
			this.recordSelector = RecordSelector.instantiateFrom(configElement.child(CONDITIONS_NAME));
		}

		public Html toHtml(ExecutionContext context) {
			StringBuilder result = new StringBuilder();
			result.append("<table class=\"component outline sortable stripe\">\n");
			result.append("<tbody>\n");

			List<CompositeFormSubmission> records = recordSelector.getRecords(context);
			
			Reference fieldReference = new Reference(fieldName, true);
			if (fieldReference.getFormName() == null) {
				Log.error(this, "Unable to extract form name from '"
						+ fieldName + "'");
				displayNoData(result, context);
			} else if(records == null || records.size() == 0) {
				displayNoData(result, context);
			} else {
				displayResults(context, records, result, fieldReference);
			}

			result.append("</tbody>\n");
			result.append("</table>\n");
			return new HtmlReadyString(result.toString());
		}

		private void displayResults(ExecutionContext context,
				List<CompositeFormSubmission> records, StringBuilder result,
				Reference fieldReference) {

			boolean evenRow = false;
			for (CompositeFormSubmission record : records) {
				FormSubmission submission = record.getFormSubmission(fieldReference);
				List<Value> values = submission.getValues(fieldReference);
				if (values.size() != 0) {
					result.append(evenRow ? "<tr class=\"even\">" : "<tr class=\"odd\">");
					evenRow = ! evenRow;
					
					result.append("<td>");

					boolean hasPreviousValue = false;
					for (Value value : values) {
						if (hasPreviousValue) {
							result.append(", ");
						}
						result.append(HtmlUtils.htmlEscape(value.toString()));
						hasPreviousValue = true;
					}
					result.append("</td>");
					result.append("</tr>\n");
				}
			}
		}

		private void displayNoData(StringBuilder result,
				ExecutionContext executionContext) {
			result.append("<tr class=\"even\">");
			result.append("<td>");
			result.append(Repository.getMessageSource().getMessage(
					new Message(NO_DATA_MESSAGE_ID),
					executionContext.getLocale()));
			result.append("</td>");
			result.append("</tr>\n");
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public ResponseCreator getResponseCreatorForComponentId(String componentId) {
			return null;
		}
	}
}
