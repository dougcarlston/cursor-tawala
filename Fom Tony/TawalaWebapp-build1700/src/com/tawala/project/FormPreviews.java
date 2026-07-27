package com.tawala.project;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

//--- TODO: use soft references or a better caching system.
public class FormPreviews {
	private Map<String, LinkToUserProject> projects = Collections
			.synchronizedMap(new HashMap<String, LinkToUserProject>());

	public void put(String userId, UserProject project) throws Exception {
		String projectId = projectId(userId, project.getName());
		projectId = URLEncoder.encode(projectId, "UTF-8");
		project.setUniqueRandomId(projectId);
		LinkToUserProject linkToUserProject = LinkToUserProject.createUnauthenticatedLink(project, projectId);
		project.getProject().populateFormTokensWithFormNames();

		projects.put(projectId, linkToUserProject);
	}

	public LinkToUserProject getProject(String userId, String projectName) {
		return projects.get(projectId(userId, projectName));
	}

	public LinkToUserProject getProject(String projectId) {
		try {
			return projects.get(URLEncoder.encode(projectId, "UTF-8"));
		} catch (UnsupportedEncodingException e) {
			throw new RuntimeException(e);
		}
	}

	private String projectId(String userId, String projectName) {
		return (userId + "-" + projectName);
	}
}
