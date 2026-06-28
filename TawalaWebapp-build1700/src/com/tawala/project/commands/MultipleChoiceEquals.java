package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class MultipleChoiceEquals extends MultipleChoiceCondition {
    public MultipleChoiceEquals(ConfigElement config) {
        super(config);
    }

	@Override
	protected boolean performOperation(List<Value> leftResponses, List<Value> rightResponses) {
        if (leftResponses == null || rightResponses == null) {
        	return false;
        }
        
        return leftResponses.equals(rightResponses);
	}

	@Override
	protected int getUniqueConditionId() {
		return 799;
	}
}
