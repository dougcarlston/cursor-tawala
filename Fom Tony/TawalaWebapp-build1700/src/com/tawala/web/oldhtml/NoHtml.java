package com.tawala.web.oldhtml;

import java.io.PrintWriter;

public class NoHtml implements Html {
	
	public static NoHtml INSTANCE = new NoHtml();

	private NoHtml() {}
	
	public void render(PrintWriter out, RenderingContext renderingContext) {
		//--- Do nothing
	}
}
