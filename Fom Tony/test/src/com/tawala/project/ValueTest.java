package com.tawala.project;

import com.tawala.TestCase;

public class ValueTest extends TestCase {

    public void testBasics() {
        Value a = new Value("one");
        Value b = new Value("one");
        assertEquals(a, b);
    }

    public void testDisplayOfDoubles() {
    	assertEquals("12.5", new Value(12.5).toString());
    	assertEquals("12", new Value(12.).toString());
    }
}
