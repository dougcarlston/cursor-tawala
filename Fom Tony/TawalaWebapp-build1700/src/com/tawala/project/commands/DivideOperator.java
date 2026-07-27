package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class DivideOperator extends ContainingOperator {

    public DivideOperator(ConfigElement config) {
        super(config);
    }

    protected SmartNumber process(SmartNumber a, SmartNumber b) {
        return a.divide(b);
    }

	@Override
	protected int getUniqueOperatorCode() {
		return 12;
	}
}
