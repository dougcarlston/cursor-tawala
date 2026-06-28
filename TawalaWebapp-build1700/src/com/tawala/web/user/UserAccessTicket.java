package com.tawala.web.user;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import com.tawala.domain.User;

@Entity
@Table(name = "access_ticket")
public class UserAccessTicket {
	@Id
	@Column(name = "access_ticket_id", length=40)
	private String accessToken;
	
	@ManyToOne(fetch = FetchType.EAGER)
	@JoinColumn(name = "user_id", nullable = false)
	private User user;
	
	@Column(name = "created_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date created;

	@Column(name = "last_used_dt", nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date lastUsed;
	
	UserAccessTicket() {
		// For Hibernate's use
	}
	
	public UserAccessTicket(String accessToken, User user) {
		this.accessToken = accessToken;
		this.user = user;
		this.created = new Date();
		this.lastUsed = null;
	}
	
	public String getAccessToken() {
		return accessToken;
	}
	public void setAccessToken(String accessToken) {
		this.accessToken = accessToken;
	}
	public Date getCreated() {
		return created;
	}
	public void setCreated(Date created) {
		this.created = created;
	}
	public Date getLastUsed() {
		return lastUsed;
	}
	public void setLastUsed(Date lastUsed) {
		this.lastUsed = lastUsed;
	}
	public User getUser() {
		return user;
	}
	public void setUser(User user) {
		this.user = user;
	}

}
