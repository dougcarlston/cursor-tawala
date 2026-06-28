package com.tawala.web.projectmanager;

import com.tawala.project.ProjectFilter;
import com.tawala.project.ProjectSortOrder;

public class ProjectPagingInfo {
	private ProjectSortOrder sortOrder;
	private ProjectFilter filter;
	private int start;
	private int max;
	private static final int MAX_NUMBER_OF_PROJECTS_PER_PAGE = 100;
	
	public int getMax() {
		return max;
	}
	public ProjectSortOrder getSortOrder() {
		return sortOrder;
	}
	public int getStart() {
		return start;
	}
	public void setMax(int max) {
		this.max = Math.min(max, MAX_NUMBER_OF_PROJECTS_PER_PAGE);
	}
	public void setSortOrder(ProjectSortOrder sortOrder) {
		this.sortOrder = sortOrder;
	}
	public void setStart(int start) {
		this.start = start;
	}
	public ProjectFilter getFilter() {
		return filter;
	}
	public void setFilter(ProjectFilter filter) {
		this.filter = filter;
	} 
}
