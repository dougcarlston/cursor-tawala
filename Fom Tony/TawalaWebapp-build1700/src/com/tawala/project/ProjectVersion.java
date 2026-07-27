package com.tawala.project;

import java.util.Date;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.Lob;
import javax.persistence.ManyToOne;
import javax.persistence.OneToOne;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_project_version_id")
@Entity
@Table(name = "project_version")
@ Cache( usage=CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region="project-versions" )
public class ProjectVersion implements Comparable<ProjectVersion> {

    @Id
    @Column(name = "project_version_id")
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
    private long id;

    @Column(name = "version_number", nullable = false)
    private int versionNumber;

    @Column(name = "description", nullable = false)
    @Lob
    private String description;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_project_id", nullable = false)
    private UserProject parent;

    @OneToOne(fetch=FetchType.LAZY, cascade={CascadeType.ALL})
    @JoinColumn(name = "project_id", nullable = false)
    private Project project;

    @ Column(name="deleted", nullable=false)
    private boolean deleted;

    @Column(name="created_dt", nullable=false)
    @Temporal(TemporalType.TIMESTAMP)
    private Date date;

    ProjectVersion() {
        //--- For Hibernate's use
    }
    
    public ProjectVersion(UserProject parent, Project project) {
        this.date = new Date();
        this.project = project;
        this.parent = parent;
    }

    public void setDescription(String text) {
        this.description = text;
    }

    public Date getDate() {
        return date;
    }

    public String getDescription() {
        return description;
    }

    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public int getVersionNumber() {
        return versionNumber;
    }

    public void setVersionNumber(int versionNumber) {
        this.versionNumber = versionNumber;
    }
    
    public boolean isDeleted() {
        return deleted;
    }

    public void setDeleted(boolean deleted) {
        this.deleted = deleted;
    }

    public int compareTo(ProjectVersion o) {
        return this.versionNumber - o.versionNumber;
    }

    /**
     * @return Returns the project.
     */
    public Project getProject() {
        return project;
    }

	public UserProject getParent() {
		return parent;
	}
	
	void setParent(UserProject parent) {
		this.parent = parent;
	}
}
