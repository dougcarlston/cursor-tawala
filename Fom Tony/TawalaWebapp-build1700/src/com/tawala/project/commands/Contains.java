package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class Contains extends StringCondition {
    public Contains(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(String left, String right) {
        return left.toLowerCase().indexOf(right.toLowerCase()) > -1;
    }

	@Override
	protected int getUniqueConditionCode() {
		return -922;
	}
}
