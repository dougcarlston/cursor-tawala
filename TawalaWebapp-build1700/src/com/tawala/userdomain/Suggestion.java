package com.tawala.userdomain;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Lob;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_suggestion_id")
@Entity
@Table(name = "suggestion")
public class Suggestion {
	@SuppressWarnings("unused")
	@Id
	@Column(name = "suggestion_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@Column(name="domain_name", length=60, nullable=true)
	private String domainName;

	@Column(name="suggestion", nullable=false)
	@Lob
	private String suggestion;
	
	@Column(name="created_dt", nullable=false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date createdDate;
	
	public Suggestion() {
	}

	public Date getCreatedDate() {
		return createdDate;
	}

	public void setCreatedDate(Date createdDate) {
		this.createdDate = createdDate;
	}

	public String getDomainName() {
		return domainName;
	}

	public void setDomainName(String domainName) {
		this.domainName = domainName;
	}

	public String getSuggestion() {
		return suggestion;
	}

	public void setSuggestion(String suggestion) {
		this.suggestion = suggestion;
	}
}