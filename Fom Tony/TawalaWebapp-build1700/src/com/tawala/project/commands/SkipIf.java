package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class SkipIf extends If {
    public SkipIf(ConfigElement config) {
        super(BooleanExpression.load(config.child("conditions").childElement(0)),
                new SkipBlock(config.child("trueSet")),
                new SkipBlock(config.child("falseSet")));
    }
}
