package com.tawala.userdomain;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.JoinTable;
import javax.persistence.Lob;
import javax.persistence.ManyToMany;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;
import org.hibernate.annotations.IndexColumn;

import com.tawala.project.library.LibraryProject;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_domain_id")
@Entity
@Table(name = "domain")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "domain")
public class UserDomain {
	@Id
	@Column(name = "domain_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@ManyToMany(cascade = { CascadeType.PERSIST, CascadeType.MERGE })
	@JoinTable(name = "domain_lib_project", joinColumns = { @JoinColumn(name = "domain_id") }, inverseJoinColumns = { @JoinColumn(name = "lib_project_id") })
	@IndexColumn(name="index")
	private List<LibraryProject> featuredProjects;

	@Column(name="name", length=60, nullable=false, unique=true)
	private String name;

	@Column(name="display_name", length=60, nullable=false, unique=true)
	private String displayName;

	@Column(name="title", length=120, nullable=false)
	private String title;
	
	@Column(name="subtitle", length=120, nullable=false)
	private String subtitle;

	@Column(name="desc_caption", length=120, nullable=false)
	private String descriptionCaption;
	
	@Column(name="description", nullable=false)
	@Lob
	private String description;
	
	@Column(name="suggestion", nullable=false)
	@Lob
	private String suggestionPrompt;

	@Column(name="solutions_caption", length=120, nullable=false)
	private String featuredSolutionsCaption;

	@Transient
	private boolean showSuggestionsBlock = true;
	
	@Transient
	private boolean showNotifyBlock = true;
	
	public UserDomain() {
	}

	public List<LibraryProject> getFeaturedProjects() {
		if(featuredProjects == null) {
			featuredProjects = new ArrayList<LibraryProject>();
		}
		return featuredProjects;
	}

	public void setFeaturedProjects(List<LibraryProject> featuredProjects) {
		this.featuredProjects = featuredProjects;
	}

	public long getId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public String getDescriptionCaption() {
		return descriptionCaption;
	}

	public void setDescriptionCaption(String descriptionCaption) {
		this.descriptionCaption = descriptionCaption;
	}

	public String getSubtitle() {
		return subtitle;
	}

	public void setSubtitle(String subtitle) {
		this.subtitle = subtitle;
	}

	public String getSuggestionPrompt() {
		return suggestionPrompt;
	}

	public void setSuggestionPrompt(String suggestionPrompt) {
		this.suggestionPrompt = suggestionPrompt;
	}

	public String getTitle() {
		return title;
	}

	public void setTitle(String title) {
		this.title = title;
	}

	public String getFeaturedSolutionsCaption() {
		return featuredSolutionsCaption;
	}

	public void setFeaturedSolutionsCaption(String featuredSolutionsCaption) {
		this.featuredSolutionsCaption = featuredSolutionsCaption;
	}

	public boolean isShowNotifyBlock() {
		return showNotifyBlock;
	}

	public void setShowNotifyBlock(boolean showNotifyBlock) {
		this.showNotifyBlock = showNotifyBlock;
	}

	public boolean isShowSuggestionsBlock() {
		return showSuggestionsBlock;
	}

	public void setShowSuggestionsBlock(boolean showSuggestionsBlock) {
		this.showSuggestionsBlock = showSuggestionsBlock;
	}

	public String getDisplayName() {
		return displayName;
	}

	public void setDisplayName(String displayName) {
		this.displayName = displayName;
	}
}
