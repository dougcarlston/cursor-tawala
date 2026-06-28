package com.tawala.web;

import java.io.IOException;
import java.io.Writer;

public interface Download {
    void render(Writer out) throws IOException;
}
