package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class SubtractOperator extends ContainingOperator {

    public SubtractOperator(ConfigElement config) {
        super(config);
    }

    protected SmartNumber process(SmartNumber a, SmartNumber b) {
        return a.subtract(b);
    }

	@Override
	protected int getUniqueOperatorCode() {
		return 78;
	}
}
