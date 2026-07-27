package com.tawala.project.formatting;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Form;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.UserProject;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.Link;

public class LinkToProject extends FormRenderableNotHoldingActiveComponents {
	public static final String DISPLAY_TEXT_ELEMENT_NAME = "displayText";
	private String projectName;
	private final String formName;
	private final String description;
	private final StringConcatenationExpression descriptionExpression;
	private final boolean privateInvitation;
	private final StringConcatenationExpression authenticationTokenExpression;
	private final boolean useOnce;

	public LinkToProject(ConfigElement element) {
		this.projectName = element.attribute("project").stringValue();
		if (this.projectName != null && this.projectName.length() == 0) {
			this.projectName = null;
		}

		this.formName = element.attribute("form").stringValue();
		if(element.hasChild(DISPLAY_TEXT_ELEMENT_NAME)) {
			this.description = null;
			this.descriptionExpression = new StringConcatenationExpression(element.child(DISPLAY_TEXT_ELEMENT_NAME));
		} else {
			this.description = element.text().trim();
			this.descriptionExpression = null;
		}
		this.privateInvitation = element.attribute("private").booleanValue();
		this.authenticationTokenExpression = new StringConcatenationExpression(
				element.child("authenticationTokenValue"));
		this.useOnce = element.attribute("onlyOneAccess").booleanValue();
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see com.tawala.project.FormRenderable#toHtml(com.tawala.project.commands.ExecutionContext)
	 */
	public Html toHtml(ExecutionContext context) {
		final String descriptionText = description == null ? descriptionExpression.evaluate(context) : description;
		if (formName == null) {
			Log.warn(this, "Form name is null");
			return displayInvalidLinkHtml(descriptionText);
		}

		UserProject userProject = null;
		if (projectName == null) {
			userProject = context.getUserProject();
		} else {
			userProject = context.getDomain().projects().get(
					context.getUserProject().getUser().getId(), projectName);
			if (userProject == null) {
				Log.warn(this, "Can't find project for "
						+ context.getUserProject().getUser().getId()
						+ " named '" + projectName + "'");
				return displayInvalidLinkHtml(descriptionText);
			}
		}
		Form form = userProject.getProject().getForm(formName);
		if (form == null) {
			Log.warn(this, "Form named '" + formName
					+ "' doesn't exist in project #"
					+ userProject.getProject().getId());
			return displayInvalidLinkHtml(descriptionText);
		}

		String url = privateInvitation ? generateUrlForPrivateInvitation(
				context, userProject, form)
				: (projectName == null ? generateUrlForPublicInvitationToCurrentProject(
						context, userProject, form)
						: generateUrlForPublicInvitationToAnotherProject(
								context, userProject, form));

		Link link = new Link(url, descriptionText, false);
		return link;
	}

	private String generateUrlForPrivateInvitation(ExecutionContext context,
			UserProject userProject, Form form) {
		String authenticationToken = authenticationTokenExpression
				.evaluate(context);
		LinkToUserProject linkToUserProject = new LinkToUserProject(
				userProject, authenticationToken);
		linkToUserProject.setUseOnce(this.useOnce);
		context.getDomain().projects().addLinkToProject(linkToUserProject);

		// --- The assumption is that we never produce private links with
		// overriding theme.
		return userProject.getUrlToForm(linkToUserProject, context
				.getEntryPointType(), form);
	}

	private String generateUrlForPublicInvitationToCurrentProject(
			ExecutionContext context, UserProject userProject, Form form) {
		return userProject.getUrlToForm(context.getLink().getId(), context
				.getEntryPointType(), form, context.getOverridingTheme(),
				context.isAdsExplicitlySurpressed());
	}

	private String generateUrlForPublicInvitationToAnotherProject(
			ExecutionContext context, UserProject userProject, Form form) {
		return userProject.getUrlToForm(userProject.getUniqueRandomId(),
				context.getEntryPointType(), form,
				context.getOverridingTheme(), context
						.isAdsExplicitlySurpressed());
	}

	private Html displayInvalidLinkHtml(String descriptionText) {
		return new Link("javascript:alert('This link is invalid. We apologize for inconvenience.')", descriptionText, false);
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}
}
