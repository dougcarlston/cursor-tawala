package mock.javax.servlet.http;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.Locale;
import java.util.Map;

import javax.servlet.ServletOutputStream;
import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletResponse;

import com.scissor.HttpDateFormat;
import com.scissor.UnimplementedException;

public class FakeHttpServletResponse implements HttpServletResponse {

    private ByteArrayOutputStream outputBuffer = new ByteArrayOutputStream();
    private PrintWriter printWriter = new PrintWriter(outputBuffer);

    private Map<String, Cookie> mapCookieNameToValue = new HashMap<String, Cookie>();

    private Map<String, String> headers = new HashMap<String, String>();
    private String redirectTo;
    private int statusCode = 200;
    private String errorMessage;
    private boolean cancelled = false;

    public String getOutput() {
        printWriter.flush();
        return outputBuffer.toString();
    }

    public byte[] getOutputBytes() {
        printWriter.flush();
        return outputBuffer.toByteArray();
    }

    public void addCookie(Cookie cookie) {
        mapCookieNameToValue.put(cookie.getName(), cookie);
    }

    public Cookie getCookie(String name) {
        return (Cookie) mapCookieNameToValue.get(name);
    }

    public Collection getCookies() {
        return mapCookieNameToValue.values();
    }

    public boolean containsHeader(String string) {
        throw new UnimplementedException();
        //return false;
    }

    public String encodeURL(String string) {
        throw new UnimplementedException();
        //return null;
    }

    public String encodeRedirectURL(String string) {
        throw new UnimplementedException();
        //return null;
    }

    /**
     * @deprecated
     */
    public String encodeUrl(String string) {
        throw new UnimplementedException();
        //return null;
    }

    /**
     * @deprecated
     */
    public String encodeRedirectUrl(String string) {
        throw new UnimplementedException();
        //return null;
    }

    public void sendError(int statusCode, String message) throws IOException {
        setStatus(statusCode);
        errorMessage = message;
    }

    public void sendError(int statusCode) throws IOException {
        sendError(statusCode, null);
    }

    public void setStatus(int statusCode) {
        this.statusCode = statusCode;
    }

    /**
     * @deprecated
     */
    public void setStatus(int statusCode, String message) {
        throw new UnsupportedOperationException();
    }

    public int getStatusCode() {
        return statusCode;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public String getRedirectTo() {
        return redirectTo;
    }

    public void sendRedirect(String redirectTo) throws IOException {
        setStatus(SC_SEE_OTHER);
        this.redirectTo = redirectTo;
    }

    public void setDateHeader(String name, long date) {
        setHeader(name, new HttpDateFormat(false).format(new Date(date)));

    }

    public void addDateHeader(String string, long l) {
        throw new UnimplementedException();
    }

    public void setHeader(String name, String value) {
        if (outputBuffer.size() > 0) {
            throw new IllegalStateException("Cannot set headers after writing output");
        }
        headers.put(name, value);
    }

    public String getHeader(String name) {
        return (String) headers.get(name);
    }

    public void addHeader(String string, String string1) {
        throw new UnimplementedException();
        //
    }

    public void setIntHeader(String string, int i) {
        throw new UnimplementedException();
        //
    }

    public void addIntHeader(String string, int i) {
        throw new UnimplementedException();
        //
    }

    public String getCharacterEncoding() {
        throw new UnimplementedException();
        //return null;
    }

    public ServletOutputStream getOutputStream() throws IOException {
        if (cancelled) throw new IOException("operation cancelled");
        return new FakeServletOutputStream(outputBuffer);
    }

    public PrintWriter getWriter() throws IOException {
        if (cancelled) throw new IOException("operation cancelled");
        return printWriter;
    }

    public void setCharacterEncoding(String string) {
        throw new UnimplementedException();
        //
    }

    public void setContentLength(int contentLength) {
        setHeader("Content-Length", "" + contentLength);
    }

    public int getContentLength() {
        return Integer.parseInt(getHeader("Content-Length"));
    }


    public String getContentType() {
        return getHeader("Content-Type");
    }


    public void setContentType(String contentType) {
        setHeader("Content-Type", contentType);
    }

    public void setBufferSize(int i) {
        throw new UnimplementedException();
        //
    }

    public int getBufferSize() {
        throw new UnimplementedException();
        //return 0;
    }

    public void flushBuffer() throws IOException {
        throw new UnimplementedException();
        //
    }

    public void resetBuffer() {
        throw new UnimplementedException();
        //
    }

    public boolean isCommitted() {
        throw new UnimplementedException();
        //return false;
    }

    public void reset() {
        throw new UnimplementedException();
        //
    }

    public void setLocale(Locale locale) {
        throw new UnimplementedException();
        //
    }

    public Locale getLocale() {
        throw new UnimplementedException();
        //return null;
    }

    public void setCancelled() {
        cancelled = true;
    }

}
