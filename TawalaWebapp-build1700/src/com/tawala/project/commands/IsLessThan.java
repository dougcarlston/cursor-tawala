package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class IsLessThan extends NumericCondition {
    public IsLessThan(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(double left, double right) {
        return left < right;
    }

	@Override
	protected int getUniqueConditionCode() {
		return -394;
	}
}
