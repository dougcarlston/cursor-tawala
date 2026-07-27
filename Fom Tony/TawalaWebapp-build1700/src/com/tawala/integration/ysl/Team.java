package com.tawala.integration.ysl;

import java.util.ArrayList;
import java.util.List;

import com.tawala.project.FormSubmission;

public class Team {
	private FormSubmission record;
	private String teamName;
	private String uniqueId;

	private List<Coach> coaches = new ArrayList<Coach>();
	
	public Team(FormSubmission teamRecord) {
		this.record = teamRecord;
	}

	public long getRecordId() {
		return record.getDatabaseId();
	}
	public String getTeamName() {
		return teamName;
	}
	public void setTeamName(String teamName) {
		this.teamName = teamName;
	}
	public String getUniqueId() {
		return uniqueId;
	}
	public void setUniqueId(String uniqueId) {
		this.uniqueId = uniqueId;
	}
	
	public void addCoach(Coach coach) {
		coaches.add(coach);
	}

	public List<Coach> getCoaches() {
		return coaches;
	}

	public FormSubmission getRecord() {
		return record;
	}
}
