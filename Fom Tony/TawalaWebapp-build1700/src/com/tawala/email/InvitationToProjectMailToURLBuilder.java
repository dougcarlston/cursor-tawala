package com.tawala.email;

import java.io.IOException;

import org.springframework.core.io.Resource;

import com.tawala.domain.notification.BaseNotification;
import com.tawala.project.Form;
import com.tawala.project.UserProject;

public class InvitationToProjectMailToURLBuilder extends MailToURLBuilder {
	private static String subjectTemplate;
	private static String bodyTemplate;

	private String subject;
	private String body;

	InvitationToProjectMailToURLBuilder() {
		// --- For Spring purposes
	}

	public InvitationToProjectMailToURLBuilder(UserProject project, Form form) {
		subject = String.format(subjectTemplate, project.getName());
		body = String.format(bodyTemplate, project.getName(), project
				.getUrlToForm(UserProject.EntryPointType.REAL_PROJECT, form),
				project.getUser().getFirstName(), project.getUser()
						.getLastName());
	}

	@Override
	protected String getBody() {
		return body;
	}

	@Override
	protected String getSubject() {
		return subject;
	}

	public void setSubjectTemplate(String subjectTemplate) {
		InvitationToProjectMailToURLBuilder.subjectTemplate = subjectTemplate;
	}

	public void setBodyTemplate(Resource bodyTemplateResource)
			throws IOException {
		InvitationToProjectMailToURLBuilder.bodyTemplate = BaseNotification
				.getResourceContentsAsString(bodyTemplateResource);
	}
}
