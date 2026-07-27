package com.tawala.userdomain;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_notification_request_id")
@Entity
@Table(name = "notification_request")
public class NotificationRequest {
	@SuppressWarnings("unused")
	@Id
	@Column(name = "notification_request_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@Column(name="domain_name", length=60, nullable=false)
	private String domainName;

	@Column(name="email", length=120, nullable=false)
	private String email;

	@Column(name="created_dt", nullable=false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date createdDate;
	
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

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}
}