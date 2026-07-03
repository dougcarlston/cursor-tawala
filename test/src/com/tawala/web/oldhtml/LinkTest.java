package com.tawala.web.oldhtml;


public class LinkTest extends HtmlTestCase {
	public void testBasics() {
		Link link = new Link("/go/here", "My link text", false);
		assertEquals("<a href=\"/go/here\"" + Link.ON_CLICK_HANDLER +
				">My link text</a>", renderHtml(link));
	}

	public void testHtmlEscaping() {
		Link link = new Link("/go/here?name=Hello World", "Link to <text> in brackets", false);
		assertEquals("<a href=\"/go/here?name=Hello World\"" + Link.ON_CLICK_HANDLER +
				">Link to &lt;text&gt; in brackets</a>", renderHtml(link));
	}

	public void testOpeningInNewWindow() {
		Link link = new Link("/go/here", "text", true);
		assertEquals("<a href=\"/go/here\" target=\"_blank\">text</a>", renderHtml(link));
	}

}
