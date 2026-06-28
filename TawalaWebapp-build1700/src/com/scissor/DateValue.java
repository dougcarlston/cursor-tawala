package com.scissor;

import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.TimeZone;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * A UTC date.
 */
public final class DateValue implements Comparable {

    private static final String monthNames[] = {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
    };

    public static boolean isValid(int year, int month, int day) {
        try {
            new DateValue(year, month, day);
            return true;
        } catch (InvalidDateException exception) {
            return false;
        }
    }

    private final int year;
    private final int month;
    private final int day;

    /**
     * @param year  four digits
     * @param month 1-12
     * @param day   1-31, or less by month
     */
    public DateValue(int year, int month, int day) {
        this(newCalendar(year, month, day));
        if (year != this.year || month != this.month || day != this.day) {
            throw new InvalidDateException("Invalid date: " + format(year, month, day) +
                    " should be " + format(this.year, this.month, this.day));
        }
    }

    public DateValue(GregorianCalendar calendar) {
        this.year = calendar.get(Calendar.YEAR);
        this.month = calendar.get(Calendar.MONTH) + 1;
        this.day = calendar.get(Calendar.DATE);
        if (year < 1000 || year > 9999) {
            throw new InvalidDateException("Invalid year in date: " +
                    format(year, month, day));
        }
    }

    public DateValue(Date date) {
        this(newCalendar(date));
    }

    public DateValue(long millis) {
        this(new Date(millis));
    }


    public int getYear() {
        return year;
    }

    public int getMonth() {
        return month;
    }

    public int getDay() {
        return day;
    }

    public boolean equals(Object o) {
        if (this == o) return true;
        return (o instanceof DateValue) && equals((DateValue) o);
    }

    public boolean equals(DateValue dateValue) {
        if (day != dateValue.day) return false;
        if (month != dateValue.month) return false;
        if (year != dateValue.year) return false;
        return true;
    }

    public int hashCode() {
        int result;
        result = year;
        result = 29 * result + month;
        result = 29 * result + day;
        return result;
    }

    public String toString() {
        return format(year, month, day);
    }

    private static String format(int year, int month, int day) {
        return year + "-" + twoDigits(month) + "-" + twoDigits(day);
    }

    public static String twoDigits(int n) {
        if (n < 10) {
            return "0" + n;
        } else {
            return "" + n;
        }
    }

    // http://www.w3.org/TR/NOTE-datetime
    private static final Pattern PATTERN = Pattern.compile("(\\d{4})-(\\d{2})-(\\d{2})");

    public static DateValue valueOf(String string) {
        Matcher matcher = PATTERN.matcher(string);
        if (matcher.matches()) {
            int year = Integer.parseInt(matcher.group(1));
            int month = Integer.parseInt(matcher.group(2));
            int day = Integer.parseInt(matcher.group(3));
            return new DateValue(year, month, day);
        } else {
            throw new InvalidDateException("Date string '" + string + "' is not valid");
        }
    }

    public int compareTo(Object o) {
        DateValue other = (DateValue) o;
        if (this.year < other.year) {
            return -1;
        } else if (this.year > other.year) {
            return 1;
        } else if (this.month < other.month) {
            return -1;
        } else if (this.month > other.month) {
            return 1;
        } else if (this.day < other.day) {
            return -1;
        } else if (this.day > other.day) {
            return 1;
        } else {
            return 0;
        }
    }

    public boolean isBefore(DateValue dateValue) {
        return compareTo(dateValue) < 0;
    }

    public DateValue addYears(int years) {
        return new DateValue(this.year + years, month, day);
    }

    public DateValue addDays(int days) {
        return new DateValue(newCalendar(year, month, day + days));
    }

    public Date toDate() {
        return toCalendar().getTime();
    }

    public String getMonthName() {
        return monthNames[getMonth() - 1];
    }

    public long getMilliseconds() {
        return toDate().getTime();
    }

    public DateValue previousDay() {
        GregorianCalendar calendar = toCalendar();
        calendar.add(Calendar.DAY_OF_MONTH, -1);
        return new DateValue(calendar);
    }

    public GregorianCalendar toCalendar() {
        return newCalendar(year, month, day);
    }

    private static GregorianCalendar newCalendar(int year, int month, int day) {
        GregorianCalendar calendar = new GregorianCalendar(standardTimeZone());
        calendar.clear();
        calendar.set(year, month - 1, day);
        return calendar;
    }

    private static GregorianCalendar newCalendar(Date date) {
        GregorianCalendar calendar = new GregorianCalendar(standardTimeZone());
        calendar.clear();
        calendar.setTime(date);
        return calendar;
    }

    static TimeZone standardTimeZone() {
        return TimeZone.getTimeZone("UTC");
    }

}
