package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public abstract class SingleValueCondition extends BooleanExpression {
    private final Operator left;
    private final Operator right;

    protected SingleValueCondition(Operator left, Operator right) {
        this.left = left;
        this.right = right;
    }

    public SingleValueCondition(ConfigElement config) {
        this(parseLeft(config), parseRight(config));
    }

    public boolean isTrue(ExecutionContext context) {
        Value lValue = left.calculateValue(context);
        Value rValue = right.calculateValue(context);
        return isTrue(lValue, rValue);
    }


    protected abstract boolean isTrue(Value lValue, Value rValue);
	protected abstract int getUniqueConditionCode();

    public Operator left() {
        return left;
    }

    public Operator right() {
        return right;
    }

    @Override
    public int nonObjectIdBasedHashCode() {
    	return left.hashCode() + 7 * right.hashCode() + getUniqueConditionCode();
    }
}
