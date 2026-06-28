package com.tawala.domain;

import java.util.Comparator;
import java.util.Set;
import java.util.SortedSet;
import java.util.TreeSet;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.JoinTable;
import javax.persistence.ManyToMany;
import javax.persistence.ManyToOne;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;
import org.hibernate.annotations.Sort;
import org.hibernate.annotations.SortType;

import com.tawala.project.UserProject;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_project_group_id")
@Entity
@Table(name = "project_group")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "users")
public class ProjectGroup {
	public static class ProjectComparator implements Comparator<UserProject> {
		public int compare(UserProject o1, UserProject o2) {
			return getDisplayableProjectName(o1).toUpperCase().compareTo(getDisplayableProjectName(o2).toUpperCase());
		}
		
		private static String getDisplayableProjectName(UserProject userProject) {
			return userProject.getUser().getId() + " - " + userProject.getName();
		}
	}
	
	@Id
	@Column(name = "project_group_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private Long id;

	@Column(name = "name", length = 200, nullable = false)
	private String name;

	@ManyToMany(targetEntity = UserProject.class, cascade = {
			CascadeType.PERSIST, CascadeType.MERGE }, fetch = FetchType.EAGER)
	@JoinTable(name = "project_group_member", joinColumns = { @JoinColumn(name = "project_group_id") }, inverseJoinColumns = { @JoinColumn(name = "user_project_id") })
	@Sort(type = SortType.COMPARATOR, comparator = ProjectComparator.class)
	private SortedSet<UserProject> userProjects;

	@ManyToOne( cascade = {CascadeType.PERSIST, CascadeType.MERGE} )
    @JoinColumn(name="user_id")
	private User groupOwner;
	
    ProjectGroup() {
		// For Hibernate's use
	}
    
    public ProjectGroup(User user, String name) {
    	this.groupOwner = user;
    	this.name = name;
    }
    

	public Set<UserProject> getUserProjects() {
		return userProjects;
	}

	public void addUserProject(UserProject project) {
		if (userProjects == null) {
			userProjects = new TreeSet<UserProject>(new ProjectComparator());
		}
		userProjects.add(project);
	}

	public void removeUserProject(UserProject project) {
		userProjects.remove(project);
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public Long getId() {
		return id;
	}

	public User getGroupOwner() {
		return groupOwner;
	}

	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((id == null) ? 0 : id.hashCode());
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		ProjectGroup other = (ProjectGroup) obj;
		if (id == null) {
			if (other.id != null)
				return false;
		} else if (!id.equals(other.id))
			return false;
		return true;
	}

	
}
