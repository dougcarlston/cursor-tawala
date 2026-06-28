package com.tawala;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Properties;

import com.tawala.domain.Domain;


public class World {
    private final Domain domain;
    private final ArrayList<String> contentRoots = new ArrayList<String>();
    private final String buildNumber;

    public World(String realPath, Properties config, String buildNumber) {
        this.domain = new Domain();
        this.buildNumber = buildNumber;

        addContentRoot(realPath);
        String contentDir = config.getProperty("content.dir");
        if (contentDir != null && !contentDir.equals("")) {
            addContentRoot(contentDir);
        }
    }
    
    public World(String realPath, Users usersImplementation) {
    	this.buildNumber = getBuildNumberSubstitute();
        this.domain = new Domain(usersImplementation);
        addContentRoot(realPath);
    }

	public static String getBuildNumberSubstitute() {
		return new SimpleDateFormat("yyyyMMdd").format(new Date());
	}

    public void addContentRoot(String path) {
        contentRoots.add(0, path);
    }

    public void removeContentRoot(String path) {
        contentRoots.remove(path);
    }


    public Domain domain() {
        return domain;
    }

    public List<String> getContentRoots() {
        return contentRoots;
    }

	public String getBuildNumber() {
		return buildNumber;
	}
}
