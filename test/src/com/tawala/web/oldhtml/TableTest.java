package com.tawala.web.oldhtml;

import java.util.ArrayList;
import java.util.List;

public class TableTest extends HtmlTestCase {
	public void testBasics() {
		assertEquals("<table>\n</table>", renderHtml(new Table()));
	}

	public void testRows() {
		Table table = new Table("style", "border: 1px");

		List<Table.Column> row = new ArrayList<Table.Column>();
		row.add(new Table.Column(new HtmlReadyString("")));

		Table.Column column = new Table.Column(new HtmlReadyString("abc"));
		row.add(column);

		column = new Table.Column(new HtmlReadyString("blue text"), "style",
				"color: blue");
		row.add(column);

		table.addRow(row);

		row = new ArrayList<Table.Column>();
		column = new Table.Column(new HtmlParagraph(
				new Html[] { new HtmlReadyString("inside paragraph") }));
		row.add(column);

		table.addRow(row);
		assertEquals("<table style=\"border: 1px\">\n" + "<tr><td></td>\n"
				+ "<td>abc</td>\n"
				+ "<td style=\"color: blue\">blue text</td>\n" + "</tr>\n"
				+ "<tr><td><p>inside paragraph</p>\n" + "</td>\n" + "</tr>\n"
				+ "</table>", renderHtml(table));
	}
}
