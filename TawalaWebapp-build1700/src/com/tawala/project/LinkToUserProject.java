package com.tawala.project;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.Lob;
import javax.persistence.OneToOne;
import javax.persistence.Table;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

@Entity
@Table(name = "user_project_link")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "projects")
public class LinkToUserProject {
	@Id
	@Column(name = "user_project_link_id", length = 20, nullable = false, unique = true)
	private String id;

	@Column(name = "is_authenticated", nullable = false)
	private boolean authenticated;
	
	@Column(name = "authentication_token", nullable = true)
	@Lob
	private String authenticationToken;

	@OneToOne(fetch = FetchType.EAGER)
	@JoinColumn(name = "user_project_id", nullable = false)
	private UserProject project;

	@Column(name = "use_once", nullable = false)
	private boolean useOnce;

	public boolean isUseOnce() {
		return useOnce;
	}

	public void setUseOnce(boolean useOnce) {
		this.useOnce = useOnce;
	}

	LinkToUserProject() {
		// --- For Hibernate's use
	}
	public static LinkToUserProject createUnauthenticatedLink(UserProject project) {
		return createUnauthenticatedLink(project, null);
	}
	
	public static LinkToUserProject createUnauthenticatedLink(UserProject project, String linkId) {
		return new LinkToUserProject(project, linkId, false);
	}
	private LinkToUserProject(UserProject project, String id, boolean unused /* needed to differentiate from another constructor */) {
		this.project = project;
		this.id = id;
	}

	public LinkToUserProject(UserProject project, String authenticationToken) {
		this.project = project;
		this.authenticated = true;
		this.authenticationToken = authenticationToken;
	}

	public LinkToUserProject(String linkId, boolean authenticated, String authenticationToken) {
		this.id = linkId;
		this.authenticated = authenticated;
		this.authenticationToken = authenticationToken;
	}

	public String getAuthenticationToken() {
		return authenticationToken;
	}

	public String getId() {
		return id;
	}

	public void setId(String id) {
		this.id = id;
	}

	public boolean isAuthenticated() {
		return authenticated;
	}

	public UserProject getProject() {
		return project;
	}

	public void setProject(UserProject project) {
		this.project = project;
	}
}
