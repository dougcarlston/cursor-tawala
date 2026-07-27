package com.scissor.xmlconfig;

import java.util.List;

import junit.framework.TestCase;

import com.scissor.Log;
import com.scissor.LogMonitor;

public class FactoryTest extends TestCase {

    public void testBasicConstruction() {
        ConfigElement config = parseConfig("<root>" +
                "    <one val=\"1\"/>" +
                "    <two val=\"2\"/>" +
                "</root>");
        Factory<Thing> factory = new Factory<Thing>();
        factory.register("one", Thing1.class);
        factory.register("two", Thing2.class);
        Thing one = factory.make(config.child("one"));
        Thing two = factory.make(config.child("two"));

        assertTrue(one instanceof Thing1);
        assertTrue(two instanceof Thing2);

        assertEquals(1, one.value);
        assertEquals(2, two.value);

        List children = factory.makeChildren(config);
        assertEquals(2, children.size());
        assertEquals(one, children.get(0));
        assertEquals(two, children.get(1));
    }

    public void testNullsAndEmpties() {
        Factory<Thing1> factory = new Factory<Thing1>();
        factory.register("one", Thing1.class);
        try {
            factory.make((ConfigElement) null); // TODO: should this return null instead of throwing?
            fail();
        } catch (NullPointerException e) {
            // ok
        }
        assertTrue(factory.makeChildren((ConfigElement)null).isEmpty());

        ConfigElement empty = ConfigItemsTest.configElement("<root/>");
        assertTrue(factory.makeChildren(empty).isEmpty());
    }

    public void testText() {
        Factory<Thing1> factory = new Factory<Thing1>();
        factory.register("one", Thing1.class);
        factory.registerText(ThingText.class);
        List items = factory.makeChildren(parseConfig("<root>text<one val=\"1\"/>more text</root>"));
        assertEquals(3, items.size());
        assertEquals("text", ((ThingText) items.get(0)).text);
        assertEquals(1, ((Thing1) items.get(1)).value);
        assertEquals("more text", ((ThingText) items.get(2)).text);
    }

    public void testWhitespaceFilter() {
        Factory<Thing1> factory = new Factory<Thing1>();
        factory.register("one", Thing1.class);
        factory.registerText(ThingText.class);
        factory.setKeepWhitespace(true);
        List items = factory.makeChildren(parseConfig("<root><one val='1'/> <one val='2'/></root>"));
        assertEquals(3, items.size());
        assertEquals(1, ((Thing1) items.get(0)).value);
        assertEquals(" ", ((ThingText) items.get(1)).text);
        assertEquals(2, ((Thing1) items.get(2)).value);
    }


    public void testUnknown() {
        Factory factory = new Factory();
        factory.ignore("two");

        LogMonitor logs = new LogMonitor();
        Log.captureLogging(logs);
        try {
            List items = factory.makeChildren(parseConfig("<root><one/><two/></root>"));
            assertEquals(0, items.size());
            assertNull(factory.make(parseConfig("<root/>")));
            assertEquals(2, logs.size());
            logs.checkMessage(0, Log.WARN, "No class registered for /root/one");
            logs.checkMessage(1, Log.WARN, "No class registered for /root");
        } finally {
            Log.normalLogging();
        }
    }


    public void testNonConformingClassesRejected() {
        Factory<Object> factory = new Factory<Object>();
        try {
            factory.register("foo", Object.class);
            fail();
        } catch (IllegalArgumentException e) {
            // good
        }
        try {
            factory.registerText(Object.class);
            fail();
        } catch (IllegalArgumentException e) {
            // good
        }
    }

    public void testAttributeMatching() {
        ConfigElement config = parseConfig("<root>" +
                "    <thing typeOne='true' val='1'/>" +
                "    <thing typeTwo='true' val='2'/>" +
                "</root>");
        Factory<Thing> factory = new Factory<Thing>();
        factory.register("thing", "typeOne", Thing1.class);
        factory.register("thing", "typeTwo", Thing2.class);
        Thing one = factory.make(config.childElement(0));
        Thing two = factory.make(config.childElement(1));

        assertTrue(one instanceof Thing1);
        assertTrue(two instanceof Thing2);
    }

    // TODO: test unknowns: should it fail immediately or just add a warning to the ConfigElement

    public void testMutualIgnorance() {
        Factory<Thing1> one = new Factory<Thing1>();
        one.register("one", Thing1.class);
        Factory<Thing2> two = new Factory<Thing2>();
        two.register("two", Thing2.class);

        one.ignore(two);
        two.ignore(one);

        LogMonitor logs = new LogMonitor();
        Log.captureLogging(logs);
        try {
            ConfigElement config = parseConfig("<root><one val='1'/><two val='2'/></root>");
            assertEquals(1, one.makeChildren(config).size());
            assertEquals(1, two.makeChildren(config).size());
            logs.checkNothingElse();
        } finally {
            Log.normalLogging();
        }

    }


    private ConfigElement parseConfig(String xml) {
        return ConfigItemsTest.configElement(xml);
    }

    private abstract static class Thing {
        protected final int value;

        public Thing(ConfigElement config) {
            this.value = config.attribute("val").intValue();
        }

    }

    private static class Thing1 extends Thing {
        public Thing1(ConfigElement config) {
            super(config);
        }

        public boolean equals(Object o) {
            if (this == o) return true;
            if (!(o instanceof Thing1)) return false;

            final Thing1 thing1 = (Thing1) o;

            if (value != thing1.value) return false;

            return true;
        }

        public int hashCode() {
            return value;
        }

    }

    private static class Thing2 extends Thing {
        public Thing2(ConfigElement config) {
            super(config);
        }

        public boolean equals(Object o) {
            if (this == o) return true;
            if (!(o instanceof Thing2)) return false;

            final Thing2 thing2 = (Thing2) o;

            if (value != thing2.value) return false;

            return true;
        }

        public int hashCode() {
            return value;
        }

    }

    private static class ThingText {
        private String text;

        public ThingText(ConfigText config) {
            this.text = config.text();
        }
    }
}
