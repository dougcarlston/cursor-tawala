package com.tawala.project;

import java.util.List;

import com.tawala.World;
import com.tawala.domain.User;

public interface Projects {
    public UserProject put(UserProject project);

    public void delete(UserProject project, World world);

    public long fileSize(Project project, World world);

    public UserProject get(String userId, String projectName);
    
    public UserProject getWithProjectRuntime(String userId, String projectName);

    public LinkToUserProject getWithProjectRuntime(String randomProjectId);
    
    public UserProject getWithVersions(String userId, String projectName);

    public int size();

    public List<UserProject> getAllForUserId(String userId);

    public void deleteAllProjectsForUser(World world, User user);

	public ProjectVersion findProjectVersion(long versionId);
	
	public UserProject deployProjectVersion(User user, long projectVersionId);

	public UserProject deleteProjectVersion(User sessionUser, long projectVersionId);

	public void changeProjectNameAndOwnership(long projectId, String newName, User newOwner);

	public long projectCountFor(User user);

	public long inactiveProjectCountFor(User user);
	
	public void addLinkToProject(LinkToUserProject linkToProject);
	
	public List<ProjectStatistics> getProjectStatistics(User user, ProjectFilter filter, ProjectSortOrder sortOrder, int startRow, int maxCount);

	public void removeDataSource(World world, Project sharedData, String dataSourceName);

	public void changeProjectTheme(String projectId, String theme);

	public boolean takeProjectOffline(User sessionUser, long projectId, boolean newStatus);
}
