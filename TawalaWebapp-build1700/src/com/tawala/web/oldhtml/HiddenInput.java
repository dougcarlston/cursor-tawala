package com.tawala.web.oldhtml;

import java.io.PrintWriter;

import org.springframework.web.util.HtmlUtils;

public class HiddenInput implements Html {
	private final String name;
	private final String value;

	public HiddenInput(String name, String value) {
		this.name = name;
		this.value = value;
	}

	public void render(PrintWriter out, RenderingContext renderingContext) {
		out.print("<input type=\"hidden\" name=\"");
		out.print(HtmlUtils.htmlEscape(name));
		out.print("\" value=\"");
		out.print(HtmlUtils.htmlEscape(value));
		out.print("\"/>\n");
	}

}
