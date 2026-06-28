package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class MultipleChoiceDoesNotEqual extends MultipleChoiceEquals {

    public MultipleChoiceDoesNotEqual(ConfigElement config) {
        super(config);
    }

    @Override
    protected boolean performOperation(List<Value> leftResponses, List<Value> rightResponses) {
    	return ! super.performOperation(leftResponses, rightResponses);
    }
    
    @Override
    protected int getUniqueConditionId() {
    	return super.getUniqueConditionId() + 3;
    }
}
