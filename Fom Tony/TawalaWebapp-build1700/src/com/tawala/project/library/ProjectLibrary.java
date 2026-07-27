package com.tawala.project.library;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

@Entity
@Table(name = "project_library")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "categories")
public class ProjectLibrary {
	public static ProjectLibrary SYSTEM_LIBRARY = new ProjectLibrary(1,
			"Tawala Application Library", true);
	public static ProjectLibrary COMMUNITY_LIBRARY = new ProjectLibrary(2,
			"User Community Library", false);
	public static ProjectLibrary SYSTEM_UNDER_CONTSTRUCTION_LIBRARY = new ProjectLibrary(
			3, "Under Construction", false);
	public static ProjectLibrary SYSTEM_EXAMPLES_LIBRARY = new ProjectLibrary(
			4, "Project Examples", false);
	public static ProjectLibrary PAID_PROJECT_LIBRARY = new ProjectLibrary(
			5, "Paid Projects", false);


	private static Map<Integer, ProjectLibrary> ID_TO_LIBRARY_MAP = new HashMap<Integer, ProjectLibrary>();
	static {
		ID_TO_LIBRARY_MAP.put(SYSTEM_LIBRARY.getId(), SYSTEM_LIBRARY);
		ID_TO_LIBRARY_MAP.put(COMMUNITY_LIBRARY.getId(), COMMUNITY_LIBRARY);
		ID_TO_LIBRARY_MAP.put(SYSTEM_UNDER_CONTSTRUCTION_LIBRARY.getId(), SYSTEM_UNDER_CONTSTRUCTION_LIBRARY);
		ID_TO_LIBRARY_MAP.put(SYSTEM_EXAMPLES_LIBRARY.getId(), SYSTEM_EXAMPLES_LIBRARY);
		ID_TO_LIBRARY_MAP.put(PAID_PROJECT_LIBRARY.getId(), PAID_PROJECT_LIBRARY);
	}
	
	public static ProjectLibrary getLibraryById(int id) {
		ProjectLibrary result = ID_TO_LIBRARY_MAP.get(id);
		if(result == null) {
			throw new IllegalArgumentException("Unable to find library by id: " + id); 
		}
		return result;
	}
	
	private static List<ProjectLibrary> librariesAvailableToAdmins = new ArrayList<ProjectLibrary>();
	static {
		librariesAvailableToAdmins.add(SYSTEM_LIBRARY);
		librariesAvailableToAdmins.add(SYSTEM_UNDER_CONTSTRUCTION_LIBRARY);
		librariesAvailableToAdmins.add(COMMUNITY_LIBRARY);
		librariesAvailableToAdmins.add(SYSTEM_EXAMPLES_LIBRARY);
		librariesAvailableToAdmins.add(PAID_PROJECT_LIBRARY);
	}
	
	public static List<ProjectLibrary> getLibrariesAvailableToAdmins() {
		return librariesAvailableToAdmins;
	}

	private static List<ProjectLibrary> librariesReadableByFullyRegisteredUsers = new ArrayList<ProjectLibrary>();
	static {
		librariesReadableByFullyRegisteredUsers.add(SYSTEM_LIBRARY);
		librariesReadableByFullyRegisteredUsers.add(COMMUNITY_LIBRARY);
	}
	
	public static List<ProjectLibrary> getLibrariesReadableByFullyRegisteredUsers() {
		return librariesReadableByFullyRegisteredUsers;
	}
	
	private static List<ProjectLibrary> librariesReadableByAnonymousUsers = new ArrayList<ProjectLibrary>();
	static {
		librariesReadableByAnonymousUsers.add(SYSTEM_LIBRARY);
	}
	
	public static List<ProjectLibrary> getLibrariesReadableByAnonymousUsers() {
		return librariesReadableByAnonymousUsers;
	}

	
	private static List<ProjectLibrary> librariesUpdateableByFullyRegisteredUsers = new ArrayList<ProjectLibrary>();
	static {
		librariesUpdateableByFullyRegisteredUsers.add(COMMUNITY_LIBRARY);
	}
	
	public static List<ProjectLibrary> getlibrariesUpdateableByFullyRegisteredUsers() {
		return librariesUpdateableByFullyRegisteredUsers;
	}

	@SuppressWarnings("unchecked")
	public static List<ProjectLibrary> getlibrariesUpdateableByAnonymousUsers() {
		return Collections.EMPTY_LIST;
	}

	@Id
	@Column(name = "project_library_id")
	private int id;
	
	@Column(name = "description", length = 100, nullable = false)
	private String description;

	@Column(name = "show_cloned_count", nullable = false) 
	private boolean showCloneCount;
	
	public static final String DEFAULT_CATEGORY_NAME = "Other";

	ProjectLibrary() {
		// --- For Hibernate's use
	}

	ProjectLibrary(int id, String description, boolean showClonedCount) {
		this.id = id;
		this.description = description;
		this.showCloneCount = showClonedCount;
	}

	public int getId() {
		return id;
	}

	public String getDescription() {
		return description;
	}

	@Override
	public int hashCode() {
		return id;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		final ProjectLibrary other = (ProjectLibrary) obj;
		if (id != other.id)
			return false;
		return true;
	}

	@Override
	public String toString() {
		return getDescription();
	}

	public boolean isShowCloneCount() {
		return showCloneCount;
	}

	public static Collection<ProjectLibrary> getAllPredeterminedLibraries() {
		return ID_TO_LIBRARY_MAP.values();
	}
}