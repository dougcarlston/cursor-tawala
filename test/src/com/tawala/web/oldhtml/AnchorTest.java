package com.tawala.web.oldhtml;


public class AnchorTest extends HtmlTestCase {
	public void testBasics() {
		Anchor anchor = new Anchor("abc");
		assertEquals("<a name=\"abc\" />", renderHtml(anchor));
	}

	public void testHtmlEscaping() {
		Anchor anchor = new Anchor("<&>\"");
		assertEquals("<a name=\"&lt;&amp;&gt;&quot;\" />", renderHtml(anchor));
	}
}
