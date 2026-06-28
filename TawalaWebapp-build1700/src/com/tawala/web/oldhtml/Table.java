package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.List;

public class Table extends AttributeSupport implements Html {
	public static class Column extends AttributeSupport implements Html {
		private final Html contents;

		public Column(Html contents, String... attributes) {
			super(attributes);
			this.contents = contents;
		}
		
		public void render(PrintWriter out, RenderingContext context) {
			out.print("<td");
			renderAttributes(out);
			out.print(">");

			contents.render(out, context);

			out.print("</td>\n");
		}
	}

	private final List<List<Column>> rows = new ArrayList<List<Column>>();

	public Table() {
	}

	public Table(String... attributes) {
		super(attributes);
	}

	public void addRow(List<Column> columns) {
		rows.add(columns);
	}

	public void render(PrintWriter out, RenderingContext context) {
		out.print("<table");
		renderAttributes(out);
		out.print(">\n");

		printRows(out, context);
		out.print("</table>");
	}

	private void printRows(PrintWriter out, RenderingContext context) {
		for (List<Column> row : rows) {
			printRow(out, context, "td", row);
		}
	}

	private void printRow(PrintWriter out, RenderingContext context, String cellType, List<Column> row) {
		out.print("<tr>");
		for (Column column : row) {
			column.render(out, context);
		}
		out.print("</tr>\n");
	}
}
