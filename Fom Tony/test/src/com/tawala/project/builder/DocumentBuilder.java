package com.tawala.project.builder;

import com.scissor.XmlBuffer;
import com.tawala.project.formatting.LinkToProject;

public class DocumentBuilder extends TagBuilder {
	private String name;
	private boolean startedOutput = false;

	public DocumentBuilder(String name) {
		this.name = name;
	}

	public DocumentBuilder(String name, String text) {
		this(name);
		addText(text);
	}

	protected void endTag(XmlBuffer xml) {
		if (startedOutput) {
			xml.endTag("font", false);
			xml.endTag("paragraph", false);
		}
		xml.endTag("xmlData");
		xml.endTag("document");
	}

	protected void startTag(XmlBuffer xml) {
		xml.startTag("document", false, "name", name);
		xml.startTag("xmlData", false);
	}

	public void addText(String text) {
		addFormattingElements();
		contents().text(text);
	}

	public void addOldStyleInvitation(String description, String formName) {
		addOldStyleInvitation(description, "", formName);
	}

	public void addOldStyleInvitation(String description, String projectName,
			String formName) {
		addFormattingElements();
		contents().startTag("invitation", false, "project", projectName,
				"form", formName);
		contents().text(description);
		contents().endTag("invitation");
	}

	public void addOldStylePrivateInvitation(String description, String projectName,
			String formName, boolean useOnce,
			String authenticationTokenExpression) {
		addFormattingElements();
		contents().startTag("invitation", false, "project", projectName,
				"form", formName, "private", "true", "onlyOneAccess",
				Boolean.toString(useOnce));
		contents().startTag("authenticationTokenValue", true);
		contents().preformattedXml(authenticationTokenExpression);
		contents().endTag("authenticationTokenValue", false);
		contents().text(description);
		contents().endTag("invitation");
	}

	public void addComponent(ComponentBuilder componentBuilder) {
		addFormattingElements();
		componentBuilder.render(contents());
	}

	private void addFormattingElements() {
		if (!startedOutput) {
			contents().startTag("paragraph", false);
			contents().startTag("font", false);
			startedOutput = true;
		}
	}

	public void addField(String id) {
		addFormattingElements();
		contents().tag("field", false, "name", id);
	}

	public void addImage(ImageInstanceBuilder imageInstanceBuilder) {
		imageInstanceBuilder.render(contents());
	}

	public String getName() {
		return name;
	}

	public void addLink(String description, String url, boolean showInNewWindow) {
		addLink(description, url, showInNewWindow, null);
	}

	public void addLink(String description, String url, boolean showInNewWindow, ConditionsBuilder displayConditions) {
		addFormattingElements();
		contents().startTag("link");
		contents().startTag("description");
		contents().tag("string", "value", description);
		contents().endTag("description");
		contents().startTag("url");
		contents().tag("string", "value", url);
		contents().endTag("url");
		if (showInNewWindow) {
			contents().tag("new-window", true);
		}
		if(displayConditions != null) {
			displayConditions.setOmitEnclosingTag(true);
        	contents().startTag("displayConditions");
        	add(displayConditions);
        	contents().endTag("displayConditions");
		}
		contents().endTag("link");
	}

	public void addNewStyleInvitation(String text, String formName) {
		addFormattingElements();
		contents().startTag("invitation", false, "project", "",
				"form", formName);
		contents().startTag(LinkToProject.DISPLAY_TEXT_ELEMENT_NAME);
		contents().preformattedXml(text);
		contents().endTag(LinkToProject.DISPLAY_TEXT_ELEMENT_NAME);
		contents().endTag("invitation");
	}
}
