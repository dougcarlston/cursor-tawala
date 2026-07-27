package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class IsBlank extends BooleanExpression {
    private final ReferenceOperator left;

    public IsBlank(ConfigElement config) {
        left = parseLeft(config);
    }

    public boolean isTrue(ExecutionContext context) {
        return "".equals(left.calculateValue(context).toString().trim());
    }

	@Override
	public int nonObjectIdBasedHashCode() {
		return left.nonObjectIdBasedHashCode() + 623;
	}
}
