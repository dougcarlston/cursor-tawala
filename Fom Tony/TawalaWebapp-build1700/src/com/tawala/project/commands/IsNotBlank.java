package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class IsNotBlank extends IsBlank {
    public IsNotBlank(ConfigElement config) {
        super(config);
    }

    public boolean isTrue(ExecutionContext context) {
        return !super.isTrue(context);
    }
}
