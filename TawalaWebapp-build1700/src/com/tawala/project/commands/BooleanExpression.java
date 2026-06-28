package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;

/**
 * TODO: currently only a handful of extending classes override hashCode and
 * equals. It doesn't present logical errors at the moment, but eventually they
 * need to be updated.
 */
public abstract class BooleanExpression {
	public static final Factory<BooleanExpression> FACTORY = new Factory<BooleanExpression>();
	public static final BooleanExpression NULL = new NullCondition();
	public static final BooleanExpression TRUE = new TrueCondition();

	static {
		FACTORY.register("and", And.class);
		FACTORY.register("or", Or.class);

		FACTORY.register("stringEquals", Equals.class);
		FACTORY.register("equals", Equals.class);
		FACTORY.register("doesNotEqual", NotEquals.class);
		FACTORY.register("beginsWith", BeginsWith.class);
		FACTORY.register("endsWith", EndsWith.class);
		FACTORY.register("contains", Contains.class);
		FACTORY.register("doesNotContain", DoesNotContain.class);
		FACTORY.register("isBlank", IsBlank.class);
		FACTORY.register("isNotBlank", IsNotBlank.class);
		FACTORY.register("mcIsBlank", IsBlank.class);
		FACTORY.register("mcIsNotBlank", IsNotBlank.class);

		FACTORY.register("isLessThan", IsLessThan.class);
		FACTORY.register("isGreaterThan", IsGreaterThan.class);
		FACTORY.register("isLessThanOrEqualTo", IsLessThanOrEqualTo.class);
		FACTORY
				.register("isGreaterThanOrEqualTo",
						IsGreaterThanOrEqualTo.class);

		FACTORY.register("mcContains", MultipleChoiceContains.class);
		FACTORY
				.register("mcDoesNotContain",
						MultipleChoiceDoesNotContain.class);

		FACTORY.register("mcEquals", MultipleChoiceEquals.class);
		FACTORY.register("mcDoesNotEqual", MultipleChoiceDoesNotEqual.class);
	}

	public abstract boolean isTrue(ExecutionContext context);
	public abstract int nonObjectIdBasedHashCode();
	
	@Override
	public final int hashCode() {
		return nonObjectIdBasedHashCode();
	}

	public static BooleanExpression load(ConfigElement config) {
		if (config == null)
			return null;
		return FACTORY.make(config);
	}

	protected static ReferenceOperator parseLeft(ConfigElement config) {
		return new ReferenceOperator(config.attribute("field").stringValue());
	}

	protected static Operator parseRight(ConfigElement config) {
		return Operator.FACTORY.makeChildren(config).get(0);
	}

	private static class NullCondition extends BooleanExpression {

		public boolean isTrue(ExecutionContext context) {
			return false;
		}

		@Override
		public int nonObjectIdBasedHashCode() {
			return 12343;
		}

		@Override
		public boolean equals(Object obj) {
			return obj.getClass() == NullCondition.class;
		}
	}

	private static class TrueCondition extends BooleanExpression {

		public boolean isTrue(ExecutionContext context) {
			return true;
		}

		@Override
		public int nonObjectIdBasedHashCode() {
			return 1235;
		}

		@Override
		public boolean equals(Object obj) {
			return obj.getClass() == TrueCondition.class;
		}
	}
}
