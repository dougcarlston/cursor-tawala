package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class DoesNotContain extends Contains {
    public DoesNotContain(ConfigElement config) {
        super(config);
    }

    protected boolean isTrue(Value lValue, Value rValue) {
        return !super.isTrue(lValue, rValue);
    }
    
    @Override
    protected int getUniqueConditionCode() {
    	return super.getUniqueConditionCode() + 123;
    }
}
