package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class Equals extends SingleValueCondition {

    public Equals(String fieldId, String match) {
        super(new ReferenceOperator(fieldId), new LiteralOperator(match));
    }

    public Equals(Operator left, Operator right) {
        super(left, right);
    }

    public Equals(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(Value lValue, Value rValue) {
        return lValue != null && lValue.equals(rValue);
    }

	@Override
	protected int getUniqueConditionCode() {
		return 456;
	}
}
