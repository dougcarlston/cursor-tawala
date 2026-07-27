package com.scissor.xmlconfig;

import java.io.File;
import java.io.IOException;
import java.util.Iterator;
import java.util.List;

import com.tawala.web.oldhtml.HtmlTestCase;

public class ConfigItemsTest extends HtmlTestCase {

    public void testConfigFormat() {
        ConfigElement config = configElement("<root format=\"1.0\" ><same /><different format=\"1.1\" /></root>");
        ConfigElement same = config.child("same");
        assertEquals("1.0", config.format().toString());
        assertEquals("1.0", same.format().toString());
        assertSame(config.format(), same.format());

        assertEquals("1.1", config.child("different").format().toString());
    }

    public void testElementAttributes() {
        ConfigElement config = configElement("<root one=\"1\" two=\"2\" />");
        assertEquals("1", config.attribute("one").stringValue());
        assertEquals("2", config.attribute("two").stringValue());
    }

    public void testBooleanAttributes() {
        ConfigElement config = configElement("<root one=\"true\" two=\"false\" />");
        assertTrue(config.attribute("one").booleanValue());
        assertFalse(config.attribute("two").booleanValue());
    }

    public void testIntegerAttributes() {
        ConfigElement config = configElement("<root negative=\"-73\" zero=\"0\" positive=\"100\" />");
        assertEquals(-73, config.attribute("negative").intValue());
        assertEquals(0, config.attribute("zero").intValue());
        assertEquals(100, config.attribute("positive").intValue());
    }

    // TODO: test element defaulting

    public void testElementChildren() {
        ConfigElement config = configElement("<root><one /><two /><two /></root>");
        assertTrue(config.hasChild("one"));
        assertTrue(config.hasChild("two"));
        assertFalse(config.hasChild("three"));

        assertEquals("one", config.child("one").getName());
        assertEquals("two", config.child("two").getName());
        assertEquals(1, config.children("one").size());
        assertEquals(2, config.children("two").size());
    }

    // todo: element text fetching
    public void testElementText() {
        ConfigElement config = configElement("<root>foo <b>bar</b> baz</root>");
        assertEquals("foo bar baz", config.text());
    }

    public void testUsedAttributes() {
        checkUnused(configElement("<root/>"), "/root");

        ConfigElement attribs = configElement("<root one=\"1\" two=\"2\" />");
        checkUnused(attribs, "/root");
        attribs.attribute("one").stringValue();
        checkUnused(attribs, "/root/@two");
        attribs.attribute("two");
        checkUnused(attribs, "/root/@two");
        attribs.attribute("two").markUsed();
        checkUnused(attribs);
    }

    public void testUsedElements() {
        ConfigElement justRoot = configElement("<root/>");
        checkUnused(justRoot, "/root");
        justRoot.children();
        checkUnused(justRoot);

        ConfigElement withChildren = configElement("<root><one /><two /></root>");
        checkUnused(withChildren, "/root");
        withChildren.childElement(0).children();
        checkUnused(withChildren, "/root/two");
        withChildren.childElement(1).children();
        checkUnused(withChildren);
    }

    public void testUnusedText() {
        ConfigElement simpleText = configElement("<root>hi!</root>");
        checkUnused(simpleText, "/root");
        ConfigText text = (ConfigText) simpleText.child(0);
        checkUnused(simpleText, "/root/text(hi!)");
        assertEquals("hi!", text.text());
        checkUnused(simpleText);

        ConfigElement compound = configElement("<root><b>hi!</b></root>");
        compound.text();
        checkUnused(compound);

        ConfigElement multiLine = configElement("<root>\n" + "  hi!\n" + "</root>\n");
        text = (ConfigText) multiLine.child(0);
        checkUnused(multiLine, "/root/text(hi!)");
        assertEquals("\n  hi!\n", text.text());
        checkUnused(multiLine);

        ConfigElement indented = configElement("<root>\n" + "  <foo />\n" + "</root>\n");
        indented.child("foo").children();
        checkUnused(indented);

        ConfigElement retainSpaces = configElement("<root>this <and/> that</root>\n");
        retainSpaces.child("and").children();
        checkUnused(retainSpaces, "/root/text(this )", "/root/text( that)");
    }

    public void testInterestingText() {
        ConfigElement assortedText = configElement("<html>" +
                "    <br />foo<br/>" +
                "</html>");
        ConfigText boring = (ConfigText) assortedText.child(0);
        assertEquals("    ", boring.text());
        assertFalse(boring.isInteresting());
        ConfigText interesting = (ConfigText) assortedText.child(2);
        assertTrue(interesting.isInteresting());
    }

    public void testReadFromFile() throws IOException {
        File content = new File(createTestDirectory(), "thingy.xml");
        writeFile(content, "<root fromFile='true'/>");
        ConfigElement element = new ConfigElement(content);
        assertTrue(element.attribute("fromFile").booleanValue());
    }

    private void checkUnused(ConfigElement config, String... unusedPaths) {
        List<ConfigItem> unusedItems = config.getUnusedItems();
        assertEquals("unused count mismatch", unusedPaths.length, unusedItems.size());
        Iterator<ConfigItem> itemIterator = unusedItems.iterator();
        for (int index = 0; index < unusedPaths.length; index++) {
            String path = unusedPaths[index];
            assertEquals("at " + index, path, itemIterator.next().path());
        }
    }

    public static ConfigElement configElement(String xml) {
        return new ConfigElement(xml);
    }

}
