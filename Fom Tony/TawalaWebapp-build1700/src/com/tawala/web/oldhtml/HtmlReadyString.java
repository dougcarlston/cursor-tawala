package com.tawala.web.oldhtml;

import java.io.PrintWriter;

public class HtmlReadyString implements Html {
    private final String contents;

    public HtmlReadyString(String contents) {
        this.contents = contents;
    }

    public void render(PrintWriter out, RenderingContext renderingContext) {
    	out.print(contents);
    }
}
