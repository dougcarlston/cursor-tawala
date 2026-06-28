package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class NotEquals extends Equals {

    public NotEquals(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(Value lValue, Value rValue) {
        return !super.isTrue(lValue, rValue);
    }
    
    @Override
    public int nonObjectIdBasedHashCode() {
    	return super.nonObjectIdBasedHashCode() + 33;
    }
}
