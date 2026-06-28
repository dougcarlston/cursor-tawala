package com.tawala.email;

import java.io.IOException;
import java.util.Map;

import org.springframework.core.io.Resource;

import com.tawala.domain.notification.BaseNotification;
import com.tawala.project.Form;
import com.tawala.project.UserProject;

public class DefaultCustomizationMailToURLBuilder extends MailToURLBuilder {
	private static String subjectTemplate;
	private static String bodyTemplate;
	
	@SuppressWarnings("unused")
	private String subject;
	private String body;

	DefaultCustomizationMailToURLBuilder() {
		// To be used by Spring.
	}
	
	public DefaultCustomizationMailToURLBuilder(UserProject project, String projectName) {
		Map<Form, String> projectUrls = project.getEntryPointURLsWithoutCustomizerForm();
		if(projectUrls.size() != 1) {
			throw new IllegalArgumentException("The project must have exactly one entry point besides customization.");
		}
		
		String url = projectUrls.values().iterator().next();
		
		subject = String.format(subjectTemplate, projectName);
		body = String.format(bodyTemplate, projectName, url);
	}
	
	@Override
	protected String getSubject() {
		//--- For some reason the subject wasn't desired
		return null;
	}
	
	@Override
	protected String getBody() {
		return body;
	}
	
	public void setSubjectTemplate(String subjectTemplate) {
		DefaultCustomizationMailToURLBuilder.subjectTemplate = subjectTemplate;
	}
	
	public void setBodyTemplate(Resource bodyTemplateResource) throws IOException {
		DefaultCustomizationMailToURLBuilder.bodyTemplate = BaseNotification.getResourceContentsAsString(bodyTemplateResource);
	}
}
