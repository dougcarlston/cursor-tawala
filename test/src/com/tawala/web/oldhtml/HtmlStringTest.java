package com.tawala.web.oldhtml;


public class HtmlStringTest extends HtmlTestCase {
    public void testEscaping() {
        assertEquals("Hi!", renderHtml(new HtmlString("Hi!")));
        assertEquals("3 < 4 & 4 > 3", renderHtml(new HtmlString("3 < 4 & 4 > 3")));
        assertEquals("She said, \"Hi!\"", renderHtml(new HtmlString("She said, \"Hi!\"")));
        assertEquals("line 1<br />\nline 2", renderHtml(new HtmlString("line 1\nline 2")));
    }

    public void testPreservingBlankSpaces() {
    	assertEquals("Text &nbsp;&nbsp;more text", renderHtml(new HtmlString("Text   more text")));
    }
    
    public void testNull() {
        assertEquals("", renderHtml(new HtmlString(null)));
    }

}
