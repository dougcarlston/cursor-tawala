package com.tawala.web.oldhtml;

import java.io.PrintWriter;

import org.springframework.web.util.HtmlUtils;

import com.tawala.project.Value;

public class TextInput extends AttributeSupport implements Html {
	public static final String ID_PREFIX = "tawalaField_";
	private final String id;
	private final int length;
	private final int height;
	private final Value value;

	public TextInput(String id, int length, int height, Value value) {
		this.id = id;
		this.length = length;
		this.height = height;
		this.value = value;
	}

	public void render(PrintWriter out, RenderingContext renderingContext) {
		String escapedId = HtmlUtils.htmlEscape(id);
		String elementId = elementId(escapedId);
		if (height == 1) {
			out.print("<input");
			out.print(" type=\"text\"");
			out.print(" class=\"text\"");
			out.print(" name=\"" + escapedId + "\"");
			out.print(" id=\"" + elementId + "\"");
			out.print(" size=\"" + length + "\"");
			if (value != Value.NULL)
				out.print(" value=\"" + HtmlUtils.htmlEscape(value.toString())
						+ "\"");
			
			renderAttributes(out);
			
			out.print(" />");
		} else {
			out.print("<textarea");
			out.print(" class=\"textArea\"");
			out.print(" name=\"" + escapedId + "\"");
			out.print(" id=\"" + elementId + "\"");
			out.print(" cols=\"" + length + "\"");
			out.print(" rows=\"" + height + "\"");
			
			renderAttributes(out);
			
			out.print(">");
			if (value != Value.NULL) {
				out.print(HtmlUtils.htmlEscape(value.toString()));
			}
			out.print("</textarea>");
		}
	}

	public static String elementId(String escapedId) {
		return ID_PREFIX + escapedId;
	}
}
