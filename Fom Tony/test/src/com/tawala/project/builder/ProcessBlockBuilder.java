package com.tawala.project.builder;

public class ProcessBlockBuilder extends ProcessBuilder {
	private String name;

	public ProcessBlockBuilder(String name) {
		super(Type.process);
		this.name = name;
	}

	protected ProcessBlockBuilder(Type type) {
		super(type);
		name = null;
	}

	protected String[] tagAttributes() {
		if (name != null) {
			return new String[] { "name", name };
		} else {
			return super.tagAttributes();
		}
	}

	public ProcessBuilder makeChild(Type type) {
		return new ProcessBlockBuilder(type);
	}

	public void addSend(String toAddress, String ccAddress, String subject,
			String body) {
		contents().startTag("send");
		contents().tag("to", true, "addressField", toAddress);
		if (ccAddress.length() > 0) {
			contents().tag("cc", true, "addressLiteral", ccAddress);
		}
		contents().startTag("subject");
		contents().text(subject);
		contents().endTag("subject");
		addBody(body);
		contents().endTag("send");
	}

	public void addSend(String toAddress, String ccAddress, String subject,
			DocumentBuilder body) {
		addSend(toAddress, ccAddress, subject, body, false, true);
	}

	public void addSend(String toAddress, String ccAddress, String subject,
			DocumentBuilder body, boolean resetDocument, boolean showHeader) {
		contents().startTag("send");
		contents().tag("to", true, "addressField", toAddress);
		if (ccAddress.length() > 0) {
			contents().tag("cc", true, "addressLiteral", ccAddress);
		}
		contents().startTag("subject");
		contents().text(subject);
		contents().endTag("subject");
		addBody(body, resetDocument, showHeader);
		contents().endTag("send");
	}

	private void addBody(DocumentBuilder doc, boolean resetDocument,
			boolean showHeader) {
		contents().tag("body", true, "document", doc.getName(), "reset",
				Boolean.toString(resetDocument), "showHeader",
				Boolean.toString(showHeader));
	}

	private void addBody(String body) {
		contents().startTag("body");
		contents().text(body);
		contents().endTag("body");
	}

	/*
	 * public void addShow(String document) { contents().tag("show", "document",
	 * document); }
	 * 
	 * public void addShow(DocumentBuilder document) {
	 * addShow(document.getName()); }
	 */

	public void addShow(FormBuilder form) {
		contents().tag("show", "form", form.getName());
	}

	public void addAppend(DocumentBuilder document1, DocumentBuilder document2) {
		addAppend(document1.getName(), document2.getName());
	}

	public void addAppend(String document1, DocumentBuilder document2) {
		addAppend(document1, document2.getName());
	}

	public void addAppend(DocumentBuilder document1, String document2) {
		addAppend(document1.getName(), document2);
	}

	public void addAppend(String doc1, String doc2) {
		contents().tag("append", "document", doc1, "appendage", doc2);
	}

	public String getName() {
		return name;
	}

	public void addShowURLAsStringValue(String URL) {
		contents().startTag("show");
		contents().startTag("url");
		contents().tag("string", "value", URL);
		contents().endTag("url");
		contents().endTag("show");
	}
}
