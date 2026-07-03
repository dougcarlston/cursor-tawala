package com.scissor;

import java.util.Date;

import com.tawala.TestCase;

public class DateTimeValueTest extends TestCase {

    public void testFromDate() {
        DateValue dateValue = new DateValue(2174, 5, 17);
        Date date = new Date(dateValue.toDate().getTime() + 8413682);
        DateTimeValue dateTimeValue = new DateTimeValue(date);
        assertEquals(dateValue, dateTimeValue.getDate());
        assertEquals(2, dateTimeValue.getHour());
        assertEquals(20, dateTimeValue.getMinute());
        assertEquals(13, dateTimeValue.getSecond());
    }

    public void testToString() {
        assertEquals("2002-10-20T17:35:52Z", new DateTimeValue(2002, 10, 20, 17, 35, 52).toString());
        assertEquals("2002-01-02T03:04:05Z", new DateTimeValue(2002, 1, 2, 3, 4, 5).toString());
    }

    public void testValueOf() {
        assertEquals(new DateTimeValue(2345, 12, 23, 17, 35, 52), DateTimeValue.valueOf("2345-12-23T17:35:52Z"));
    }

}
