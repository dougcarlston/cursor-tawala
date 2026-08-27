package com.tawala.web.oldhtml;

import java.io.PrintWriter;

import org.springframework.web.util.HtmlUtils;

import com.tawala.project.Value;
import com.tawala.util.LineEndings;

public class TextInput extends AttributeSupport implements Html {
	public static final String ID_PREFIX = "tawalaField_";

	private final String id;
	private final int length;
	private final int height;
	private final Value value;
	private final boolean richTextOnLiveForm;

	public TextInput(String id, int length, int height, Value value) {
		this(id, length, height, value, false);
	}

	public TextInput(String id, int length, int height, Value value,
			boolean richTextOnLiveForm) {
		this.id = id;
		this.length = length;
		this.height = height;
		this.value = value;
		this.richTextOnLiveForm = richTextOnLiveForm && height > 1;
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
				out.print(" value=\"" + escapeFieldValue(value.toString())
						+ "\"");
			
			renderAttributes(out);
			
			out.print(" />");
		} else {
			out.print("<textarea");
			out.print(" class=\"textArea");
			if (richTextOnLiveForm) {
				out.print(" mceRichText");
			}
			out.print("\"");
			out.print(" name=\"" + escapedId + "\"");
			out.print(" id=\"" + elementId + "\"");
			out.print(" cols=\"" + length + "\"");
			out.print(" rows=\"" + height + "\"");
			
			renderAttributes(out);
			
			out.print(">");
			if (value != Value.NULL) {
				out.print(escapeFieldValue(value.toString()));
			}
			out.print("</textarea>");
		}
	}

	/**
	 * Decode stored {@code &#x0D;} (XStream CLOB residue) to a newline, then
	 * escape once. Do not htmlEscape first — that would turn {@code &} into
	 * {@code &amp;} and show the entity as glyphs.
	 */
	static String escapeFieldValue(String raw) {
		return HtmlUtils.htmlEscape(LineEndings.toUnixNewlines(raw));
	}

	public static String elementId(String escapedId) {
		return ID_PREFIX + escapedId;
	}
}
