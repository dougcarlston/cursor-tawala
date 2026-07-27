package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class SkipCommand extends ProcessCommand {
    private String skipTo;

    public ExecutionResult execute(ExecutionContext context) {
        return new SkipExecutionResult(skipTo);
    }

    public SkipCommand(ConfigElement config) {
        this.skipTo = config.attribute("to").stringValue();
    }
}
