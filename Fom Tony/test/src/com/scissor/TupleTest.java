package com.scissor;

import java.util.HashMap;
import java.util.Map;

import junit.framework.TestCase;

public class TupleTest extends TestCase {

    public void testBasics() {
        Tuple id = new Tuple(1, 'a', "a");
        Tuple equalId = new Tuple(1, 'a', "a");
        assertEquals(id, equalId);
        assertEquals(equalId, id);
        assertEquals(id.hashCode(), equalId.hashCode());
    }

    public void testUseInMap() {

        Tuple id = new Tuple(1, 2, 3);
        Tuple equalId = new Tuple(1, 2, 3);
        Integer value = 123;

        Map<Tuple, Integer> stuff = new HashMap<Tuple, Integer>();
        stuff.put(id, value);
        assertSame(value, stuff.get(id));
        assertSame(value, stuff.get(equalId));
        assertNull(stuff.get(new Tuple(1)));
        assertNull(stuff.get(new Tuple(1, 2)));
        assertNull(stuff.get(new Tuple(1, 2, 3, 4)));
        assertNull(stuff.get(new Tuple(1, 2, 4)));
    }

    public void testToString() {
        assertEquals("Tuple(1,a,a)", new Tuple(1, 'a', "a").toString());
    }
}
