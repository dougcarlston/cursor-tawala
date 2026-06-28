package com.scissor;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.Reader;

public class StreamCopier {

    /**
     * Copy all the bytes from in to out. Does not close or flush either stream.
     */
    public static void copy(InputStream in, OutputStream out) throws IOException {
        int n;
        byte[] buf = new byte[8192];
        while ((n = in.read(buf)) != -1) {
            out.write(buf, 0, n);
        }
    }

    public static String readEntireFile(Reader reader) throws IOException {
        StringBuffer result = new StringBuffer();
        int nread;
        char[] cbuf = new char[8192];
        while ((nread = reader.read(cbuf)) > 0) {
            result.append(cbuf, 0, nread);
        }
        return result.toString();
    }

}
