package com.tawala.project.data;

import junit.framework.TestCase;

public class StringFieldTest extends TestCase {
	public void testValidFieldNames() {
		String[] validNames = { "abc", "a12312", "field with spaces",
				"_underscored",
				"Other crazy stuff >>>> --- ~!@#$%^&*()_+|}}{\";'?><,./'" };
		for (String fieldName : validNames) {
			doTest(true, fieldName);
		}
	}

	public void testInvalidFieldNames() {
		String[] invalidNames = { " abc", "a12:312", "123", "123abc", "__myvar" };
		for (String fieldName : invalidNames) {
			doTest(false, fieldName);
		}
	}

	private void doTest(boolean expected, String fieldName) {
		assertEquals("Testing \"" + fieldName + "\"", expected, StringField
				.isValidFieldName(fieldName));
	}
}
