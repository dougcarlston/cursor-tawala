package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class AddOperator extends ContainingOperator {

    public AddOperator(ConfigElement config) {
        super(config);
    }

    protected SmartNumber process(SmartNumber a, SmartNumber b) {
        return a.add(b);
    }

	@Override
	protected int getUniqueOperatorCode() {
		return 23;
	}
}
