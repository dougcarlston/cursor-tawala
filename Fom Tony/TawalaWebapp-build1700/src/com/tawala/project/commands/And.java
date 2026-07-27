package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class And extends ConditionList {
    public And(ConfigElement config) {
        super(config);
    }
    
    public And() {
    }

    protected boolean add(boolean a, boolean b) {
        return a && b;
    }

	@Override
	protected int getUniqueConditionId() {
		return 99;
	}
}
