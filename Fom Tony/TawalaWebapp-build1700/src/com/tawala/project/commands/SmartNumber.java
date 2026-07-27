package com.tawala.project.commands;

import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class SmartNumber {

    private static final NumberFormat[] OUTPUT_FORMATS = outputFormats();
    private static final Pattern[] INPUT_FORMATS = new Pattern[]{
            Pattern.compile("-?\\d+\\.\\d+"), // full decimal
            Pattern.compile("-?\\.\\d+"), // decimal with implicit zero
            Pattern.compile("-?\\d+"), // integer
    };
    private static final Pattern DIGIT = Pattern.compile("\\d");
    private static final int MAX_DIGITS = 8;
    private static final double PRACTICALLY_ZERO = 1 / Math.pow(10, MAX_DIGITS + 1);

    private static enum Type {
        NUMERIC, CONTAINS_NUMBER, NONNUMERIC
    }

    private final double value;
    private final int precision;
    private final Type type;


    public SmartNumber(double value, int precision) {
        this.value = value;
        this.precision = precision;
        type = Type.NUMERIC;
    }

    public SmartNumber(long value) {
        this(value, 0);
    }

    public SmartNumber(String asString) {
        String extractedNumber = extractNumberString(asString);

        if (extractedNumber == null) {
            value = 0;
            precision = 0;
            type = Type.NONNUMERIC;
        } else {
            if (containsDecimal(extractedNumber)) {
                value = parseDecimal(extractedNumber);
                precision = precisionFor(extractedNumber);
            } else {
                value = parseWholeNumber(extractedNumber);
                precision = 0;
            }
            if (extractedNumber.equals(asString.trim())) {
                type = Type.NUMERIC;
            } else if (containsDigits(extractedNumber)) {
                type = Type.CONTAINS_NUMBER;
            } else {
                type = Type.NONNUMERIC;
            }
        }
    }

    private String extractNumberString(String asString) {
        if (asString == null) return null;

        for (Pattern pattern : INPUT_FORMATS) {
            Matcher matcher = pattern.matcher(asString);
            if (matcher.find()) {
                return matcher.group();
            }
        }
        return null;
    }

    private static int precisionFor(String string) {
        int lastChar = string.length() - 1;

        int decimalAt = string.indexOf('.');
        if (decimalAt == lastChar) return 0;
        int start = decimalAt + 1;
        int offset = 0;
        while (start + offset <= lastChar && Character.isDigit(string.charAt(start + offset))) {
            offset++;
        }
        return offset;
    }

    private boolean containsDecimal(String extractedNumber) {
        return extractedNumber.indexOf('.') > -1;
    }

    private boolean containsDigits(String extractedNumber) {
        return DIGIT.matcher(extractedNumber).find();
    }


    public SmartNumber add(SmartNumber other) {
        return new SmartNumber(value + other.value, Math.max(precision, other.precision));
    }

    public SmartNumber subtract(SmartNumber other) {
        return new SmartNumber(value - other.value, Math.max(precision, other.precision));
    }

    public SmartNumber multiply(SmartNumber other) {
        return new SmartNumber(value * other.value, Math.max(precision, other.precision));
    }

    public SmartNumber divide(SmartNumber other) {
        int greaterPrecision = Math.max(precision, other.precision);
        if (Math.abs(other.value) < PRACTICALLY_ZERO) return new SmartNumber(0, greaterPrecision);
        double result = value / other.value;
        return new SmartNumber(result, Math.min(greaterPrecision + 2, precisionFor(result)));
    }

    private int precisionFor(double number) {
        for (int result = MAX_DIGITS; result > 0; result--) {
            double scale = Math.pow(10, result - 1);
            double remainder = Math.abs(number * scale - Math.rint(number * scale));
            if (remainder >= 0.05) return result;
        }
        return 0;
    }

    private static long parseWholeNumber(String asString) {
        long result;
        try {
            result = Long.parseLong(asString);
        } catch (NumberFormatException e) {
            result = 0;
        }
        return result;
    }

    private static double parseDecimal(String asString) {
        double result;
        try {
            result = Double.parseDouble(asString);
        } catch (NumberFormatException e) {
            result = 0;
        }
        return result;
    }

    public String toString() {
        return OUTPUT_FORMATS[Math.min(MAX_DIGITS, precision)].format(value);
    }

    private static NumberFormat[] outputFormats() {
        NumberFormat[] formats = new NumberFormat[MAX_DIGITS + 1];
        formats[0] = new DecimalFormat("0");
        StringBuffer formatString = new StringBuffer("0.");
        for (int i = 1; i < MAX_DIGITS + 1; i++) {
            formatString.append("0");
            DecimalFormat format = new DecimalFormat(formatString.toString());
            //--- TODO: when 1.6 becomes available.
            //format.setRoundingMode(RoundingMode.HALF_UP);
			formats[i] = format;
        }
        return formats;
    }


    public double doubleValue() {
        return value;
    }
    
    public int intValue() {
    	return Double.valueOf(value).intValue();
    }

    public int getPrecision() {
        return precision;
    }

    public boolean isNumeric() {
        return type == Type.NUMERIC;
    }

    public boolean containsNumber() {
        return type == Type.NUMERIC || type == Type.CONTAINS_NUMBER;
    }
}
