package com.tawala.web;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

import javax.servlet.ServletContextEvent;
import javax.servlet.ServletContextListener;

import org.apache.log4j.PropertyConfigurator;

import com.scissor.Log;

/**
 * Attempts to initialize the log4j library if it can find the configuration
 * file. Currently supports .properties file format. XML configuration can be
 * added as needed.
 * 
 * @author Sergei Lilichenko
 */
public class LoggingInitializer implements ServletContextListener {

    public static final String LOG4J_CONFIG_FILE = "log4j.config.file";

    public void contextInitialized(ServletContextEvent contextEvent) {
        String propertiesName = contextEvent.getServletContext()
                .getInitParameter(LOG4J_CONFIG_FILE);
        if (propertiesName == null || propertiesName.length() == 0)
            return;

        Properties log4jProperties = new Properties();

        InputStream inputStream = LoggingInitializer.class.getClassLoader()
                .getResourceAsStream(propertiesName);
        if (inputStream == null) {
            Log.warn(this, "Unable to find log4j configuration file '"
                    + propertiesName + "'.");
            return;
        }

        Log.info(this, "Configuring log4j using " + propertiesName);
        try {
            log4jProperties.load(inputStream);
        } catch (IOException e) {
            throw new RuntimeException("Unable to load " + propertiesName, e);
        } finally {
            try {
                inputStream.close();
            } catch (IOException e) {
                // Do nothing - we can't do much about it.
            }
        }

        PropertyConfigurator.configure(log4jProperties);
    }

    public void contextDestroyed(ServletContextEvent contextEvent) {
        // Do nothing
    }
}
