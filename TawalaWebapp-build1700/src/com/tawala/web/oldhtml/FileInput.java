package com.tawala.web.oldhtml;

import java.io.PrintWriter;

import org.springframework.web.util.HtmlUtils;

public class FileInput extends AttributeSupport implements Html {
	public static final String ID_PREFIX = "tawalaField_";
	private final String id;
	private final int length;

	public FileInput(String id, int length) {
		this.id = id;
		this.length = length;
	}

	public void render(PrintWriter out, RenderingContext renderingContext) {
		String escapedId = HtmlUtils.htmlEscape(id);
		out.print("<input");
		out.print(" type=\"file\"");
		out.print(" class=\"file\"");
		out.print(" name=\"" + escapedId + "\"");
		out.print(" id=\"" + ID_PREFIX + escapedId + "\"");
		out.print(" size=\"" + length + "\"");

		renderAttributes(out);

		out.print(" />");
	}
}
