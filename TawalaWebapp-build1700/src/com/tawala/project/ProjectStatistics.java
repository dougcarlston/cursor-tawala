package com.tawala.project;

import java.util.Date;

public class ProjectStatistics {
	private long id;
	private String name;
	private long responseCount;
	private Date created;
	private Date lastUpdated;
	private Date lastAccessed;
	private boolean offline;
	
	public boolean isOffline() {
		return offline;
	}
	public void setOffline(boolean offline) {
		this.offline = offline;
	}
	public Date getCreated() {
		return created;
	}
	public void setCreated(Date created) {
		this.created = created;
	}
	public long getId() {
		return id;
	}
	public void setId(long id) {
		this.id = id;
	}
	public Date getLastAccessed() {
		return lastAccessed;
	}
	public void setLastAccessed(Date lastAccessed) {
		this.lastAccessed = lastAccessed;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public long getResponseCount() {
		return responseCount;
	}
	public void setResponseCount(long responseCount) {
		this.responseCount = responseCount;
	}
	public Date getLastUpdated() {
		return lastUpdated;
	}
	public void setLastUpdated(Date lastUpdated) {
		this.lastUpdated = lastUpdated;
	}

}
