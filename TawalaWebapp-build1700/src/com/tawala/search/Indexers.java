package com.tawala.search;

import java.io.IOException;

public class Indexers {
    private static ProjectIndexer projectIndexer;
    private static UserIndexer userIndexer;

    private Indexers() {
        
    }
    
    public static void initIndexers(ProjectIndexer projectIndexer, UserIndexer userIndexer)
            throws IOException {
        destroy();
    
        Indexers.projectIndexer = projectIndexer;
        Indexers.userIndexer = userIndexer;
    }

    public static void destroy() {
        if (projectIndexer != null)
            projectIndexer.destroy();
        if (userIndexer != null)
            userIndexer.destroy();
    }

    /**
     * @return Returns the projectIndexer.
     */
    public static ProjectIndexer getProjectIndexer() {
        return projectIndexer;
    }

    /**
     * @return Returns the userIndexer.
     */
    public static UserIndexer getUserIndexer() {
    	if(userIndexer == null) {
    		throw new IllegalStateException("User Indexer is not set up.");
    	}
        return userIndexer;
    }
}
