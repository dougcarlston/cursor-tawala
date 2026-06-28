package com.tawala.domain.notification;

import java.text.MessageFormat;
import java.util.Map;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.core.io.Resource;

import com.tawala.project.Form;
import com.tawala.project.UserProject;

public class ProjectLinksMessage extends BaseNotification {
	public static StaticParameters staticParameters;
	private final UserProject project;
	private final String projectName;

	public ProjectLinksMessage(String to, UserProject project, String projectName) {
		super(to);
		this.project = project;
		this.projectName = projectName;
	}

	@Override
	protected String getText() {
		StringBuilder links = new StringBuilder();
		for (Map.Entry<Form, String> entryPoint : project.getEntryPointURLs().entrySet()) {
			links.append(entryPoint.getKey().getName()).append(": ").append(entryPoint.getValue()).append("\n");
		}
		
		String result = MessageFormat.format(staticParameters
				.getMessageTemplateText(), new Object[] {projectName, links});
		return result;
	}

	public static class StaticParameters extends
			BaseNotification.BaseParameters implements InitializingBean {

		public void afterPropertiesSet() throws Exception {
			ProjectLinksMessage.staticParameters = this;
		}
	}

	@Override
	protected Resource getLogoImage() {
		return staticParameters.getLogoImage();
	}

	@Override
	protected String getSubject() {
		return staticParameters.getSubject();
	}

	@Override
	protected String getFrom() {
		return staticParameters.getFrom();
	}
	
	@Override
	protected boolean isHtmlMessage() {
		return false;
	}
}
