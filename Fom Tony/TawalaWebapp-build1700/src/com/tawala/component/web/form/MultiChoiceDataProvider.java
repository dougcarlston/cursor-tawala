package com.tawala.component.web.form;

import java.util.List;

import com.tawala.project.Checkbox;
import com.tawala.project.MultipleChoice;
import com.tawala.project.commands.ExecutionContext;

public interface MultiChoiceDataProvider {
	void setQuestion(MultipleChoice question);
	List<Checkbox> getChoices(ExecutionContext context);
	boolean isSafeToGetChoicesWithoutRealContext();
}
