package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.Factory;
import com.tawala.project.Value;

public abstract class Operator {
    public static final Factory<Operator> FACTORY = new Factory<Operator>();
    public static final Operator NULL = new NullOperator();

    static {
        Operator.FACTORY.register("string", "value", LiteralOperator.class);
        Operator.FACTORY.register("string", "field", ReferenceOperator.class);
        Operator.FACTORY.register("operand", "value", LiteralOperator.class);
        Operator.FACTORY.register("operand", "field", ReferenceOperator.class);
        Operator.FACTORY.register("field", ReferenceOperator.class);
        Operator.FACTORY.register("add", AddOperator.class);
        Operator.FACTORY.register("sub", SubtractOperator.class);
        Operator.FACTORY.register("mul", MultiplyOperator.class);
        Operator.FACTORY.register("div", DivideOperator.class);
    }

    public abstract Value calculateValue(ExecutionContext context);
    public abstract int nonObjectIdBasedHashCode();
    
    @Override
    public final int hashCode() {
    	return nonObjectIdBasedHashCode();
    }

    public List<Value> getValues(ExecutionContext context) {
        return new ArrayList<Value>();
    }

    private static class NullOperator extends Operator {
        
    	public Value calculateValue(ExecutionContext context) {
            return Value.NULL;
        }

		@Override
		public int nonObjectIdBasedHashCode() {
			return -90321;
		}
    }
}
