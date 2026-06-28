package com.tawala.domain;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_visitor_id")
@Entity
@Table(name = "visitor")
public class Visitor {
	@Id
	@Column(name = "visitor_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@SuppressWarnings("unused")
	@Column(name = "referrer", length = 300)
	private String referrer;

	@SuppressWarnings("unused")
	@Column(name = "landed_on", length = 100)
	private String landedOn;

	@SuppressWarnings("unused")
	@Column(name = "remote_host", length = 100)
	private String remoteHost;

	@SuppressWarnings("unused")
	@Column(name = "user_agent", length = 100)
	private String userAgent;

	public long getId() {
		return id;
	}

	public void setReferrer(String referrer) {
		this.referrer = trimToLength(referrer, 300);
	}

	public void setLandedOn(String landedOn) {
		this.landedOn = trimToLength(landedOn, 100);
	}

	public void setRemoteHost(String remoteHost) {
		this.remoteHost = trimToLength(remoteHost, 100);
	}

	public void setUserAgent(String userAgent) {
		this.userAgent = trimToLength(userAgent, 100);
	}

	private String trimToLength(String value, int length) {
		return value == null || value.length() <= length ? value : value.substring(0, length);
	}
}
