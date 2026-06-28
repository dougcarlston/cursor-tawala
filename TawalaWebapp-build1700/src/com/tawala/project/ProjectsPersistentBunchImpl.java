package com.tawala.project;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import com.tawala.World;
import com.tawala.domain.DomainMetadata;
import com.tawala.domain.User;
import com.tawala.persistence.InMemoryPersistenceStrategy;
import com.tawala.persistence.PersistenceStrategy;
import com.tawala.persistence.PersistentBunch;
import com.tawala.util.RandomTokenGenerator;

public class ProjectsPersistentBunchImpl extends PersistentBunch implements
		Projects {
	private Map<ProjectId, UserProject> contents = new TreeMap<ProjectId, UserProject>();
	private Map<String, LinkToUserProject> mapByRandomId = new HashMap<String, LinkToUserProject>();

	public ProjectsPersistentBunchImpl() {
		this(new InMemoryPersistenceStrategy());
	}

	public ProjectsPersistentBunchImpl(PersistenceStrategy persistenceStrategy) {
		super(persistenceStrategy);
	}

	public synchronized UserProject put(UserProject project) {
		justPut(project);
		getPersistenceStrategy().save(project);
		return project;
	}

	private void justPut(UserProject project) {
		contents.put(new ProjectId(project), project);

		LinkToUserProject linkToProject = LinkToUserProject.createUnauthenticatedLink(project, project.getUniqueRandomId());

		mapByRandomId.put(linkToProject.getId(), linkToProject);
	}

	public void delete(UserProject project, World world) {
		if (contents.containsValue(project)) {
			for (Form form : project.getProject().getForms()) {
				world.domain().storedData().eraseResponsesFor(
						project.getProject(), form.getName());
			}
			getPersistenceStrategy().delete(project);
			contents.remove(new ProjectId(project));
			mapByRandomId.remove(project.getUniqueRandomId());
		}
	}

	public long fileSize(Project project, World world) {
		return getPersistenceStrategy().size(project);
	}

	public synchronized UserProject get(String userId, String projectName) {
		if (userId == null || projectName == null)
			return null;
		return contents.get(new ProjectId(userId, projectName));
	}

	public int size() {
		return contents.size();
	}

	public List<UserProject> getAllForUserId(String userId) {
		List<UserProject> projects = new ArrayList<UserProject>();
		for (UserProject project : contents.values()) {
			if (project.getUser().getId().equals(userId))
				projects.add(project);
		}
		return projects;
	}

	private static class ProjectId implements Comparable {
		final String userId;
		final String projectName;

		public ProjectId(UserProject project) {
			this(project.getUser().getId(), project.getName());
		}

		public ProjectId(String userId, String projectName) {
			this.userId = userId;
			this.projectName = projectName;
		}

		public boolean equals(Object o) {
			if (this == o)
				return true;
			if (!(o instanceof ProjectId))
				return false;

			final ProjectId projectId = (ProjectId) o;

			if (!projectName.equals(projectId.projectName))
				return false;
			if (!userId.equals(projectId.userId))
				return false;

			return true;
		}

		public int hashCode() {
			int result;
			result = userId.hashCode();
			result = 29 * result + projectName.hashCode();
			return result;
		}

		public int compareTo(Object o) {
			ProjectId other = (ProjectId) o;
			if (!this.userId.equals(other.userId)) {
				return this.userId.compareTo(other.userId);
			}
			if (!this.projectName.equals(other.projectName)) {
				return this.projectName.compareTo(other.projectName);
			}
			return 0;
		}
	}

	public void deleteAllProjectsForUser(World world, User user) {
		Collection<UserProject> allUserProjects = getAllForUserId(user.getId());
		for (UserProject project : allUserProjects) {
			delete(project, world);
		}
	}

	public UserProject getWithProjectRuntime(String userId, String projectName) {
		return get(userId, projectName);
	}

	public UserProject getWithVersions(String userId, String projectName) {
		return get(userId, projectName);
	}

	public ProjectVersion findProjectVersion(long versionId) {
		throw new IllegalStateException("findProjectVersion is not implemented");
	}

	public UserProject deployProjectVersion(User user, long projectVersionId) {
		throw new IllegalStateException(
				"deployProjectVersion is not implemented");
	}

	public UserProject deleteProjectVersion(User sessionUser,
			long projectVersionId) {
		throw new IllegalStateException(
				"deleteProjectVersion is not implemented");
	}

	public LinkToUserProject getWithProjectRuntime(String randomProjectId) {
		return mapByRandomId.get(randomProjectId);
	}

	public void changeProjectNameAndOwnership(long projectId, String newName,
			User newOwner) {
		throw new IllegalStateException(
				"changeProjectOwnership is not implemented");
	}

	public long projectCountFor(User user) {
		throw new IllegalStateException(
				"projectCountFor(user) is not implemented");
	}

	public void addLinkToProject(LinkToUserProject linkToProject) {
		do {
			linkToProject.setId(generateUniqueRandomProjectId());
		} while (mapByRandomId.containsKey(linkToProject.getId()));

		mapByRandomId.put(linkToProject.getId(), linkToProject);
	}

	private String generateUniqueRandomProjectId() {
		return RandomTokenGenerator.getRandomToken(DomainMetadata.instance
				.getProjectUniqueTokenLength());
	}

	public List<ProjectStatistics> getProjectStatistics(User user,
			ProjectFilter filter, ProjectSortOrder sortOrder, int startRow, int maxCount) {
		throw new IllegalStateException(
				"getProjectStatistics is not implemented");
	}

	public void removeDataSource(World world, Project sharedData,
			String dataSourceName) {
		throw new IllegalStateException("removeDataSource is not implemented");
	}

	public void changeProjectTheme(String projectId, String theme) {
		throw new IllegalStateException("changeProjectTheme is not implemented");
	}

	public boolean takeProjectOffline(User sessionUser, long projectId, boolean newStatus) {
		throw new IllegalStateException("changeOnlineStatus is not implemented");
	}

	public long inactiveProjectCountFor(User user) {
		throw new IllegalStateException("inactiveProjectCountFor is not implemented");
	}
}
