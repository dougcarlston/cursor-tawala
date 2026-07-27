package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;

public class StringConcatenationExpression {
	private List<Operator> operators;
	
	public StringConcatenationExpression(ConfigElement config) {
		operators = Operator.FACTORY.makeChildren(config);
	}

	public String evaluate(ExecutionContext context) {
		switch (operators.size()) {
		case 0:
			return "";

		case 1:
			return operators.get(0).calculateValue(context).toString();

		default:
			StringBuilder buffer = new StringBuilder();
			for (Operator operator: operators) {
				buffer.append(operator.calculateValue(context).toString());
			}
			return buffer.toString();
		}
	}
}
