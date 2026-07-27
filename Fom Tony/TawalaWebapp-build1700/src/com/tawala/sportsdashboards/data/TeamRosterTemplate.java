package com.tawala.sportsdashboards.data;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.core.io.Resource;

import com.tawala.domain.notification.BaseNotification;

public class TeamRosterTemplate {
	private static Map<String, TeamRosterTemplate> templates = new LinkedHashMap<String, TeamRosterTemplate>();
	
	private String id;
	private String description;
	private byte[] serializedTemplate;
	private int maxNumberOfCoaches;
	private int maxNumberOfPlayers;
	
	public TeamRosterTemplate(String id, String descripton, Resource template) throws IOException {
		this.id =id;
		this.description = descripton;
		this.serializedTemplate = BaseNotification.getResourceAsByteArray(template).toByteArray();
	}

	public static TeamRosterTemplate getTemplateById(String id) {
		return templates.get(id);
	}

	public static Collection<TeamRosterTemplate> getAllTemplates() {
		return templates.values();
	}
	
	public static boolean isTemplateExists(String templateId) {
		if(templateId == null || templateId.length() == 0) {
			return false;
		}
		return templates.containsKey(templateId);
	}

	public static class Initializer {
		public void setTemplates(List<TeamRosterTemplate> templates) throws IOException {
			for (TeamRosterTemplate teamRosterTemplate : templates) {
				TeamRosterTemplate.templates.put(teamRosterTemplate.getId(), teamRosterTemplate);
			}
		}
	}

	public String getId() {
		return id;
	}

	public String getDescription() {
		return description;
	}

	public int getMaxNumberOfCoaches() {
		return maxNumberOfCoaches;
	}

	public void setMaxNumberOfCoaches(int maxNumberOfCoaches) {
		this.maxNumberOfCoaches = maxNumberOfCoaches;
	}

	public int getMaxNumberOfPlayers() {
		return maxNumberOfPlayers;
	}

	public void setMaxNumberOfPlayers(int maxNumberOfPlayers) {
		this.maxNumberOfPlayers = maxNumberOfPlayers;
	}

	public HSSFWorkbook getWorkbook() throws IOException {
		return new HSSFWorkbook(new ByteArrayInputStream(this.serializedTemplate));
	}
}
