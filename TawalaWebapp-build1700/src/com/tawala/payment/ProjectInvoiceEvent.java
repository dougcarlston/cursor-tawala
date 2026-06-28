package com.tawala.payment;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.Lob;
import javax.persistence.ManyToOne;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

import com.tawala.payment.paypal.InstantPaymentNotification;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_project_invoice_event_id")

@Entity
@Table(name = "project_invoice_event")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "projects")

public class ProjectInvoiceEvent {
	@Id
	@Column(name = "notification_request_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@ManyToOne()
	@JoinColumn(name = "project_invoice_id", nullable = false)
	private ProjectInvoice invoice;
	
	@Column(name = "status", length=25, nullable=false)
	private String status;
	
	@Lob
	@Column(name = "payload", nullable=false)
	private String payload;
	
	@Column(name = "event_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date eventDate;

	@Column(name = "created_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date createdDate;

	//--- For Hibernate's use.
	ProjectInvoiceEvent() {
	}
	
	public ProjectInvoiceEvent(ProjectInvoice invoice, InstantPaymentNotification notification) {
		this.status = notification.getPaymentStatus();
		this.invoice = invoice;
		this.createdDate = new Date();
		this.eventDate = notification.getTransactionDate();
		this.payload = notification.getOriginalPayload();
	}

	public Date getCreatedDate() {
		return createdDate;
	}

	public void setCreatedDate(Date createdDate) {
		this.createdDate = createdDate;
	}

	public Date getEventDate() {
		return eventDate;
	}

	public void setEventDate(Date eventDate) {
		this.eventDate = eventDate;
	}

	public long getId() {
		return id;
	}

	public void setId(long id) {
		this.id = id;
	}

	public ProjectInvoice getInvoice() {
		return invoice;
	}

	public void setInvoice(ProjectInvoice invoice) {
		this.invoice = invoice;
	}

	public String getPayload() {
		return payload;
	}

	public void setPayload(String payload) {
		this.payload = payload;
	}

	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
	}
}
