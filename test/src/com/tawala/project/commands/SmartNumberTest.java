package com.tawala.project.commands;

import com.tawala.TestCase;

public class SmartNumberTest extends TestCase {

    public void testParsing() {
        // integer
        checkParsing(9, 0, "9", "9");
        checkParsing(900, 0, "900", "900");
        checkParsing(0, 0, "0", "0");
        checkParsing(0, 0, "0", "00");
        checkParsing(-9, 0, "-9", "-9");

        // positive decimal
        checkParsing(9, 1, "9.0", "9.0");
        checkParsing(9, 1, "9.0", "09.0");
        checkParsing(9, 0, "9", "9.");
        checkParsing(0.9, 1, "0.9", "0.9");
        checkParsing(0.9, 1, "0.9", ".9");

        // negative decimal
        checkParsing(-9, 1, "-9.0", "-9.0");
        checkParsing(-9, 1, "-9.0", "-09.0");
        checkParsing(-9, 0, "-9", "-9.");
        checkParsing(-0.9, 1, "-0.9", "-0.9");
        checkParsing(-0.9, 1, "-0.9", "-.9");

        // whitespace
        checkParsing(9, 0, "9", " 9");
        checkParsing(9, 0, "9", "9 ");

        // currency
        checkParsing(9, 0, "9", "$9");
        checkParsing(9, 2, "9.00", "$9.00");

        // extra text
        checkParsing(9, 0, "9", "9 dollars");

        // 1.2.3

        // non-number
        checkParsing(0, 0, "0", "foo");
        checkParsing(0, 0, "0", "foo.bar");
    }

    private void checkParsing(double expectedValue, int expectedPrecision, String expectedString, String inputString) {
        SmartNumber number = new SmartNumber(inputString);
        assertEquals(expectedValue, number.doubleValue(), 0.0001);
        assertEquals(expectedPrecision, number.getPrecision());
        assertEquals(expectedString, number.toString());
    }

    public void testAddition() {
        checkAddition("2", "1", "1");
        checkAddition("2", "1", "1.");
        checkAddition("1", "2", "-1");
        checkAddition("2.0", "1.0", "1.0");
        checkAddition("2.00", "1.00", "1");
        checkAddition("2.12345678", "1", "1.12345678");
        checkAddition("2.12345679", "1", "1.123456789");
        checkAddition("2", "2", "foo");
    }

    public void testSubtraction() {
        checkSubtraction("1", "1", "0");
        checkSubtraction("-5", "3", "8");
        checkSubtraction("1", "2", "1");
        checkSubtraction("3", "1", "-2");
        checkSubtraction("0.5", "1", ".5");
        checkSubtraction("0.0", "0.5", "0.5");
    }

    public void testMultiplication() {
        checkMultiplication("0", "1", "0");
        checkMultiplication("1", "1", "1");
        checkMultiplication("2", "1", "2");
        checkMultiplication("2", "2", "1");
        checkMultiplication("4", "2", "2");
        checkMultiplication("1.0", "2", "0.5");
        checkMultiplication("-2", "2", "-1");
    }

    public void testDivision() {
        // normal
        checkDivision("1", "1", "1");
        checkDivision("2", "2", "1");
        checkDivision("1", "2", "2");
        checkDivision("4", "8", "2");

        // negative
        checkDivision("-1", "1", "-1");
        checkDivision("-1", "-1", "1");
        checkDivision("1", "-1", "-1");

        // fraction
        checkDivision("0.5", "1", "2");
        checkDivision("0.33", "1", "3");
        checkDivision("0.25", "1", "4");
        checkDivision("0.2", "1", "5");
        checkDivision("0.17", "1", "6");

        // decimal from decimal: two extra places, max 8
        checkDivision("0.33", "1", "3");
        checkDivision("0.333", "1.0", "3");
        checkDivision("0.333", "1.0", "3.0");
        checkDivision("0.3333", "1.00", "3");
        checkDivision("0.33333333", "1.00000000000000", "3");
        checkDivision("3.33", "10", "3");
        checkDivision("0.033", "0.1", "3");

        // divide by zero
        checkDivision("0", "0", "0");
        checkDivision("0", "1", "0");
    }

    private static enum Numeric {
        FULL, PART, NOT
    }

    ;

    public void testNumericParsing() {
        checkNumeric(Numeric.FULL, "0", 0);
        checkNumeric(Numeric.FULL, "1", 1);
        checkNumeric(Numeric.FULL, "1.0", 1);
        checkNumeric(Numeric.FULL, "1.00000", 1);

        checkNumeric(Numeric.FULL, "-1", -1);
        checkNumeric(Numeric.FULL, "-1.0", -1);

        checkNumeric(Numeric.FULL, "1 ", 1);
        checkNumeric(Numeric.FULL, " 1", 1);
        checkNumeric(Numeric.FULL, " 1 ", 1);

        checkNumeric(Numeric.PART, "$1", 1);
        checkNumeric(Numeric.PART, "1x", 1);
        checkNumeric(Numeric.PART, "1.0.", 1);
        checkNumeric(Numeric.PART, "x1.0x", 1);

        checkNumeric(Numeric.NOT, "blah", 0);
        checkNumeric(Numeric.NOT, "foo.bar", 0);
        checkNumeric(Numeric.NOT, "", 0);

    }

    private void checkNumeric(Numeric type, String string, double expectedValue) {
        SmartNumber value = new SmartNumber(string);

        switch (type) {
            case FULL:
                assertTrue(value.isNumeric());
                assertTrue(value.containsNumber());
                break;
            case PART:
                assertFalse(value.isNumeric());
                assertTrue(value.containsNumber());
                break;
            case NOT:
                assertFalse(value.isNumeric());
                assertFalse(value.containsNumber());
                break;
        }

        assertEquals(expectedValue, value.doubleValue(), 0.000000001);
    }


    private void checkAddition(String expected, String inputA, String inputB) {
        SmartNumber result = new SmartNumber(inputA).add(new SmartNumber(inputB));
        assertEquals(expected, result.toString());
    }

    private void checkSubtraction(String expected, String inputA, String inputB) {
        SmartNumber result = new SmartNumber(inputA).subtract(new SmartNumber(inputB));
        assertEquals(expected, result.toString());
    }

    private void checkMultiplication(String expected, String inputA, String inputB) {
        SmartNumber result = new SmartNumber(inputA).multiply(new SmartNumber(inputB));
        assertEquals(expected, result.toString());
    }

    private void checkDivision(String expected, String inputA, String inputB) {
        SmartNumber result = new SmartNumber(inputA).divide(new SmartNumber(inputB));
        assertEquals(expected, result.toString());
    }
}
