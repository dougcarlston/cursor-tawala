package com.tawala.email;

import java.io.IOException;
import java.util.Date;

import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;

import com.tawala.project.UserProject;

@Entity
@DiscriminatorValue("user-project")
public class UserProjectEmail extends UniqueBodyEmail {
	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "user_project_id", nullable = true)
	private UserProject userProject;

	UserProjectEmail() {
		// -- For Hibernate's use
	}

	public UserProjectEmail(UserProject userProject, String from, String to,
			String cc, String subject, Type type, String body) throws IOException {
		super(from, to, cc, subject, type, body);
		this.userProject = userProject;
	}

	//--- Used by a Hibernate query in EmailService
	public UserProjectEmail(long id, String from, String to, String cc,
			String subject, Email.State state, String customerErrorReason,
			String errorReason, Date createdDate, Date sentDate) {
		setId(id);
		setFrom(from);
		setTo(to);
		setCc(cc);
		setSubject(subject);
		setState(state);
		setCustomerErrorReason(customerErrorReason);
		setErrorReason(errorReason);
		setCreatedDate(createdDate);
		setSentDate(sentDate);
	}

	public UserProject getUserProject() {
		return userProject;
	}

	void setUserProject(UserProject userProject) {
		this.userProject = userProject;
	}
}
