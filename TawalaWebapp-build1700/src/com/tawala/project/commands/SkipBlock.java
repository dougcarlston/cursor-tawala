package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;

public class SkipBlock extends ProcessCommandList {
    private static final Factory<ProcessCommand> FACTORY = new Factory<ProcessCommand>();
    public static final String SKIP_TO_END = "__EndOfForm__";

    static {
        FACTORY.register("skip", SkipCommand.class);
        FACTORY.register("if", SkipIf.class);
        FACTORY.register("set", SetCommand.class);
        FACTORY.register("addTo", MathCommand.AddTo.class);
        FACTORY.register("subtractFrom", MathCommand.SubtractFrom.class);
        FACTORY.register("multiplyBy", MathCommand.MultiplyBy.class);
        FACTORY.register("divideBy", MathCommand.DivideBy.class);
        FACTORY.ignore("comment");
    }

    public SkipBlock(ConfigElement config) {
        super();
        addAll(FACTORY.makeChildren(config));
    }

    public SkipExecutionResult execute(ExecutionContext context) {
        SkipExecutionResult finalResult = new SkipExecutionResult();
        for (ProcessCommand command : getContents()) {
            finalResult = finalResult.add(command.execute(context));
            if (finalResult.hasSkipTo()) {
                break;
            }
        }
        return finalResult;
    }


}
