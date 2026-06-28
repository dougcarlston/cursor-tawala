package com.tawala.web.oldhtml;

import java.io.PrintWriter;

public interface Html {
    public void render(PrintWriter out, RenderingContext renderingContext);
}
