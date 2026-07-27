package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class Or extends ConditionList {
    public Or(ConfigElement config) {
        super(config);
    }

    protected boolean add(boolean a, boolean b) {
        return a || b;
    }

	@Override
	protected int getUniqueConditionId() {
		return 932;
	}
}
