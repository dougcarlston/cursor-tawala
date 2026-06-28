package com.tawala.component.web.form;

import com.tawala.project.MultipleChoice;


public class HorizontalMultiChoiceDisplayComponent extends
		DefaultMultiChoiceDisplayComponent {
	
	public HorizontalMultiChoiceDisplayComponent(MultipleChoice question) {
		super(question);
	}

	@Override
	protected String getOnlyOneStyleClass() {
		return "mcRadio horizontal";
	}
	
	@Override
	protected String getMultipleStyleClass() {
		return "mcCheckbox horizontal";
	}
}
