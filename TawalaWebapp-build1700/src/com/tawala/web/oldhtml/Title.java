package com.tawala.web.oldhtml;

import java.io.PrintWriter;

public class Title implements Html {
    private HtmlString contents;

    public Title(String contents) {
        this.contents = new HtmlString(contents);
    }

    public void render(PrintWriter out, RenderingContext renderingContext) {
        out.print("<h1>");
        contents.render(out, renderingContext);
        out.print("</h1>\n");
    }
}
