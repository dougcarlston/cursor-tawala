package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.util.ArrayList;

import org.springframework.web.util.HtmlUtils;

import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.commands.ExecutionContext;

public class HtmlString extends FormRenderableNotHoldingActiveComponents
		implements Html {
	private final String contents;

	public HtmlString(String contents) {
		this.contents = contents;
	}

	public void render(PrintWriter out, RenderingContext renderingContext) {
		printHTMLPreparedText(contents, out);
	}

	public static void printHTMLPreparedText(String contents, PrintWriter out) {
		if (contents == null)
			return;

//		String escapedContents = HtmlUtils.htmlEscape(contents);
		String escapedContents = contents;
		escapedContents = HTMLEntityEncode(escapedContents);

		char[] chars = escapedContents.toCharArray();
		char previous = '.';
		
		for (int i = 0; i < chars.length; i++) {
			switch (chars[i]) {
			case '\n':
				String prev = previousChars(4, i, chars); 
				if( prev.contains("</p>") ) {
					out.print("\n");					
				}else{
					out.print("<br />\n");
				}
				break;
			case ' ':
				out.print(previous == ' ' ? "&nbsp;" : " ");
				break;

			default:
				out.print(chars[i]);
			}
			previous = chars[i];
		}
	}

	// Return the previous n characters
	static String previousChars(int count, int index, char[] chars) {
		String result = "";

		ArrayList<Character> skipChars = new ArrayList<Character>(); 
		skipChars.add(' ');
		skipChars.add('\r');

		while(result.length() < count && index != 0) {
			index = index - 1;
			if(!skipChars.contains(chars[index])) {
				result = chars[index] + result;
			}
		}
		return result;
	}

	// TODO: Find a way to un-duplicate the logic
	public void render(StringBuffer stringBuffer) {
		if (contents == null)
			return;

		String escapedContents = HtmlUtils.htmlEscape(contents);
		char[] chars = escapedContents.toCharArray();
		for (char c : chars) {
			switch (c) {
			case '\n':
				stringBuffer.append("<br />\n");
				break;
			default:
				stringBuffer.append(c);
			}
		}
	}

	public static String HTMLEntityEncode(String s) {
		StringBuffer buf = new StringBuffer();
		int len = (s == null ? -1 : s.length());

		for (int i = 0; i < len; i++) {
			char c = s.charAt(i);
			int intValue = (int) c;
			if (intValue >= 0 && intValue <= 127) {
				buf.append(c);
			} else {
				buf.append("&#" + intValue + ";");
			}
		}
		return buf.toString();
	}

	@Override
	public String toString() {
		StringBuffer result = new StringBuffer();
		render(result);
		return result.toString();
	}

	public Html toHtml(ExecutionContext executionContext) {
		return this;
	}

	public boolean isEmpty(ExecutionContext context) {
		return contents.trim().length() == 0;
	}
}
