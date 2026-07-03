package mock.javax.servlet.http;

import java.io.IOException;
import java.io.OutputStream;

import javax.servlet.ServletOutputStream;

public class FakeServletOutputStream extends ServletOutputStream {

    private OutputStream stream;

    public FakeServletOutputStream(OutputStream stream) {
        this.stream = stream;
    }

    public void write(int b) throws IOException {
        stream.write(b);
    }
}
