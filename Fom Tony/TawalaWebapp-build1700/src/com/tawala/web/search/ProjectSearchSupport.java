package com.tawala.web.search;

import com.tawala.search.InProcessProjectIndexer;

abstract public class ProjectSearchSupport extends SerializationSupport {
	private InProcessProjectIndexer projectIndexer;

	public InProcessProjectIndexer getProjectIndexer() {
		return projectIndexer;
	}

	public void setProjectIndexer(InProcessProjectIndexer projectIndexer) {
		this.projectIndexer = projectIndexer;
	}
}
