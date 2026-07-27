package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class IsLessThanOrEqualTo extends NumericCondition {
    public IsLessThanOrEqualTo(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(double left, double right) {
        return left <= right;
    }

	@Override
	protected int getUniqueConditionCode() {
		return -9823;
	}
}
