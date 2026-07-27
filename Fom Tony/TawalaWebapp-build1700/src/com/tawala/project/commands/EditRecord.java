package com.tawala.project.commands;

import java.util.Collections;
import java.util.List;
import java.util.Set;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;

public class EditRecord extends ProcessCommand {
	private final String formName;
	private final boolean updateExistingRecord;
	private final BooleanExpression condition;
	private final RecordSelector recordSelector;

	public EditRecord(ConfigElement config) {
		this.formName = config.attribute("form").stringValue();

		ConfigElement conditionsElement = config.child("conditions");
		this.condition = conditionsElement == null
				|| conditionsElement.childElements().size() == 0 ? BooleanExpression.TRUE
				: BooleanExpression.load(conditionsElement.childElement(0));

		this.updateExistingRecord = !"new".equals(config.attribute("submit")
				.stringValue());

		RecordSelector.FormDataProvider formDataProvider = new RecordSelector.CurrentProjectFormDataProvider(
				formName);

		recordSelector = new RecordSelector(Collections
				.singletonList(formDataProvider), condition,
				RecordSelector.DEFAULT_RECORD_LIST_NAME);
	}

	public ExecutionResult execute(ExecutionContext context) {
		List<CompositeFormSubmission> result = recordSelector
				.getRecords(context);
		if (result == null || result.size() == 0) {
			return ExecutionResult.NULL;
		}

		CompositeFormSubmission firstRecord = result.get(0);
		FormSubmission submission = firstRecord.getAllSubmissions().iterator()
				.next();

		// --- This is done to force project initialization by Hibernate.
		// --- The result of a weird bug that couldn't reproduce in dev.
		submission.getProject().getForm("anyform");
	
		if (! updateExistingRecord) {
			// --- Make a copy of the stored submission.
			submission = new FormSubmission(submission);
		}
		submission.setBeingEdited(true);
		context.setSubmission(submission);

		return new ExecutionResult(formName);
	}

	@Override
	public void addFormsNamesCanNavigateTo(Set<String> result) {
		result.add(formName);
	}
}
