package com.tawala.web.oldhtml;


public class OldPageTest extends HtmlTestCase {
    public void testParagraph() {
        OldPage page = new OldPage();
        page.add(new HtmlParagraph("Hi!"));
        assertContains("<p>Hi!</p>\n", renderHtml(page));
    }
}
