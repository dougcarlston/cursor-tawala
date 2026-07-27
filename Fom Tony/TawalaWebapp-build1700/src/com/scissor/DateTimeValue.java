package com.scissor;

import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * A UTC date and time.
 */
public class DateTimeValue implements Comparable {

    private final DateValue date;
    private final int hour;
    private final int minute;
    private final int second;

    public DateTimeValue(Date date) {
        this.date = new DateValue(date);
        int secondsSinceMidnight = (int) (date.getTime() - this.date.getMilliseconds()) / 1000;
        this.hour = secondsSinceMidnight / 3600;
        int secondsSinceHour = secondsSinceMidnight - hour * 3600;
        this.minute = secondsSinceHour / 60;
        int secondsSinceMinute = secondsSinceHour - minute * 60;
        this.second = secondsSinceMinute;
    }

    public DateTimeValue(int year, int month, int day, int hour, int minute, int second) {
        this(new DateValue(year, month, day), hour, minute, second);
    }

    public DateTimeValue(DateValue date, int hour, int minute, int second) {
        this.date = date;
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    public DateTimeValue(long milliseconds) {
        this(new Date(milliseconds));
    }

    public DateValue getDate() {
        return date;
    }

    public int getHour() {
        return hour;
    }

    public int getMinute() {
        return minute;
    }

    public int getSecond() {
        return second;
    }

    public int compareTo(Object o) {
        DateTimeValue other = (DateTimeValue) o;
        int dateComparison = this.date.compareTo(other.date);
        if (dateComparison != 0) {
            return dateComparison;
        } else if (this.hour < other.hour) {
            return -1;
        } else if (this.hour > other.hour) {
            return 1;
        } else if (this.minute < other.minute) {
            return -1;
        } else if (this.minute > other.minute) {
            return 1;
        } else if (this.second < other.second) {
            return -1;
        } else if (this.second > other.second) {
            return 1;
        } else {
            return 0;
        }
    }

    public boolean isBefore(DateTimeValue other) {
        return compareTo(other) < 0;
    }

    public final boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof DateTimeValue)) return false;

        final DateTimeValue dateTimeValue = (DateTimeValue) o;

        if (hour != dateTimeValue.hour) return false;
        if (minute != dateTimeValue.minute) return false;
        if (second != dateTimeValue.second) return false;
        if (!date.equals(dateTimeValue.date)) return false;

        return true;
    }

    public final int hashCode() {
        int result;
        result = date.hashCode();
        result = 29 * result + hour;
        result = 29 * result + minute;
        result = 29 * result + second;
        return result;
    }

    public long getMilliseconds() {
        return date.getMilliseconds() + (hour * 3600 + minute * 60 + second) * 1000;
    }

    public String toString() {
        return date.toString() + "T" + DateValue.twoDigits(hour)
                + ":" + DateValue.twoDigits(minute) + ":" + DateValue.twoDigits(second) + "Z";
    }

    // http://www.w3.org/TR/NOTE-datetime
    private static final Pattern PATTERN = Pattern.compile("(\\d{4}-\\d{2}-\\d{2})T(\\d{2}):(\\d{2}):(\\d{2})Z");

    public static DateTimeValue valueOf(String string) {
        Matcher matcher = PATTERN.matcher(string);
        if (matcher.matches()) {
            DateValue date = DateValue.valueOf(matcher.group(1));
            int hour = Integer.parseInt(matcher.group(2));
            int minute = Integer.parseInt(matcher.group(3));
            int second = Integer.parseInt(matcher.group(4));
            return new DateTimeValue(date, hour, minute, second);
        } else {
            throw new InvalidDateException("Date string '" + string + "' is not valid");
        }
    }

    public DateTimeValue addYears(int years) {
        return new DateTimeValue(date.addYears(years), hour, minute, second);
    }

    public DateTimeValue addDays(int days) {
        return new DateTimeValue(date.addDays(days), hour, minute, second);
    }

    public DateTimeValue addSeconds(int seconds) {
        Calendar calendar = toCalendar();
        calendar.add(Calendar.SECOND, seconds);
        return new DateTimeValue(calendar.getTime());
    }

    public GregorianCalendar toCalendar() {
        GregorianCalendar calendar = date.toCalendar();
        calendar.set(Calendar.HOUR_OF_DAY, hour);
        calendar.set(Calendar.MINUTE, minute);
        calendar.set(Calendar.SECOND, second);
        return calendar;
    }

    public Date toDate() {
        return toCalendar().getTime();
    }

}
