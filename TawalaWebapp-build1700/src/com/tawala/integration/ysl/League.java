package com.tawala.integration.ysl;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.tawala.project.FormSubmission;

public class League {
	private String name;
	private FormSubmission setupRecord;
	private List<Team> teams = new ArrayList<Team>();
	private Map<String, Team> teamByIdMap = new HashMap<String, Team>();
	private Map<String, Team> teamByUniqueIdMap = new HashMap<String, Team>();
	
	public League(String name, FormSubmission setupRecord) {
		this.setupRecord = setupRecord;
		this.name = name;
	}
	
	public String getName() {
		return name;
	}

	public List<Team> getTeams() {
		return teams;
	}
	
	public void addTeam(Team team) {
		this.teams.add(team);
		this.teamByIdMap.put(team.getTeamName(), team);
		this.teamByUniqueIdMap.put(team.getUniqueId(), team);
	}
	
	public Team getTeamById(String teamId) {
		return this.teamByIdMap.get(teamId);
	}
	
	public Team getTeamByUniqueId(String uniqueId) {
		return this.teamByUniqueIdMap.get(uniqueId);
	}

	public FormSubmission getSetupRecord() {
		return setupRecord;
	}
}
