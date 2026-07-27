package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class EndsWith extends StringCondition {

    public EndsWith(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(String left, String right) {
        return left.toLowerCase().endsWith(right.toLowerCase());
    }

	@Override
	protected int getUniqueConditionCode() {
		return -782;
	}

}
