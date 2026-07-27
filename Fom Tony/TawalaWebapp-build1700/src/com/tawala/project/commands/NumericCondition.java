package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public abstract class NumericCondition extends SingleValueCondition {

    protected NumericCondition(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(Value lValue, Value rValue) {
        return lValue != null && isTrue(lValue.asNumber().doubleValue(), rValue.asNumber().doubleValue());
    }

    protected abstract boolean isTrue(double left, double right);
}
