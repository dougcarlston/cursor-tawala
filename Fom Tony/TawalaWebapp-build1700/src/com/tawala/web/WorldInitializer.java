package com.tawala.web;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Enumeration;
import java.util.Properties;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.servlet.ServletConfig;
import javax.servlet.ServletContext;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;

import com.scissor.Log;
import com.scissor.StreamCopier;
import com.tawala.World;

public class WorldInitializer extends HttpServlet {
    private static final long serialVersionUID = 1L;
    private static World world;
    private static String realPath;

    @Override
    public void init(ServletConfig servletConfig) throws ServletException {
        ServletContext context = servletConfig.getServletContext();
        Properties config = new Properties();
        Enumeration paramNames = context.getInitParameterNames();
        while (paramNames.hasMoreElements()) {
            String name = (String) paramNames.nextElement();
            config.setProperty(name, context.getInitParameter(name));
        }
        realPath = context.getRealPath("/");
        world = new World(realPath, config, getBuildNumber(context));
    }

    private String getBuildNumber(ServletContext context) {
    	InputStream inputStream = context.getResourceAsStream("/buildinfo.txt");
    	String result = null;
    	if(inputStream == null) {
			Log.error(this, "Unable to load buildinfo.txt; using default build number.");
    	} else {
    		ByteArrayOutputStream out = new ByteArrayOutputStream();
    		try {
				StreamCopier.copy(inputStream, out);
	    		String buildInfo = new String(out.toByteArray());
	    		
	    		result = extractBuildNumber(buildInfo);
			} catch (IOException e) {
				Log.error(this, "Unable to read buildinfo.txt: ", e);
			}
    	}
    	if(result == null) {
    		return World.getBuildNumberSubstitute();
    	}
		return result;
	}

	public static String extractBuildNumber(String buildInfo) {
		String result = null;
		Pattern pattern = Pattern.compile("Build label: (.*)", Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(buildInfo);
		if(matcher.find()) {
			result = matcher.group(1);
		} else {
			Log.error(WorldInitializer.class, "Unable to extract build number from buildinfo.txt");
		}
		return result;
	}

	public static World getDefaultWorld() {
        if (world == null)
            throw new IllegalStateException("World is not initialized.");
        return world;
    }

    /**
     * @return Returns the realPath.
     */
    public static String getRealPath() {
        return realPath;
    }
}
