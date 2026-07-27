package com.scissor.xmlconfig;

import junit.framework.TestCase;


public class FormatTest extends TestCase {

    public void testBasics() {
        Format format = new Format("1.2");
        assertEquals("1.2", format.toString());
        assertEquals(1, format.major());
        assertEquals(2, format.minor());
    }

    public void testComparable() {
        Format oneDotOh = new Format("1.0");
        Format oneDotTwo = new Format("1.2");
        Format oneDotTwelve = new Format("1.12");
        Format twoDotOne = new Format("2.1");
        assertEquals(0, oneDotOh.compareTo(oneDotOh));

        assertAscending(oneDotOh, oneDotTwo, oneDotTwelve, twoDotOne);
    }

    public void testIsAtLeast() {
        Format oneDotOh = new Format("1.0");
        Format oneDotTwo = new Format("1.2");
        assertTrue(oneDotOh.isAtLeast(oneDotOh));
        assertTrue(oneDotTwo.isAtLeast(oneDotOh));
        assertFalse(oneDotOh.isAtLeast(oneDotTwo));
    }

    public void testIsLessThan() {
        Format oneDotOh = new Format("1.0");
        Format oneDotTwo = new Format("1.2");
        assertFalse(oneDotOh.isLessThan(oneDotOh));
        assertFalse(oneDotTwo.isLessThan(oneDotOh));
        assertTrue(oneDotOh.isLessThan(oneDotTwo));
    }

    private void assertAscending(Format... things) {
        for (int i = 0; i < things.length; i++) {
            Format focus = things[i];

            for (int j = 0; j < i; j++) {
                Format before = things[j];
                assertTrue("for " + focus + ", " + before, focus.compareTo(before) < 0);
            }

            for (int j = i + 1; j < things.length; j++) {
                Format after = things[j];
                assertTrue("for " + focus + ", " + after, focus.compareTo(after) > 0);
            }
        }
    }
}
