package com.tawala.email;

import java.io.IOException;
import java.text.MessageFormat;
import java.util.Date;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

@Entity
@DiscriminatorValue("sdb-contact-us")
public class SportsDashboardContactUsEmail extends UniqueBodyEmail {
	public static class EmailTemplate {
		private String from;
		private String toAddress;
		private String ccAddress;
		private String subject;
		private String messageTemplate;
		private String sport;

		public String getCcAddress() {
			return ccAddress;
		}

		public void setCcAddress(String ccAddress) {
			this.ccAddress = ccAddress;
		}

		public String getMessageTemplate() {
			return messageTemplate;
		}

		public void setMessageTemplate(String messageTemplate) {
			this.messageTemplate = messageTemplate;
		}

		public String getSubject() {
			return subject;
		}

		public void setSubject(String subject) {
			this.subject = subject;
		}

		public String getToAddress() {
			return toAddress;
		}

		public void setToAddress(String toAddress) {
			this.toAddress = toAddress;
		}

		public String getFrom() {
			return from;
		}

		public void setFrom(String from) {
			this.from = from;
		}

		public String getSport() {
			return sport;
		}

		public void setSport(String sport) {
			this.from = sport;
		}
	}

	private static SportsDashboardContactUsEmail.EmailTemplate emailTemplate;

	public void setEmailTemplate(
			SportsDashboardContactUsEmail.EmailTemplate template) {
		emailTemplate = template;
	}

	SportsDashboardContactUsEmail() {
		// -- For Hibernate's use
	}

	public SportsDashboardContactUsEmail(String name, String URL, String email, String sport)
			throws IOException {
		super(emailTemplate.getFrom(), emailTemplate.getToAddress(),
				emailTemplate.getCcAddress(), emailTemplate.getSubject(),
				UniqueBodyEmail.Type.TEXT, formatBody(name, URL, email, sport));
	}

	private static String formatBody(String name, String URL, String email, String sport) {
		return MessageFormat.format(emailTemplate.getMessageTemplate(),
				new Object[] { name, email, sport, URL, new Date()});
	}
}
