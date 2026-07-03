package mock.javax.servlet.http;

import java.util.Enumeration;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpSession;

import com.scissor.UnimplementedException;

public class FakeHttpSession implements HttpSession {

    private static int nextId = 0;
    private Map<String, Object> attributes = new HashMap<String, Object>();
    private int id = nextId++;
    private int maxInactiveInterval;

    public long getCreationTime() {
        throw new UnimplementedException();
    }

    public String getId() {
        return "id-" + id;
    }

    public long getLastAccessedTime() {
        throw new UnimplementedException();
    }

    public ServletContext getServletContext() {
        throw new UnimplementedException();
    }

    public void setMaxInactiveInterval(int seconds) {
        this.maxInactiveInterval = seconds;
    }

    public int getMaxInactiveInterval() {
        return maxInactiveInterval;
    }

    /**
     * @deprecated
     */
    public javax.servlet.http.HttpSessionContext getSessionContext() {
        throw new UnimplementedException();
    }

    public Object getAttribute(String name) {
        return attributes.get(name);
    }

    public Object getValue(String s) {
        throw new UnimplementedException();
    }

    public Enumeration getAttributeNames() {
        throw new UnimplementedException();
    }

    public String[] getValueNames() {
        throw new UnimplementedException();
    }

    public void setAttribute(String name, Object value) {
        attributes.put(name, value);
    }

    public void putValue(String s, Object o) {
        throw new UnimplementedException();
    }

    public void removeAttribute(String s) {
        throw new UnimplementedException();
    }

    public void removeValue(String s) {
        throw new UnimplementedException();
    }

    public void invalidate() {
        throw new UnimplementedException();
    }

    public boolean isNew() {
        throw new UnimplementedException();
    }
}
