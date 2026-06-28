package com.tawala.project.library;

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

import com.tawala.project.Project;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_lib_project_version_id")
@Entity
@Table(name = "lib_project_version")
@ Cache( usage=CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region="library-project-versions" )
public class LibraryProjectVersion implements Comparable<LibraryProjectVersion> {

    @Id
    @Column(name = "lib_project_version_id")
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
    private long id;
    
    @ManyToOne
    @JoinColumn(name="lib_project_id")
    private LibraryProject libraryProject;

    @Column(name = "version_number", nullable = false)
    private int versionNumber;

    @Column(name = "user_id", length = 20, nullable = false)
    private String userId;

    @Column(name = "description", nullable = false)
    @Lob
    private String text;
    
    @OneToOne(fetch=FetchType.EAGER, cascade={CascadeType.PERSIST, CascadeType.REMOVE})
    @JoinColumn(name = "project_id", nullable = false)
    private Project project;

    @ Column(name="deleted", nullable=false)
    private boolean deleted;

    @Column(name="created_dt", nullable=false)
    @Temporal(TemporalType.TIMESTAMP)
    private Date date;

    LibraryProjectVersion() {
        //--- For Hibernate's use
    }
    
    public LibraryProjectVersion(LibraryProject libraryProject, String userId, Project project) {
    	if(libraryProject == null) {
    		throw new IllegalStateException("Parent library project must be provided.");
    	}
    	this.libraryProject = libraryProject;
        this.userId = userId;
        this.date = new Date();
        this.project = project;
    }

    public void setText(String text) {
        this.text = text;
    }

    public LibraryProject getLibraryProject() {
		return libraryProject;
	}

	public Date getDate() {
        return date;
    }

    public String getText() {
        return text;
    }

    public String getUserId() {
        return userId;
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

    public int compareTo(LibraryProjectVersion o) {
        return this.versionNumber - o.versionNumber;
    }

    /**
     * @return Returns the project.
     */
    public Project getProject() {
        return project;
    }
}
