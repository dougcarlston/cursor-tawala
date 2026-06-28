package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class MultiplyOperator extends ContainingOperator {

    public MultiplyOperator(ConfigElement config) {
        super(config);
    }

    protected SmartNumber process(SmartNumber a, SmartNumber b) {
        return a.multiply(b);
    }

	@Override
	protected int getUniqueOperatorCode() {
		return 45;
	}
}
