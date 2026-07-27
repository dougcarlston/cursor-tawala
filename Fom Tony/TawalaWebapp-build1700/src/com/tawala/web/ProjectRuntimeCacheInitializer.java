package com.tawala.web;

import javax.servlet.ServletContextEvent;
import javax.servlet.ServletContextListener;

import com.tawala.project.caching.ProjectRuntimeCache;

/**
 * Sets up the cache for the project runtime.
 * 
 * @author Sergei Lilichenko
 */
public class ProjectRuntimeCacheInitializer implements ServletContextListener {
    public void contextInitialized(ServletContextEvent contextEvent) {
        ProjectRuntimeCache.instance.start();
    }

    public void contextDestroyed(ServletContextEvent contextEvent) {
        ProjectRuntimeCache.instance.stop();
    }
}
