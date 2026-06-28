package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class BeginsWith extends StringCondition {

    public BeginsWith(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(String left, String right) {
        return left.toLowerCase().startsWith(right.toLowerCase());
    }

	@Override
	protected int getUniqueConditionCode() {
		return 1123;
	}
}
