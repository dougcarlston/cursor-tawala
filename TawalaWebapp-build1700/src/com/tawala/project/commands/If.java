package com.tawala.project.commands;

import java.util.Set;

import com.scissor.xmlconfig.ConfigElement;

public class If extends ProcessCommand {
    private final BooleanExpression condition;
    private final ProcessCommand then;
    private final ProcessCommand otherwise;

    public If(BooleanExpression condition, ProcessCommand then) {
        this(condition, then, ProcessCommand.NULL);
    }

    public If(BooleanExpression condition, ProcessCommand then, ProcessCommand otherwise) {
        super();
        this.condition = (condition == null ? BooleanExpression.NULL : condition);
        this.then = (then == null ? ProcessCommand.NULL : then);
        this.otherwise = (otherwise == null ? ProcessCommand.NULL : otherwise);
    }

    public If(ConfigElement config) {
        this(BooleanExpression.load(config.child("conditions").childElement(0)),
                new ProcessCommandList(config.child("trueSet")),
                new ProcessCommandList(config.child("falseSet")));
    }

    public ExecutionResult execute(ExecutionContext context) {
        if (condition.isTrue(context)) {
            return then.execute(context);
        } else {
            return otherwise.execute(context);
        }
    }

    public BooleanExpression condition() {
        return condition;
    }

    public ProcessCommand getThen() {
        return then;
    }

    public ProcessCommand getOtherwise() {
        return otherwise;
    }
    
    @Override
    public void addFormsNamesCanNavigateTo(Set<String> result) {
    	then.addFormsNamesCanNavigateTo(result);
    	otherwise.addFormsNamesCanNavigateTo(result);
    }
}
