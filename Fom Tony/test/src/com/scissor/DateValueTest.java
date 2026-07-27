package com.scissor;

import com.tawala.TestCase;

public class DateValueTest extends TestCase {

    public void testConstructor() {
        checkValid(2004, 2, 29);
        checkValid(2004, 1, 31);
        checkValid(1900, 1, 1);
        checkValid(2050, 1, 1);

        checkInvalid("Invalid date: 2000-13-01 should be 2001-01-01", 2000, 13, 1);
        checkInvalid(2000, 13, 1);
        checkInvalid(2000, 2, 31);
        checkInvalid(2001, 2, 29);
        checkInvalid(1900, 2, 29);
        checkInvalid(2004, 1, 32);

        checkInvalid("Invalid year in date: 999-01-01", 999, 1, 1);
        checkInvalid(999, 1, 1);
        checkInvalid(10000, 1, 1);

        checkValid(1000, 1, 1);
        checkValid(9999, 12, 31);
    }

    private void checkInvalid(int year, int month, int day) {
        checkInvalid(null, year, month, day);
    }

    private void checkInvalid(String message, int year, int month, int day) {
        try {
            new DateValue(year, month, day);
            fail();
        } catch (InvalidDateException e) {
            if (message != null) {
                assertEquals(message, e.getMessage());
            }
        }
    }

    private void checkValid(int year, int month, int day) {
        new DateValue(year, month, day);
    }

    public void testGetters() {
        DateValue value = new DateValue(2003, 5, 24);
        assertEquals(2003, value.getYear());
        assertEquals(5, value.getMonth());
        assertEquals(24, value.getDay());
    }

    public void testToString() {
        assertEquals("2002-10-20", new DateValue(2002, 10, 20).toString());
        assertEquals("2002-01-02", new DateValue(2002, 1, 2).toString());
    }

    public void testValueOf() {
        assertEquals(new DateValue(2345, 12, 23), DateValue.valueOf("2345-12-23"));
    }

    public void testEquals() {
        assertEquals(new DateValue(1995, 7, 12), new DateValue(1995, 7, 12));
        assertNotEquals(new DateValue(1995, 7, 12), new DateValue(1996, 7, 12));
        assertNotEquals(new DateValue(1995, 7, 12), new DateValue(1995, 8, 12));
        assertNotEquals(new DateValue(1995, 7, 12), new DateValue(1995, 7, 13));
    }

    public void testOrder() {
        assertCompareLess(new DateValue(1999, 1, 1), new DateValue(2000, 1, 1));
        assertCompareLess(new DateValue(1999, 1, 1), new DateValue(1999, 2, 1));
        assertCompareLess(new DateValue(1999, 1, 1), new DateValue(1999, 1, 2));
        assertCompareLess(new DateValue(1999, 12, 31), new DateValue(2000, 1, 1));
        assertCompareLess(new DateValue(2000, 1, 31), new DateValue(2000, 2, 1));
        assertCompareEquals(new DateValue(2000, 1, 31), new DateValue(2000, 1, 31));
    }

    public void testIsBefore() {
        assertTrue(new DateValue(1999, 1, 1).isBefore(new DateValue(1999, 1, 2)));
        assertFalse(new DateValue(1999, 1, 1).isBefore(new DateValue(1999, 1, 1)));
    }

    public void testAddYears() {
        assertEquals(new DateValue(1950, 5, 12), new DateValue(1965, 5, 12).addYears(-15));

        try {
            new DateValue(1900, 5, 6).addYears(-901);
            fail();
        } catch (InvalidDateException expected) {
        }

        try {
            new DateValue(1900, 5, 6).addYears(+8100);
            fail();
        } catch (InvalidDateException expected) {
        }
    }

    public void testAddDays() {
        assertEquals(new DateValue(1965, 6, 3), new DateValue(1965, 5, 29).addDays(5));
    }

    public void testGetMonthName() {
        checkMonthName("January", 1);
        checkMonthName("February", 2);
        checkMonthName("March", 3);
        checkMonthName("April", 4);
        checkMonthName("May", 5);
        checkMonthName("June", 6);
        checkMonthName("July", 7);
        checkMonthName("August", 8);
        checkMonthName("September", 9);
        checkMonthName("October", 10);
        checkMonthName("November", 11);
        checkMonthName("December", 12);
    }

    private void checkMonthName(String expectedName, int monthNumber) {
        assertEquals(expectedName, new DateValue(2000, monthNumber, 28).getMonthName());
    }

    public void testTimeZone() {
        assertEquals("UTC", DateValue.standardTimeZone().getID());
    }
}
    