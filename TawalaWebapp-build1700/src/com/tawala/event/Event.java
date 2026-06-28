package com.tawala.event;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.servlet.http.HttpServletRequest;

import com.tawala.web.controller.VisitorTrackerInterceptor;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_event_id")
@Entity
@Table(name = "event")
public class Event {
	@SuppressWarnings("unused")
	@Id
	@Column(name = "event_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@SuppressWarnings("unused")
	@Column(name = "visitor_id")
	private Long visitorId;

	@SuppressWarnings("unused")
	@Column(name = "user_id")
	private Long userId;

	@SuppressWarnings("unused")
	@Column(name = "type", nullable = false, length=40)
	private String type;

	@SuppressWarnings("unused")
	@Column(name = "param1", length = 300)
	private String parameter1;

	public Event(String type, String parameter1) {
		this.type = type;
		setParameter1(parameter1);
	}

	public Event(String type, HttpServletRequest request) {
		this.type = type;
		VisitorTrackerInterceptor.populateEvent(this, request);
	}

	public Event(String type, HttpServletRequest request, String parameter1) {
		this(type, request);
		setParameter1(parameter1);
	}

	public void setParameter1(String parameter1) {
		this.parameter1 = parameter1 == null || parameter1.length() <=300 ? parameter1 : parameter1.substring(0, 300);
	}

	public void setType(String type) {
		this.type = type;
	}

	public void setUserId(Long userId) {
		this.userId = userId;
	}

	public void setVisitorId(Long visitorId) {
		this.visitorId = visitorId;
	}
}
