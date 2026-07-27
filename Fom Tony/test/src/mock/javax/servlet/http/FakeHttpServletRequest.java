package mock.javax.servlet.http;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.PrintStream;
import java.io.UnsupportedEncodingException;
import java.security.Principal;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Locale;
import java.util.Map;

import javax.servlet.RequestDispatcher;
import javax.servlet.ServletInputStream;
import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import com.scissor.UnimplementedException;

public class FakeHttpServletRequest implements HttpServletRequest {

    private String pathInfo = null;
    private String servletPath = "/";
    private Map<String, String[]> parameters = new HashMap<String, String[]>();
    private List<Cookie> cookies = new ArrayList<Cookie>();
    private String scheme = "http";
    private String serverName;
    private int serverPort;
    private String contextPath;
    private String queryString;
    private FakeHttpSession session;
    private Map<String, String> headers = new HashMap<String, String>();
    private InputStream inputStream;
    private String method = "GET";
    private String remoteAddr = "127.0.0.1";
    private StringBuffer requestUrl = null;

    public String getAuthType() {
        throw new UnimplementedException();
        //return null;
    }

    public Cookie[] getCookies() {
        if (cookies.size() == 0) return null; // emulating dumb servlet behavior
        return cookies.toArray(new Cookie[cookies.size()]);
    }

    public void setCookie(String name, String value) {
        cookies.add(new Cookie(name, value));
    }

    public long getDateHeader(String string) {
        throw new UnimplementedException();
        //return 0;
    }

    public String getHeader(String name) {
        return (String) headers.get(name.toLowerCase());
    }

    public void setHeader(String name, String value) {
        headers.put(name.toLowerCase(), value);
    }

    public Enumeration getHeaders(String string) {
        throw new UnimplementedException();
        //return null;
    }

    public Enumeration getHeaderNames() {
        throw new UnimplementedException();
        //return null;
    }

    public int getIntHeader(String string) {
        throw new UnimplementedException();
        //return 0;
    }

    public String getMethod() {
        return method;
    }

    public void setMethod(String method) {
        this.method = method;
    }

    public String getPathInfo() {
        return pathInfo;
    }

    public void setPathInfo(String pathInfo) {
        this.pathInfo = pathInfo;
    }

    public String getPathTranslated() {
        throw new UnimplementedException();
        //return null;
    }

    public String getContextPath() {
        return contextPath;
    }

    public void setContextPath(String contextPath) {
        this.contextPath = contextPath;
    }

    public String getQueryString() {
        return queryString;
    }

    public void setQueryString(String queryString) {
        this.queryString = queryString;
    }

    public String getRemoteUser() {
        throw new UnimplementedException();
        //return null;
    }

    public boolean isUserInRole(String string) {
        throw new UnimplementedException();
        //return false;
    }

    public Principal getUserPrincipal() {
        throw new UnimplementedException();
        //return null;
    }

    public String getRequestedSessionId() {
        throw new UnimplementedException();
        //return null;
    }

    public String getRequestURI() {
        throw new UnimplementedException();
        //return null;
    }

    public StringBuffer getRequestURL() {
        return requestUrl;
    }

    public void setRequestUrl(String requestUrl) {
        this.requestUrl = new StringBuffer(requestUrl);
    }

    public String getServletPath() {
        return servletPath;
    }

    public void setServletPath(String servletPath) {
        this.servletPath = servletPath;
    }

    public HttpSession getSession(boolean b) {
        if (b && session == null) {
            session = new FakeHttpSession();
        }
        return session;
    }

    public HttpSession getSession() {
        return getSession(true);
    }

    public boolean isRequestedSessionIdValid() {
        throw new UnimplementedException();
        //return false;
    }

    public boolean isRequestedSessionIdFromCookie() {
        throw new UnimplementedException();
        //return false;
    }

    public boolean isRequestedSessionIdFromURL() {
        throw new UnimplementedException();
        //return false;
    }

    /**
     * @deprecated
     */
    public boolean isRequestedSessionIdFromUrl() {
        throw new UnimplementedException();
        //return false;
    }

    public Object getAttribute(String string) {
        throw new UnimplementedException();
        //return null;
    }

    public Enumeration getAttributeNames() {
        throw new UnimplementedException();
        //return null;
    }

    public String getCharacterEncoding() {
        throw new UnimplementedException();
        //return null;
    }

    public void setCharacterEncoding(String string) throws UnsupportedEncodingException {
        throw new UnimplementedException();
        //
    }

