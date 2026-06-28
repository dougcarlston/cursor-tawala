package com.tawala.web;

import java.io.IOException;

import javax.servlet.ServletContextEvent;
import javax.servlet.ServletContextListener;

import org.springframework.web.context.support.XmlWebApplicationContext;

import com.tawala.search.Indexers;
import com.tawala.search.ProjectIndexer;
import com.tawala.search.UserIndexer;

public class LuceneInitializer implements ServletContextListener {
	public static final String USE_IN_PROCESS_INDEXERS = "lucene.use.in.process";

	/*
	 * (non-Javadoc)
	 * 
	 * @see javax.servlet.ServletContextListener#contextInitialized(javax.servlet.ServletContextEvent)
	 */
	public void contextInitialized(ServletContextEvent contextEvent) {
		String useInProcessParameter = getContextParameterOrSystemProperty(
				contextEvent, USE_IN_PROCESS_INDEXERS);

		if (useInProcessParameter == null) {
			useInProcessParameter = "false";
		}

		boolean useInProcess = Boolean.parseBoolean(useInProcessParameter);
		String[] luceneConfigFiles = new String[] { "/WEB-INF/"
				+ (useInProcess ? "lucene-local-config.xml"
						: "lucene-remote-config.xml") };

		XmlWebApplicationContext appContext = new XmlWebApplicationContext();
		appContext.setServletContext(contextEvent.getServletContext());
		appContext.setConfigLocations(luceneConfigFiles);

		appContext.refresh();
		
		ProjectIndexer projectIndexer = (ProjectIndexer) appContext
				.getBean("projectIndexer");
		UserIndexer userIndexer = (UserIndexer) appContext
				.getBean("userIndexer");

		try {
			Indexers.initIndexers(projectIndexer, userIndexer);
		} catch (IOException e) {
			throw new IllegalStateException(e);
		}
	}

	private String getContextParameterOrSystemProperty(
			ServletContextEvent contextEvent, String parameterName) {
		String result = contextEvent.getServletContext().getInitParameter(
				parameterName);
		if (result == null || result.length() == 0) {
			result = System.getProperty(parameterName);
		}
		return result;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see javax.servlet.ServletContextListener#contextDestroyed(javax.servlet.ServletContextEvent)
	 */
	public void contextDestroyed(ServletContextEvent contextEvent) {
		Indexers.destroy();
	}
}
