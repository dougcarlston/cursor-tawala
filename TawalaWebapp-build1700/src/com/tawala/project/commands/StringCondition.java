package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public abstract class StringCondition extends SingleValueCondition {

    protected StringCondition(ConfigElement config) {
        super(config);
    }


    protected boolean isTrue(Value lValue, Value rValue) {
        String left = lValue.toString();
        String right = rValue.toString();
        return lValue != null && isTrue(left, right);
    }

    protected abstract boolean isTrue(String left, String right);
}
