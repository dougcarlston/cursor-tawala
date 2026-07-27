package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class MultipleChoiceContains extends MultipleChoiceCondition {

    public MultipleChoiceContains(ConfigElement config) {
        super(config);
    }

    @Override
    protected boolean performOperation(List<Value> leftResponses, List<Value> rightResponses) {
		if (leftResponses == null || rightResponses == null) {
			return false;
		}

		if (rightResponses.size() == 0) {
			return false;
		}

		// the assumption right now is that we are not comparing multi-value
		// mcq to a multi-value mcq.
		return leftResponses.contains(rightResponses.get(0));
    }

	@Override
	protected int getUniqueConditionId() {
		return 146;
	}

}
