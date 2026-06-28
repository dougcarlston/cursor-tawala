package com.tawala.component.web.form;

import com.tawala.project.MultipleChoice;


public class VerticalMultiChoiceDisplayComponent extends
		DefaultMultiChoiceDisplayComponent {

	public VerticalMultiChoiceDisplayComponent(MultipleChoice question) {
		super(question);
	}

	@Override
	protected String getOnlyOneStyleClass() {
		return "mcRadio vertical";
	}

	@Override
	protected String getMultipleStyleClass() {
		return "mcCheckbox vertical";
	}
}