    public int getContentLength() {
        if (getHeader("Content-Length") == null) {
            return -1;
        } else {
            return Integer.parseInt(getHeader("Content-Length"));
        }
    }

    public void setContentLength(int contentLength) {
        setHeader("Content-Length", "" + contentLength);
    }

    public String getContentType() {
        return getHeader("Content-Type");
    }

    public void setContentType(String contentType) {
        setHeader("Content-Type", contentType);
    }

    public ServletInputStream getInputStream() throws IOException {
        return new ServletInputStream() {
            public int read() throws IOException {
                return inputStream.read();
            }
        };
    }

    public void setInputStream(InputStream inputStream) {
        this.inputStream = inputStream;
    }


    public String getParameter(String name) {
        String[] values = (String[]) parameters.get(name);
        if (values == null) return null;
        return values[0];
    }

    public Enumeration getParameterNames() {
        return Collections.enumeration(parameters.keySet());
    }

    public String[] getParameterValues(String name) {
        return (String[]) parameters.get(name);
    }

    public void setParameter(String name, String value) {
        parameters.put(name, new String[]{value});
    }

    public void setParameterValues(String name, String[] values) {
        parameters.put(name, values);
    }

    public void setParameters(Map params) {
        for (Iterator it = params.entrySet().iterator(); it.hasNext();) {
            Map.Entry entry = (Map.Entry) it.next();
            setParameter((String) entry.getKey(), (String) entry.getValue());
        }
    }

    public Map getParameterMap() {
        return new HashMap<String, String[]>(parameters);
    }

    public String getProtocol() {
        throw new UnimplementedException();
    }


    public String getScheme() {
        return scheme;
    }

    public void setScheme(String scheme) {
        this.scheme = scheme;
    }


    public String getServerName() {
        return serverName;
    }

    public void setServerName(String host) {
        this.serverName = host;
    }


    public int getServerPort() {
        return serverPort;
    }

    public void setServerPort(int port) {
        this.serverPort = port;
    }


    public BufferedReader getReader() throws IOException {
        throw new UnimplementedException();
        //return null;
    }

    public String getRemoteAddr() {
        return remoteAddr;
    }

    public void setRemoteAddr(String remoteAddr) {
        this.remoteAddr = remoteAddr;
    }

    public String getRemoteHost() {
        throw new UnimplementedException();
        //return null;
    }

    public void setAttribute(String string, Object object) {
        throw new UnimplementedException();
        //
    }

    public void removeAttribute(String string) {
        throw new UnimplementedException();
        //
    }

    public Locale getLocale() {
        throw new UnimplementedException();
        //return null;
    }

    public Enumeration getLocales() {
        throw new UnimplementedException();
        //return null;
    }

    public boolean isSecure() {
        return "https".equals(scheme);
    }

    public RequestDispatcher getRequestDispatcher(String string) {
        throw new UnimplementedException();
        //return null;
    }

    /**
     * @deprecated
     */
    public String getRealPath(String string) {
        throw new UnimplementedException();
        //return null;
    }

    public int getRemotePort() {
        throw new UnimplementedException();
        //return 0;
    }

    public String getLocalName() {
        throw new UnimplementedException();
        //return null;
    }

    public String getLocalAddr() {
        throw new UnimplementedException();
        //return null;
    }

    public int getLocalPort() {
        throw new UnimplementedException();
        //return 0;
    }

    public void dump(PrintStream out) {
        String queryString = this.queryString == null ? "" : "?" + this.queryString;
        String pathInfo = this.pathInfo == null ? "" : this.pathInfo;
        out.println(method + " " + servletPath + pathInfo + queryString);
        out.println();
        List<String> keys = new ArrayList<String>(parameters.keySet());
        Collections.sort(keys);
        for (Iterator it = keys.iterator(); it.hasNext();) {
            String key = (String) it.next();
            String[] values = (String[]) parameters.get(key);
            if (values.length > 0) {
                out.println("  " + key + "[0] = " + values[0]);
            }
            if (values.length > 1) out.println(" (additional values omitted)");
        }
    }

    public void copyCookiesFrom(FakeHttpServletResponse response) {
        Collection cookies = response.getCookies();
        for (Iterator i = cookies.iterator(); i.hasNext();) {
            Cookie cookie = (Cookie) i.next();
            if (cookie.getMaxAge() != 0) {
                setCookie(cookie.getName(), cookie.getValue());
            }
        }
    }

    public void setReferrer(String location) {
        setHeader("Referer", location);
    }

}
