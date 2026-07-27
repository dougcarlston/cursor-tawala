package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.commands.RecordSelector.FormDataProvider;

public class Get extends ProcessCommand {
	private List<FormDataProvider> formDataProviders = new ArrayList<FormDataProvider>();
	private String recordListName;
	private BooleanExpression condition;

	public Get(ConfigElement config) {
		super();
		recordListName = config.attribute("recordList").stringValue();
		ConfigElement forms = config.child("forms");
		if (forms == null) {
			throw new IllegalArgumentException("Unable to find 'forms' element");
		}

		formDataProviders = RecordSelector.FORM_DATA_PROVIDER_FACTORY
				.makeChildren(forms);

		ConfigElement conditionElement = config.child("conditions");

		condition = conditionElement == null
				|| conditionElement.childElements().size() == 0 ? BooleanExpression.TRUE
				: BooleanExpression.load(conditionElement.childElement(0));
	}

	public ExecutionResult execute(ExecutionContext context) {

		RecordSelector recordSelector = new RecordSelector(formDataProviders,
				condition, recordListName);
		List<CompositeFormSubmission> result = recordSelector
				.getRecords(context);

		if (result == null) {
			context.removeRecordList(recordListName);
		} else {
			context.setRecordList(recordListName, result);
		}

		return ExecutionResult.NULL;
	}

	public String recordListName() {
		return recordListName;
	}

	public List<FormDataProvider> getFormDataProviders() {
		return formDataProviders;
	}
}
