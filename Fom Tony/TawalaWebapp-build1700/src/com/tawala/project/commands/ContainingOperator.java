package com.tawala.project.commands;

import java.util.Iterator;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public abstract class ContainingOperator extends Operator {
    private final Operators operators;

    public ContainingOperator(ConfigElement config) {
        operators = Operators.parseChildren(config);
    }

    public Value calculateValue(ExecutionContext context) {
        Iterator<Operator> i = getOperators().iterator();
        if (!i.hasNext()) return Value.NULL;

        SmartNumber total = i.next().calculateValue(context).asNumber();
        while (i.hasNext()) {
            total = process(total, i.next().calculateValue(context).asNumber());
        }
        return new Value(total.toString());
    }

    protected abstract SmartNumber process(SmartNumber a, SmartNumber b);
	protected abstract int getUniqueOperatorCode();

    protected Operators getOperators() {
        return operators;
    }
    
    @Override
    public final int nonObjectIdBasedHashCode() {
    	int result = 0;
    	for (Operator operator : getOperators()) {
			result += operator.hashCode();
		}
    	return result + getUniqueOperatorCode();
    }
}
