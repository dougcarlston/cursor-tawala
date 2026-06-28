package com.tawala.payment;

import java.math.BigDecimal;
import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

import com.tawala.domain.User;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;

@Entity
@Table(name = "project_invoice")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "projects")

public class ProjectInvoice {
	@Id
	@Column(name = "project_invoice_id", length=20)
	private String id;
	
	@ManyToOne()
	@JoinColumn(name = "submission_id", nullable = true)
	private FormSubmission formSubmission;
	
	@ManyToOne()
	@JoinColumn(name = "user_project_id", nullable = true)
	private UserProject userProject;

	@ManyToOne()
	@JoinColumn(name = "user_id", nullable = true)
	private User user;
	
	@Column(name = "status", length=25, nullable=false)
	private String status;
	
	@Column(name = "invoice_amount", nullable=false)
	private BigDecimal requestedAmount;

	@Column(name = "status_field_name", nullable=false, length=100)
	private String statusFieldName;

	@Column(name = "paid_amount_field_name", nullable=true, length=100)
	private String amountFieldName;
	
	@Column(name = "invoice_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date invoiceDate;
	
	@Column(name = "paid_amount", nullable = true)
	private BigDecimal paidAmount;

	@Column(name = "paid_dt", nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date paidDate;
	
	//--- For Hibernate's use.
	ProjectInvoice() {
	}
	
	public ProjectInvoice(FormSubmission submission, UserProject userProject, BigDecimal requestedAmount, String statusFieldName, String fieldToRecordAmount) {
		this.status = "New";
		this.invoiceDate = new Date();
		this.formSubmission = submission;
		this.userProject = userProject;
		this.user = userProject.getUser();
		this.requestedAmount = requestedAmount;
		this.statusFieldName = statusFieldName;
		this.amountFieldName = fieldToRecordAmount;
	}

	public FormSubmission getFormSubmission() {
		return formSubmission;
	}

	public String getId() {
		return id;
	}

	public Date getInvoiceDate() {
		return invoiceDate;
	}

	public BigDecimal getPaidAmount() {
		return paidAmount;
	}

	public Date getPaidDate() {
		return paidDate;
	}

	public BigDecimal getRequestedAmount() {
		return requestedAmount;
	}

	public String getStatusFieldName() {
		return statusFieldName;
	}

	public User getUser() {
		return user;
	}

	public UserProject getUserProject() {
		return userProject;
	}
	
	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
	}

	public void setId(String id) {
		this.id = id;
	}

	public void setPaidAmount(BigDecimal paidAmount) {
		this.paidAmount = paidAmount;
	}

	public void setPaidDate(Date paidDate) {
		this.paidDate = paidDate;
	}

	public String getAmountFieldName() {
		return amountFieldName;
	}
}
