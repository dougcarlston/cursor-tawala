package com.tawala.web;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;

import org.apache.log4j.Level;
import org.mortbay.jetty.Connector;
import org.mortbay.jetty.Server;
import org.mortbay.jetty.bio.SocketConnector;
import org.mortbay.jetty.webapp.WebAppContext;
import org.xml.sax.SAXException;

import com.meterware.httpunit.PostMethodWebRequest;
import com.meterware.httpunit.WebRequest;
import com.meterware.httpunit.WebResponse;
import com.meterware.servletunit.ServletRunner;
import com.meterware.servletunit.ServletUnitClient;
import com.scissor.Log;
import com.scissor.LogMonitor;
import com.tawala.TestCase;

public abstract class ServletTestCase extends TestCase {
    protected static ServletRunner runner;

    private static final String SEARCH_CONTEXT_PATH = "/search";
	
    static {
    	//System.setProperty(LuceneInitializer.USE_IN_PROCESS_INDEXERS, "true");
    	
        try {
        	setUpSearchWebApp();
        	
        	String newXml = prepareWebXmlForUnitTests();
            
            String testWebXmlFile = "web/WEB-INF/junit-web.xml";
            FileWriter writer = new FileWriter(testWebXmlFile);
            writer.write(newXml);
            writer.close();
            
            runner = new ServletRunner(new File(testWebXmlFile), "");
        } catch (Throwable e) {
            throw new RuntimeException(e);
        }
    }

	private static void setUpSearchWebApp() throws Exception {
		Server server = new Server();
		
		SocketConnector socketConnector = new SocketConnector();
		server.setConnectors(new Connector[] {socketConnector});

		WebAppContext searchWebAppContext = new WebAppContext("search-web-app", SEARCH_CONTEXT_PATH);
		server.addHandler(searchWebAppContext);
		
		server.start();
		
		int port = socketConnector.getLocalPort();
		prepareLuceneProperties(port);
		
		System.setProperty(LuceneInitializer.USE_IN_PROCESS_INDEXERS, Boolean.FALSE.toString());
	}

	private static void prepareLuceneProperties(int port) throws FileNotFoundException {
		System.setProperty("lucene.url.base.users", "http://127.0.0.1:" + port + SEARCH_CONTEXT_PATH);
		System.setProperty("lucene.url.base.projects", "http://127.0.0.1:" + port  + SEARCH_CONTEXT_PATH);
	}

    /**
     * @return
     * @throws FileNotFoundException
     * @throws IOException
     */
    public static String prepareWebXmlForUnitTests() throws FileNotFoundException, IOException {
        FileReader reader = new FileReader("web/WEB-INF/web.xml");
        char [] largeBuffer = new char[64000];
        int length = reader.read(largeBuffer);
        reader.close();
        
        String xml = new String(largeBuffer,0, length);
        String newXml = xml.replaceFirst("<!-- Default servlet definition for unit tests goes here -->", 
                "<servlet>\n" +
                "<servlet-name>default</servlet-name>\n" + 
                "<servlet-class>" + TawalaDefaultServlet.class.getName() + "</servlet-class>\n"  +
                "<load-on-startup>1</load-on-startup>\n" +
                "</servlet>\n"  +
                
                " <servlet>\n" +
                "<servlet-name>jsp</servlet-name>\n" +
                "<servlet-class>org.apache.jasper.servlet.JspServlet</servlet-class>\n" +
                "<init-param>\n" +
                "<param-name>fork</param-name>\n" +
                "<param-value>false</param-value>\n" +
                "</init-param>\n" +
                "<init-param>\n" +
                "<param-name>xpoweredBy</param-name>\n" +
                "<param-value>false</param-value>\n" +
                "</init-param>\n" +
                "<init-param>\n" +
                "<param-name>compilerTargetVM</param-name>\n" +
                "<param-value>1.5</param-value>\n" +
                "</init-param>\n" +
                "<init-param>\n" +
                "<param-name>compilerSourceVM</param-name>\n" +
                "<param-value>1.5</param-value>\n" +
                "</init-param>\n" +
                "<load-on-startup>3</load-on-startup>\n" +
                "</servlet>");
        
        newXml = newXml.replaceFirst("<!-- Mime mappings for unit tests goes here -->",     
                "<mime-mapping>\n" +
                "<extension>jpg</extension>\n" +
                "<mime-type>image/jpeg</mime-type>\n" +
                "</mime-mapping>\n" +
                "<mime-mapping>\n" +
                "<extension>css</extension>\n" +
                "<mime-type>text/css</mime-type>\n" +
                "</mime-mapping>\n" +
                "<mime-mapping>\n" +
                "<extension>js</extension>\n" +
                "<mime-type>text/javascript</mime-type>\n" +
                "</mime-mapping>\n");
        return newXml;
    }

    protected ServletUnitClient client;
    protected WebResponse response;
    protected LogMonitor logs;
    
    protected void setUp() throws Exception {
        super.setUp();
        logs = new LogMonitor();
        logs.ignoreMessage(Level.INFO, "Storing data in memory only");
        logs.ignoreMessage(Level.WARN,
                "Unable to find log4j configuration file '"
                        + "log4j.properties" + "'.");
        Log.captureLogging(logs);

        /* Until we need it.
        DB4OConfigurator.configure();
        ObjectContainer objectContainer = Db4o.openFile(System
                .getProperty(DB4OInitializer.DB_FILE));
        ObjectContainerProvider.setObjectContainer(objectContainer);
        */

        client = runner.newClient();
    }

    protected void tearDown() throws Exception {
        super.tearDown();
        // ObjectContainerProvider.closeContainer();

        logs.dumpUnseenErrors(this);
        Log.normalLogging();
    }

    protected void checkMatches(String expected, WebResponse actual)
            throws IOException {
        assertMatches(expected, actual.getText());
    }

    protected WebResponse getResponse(WebRequest request, ServletUnitClient client) throws IOException,
            SAXException {
        response = client.getResponse(request);
        return response;
    }

    protected WebResponse post(String url) throws IOException, SAXException {
        return getResponse(new PostMethodWebRequest(url), client);
    }

    protected WebResponse get(String url) throws IOException, SAXException {
        response = client.getResponse(url);
        return response;
    }

    protected void dumpCookies(SiteRobot bot) {
        String[] cookieNames = bot.getCookieNames();
        for (String cookieName : cookieNames) {
            System.out.println(cookieName + "="
                    + bot.getCookieValue(cookieName));
        }
    }

}
