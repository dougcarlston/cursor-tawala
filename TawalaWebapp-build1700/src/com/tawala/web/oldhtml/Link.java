package com.tawala.web.oldhtml;

import java.io.PrintWriter;

import org.springframework.web.util.HtmlUtils;

import com.tawala.project.FormSegment;

public class Link implements Html {
	public static final String ON_CLICK_HANDLER = " onClick=\""
							+ FormSegment.CONFIRM_NAVIGATION_AWAY_FROM_PAGE
							+ "=false;" + "\"";
	private String URL;
	private boolean openInNewWindow;
	private String text;

	public Link(String URL, String text, boolean openInNewWindow) {
		this.URL = HtmlUtils.htmlEscape(URL);
		this.openInNewWindow = openInNewWindow;

		this.text = text;
	}

	public void render(PrintWriter out, RenderingContext context) {
		out.print("<a href=\"");
		out.print(context.renderUrl(URL));
		out.print("\"");
		if (openInNewWindow) {
			out.print(" target=\"_blank\"");
		} else {
			if (!context.isEmailDestination()) {
				out.print(ON_CLICK_HANDLER);
			}
		}
		out.print(">");
		out.print(HtmlUtils.htmlEscape(text));
		out.print("</a>");
	}
}
