package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public class ReferenceOperator extends Operator {
    private final String name;

    public ReferenceOperator(String name) {
        this.name = name;
    }

    public ReferenceOperator(ConfigElement config) {
        this(config.attribute(config.hasAttribute("field") ? "field" : "name").stringValue());
    }

    public String getName() {
        return name;
    }

    public Value calculateValue(ExecutionContext context) {
        return context.getValue(name);
    }

    public List<Value> getValues(ExecutionContext context) {
        return context.getValues(name);
    }

    public String toString() {
        return "Reference to '" + name + "'";
    }

    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof ReferenceOperator)) return false;

        final ReferenceOperator referenceOperator = (ReferenceOperator) o;

        if (name != null ? !name.equals(referenceOperator.name) : referenceOperator.name != null) return false;

        return true;
    }

    @Override
    public int nonObjectIdBasedHashCode() {
        return (name != null ? name.hashCode() : 35);
    }
}
