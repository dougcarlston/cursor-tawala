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

        InternetAddress tawalaSender = new InternetAddress("sportsdashboards@tawala.com", from);
		InternetAddress fromAddress = new InternetAddress(from);
        //mimeMessage.setSender(tawalaSender);
		mimeMessage.setFrom(tawalaSender);
        mimeMessage.setReplyTo(new Address[] {fromAddress});

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
