package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class SetCommand extends ProcessCommand {
    private StringConcatenationExpression expression;
    private String fieldId;

    public SetCommand(ConfigElement config) {
        super();
        this.expression = new StringConcatenationExpression(config);
        this.fieldId = config.attribute("field").stringValue();
        config.attribute("expression").markUsed();
    }


    public ExecutionResult execute(ExecutionContext context) {
        context.setValue(fieldId, expression.evaluate(context));
        return ExecutionResult.NULL;
    }
}
