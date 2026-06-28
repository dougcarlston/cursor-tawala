package com.tawala.component.web.form;

import com.tawala.project.MultipleChoice;

abstract class MultiChoiceDataProviderRuntimeSupport implements MultiChoiceDataProvider {
	private MultipleChoice question;

	final public void setQuestion(MultipleChoice question) {
		this.question = question;
	}

	final public MultipleChoice getQuestion() {
		return question;
	}
}
