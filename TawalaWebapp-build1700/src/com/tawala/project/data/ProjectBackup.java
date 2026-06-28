package com.tawala.project.data;

import java.util.List;
import java.util.Properties;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;

public class ProjectBackup {
	private Properties backupProperties;
	private HSSFWorkbook data;
	private Project project;
	private String projectProperties;
	private List<LinkToUserProject> links;
	
	public HSSFWorkbook getData() {
		return data;
	}
	public void setData(HSSFWorkbook data) {
		this.data = data;
	}
	public List<LinkToUserProject> getLinks() {
		return links;
	}
	public void setLinks(List<LinkToUserProject> links) {
		this.links = links;
	}
	public String getProjectProperties() {
		return projectProperties;
	}
	public void setProjectProperties(String projectProperties) {
		this.projectProperties = projectProperties;
	}
	public Project getProject() {
		return project;
	}
	public void setProject(Project project) {
		this.project = project;
	}
	public Properties getBackupProperties() {
		return backupProperties;
	}
	public void setBackupProperties(Properties backupProperties) {
		this.backupProperties = backupProperties;
	}
	public String getOriginalProjectName() {
		return backupProperties.getProperty(ProjectBackupCreator.PROPERTY_PROJECT_NAME);
	}
	public String getProjectUniqueId() {
		return backupProperties.getProperty(ProjectBackupCreator.PROPERTY_PROJECT_UNIQUE_ID);
	}
	public long getOriginalProjectId() {
		return Long.parseLong(backupProperties.getProperty(ProjectBackupCreator.PROPERTY_PROJECT_ID));
	}
}
