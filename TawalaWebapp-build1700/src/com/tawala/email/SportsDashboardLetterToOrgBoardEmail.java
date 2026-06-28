package com.tawala.email;

import java.io.IOException;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.text.MessageFormat;
import java.util.List;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.scissor.ImpossibleException;
import com.tawala.project.UserProject;
import com.tawala.web.oldhtml.HtmlString;

@Entity
@DiscriminatorValue("sdb-letter-to-board")
public class SportsDashboardLetterToOrgBoardEmail extends UniqueBodyEmail {
	public static class EmailTemplate {
		private String subject;
		private String messageTemplate;
		private String bcc;

		public String getBcc() {
			return bcc;
		}

		public void setBcc(String bcc) {
			this.bcc = bcc;
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
	}

	private static SportsDashboardLetterToOrgBoardEmail.EmailTemplate emailTemplate;

	public void setEmailTemplate(
			SportsDashboardLetterToOrgBoardEmail.EmailTemplate template) {
		emailTemplate = template;
	}

	SportsDashboardLetterToOrgBoardEmail() {
		// -- For Hibernate's use
	}

	public SportsDashboardLetterToOrgBoardEmail(String from,
			List<String> toAddresses, String messageText) throws IOException {
		super(from, convertAddressesToList(toAddresses), null, emailTemplate
				.getSubject(), UniqueBodyEmail.Type.HTML,
				formatBody(messageText));
		setBcc(emailTemplate.getBcc());
	}

	private static String convertAddressesToList(List<String> toAddresses) {
		StringBuilder result = new StringBuilder(toAddresses.get(0));
		for (int i = 1; i < toAddresses.size(); i++) {
			String nextAddress = toAddresses.get(i);
			if (nextAddress.trim().length() == 0) {
				continue;
			}
			result.append(", ").append(nextAddress);
		}
		return result.toString();
	}

	private static String formatBody(String messageText) {
		StringWriter preparedMessage = new StringWriter();
		PrintWriter printWriter = new PrintWriter(
						preparedMessage);
		HtmlString.printHTMLPreparedText(messageText, printWriter);
		printWriter.close();
		try {
			preparedMessage.close();
		} catch (IOException e) {
			throw new ImpossibleException(e);
		}

		return MessageFormat.format(emailTemplate.getMessageTemplate(),
				new Object[] { preparedMessage.toString(),
						"http://" + UserProject.getWebsiteHostName() });
	}
}
