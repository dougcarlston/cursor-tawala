package com.tawala.email;

import java.io.IOException;
import java.util.Date;

import javax.mail.Address;
import javax.mail.MessagingException;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;
import javax.persistence.Column;
import javax.persistence.DiscriminatorColumn;
import javax.persistence.DiscriminatorType;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Inheritance;
import javax.persistence.InheritanceType;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.persistence.Transient;

import com.scissor.Log;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_email_id")
@Entity
@Inheritance(strategy = InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(name = "email_type", discriminatorType = DiscriminatorType.STRING, length = 30)
@Table(name = "email")
abstract public class Email {

	public static final int MAX_ERROR_LENGTH = 1000; 
	
	public static enum State {
		READY {
			public String getShortDescription() {
				return "Waiting";
			}
			public String getLongDescription() {
				return "Waiting to be sent.";
			}
		}
		, SENDING {
			public String getShortDescription() {
				return "Sending";
			}
			public String getLongDescription() {
				return "Sending.";
			}
		}, SENT {
			public String getShortDescription() {
				return "Sent";
			}
			public String getLongDescription() {
				return "Sent.";
			}
		}, ERROR {
			public String getShortDescription() {
				return "Failed";
			}
			public String getLongDescription() {
				return "Email delivery failed.";
			}
		}, BOUNCEBACK {
			public String getShortDescription() {
				return "Returned";
			}
			public String getLongDescription() {
				return "Email was returned as undeliverable.";
			}
		};
		
		abstract public String getShortDescription(); 
		abstract public String getLongDescription(); 
	}

	@Id
	@Column(name = "email_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@Column(name = "from_address", length = 500, nullable = false)
	private String from;

	@Column(name = "to_address", length = 1000, nullable = true)
	private String to;

	@Column(name = "cc_address", length = 1000, nullable = true)
	private String cc;

	//--- TODO: add to the database?
	@Transient
	private String bcc;
	
	@Column(name = "subject", length = 1000, nullable = false)
	private String subject;

	@Column(name = "state", length = 10, nullable = false)
	@Enumerated(EnumType.STRING)
	private State state;

	@Column(name = "error_reason", length = MAX_ERROR_LENGTH, nullable = true)
	private String errorReason;

	@Column(name = "cust_error_reason", length = MAX_ERROR_LENGTH, nullable = true)
	private String customerErrorReason;
	
	@Column(name = "create_dt")
	@Temporal(TemporalType.TIMESTAMP)
	private Date createdDate;

	@Column(name = "sent_dt")
	@Temporal(TemporalType.TIMESTAMP)
	private Date sentDate;

	Email() {
		// --- For Hibernate's use
	}

	Email(String from, String to, String cc, String subject) {
		this.from = from;
		this.to = to;
		this.cc = cc;
		this.subject = subject;
		this.createdDate = new Date();
	}

	public String getCc() {
		return cc;
	}

	public void setCc(String cc) {
		this.cc = cc;
	}

	public String getFrom() {
		return from;
	}

	public void setFrom(String from) {
		this.from = from;
	}

	public String getTo() {
		return to;
	}

	public void setTo(String to) {
		this.to = to;
	}

	public long getId() {
		return id;
	}

	public void setId(long id) {
		this.id = id;
	}

	public String getSubject() {
		return subject;
	}

	public void setSubject(String subject) {
		this.subject = subject;
	}

	public State getState() {
		return state;
	}

	public void setState(State state) {
		this.state = state;
	}

	public String getErrorReason() {
		return errorReason;
	}

	public void setErrorReason(String errorReason) {
		this.errorReason = errorReason;
	}

	public MimeMessage toMimeMessage(MimeMessage mimeMessage) throws Exception {
		if (to == null && cc == null) {
			throw new IllegalStateException(
					"Both to and cc addresses are empty.");
		}

		// Server-owned verified From (SMTP / visible sender). Process "From" is Reply-To.
		// Never copy Reply-To display names into SMTP From — aliases may contain '<' '>'
		// (e.g. Signup Sheet Send: "<<Form 1:FirstName>> <<Form 1:LastName>>").
		EmailRuntimeConfig mailConfig = EmailRuntimeConfig.get();
		String smtpAddress = EmailRuntimeConfig.normalizeEmailAddress(mailConfig
				.getFromAddress());
		String smtpName = mailConfig.getFromName();
		InternetAddress smtpFrom;
		if (smtpName == null || smtpName.trim().length() == 0) {
			smtpFrom = new InternetAddress(smtpAddress);
		} else {
			smtpFrom = new InternetAddress(smtpAddress, smtpName.trim());
		}
		mimeMessage.setFrom(smtpFrom);

		// Reply-To is process-supplied and may still contain unresolved field tokens
		// or a malformed/empty address (e.g. "<<Form 1:FirstName>> <<Form 1:LastName>>").
		// Resend hard-rejects (550) any Reply-To with stray '<'/'>' or no valid mailbox,
		// so sanitize down to a bare, validated address (or omit the header entirely).
		Address replyTo = buildSafeReplyTo(from);
		if (replyTo != null) {
			mimeMessage.setReplyTo(new Address[] { replyTo });
		}

		if (to != null) {
			mimeMessage.setRecipients(MimeMessage.RecipientType.TO, to);
		}
		if (cc != null) {
			mimeMessage.setRecipients(MimeMessage.RecipientType.CC, cc);
		}
		if(bcc != null) {
			mimeMessage.setRecipients(MimeMessage.RecipientType.BCC, bcc);
		}

		mimeMessage.setSubject(subject);

		createBody(mimeMessage);

		return mimeMessage;
	}

	abstract protected void createBody(MimeMessage mimeMessage)
			throws MessagingException, IOException;

	/**
	 * Sanitizes a process-supplied "From" value into a safe Reply-To address.
	 * Returns null (omit the header) rather than ever emitting a value Resend
	 * would reject with 550 Invalid reply_to field.
	 */
	static Address buildSafeReplyTo(String from) {
		if (from == null || from.trim().length() == 0) {
			return null;
		}

		String personal = null;
		String address = null;
		try {
			InternetAddress parsed = new InternetAddress(from.trim());
			address = parsed.getAddress();
			personal = parsed.getPersonal();
		} catch (Exception e) {
			// Whole value failed to parse as an RFC822 address (e.g. wholly unresolved
			// field tokens). Fall through with address == null; handled below.
		}

		// Unresolved field tokens ("<<Form 1:FirstName>>") or any stray angle bracket
		// in either part means the value isn't a real display name / address.
		if (personal != null && (personal.indexOf('<') >= 0 || personal.indexOf('>') >= 0)) {
			personal = null;
		}
		if (address != null && (address.indexOf('<') >= 0 || address.indexOf('>') >= 0)) {
			address = null;
		}

		if (address == null || address.trim().length() == 0) {
			Log.warn(Email.class, "Omitting Reply-To: no valid address in '" + from + "'");
			return null;
		}

		try {
			InternetAddress safe = new InternetAddress(address.trim());
			safe.validate();
			if (personal != null && personal.trim().length() > 0) {
				safe.setPersonal(personal.trim());
			}
			return safe;
		} catch (Exception e) {
			Log.warn(Email.class, "Omitting Reply-To: invalid address '" + address
					+ "' derived from '" + from + "'");
			return null;
		}
	}

	public Date getCreatedDate() {
		return createdDate;
	}

	public void setCreatedDate(Date createdDate) {
		this.createdDate = createdDate;
	}

	public Date getSentDate() {
		return sentDate;
	}

	public void setSentDate(Date sentDate) {
		this.sentDate = sentDate;
	}

	public String getCustomerErrorReason() {
		return customerErrorReason;
	}

	public void setCustomerErrorReason(String customerErrorReason) {
		this.customerErrorReason = customerErrorReason;
	}

	public String getBcc() {
		return bcc;
	}

	public void setBcc(String bcc) {
		this.bcc = bcc;
	}
	
	public void markAsSent() {
		setState(Email.State.SENT);
		setSentDate(new Date());
		setErrorReason(null);
		setCustomerErrorReason(null);
	}
}
