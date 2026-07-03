package com.tawala.project;

import java.io.IOException;
import java.io.PrintWriter;
import java.io.StringWriter;

import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.RenderingContext;

/*
 * This is a pretty lean test case. The point is to make sure we can parse the basics, 
 * and because all the work of the Document is delegated to TextBlock the majority of tests are done there. 
 */
public class DocumentTest extends TestCase {
	private User projectOwner = UserTest.aUser("tester");

	public void testConstructFromXmlHtml() throws IOException {
		String xml = "<document name=\"Hello\">" + "<xmlData>"
				+ "<paragraph><font>Basic Text</font></paragraph>"
				+ "</xmlData>" + "</document>\n";

		Document doc = new Document(parseConfig(xml));
		assertEquals("Hello", doc.getName());
		assertMatches("<div class=\"document\">.*Basic Text.*</div>" + NEWLINE,
				render(projectOwner, doc));
	}

	private String render(User projectOwner, Document doc, String... q1Values) throws IOException {
		FakeExecutionContext context = FakeExecutionContext
				.contextWithFibValues(projectOwner, q1Values);
		Html html = doc.toHtml(context);
		StringWriter output = new StringWriter();
		html.render(new PrintWriter(output), new RenderingContext());
		return output.toString();
	}
}
