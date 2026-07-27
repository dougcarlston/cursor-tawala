package com.tawala.project;

import com.tawala.project.commands.SmartNumber;

public class Value {
	private final String value;

	public static final Value NULL = new NullValue();

	public Value(String value) {
		this.value = value;
	}

	public Value(int value) {
		this(String.valueOf(value));
	}

	public Value(double value) {
		this(convertDoubleToString(value));
	}

	private static String convertDoubleToString(double value) {
		long longValue = (long)value;
		return longValue == value ? String.valueOf(longValue) : String.valueOf(value) ;
	}
	
	public String toString() {
		return value;
	}

	public SmartNumber asNumber() {
		return new SmartNumber(value);
	}

	public boolean isNumeric() {
		return asNumber().isNumeric();
	}

	public boolean containsNumber() {
		return asNumber().containsNumber();
	}

	public boolean isEmpty() {
		return value == null || value.trim().equals("");
	}

	private static class NullValue extends Value {
		public NullValue() {
			super(null);
		}

		public String toString() {
			return "";
		}

		@Override
		public boolean equals(Object o) {
			// The assumption is that null value doesn't equal anything,
			// including another null value. It has the same semantics as SQL
			// null value.
			return false;
		}
	}

	public boolean equals(Object o) {
		if (this == o)
			return true;
		if (!(o instanceof Value))
			return false;

		final Value value1 = (Value) o;

		if (value == null && value1.value == null)
			return true;
		if (value == null || value1.value == null)
			return false;
		if (isNumeric() && value1.isNumeric())
			return asNumber().doubleValue() == value1.asNumber().doubleValue();

		return value.trim().equalsIgnoreCase(value1.value.trim());
	}

	public int hashCode() {
		return (value != null ? value.hashCode() : 0);
	}
}
