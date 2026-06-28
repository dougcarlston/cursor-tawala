package com.tawala.project.commands;

import java.util.Collections;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class LiteralOperator extends Operator {
    private final Value value;

    public LiteralOperator(Value value) {
        this.value = value;
    }

    public LiteralOperator(String value) {
        this(new Value(value));
    }

    public LiteralOperator(ConfigElement config) {
        this(config.attribute("value").stringValue());
    }

    public Value calculateValue(ExecutionContext context) {
        return value;
    }

    public String toString() {
        return "Literal '" + value.toString() + "'";
    }

    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof LiteralOperator)) return false;

        final LiteralOperator literalOperator = (LiteralOperator) o;

        if (value != null ? !value.equals(literalOperator.value) : literalOperator.value != null) return false;

        return true;
    }

    @Override
    public int nonObjectIdBasedHashCode() {
        return (value != null ? value.hashCode() : 0);
    }

    /*
     * @see com.tawala.project.commands.Operator#getValues(com.tawala.project.commands.ExecutionContext)
     */
    @Override
    public List<Value> getValues(ExecutionContext context) {
        return Collections.singletonList(value);
    }
}
