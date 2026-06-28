package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Value;

public abstract class MultipleChoiceCondition extends BooleanExpression {
	private final String fieldId;
	private Operator rightOperand;

	public MultipleChoiceCondition(ConfigElement config) {
		this.fieldId = config.attribute("field").stringValue();

		String valueString = config.attribute("value").stringValue();
		if (valueString == null) {
			rightOperand = Operator.FACTORY.make(config.child("string"));
		} else {
			rightOperand = new LiteralOperator(valueString);
		}
	}

	@Override
	public final boolean isTrue(ExecutionContext context) {
		List<Value> leftResponses = context.getValues(fieldId);
		List<Value> rightResponses = rightOperand.getValues(context);

		return performOperation(leftResponses, rightResponses);
	}

	protected abstract boolean performOperation(List<Value> leftResponses,
			List<Value> rightResponses);

	protected abstract int getUniqueConditionId();

	public Operator operator() {
		return rightOperand;
	}

	public String fieldId() {
		return fieldId;
	}
	
	@Override
	public int nonObjectIdBasedHashCode() {
		int result = fieldId.hashCode();
		result += rightOperand.nonObjectIdBasedHashCode();
		
		return result + getUniqueConditionId();
	}
}
