package com.tawala.project.builder;

import java.io.StringReader;

import org.dom4j.DocumentException;
import org.dom4j.Element;
import org.dom4j.io.SAXReader;

import com.scissor.XmlBuffer;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.project.Project;

/**
 * A mutable factory for immutable projects
 */
public class ProjectBuilder extends TagBuilder {
	private String projectThemePath;
	private Forms forms = new Forms();
	private Documents documents = new Documents();
	private Processes processes = new Processes();
	private String projectName = "projectname";
	private String format = "1.3";
	private String designerVersion;
	private PageHeaderBuilder pageHeaderBuilder;

	public String getFormat() {
		return format;
	}

	public void setFormat(String format) {
		this.format = format;
	}

	public ProjectBuilder() {
		this("default");
	}

	public ProjectBuilder(String projectThemePath) {
		this.projectThemePath = projectThemePath;
		add(forms);
		add(processes);
		add(documents);
	}

	public Project build() {
		ConfigElement config = asConfig();
		Project project = new Project(config);
		config.dumpUnused();
		return project;
	}

	public Project build(String projectName) {
		this.projectName = projectName;
		return build();
	}

	protected void startTag(XmlBuffer xml) {
		xml
				.startTag("project", "name", projectName, "format", format,
						"themePath", projectThemePath, "designerBuild",
						designerVersion);
	}

	protected void endTag(XmlBuffer xml) {
		xml.endTag("project");
	}

	public FormBuilder addForm(String name) {
		FormBuilder form = new FormBuilder(name);
		forms.add(form);
		return form;
	}

	public FormBuilder addForm(String name, Boolean isStartingPoint) {
		FormBuilder form = new FormBuilder(name, isStartingPoint);
		forms.add(form);
		return form;
	}

	public FormBuilder addForm(String name, Boolean isStartingPoint,
			Boolean isDataEntryOnly) {
		FormBuilder form = new FormBuilder(name, isStartingPoint,
				isDataEntryOnly);
		forms.add(form);
		return form;
	}

	public FormBuilder addForm(String name, Boolean isStartingPoint,
			String themePath) {
		FormBuilder form = new FormBuilder(name, isStartingPoint, themePath);
		forms.add(form);
		return form;
	}

	public FormBuilder addForm(String name, ProcessBlockBuilder process) {
		FormBuilder form = addForm(name);
		form.setPostProcess(process);
		return form;
	}

	public com.tawala.project.builder.ProcessBlockBuilder addProcess(String name) {
		com.tawala.project.builder.ProcessBlockBuilder process = new ProcessBlockBuilder(
				name);
		processes.add(process);
		return process;
	}

	public DocumentBuilder addDocument(String name) {
		DocumentBuilder document = new DocumentBuilder(name);
		documents.add(document);
		return document;
	}

	public DocumentBuilder addDocument(String docId, String text) {
		DocumentBuilder docBuilder = new DocumentBuilder(docId, text);
		documents.add(docBuilder);
		return docBuilder;
	}

	public static Project newProject(String xml) {
		return newProject(TestCase.parseConfig(xml));
	}

	public static Project newProject(ConfigElement config) {
		Project project = new Project(config);
		config.dumpUnused();
		return project;
	}

	public static Project buildMinimalisticProject() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addForm("Form1").addText("Hello, World!");
		return projectBuilder.build();
	}

	public static Project addForm(Project project, String formText) {
		String elementParentName = "forms";
		return addElementInPlace(project, formText, elementParentName);
	}

	public static Project addProcess(Project project, String processText) {
		String elementParentName = "processes";
		return addElementInPlace(project, processText, elementParentName);
	}

	public static Project addDocument(Project project, String documentText) {
		String elementParentName = "processes";
		return addElementInPlace(project, documentText, elementParentName);
	}

	private static Project addElementInPlace(Project project,
			String elementText, String elementParentName)
			throws IllegalStateException, IllegalArgumentException {
		Element projectXML = project.getXml();
		Element parent = projectXML.element(elementParentName);
		if (parent == null) {
			throw new IllegalStateException("Unable to find <"
					+ elementParentName + "> element in "
					+ projectXML.asXML());
		}

		if (!elementText.startsWith("<?xml version=\"1.0\" ")) {
			elementText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
					+ elementText;
		}

		SAXReader xmlReader = new SAXReader();
		Element form;
		try {
			form = xmlReader.read(new StringReader(elementText))
					.getRootElement();
		} catch (DocumentException e) {
			throw new IllegalArgumentException("Unable to parse form:", e);
		}
		parent.add(form);

		return new Project(new ConfigElement(projectXML));
	}

	/**
	 * Parent class for uninteresting container tags
	 */
	private abstract static class SimpleTagBuilder extends TagBuilder {

		protected void startTag(XmlBuffer xml) {
			xml.startTag(tag());
		}

		protected abstract String tag();

		protected void endTag(XmlBuffer xml) {
			xml.endTag(tag());
		}
	}

	private static class Forms extends SimpleTagBuilder {
		protected String tag() {
			return "forms";
		}
	}

	private static class Processes extends SimpleTagBuilder {
		protected String tag() {
			return "processes";
		}
	}

	private static class Documents extends SimpleTagBuilder {
		protected String tag() {
			return "documents";
		}
	}

	public String toStringAsElement(String projectName) {
		this.projectName = projectName;
		return super.toStringAsElement();
	}

	public PageHeaderBuilder addPageHeader() {
		if (pageHeaderBuilder == null) {
			pageHeaderBuilder = new PageHeaderBuilder();
			add(pageHeaderBuilder);
		}

		return pageHeaderBuilder;
	}

	public void setDesignerVersion(String version) {
		this.designerVersion = version;
	}
}
