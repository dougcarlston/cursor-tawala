package mock.javax.servlet.http;

import java.util.Enumeration;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.ServletConfig;
import javax.servlet.ServletContext;

import com.scissor.UnimplementedException;

public class FakeServletConfig implements ServletConfig {

    private ServletContext fakeServletContext = new FakeServletContext();
    private Map<String, String> initParameters = new HashMap<String, String>();

    public FakeServletConfig() {
        this(true);
    }

    public FakeServletConfig(boolean setInUnitTest) {
        if (setInUnitTest) {
            setInitParameter("_IN_UNIT_TEST", "true");
        }
    }

    public String getServletName() {
        return "SomeServletOrAnother";
    }

    public ServletContext getServletContext() {
        return fakeServletContext;
    }

    public void setInitParameter(String name, String value) {
        initParameters.put(name, value);
    }

    public String getInitParameter(String name) {
        return (String) initParameters.get(name);
    }

    public Enumeration getInitParameterNames() {
        throw new UnimplementedException();
    }

}
