package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class IsGreaterThan extends NumericCondition {
    public IsGreaterThan(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(double left, double right) {
        return left > right;
    }

	@Override
	protected int getUniqueConditionCode() {
		return -12;
	}
}
