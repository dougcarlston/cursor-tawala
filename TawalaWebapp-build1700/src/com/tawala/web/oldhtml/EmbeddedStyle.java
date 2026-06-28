package com.tawala.web.oldhtml;

import java.io.PrintWriter;

public class EmbeddedStyle implements Html {
    private String body;
    
    public EmbeddedStyle(String body) {
        this.body = body;
    }

    public void render(PrintWriter out, RenderingContext renderingContext) {
        out.write("<style type=\"text/css\"><!--\n");
        out.write(body);
        out.write("\n--></style>\n");
    }
}
