package com.tawala.web.oldhtml;

import java.io.PrintWriter;

import org.springframework.web.util.HtmlUtils;

public class Anchor implements Html {
	private final String name;
	
	public Anchor(String name) {
		this.name = HtmlUtils.htmlEscape(name);
	}

	public void render(PrintWriter out, RenderingContext renderingContext) {
        out.print("<a name=\"" + name  + "\" />");
	}
}
