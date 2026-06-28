package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public abstract class MathCommand extends ProcessCommand {
    private String fieldId;
    private Operator operator;

    public MathCommand(ConfigElement config) {
        fieldId = config.attribute("field").stringValue();
        operator = Operator.FACTORY.makeChildren(config).get(0);

    }

    public ExecutionResult execute(ExecutionContext context) {
        Value operatorValue = operator.calculateValue(context);
        Value fieldValue = context.getValue(fieldId);
        SmartNumber result = calculate(operatorValue, fieldValue);
        context.setValue(fieldId, result.toString());
        return ExecutionResult.NULL;
    }

    protected abstract SmartNumber calculate(Value a, Value b);

    public static class AddTo extends MathCommand {
        public AddTo(ConfigElement config) {
            super(config);
        }

        protected SmartNumber calculate(Value a, Value b) {
            return b.asNumber().add(a.asNumber());
        }
    }

    public static class SubtractFrom extends MathCommand {
        public SubtractFrom(ConfigElement config) {
            super(config);
        }

        protected SmartNumber calculate(Value a, Value b) {
            return b.asNumber().subtract(a.asNumber());
        }
    }

    public static class MultiplyBy extends MathCommand {
        public MultiplyBy(ConfigElement config) {
            super(config);
        }

        protected SmartNumber calculate(Value a, Value b) {
            return b.asNumber().multiply(a.asNumber());
        }
    }

    public static class DivideBy extends MathCommand {
        public DivideBy(ConfigElement config) {
            super(config);
        }

        protected SmartNumber calculate(Value a, Value b) {
            return b.asNumber().divide(a.asNumber());
        }
    }
}
