package com.tawala.jbpm.sportsdashboards;

import java.math.BigDecimal;
import java.util.Date;

public class SportsDashboardStatistics {
	private String userId;
	private long userProjectId;
	private String projectName;
	private int registrationCount;
	private int registrationCountLast5Days;
	private Date registrationOpen;
	private Date lastRegistrationDate;
	private Date registrationClose;
	private Date invoiceDate;
	private BigDecimal registrationFee;
	
	public String getUserId() {
		return userId;
	}
	public void setUserId(String userId) {
		this.userId = userId;
	}
	public long getUserProjectId() {
		return userProjectId;
	}
	public void setUserProjectId(long userProjectId) {
		this.userProjectId = userProjectId;
	}
	public String getProjectName() {
		return projectName;
	}
	public void setProjectName(String projectName) {
		this.projectName = projectName;
	}
	public int getRegistrationCount() {
		return registrationCount;
	}
	public void setRegistrationCount(int registrationCount) {
		this.registrationCount = registrationCount;
	}
	public int getRegistrationCountLast5Days() {
		return registrationCountLast5Days;
	}
	public void setRegistrationCountLast5Days(int registrationCountLast5Days) {
		this.registrationCountLast5Days = registrationCountLast5Days;
	}
	public Date getRegistrationOpen() {
		return registrationOpen;
	}
	public void setRegistrationOpen(Date registrationOpen) {
		this.registrationOpen = registrationOpen;
	}
	public Date getRegistrationClose() {
		return registrationClose;
	}
	public void setRegistrationClose(Date registrationClose) {
		this.registrationClose = registrationClose;
	}
	public Date getInvoiceDate() {
		return invoiceDate;
	}
	public void setInvoiceDate(Date invoiceDate) {
		this.invoiceDate = invoiceDate;
	}
	public BigDecimal getRegistrationFee() {
		return registrationFee;
	}
	public void setRegistrationFee(BigDecimal registrationFee) {
		this.registrationFee = registrationFee;
	}
	public Date getLastRegistrationDate() {
		return lastRegistrationDate;
	}
	public void setLastRegistrationDate(Date lastRegistrationDate) {
		this.lastRegistrationDate = lastRegistrationDate;
	}
	public BigDecimal getBillableAmount() {
		return registrationFee == null ? null : registrationFee.multiply(new BigDecimal(registrationCount));
	}
}
