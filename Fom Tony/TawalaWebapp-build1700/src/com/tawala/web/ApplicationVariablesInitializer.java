package com.tawala.web;

import java.io.File;

import javax.servlet.ServletContextEvent;
import javax.servlet.ServletContextListener;

import org.springframework.web.util.WebUtils;

import com.tawala.domain.DomainMetadata;
import com.tawala.web.controller.WellKnown;

public class ApplicationVariablesInitializer implements ServletContextListener {

    public void contextInitialized(ServletContextEvent event) {
        event.getServletContext().setAttribute("urls", WellKnown.urls);
        event.getServletContext().setAttribute("meta", DomainMetadata.instance);
        
        if(event.getServletContext().getAttribute(WebUtils.TEMP_DIR_CONTEXT_ATTRIBUTE) == null) {
            File tempDir = new File(System.getProperty("user.home") + "/_tawala_temp");
            event.getServletContext().setAttribute(WebUtils.TEMP_DIR_CONTEXT_ATTRIBUTE, tempDir);
        }
    }

    public void contextDestroyed(ServletContextEvent arg0) {
        // Do nothing
    }

}
