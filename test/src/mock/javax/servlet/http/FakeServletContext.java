package mock.javax.servlet.http;

import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Set;
import java.util.Vector;

import javax.servlet.RequestDispatcher;
import javax.servlet.Servlet;
import javax.servlet.ServletContext;
import javax.servlet.ServletException;

import junit.framework.Assert;

import com.scissor.UnimplementedException;

@SuppressWarnings({"unchecked"})
public class FakeServletContext implements ServletContext {

	private Map<String, byte[]> resources = new HashMap<String, byte[]>();
    private final Map<String, String> initParameters = new LinkedHashMap<String, String>();


    public String getContextPath() {
        throw new UnimplementedException();	}

	public ServletContext getContext(String name) {
        throw new UnimplementedException();
    }

    public int getMajorVersion() {
        throw new UnimplementedException();
    }

    public int getMinorVersion() {
        throw new UnimplementedException();
    }

    public String getMimeType(String name) {
        throw new UnimplementedException();
    }

    public Set getResourcePaths(String prefix) {
        Set<String> paths = resources.keySet();
        Set<String> results = new HashSet<String>();
        for (String fullPath : paths) {
            if (!fullPath.startsWith(prefix)) {
                continue;
            }
            String rest = fullPath.substring(prefix.length());
            String directoryPath = rest.replaceFirst("/.*$", "/");
            results.add(prefix + directoryPath);
        }
        return results.isEmpty() ? null : results;
    }

    public URL getResource(String name) throws MalformedURLException {
        return new URL("file://" + name);
    }

    public InputStream getResourceAsStream(String name) {
        if (resources.containsKey(name)) {
            return new ByteArrayInputStream(resources.get(name));
        } else {
            return null;
        }
    }

    public void setResource(String name, byte[] contents) {
        Assert.assertTrue(name.startsWith("/"));
        resources.put(name, contents);
    }

    public RequestDispatcher getRequestDispatcher(String name) {
        throw new UnimplementedException();
    }

    public RequestDispatcher getNamedDispatcher(String name) {
        throw new UnimplementedException();
    }

    public Servlet getServlet(String name) throws ServletException {
        throw new UnimplementedException();
    }

    public Enumeration getServlets() {
        throw new UnimplementedException();
    }

    public Enumeration getServletNames() {
        throw new UnimplementedException();
    }

    public void log(String message) {

    }

    public void log(Exception e, String name) {
        throw new UnimplementedException();
    }

    public void log(String name, Throwable throwable) {
        throw new UnimplementedException();
    }

    public String getRealPath(String name) {
        if (".".equals(name) || "/".equals(name)) {
            return "/usr/someServletContainer/webapps/root";
        } else {
            throw new UnsupportedOperationException();
        }
    }

    public String getServerInfo() {
        throw new UnimplementedException();
    }

    public String getInitParameter(String name) {
        return initParameters.get(name);
    }

    public void setInitParameter(String name, String value) {
        initParameters.put(name, value);
    }

    public Enumeration getInitParameterNames() {
        return new Vector(initParameters.keySet()).elements();
    }

    public Object getAttribute(String name) {
        throw new UnimplementedException();
    }

    public Enumeration getAttributeNames() {
        throw new UnimplementedException();
    }

    public void setAttribute(String name, Object o) {
        throw new UnimplementedException();
    }

    public void removeAttribute(String name) {
        throw new UnimplementedException();
    }

    public String getServletContextName() {
        throw new UnimplementedException();
    }
}
