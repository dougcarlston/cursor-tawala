package com.tawala.project;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.ListIterator;
import java.util.Map;
import java.util.Properties;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Embedded;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Lob;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.persistence.Transient;

import org.dom4j.Attribute;
import org.dom4j.DocumentFactory;
import org.dom4j.Element;
import org.hibernate.annotations.AccessType;
import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.scissor.xmlconfig.Format;
import com.tawala.UsersHibernateImpl;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.caching.ProjectRuntimeCache;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.ExecutionResult;
import com.tawala.project.data.DataSource;
import com.tawala.project.theme.CommonTheme;
import com.tawala.project.theme.ProjectTheme;
import com.tawala.project.theme.UserDefinedTheme;
import com.tawala.util.RandomTokenGenerator;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_project_id")
@Entity
@Table(name = "project")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "projects")
public class Project {
	private static final String FORM_NAME_LIST_DELIMITER = ":";
	private static final String THEME_PATH_ATTRIBUTE_NAME = "themePath";
	public static final Format ONE_DOT_THREE = new Format("1.3");
	public static final Format MINIMUM_SAFE = ONE_DOT_THREE;
	public static final Format CURRENT_VERSION = new Format("1.4");

	private static final String FORM_NAME_TO_RANDOM_TOKEN_PREFIX = "form.to.token.";
	private static final String RANDOM_TOKEN_TO_FORM_NAME_PREFIX = "token.to.form";

	private static final Factory<Form> FORM_FACTORY = new Factory<Form>();
	private static final Factory<Document> DOCUMENT_FACTORY = new Factory<Document>();
	private static final Factory<Process> PROCESS_FACTORY = new Factory<Process>();
	private static final Factory<Image> IMAGE_FACTORY = new Factory<Image>();
	private static final Factory<DataSource> DATA_SOURCE_FACTORY = new Factory<DataSource>();

	static {
		FORM_FACTORY.register("form", Form.class);
		DOCUMENT_FACTORY.register("document", Document.class);
		PROCESS_FACTORY.register("process", Process.class);
		IMAGE_FACTORY.register("imagedef", Image.class);

		DATA_SOURCE_FACTORY.register("datasource", DataSource.class);
	}

	public static final Set<String> SETUP_WIZARD_FORM_NAMES = new HashSet<String>();
	static {
		SETUP_WIZARD_FORM_NAMES.add("Setup");
		SETUP_WIZARD_FORM_NAMES.add("Customize");
		SETUP_WIZARD_FORM_NAMES.add("setup");
		SETUP_WIZARD_FORM_NAMES.add("customize");
	}

	public static final Set<String> ADMINISTRATION_FORM_NAMES = new HashSet<String>();
	static {
		ADMINISTRATION_FORM_NAMES.add("Administration");
		ADMINISTRATION_FORM_NAMES.add("administration");
	}

	public static final Set<String> CUSTOMIZATION_PREVIEW_FORM_NAMES = new HashSet<String>();
	static {
		CUSTOMIZATION_PREVIEW_FORM_NAMES.add("CustomizationPreview");
	}

	@Id
	@Column(name = "project_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@Embedded
	private Format format;

	@Column(name = "designer_version")
	private String designerVersion;

	@Transient
	private PageHeader pageHeader;

	@Transient
	private final List<Form> forms = new ArrayList<Form>();

	@Transient
	private final List<Document> documents = new ArrayList<Document>();

	@Transient
	private final List<Process> processes = new ArrayList<Process>();

	@Transient
	private final Map<String, Image> images = new HashMap<String, Image>();

	@Transient
	private List<DataSource> dataSources;

	@Column(name = "project_definition", nullable = false)
	@Lob
	@AccessType("property")
	private String projectXmlDefinition;

	@Column(name = "theme_path", length = 60, nullable = true)
	private String themePath;

	@SuppressWarnings("unused")
	@Column(name = "created_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date createdDate = new Date();

	@Transient
	private boolean invalid = false;

	@Transient
	private boolean needToDeserizalize = false;

	@Column(name = "properties", nullable = true)
	@Lob
	@AccessType("property")
	private String propertiesAsString;

	@Transient
	private Properties properties;

	@Transient
	private boolean instantiatePropertiesWithLegacyFormNames = false;
	// --- this is done to bypass PayPal's inability to handle URLs that don't
	// end in .jpg or .gif
	// --- the actual MIME type of the image is served when the image data is
	// loaded.
	public static final String IMAGE_SUFFIX = ".jpg";

	private static final String SELECTED_FORM_NAMES_PROPERTY = "selected.form.names";
	private static final String SHOW_SELECTED_FORMS_BY_DEFAULT = "show.selected.forms";

	// TODO: version handling
	// TODO: how to handle warnings?

	Project() {
		// For Hibernate's use
	}

	Project(Format format) {
		this.format = format;
	}

	public Project(ConfigElement config) {
		Element xml = deserializeConfig(config);
		serializeProjectDefinition(xml);
	}

	/**
	 * 
	 */
	private void serializeProjectDefinition(Element xml) {
		projectXmlDefinition = DocumentFactory.getInstance().createDocument(
				xml.createCopy()).asXML();
	}

	/**
	 * @param config
	 * @throws NumberFormatException
	 */
	private Element deserializeConfig(ConfigElement config)
			throws NumberFormatException {
		// TODO: if designer attribute is not used it should be removed.
		// This code exists to supress warnings in test cases.
		config.attribute("designer");
		if (themePath == null) {
			if (config.attribute(THEME_PATH_ATTRIBUTE_NAME).stringValue() != "") {
				this.themePath = config.attribute(THEME_PATH_ATTRIBUTE_NAME)
						.stringValue();
			} else {
				this.themePath = "default";
			}
		}
		this.format = new Format(config.attribute("format").stringValue());
		this.designerVersion = config.attribute("designerBuild").stringValue();

		List<Form> newForms = FORM_FACTORY.makeChildren(config.child("forms"));
		for (Form form : newForms) {
			form.setProject(this);
		}
		forms.addAll(newForms);
		documents.addAll(DOCUMENT_FACTORY.makeChildren(config
				.child("documents")));
		processes.addAll(PROCESS_FACTORY
				.makeChildren(config.child("processes")));

		for (Image image : IMAGE_FACTORY.makeChildren(config.child("images"))) {
			images.put(image.getId(), image);
		}

		dataSources = DATA_SOURCE_FACTORY.makeChildren(config
				.child("datasources"));

		ConfigElement headerElement = config.child("pageHeader");
		if (headerElement != null) {
			pageHeader = new PageHeader(headerElement);
		}

		return config.getRawElement();
	}

	// TODO: These three methods must NOT be used in production code; they break
	// persistence.
	public void add(Document doc) {
		documents.add(doc);
	}

	public void add(Form form) {
		forms.add(form);
	}

	public void add(Process process) {
		processes.add(process);
	}

	public List<Form> getForms() {
		doDeserializeIfNeeded();

		return Collections.unmodifiableList(forms);
	}

	public List<Document> getDocuments() {
		doDeserializeIfNeeded();

		return Collections.unmodifiableList(documents);
	}

	public List<Process> getProcesses() {
		doDeserializeIfNeeded();

		return Collections.unmodifiableList(processes);
	}

	public Document getDocument(String documentName) {
		doDeserializeIfNeeded();

		for (Document document : documents) {
			if (documentName.equals(document.getName()))
				return document;
		}
		return null;
	}

	public Form defaultForm() {
		doDeserializeIfNeeded();

		if (forms.isEmpty())
			return null;
		return forms.get(0);
	}

	public Process getProcess(String processName) {
		doDeserializeIfNeeded();

		for (Process process : processes) {
			if (process.getName().equals(processName))
				return process;
		}
		return null;
	}

	public ExecutionResult executeProcess(ExecutionContext context,
			String processName) {
		if (processName == null) {
			return ExecutionResult.NULL;
		} else {
			Process process = getProcess(processName);
			if (process == null) {
				Log.error(this, "Unable to find process named '" + processName
						+ "'.");
				return ExecutionResult.NULL;
			} else {
				return process.execute(context);
			}
		}
	}

	public Form getForm(String formName) {
		if (formName == null) {
			return null;
		}

		doDeserializeIfNeeded();

		for (Form form : forms) {
			if (formName.equals(form.getName()))
				return form;
		}
		return null;
	}

	public Format getFormat() {
		return format;
	}

	public Element getXml() {
		doDeserializeIfNeeded();
		return new ConfigElement(getProjectXmlDefinition()).getRawElement();
	}

	public String getProjectXmlDefinition() {
		return projectXmlDefinition;
	}

	public void setProjectXmlDefinition(String projectXmlDefinition) {
		forms.clear();
		documents.clear();
		images.clear();
		processes.clear();

		this.projectXmlDefinition = projectXmlDefinition;
		this.needToDeserizalize = true;
	}

	public void doDeserializeIfNeeded() {
		if (needToDeserizalize) {
			needToDeserizalize = false;
			try {
				long start = System.currentTimeMillis();
				deserializeConfig(new ConfigElement(projectXmlDefinition));
				Log.info(this, "Deserialized project #" + getId() + " in "
						+ (System.currentTimeMillis() - start) + " msecs.");
			} catch (Throwable e) {
				Log.error(this, "Error deserializing project #" + getId()
						+ FORM_NAME_LIST_DELIMITER, e);
				invalid = true;
			}
		}
	}

	public ProjectTheme getTheme() {
		if (themePath == null) {
			return CommonTheme.DEFAULT_THEME;
		}
		if (Character.isDigit(themePath.charAt(0))) {
			UserDefinedTheme theme = UsersHibernateImpl.getUserThemeById(Long
					.parseLong(themePath));
			if (theme == null) {
				Log.error(this, "Unable to find user defined theme: '"
						+ themePath + "'");
				return CommonTheme.DEFAULT_THEME;
			} else {
				return theme;
			}
		} else {
			CommonTheme theme = CommonTheme.getThemeByPath(themePath);
			if (theme == null) {
				Log.error(this, "Unable to find common theme: '" + themePath
						+ "'");
				theme = CommonTheme.DEFAULT_THEME;
			}
			return theme;
		}
	}

	public long getId() {
		return id;
	}

	public void setId(long id) {
		this.id = id;
	}

	@Override
	public boolean equals(Object obj) {
		// TODO: take another look at it.
		Project other = (Project) obj;
		return this.getId() == other.getId()
		/*
		 * &&
		 * this.getProjectXmlDefinition().equals(other.getProjectXmlDefinition
		 * ())
		 */;
	}

	public Image getImage(String id) {
		doDeserializeIfNeeded();

		Image result = images.get(id);
		if (result == null && id.endsWith(Project.IMAGE_SUFFIX)) {
			result = images.get(id.substring(0, id.length()
					- Project.IMAGE_SUFFIX.length()));
		}
		return result;
	}

	public Project makeCopy() {
		doDeserializeIfNeeded();
		return new Project(new ConfigElement(getProjectXmlDefinition()));
	}

	@Override
	public String toString() {
		return "Project #" + id;
	}

	public int getStartingPointCount() {
		doDeserializeIfNeeded();
		int result = 0;
		for (Form form : forms) {
			if (form.isStartingPoint()) {
				++result;
			}
		}
		return result;
	}

	public Form getCustomizerForm() {
		return getFormNamed(SETUP_WIZARD_FORM_NAMES);
	}

	public Form getAdministrationForm() {
		return getFormNamed(ADMINISTRATION_FORM_NAMES);
	}

	private Form getFormNamed(Set<String> nameSet) {
		for (String formName : nameSet) {
			Form form = getForm(formName);
			if (form != null && form.isStartingPoint()) {
				return form;
			}
		}
		return null;
	}

	public Form getCustomizationPreviewForm() {
		Form result = getFormNamed(CUSTOMIZATION_PREVIEW_FORM_NAMES);
		if (result != null) {
			return result;
		}
		return getSingleNonCustomizerStartingPoint();
	}

	public Form getSingleNonCustomizerStartingPoint() {
		doDeserializeIfNeeded();
		for (Form form : forms) {
			if (form.isStartingPoint()
					&& !SETUP_WIZARD_FORM_NAMES.contains(form.getName())
					&& !ADMINISTRATION_FORM_NAMES.contains(form.getName())
					&& !CUSTOMIZATION_PREVIEW_FORM_NAMES.contains(form
							.getName())) {
				return form;
			}
		}
		throw new IllegalStateException(
				"Unable to find a starting point form that is not customizable");
	}

	public List<Form> getUserStartingPoints() {
		doDeserializeIfNeeded();
		List<Form> result = new ArrayList<Form>();
		for (Form form : forms) {
			if (form.isStartingPoint()
					&& !SETUP_WIZARD_FORM_NAMES.contains(form.getName())
					&& !ADMINISTRATION_FORM_NAMES.contains(form.getName())
					&& !CUSTOMIZATION_PREVIEW_FORM_NAMES.contains(form
							.getName())) {
				result.add(form);
			}
		}
		return result;
	}

	public List<Form> getFormsSuitableForTestDrive() {
		doDeserializeIfNeeded();
		List<Form> result = new ArrayList<Form>();
		for (Form form : forms) {
			if (form.isStartingPoint()
					&& !SETUP_WIZARD_FORM_NAMES.contains(form.getName())
					&& !CUSTOMIZATION_PREVIEW_FORM_NAMES.contains(form
							.getName())) {
				result.add(form);
			}
		}
		return result;
	}

	public List<Form> getFormsSuitableForSavingDuringCustomization() {
		doDeserializeIfNeeded();
		List<Form> result = new ArrayList<Form>();
		for (Form form : forms) {
			if (form.isStartingPoint()
					&& !CUSTOMIZATION_PREVIEW_FORM_NAMES.contains(form
							.getName())) {
				result.add(form);
			}
		}
		return result;
	}

	public boolean isInvalid() {
		return invalid;
	}

	public List<DataSource> getDataSourcesDefinedInForms() {
		List<DataSource> result = null;

		for (Form form : getForms()) {
			DataSource dataSource = form.extractDataSource();
			if (dataSource != null) {
				if (result == null) {
					result = new ArrayList<DataSource>();
				}
				result.add(dataSource);
			}
		}

		return result;
	}

	public List<DataSource> getDataSources() {
		doDeserializeIfNeeded();
		return dataSources;
	}

	public DataSource getDataSourceNamed(String name) {
		List<DataSource> dataSources = getDataSources();
		if (dataSources == null) {
			return null;
		}

		for (DataSource source : dataSources) {
			if (source.getName().equals(name)) {
				return source;
			}
		}

		return null;
	}

	public boolean addDataSources(List<DataSource> newDataSources) {
		doDeserializeIfNeeded();
		if (newDataSources == null || newDataSources.size() == 0) {
			return false;
		}

		boolean foundChanges = false;

		if (dataSources == null) {
			dataSources = new ArrayList<DataSource>(newDataSources);
			foundChanges = true;
		} else {
			for (DataSource source : newDataSources) {
				if (!dataSources.contains(source)) {
					foundChanges = true;
					removeDataSourcesNamed(source.getName());
					dataSources.add(source);
				}
			}
		}
		if (foundChanges) {
			projectXmlDefinition = createProjectDefinitionForDataSources()
					.asXML();
		}
		return foundChanges;
	}

	private void removeDataSourcesNamed(String name) {
		ListIterator<DataSource> dataSourceIterator = dataSources
				.listIterator();
		while (dataSourceIterator.hasNext()) {
			DataSource existingDataSource = dataSourceIterator.next();
			if (existingDataSource.getName().equals(name)) {
				dataSourceIterator.remove();
			}
		}
	}

	private org.dom4j.Document createProjectDefinitionForDataSources() {
		org.dom4j.Document result = DocumentFactory.getInstance()
				.createDocument();
		Element projectElement = result.addElement("project");
		projectElement.addAttribute("name", "Static Data");
		projectElement.addAttribute("format", "1.4");

		Element dataSourcesElement = projectElement.addElement("datasources");
		for (DataSource dataSource : dataSources) {
			dataSource.toXml(dataSourcesElement);
		}

		return result;
	}

	public void setThemePath(String themePath) {
		// --- It should be removed from the main project definitions and be
		// packed into separate metadata.
		this.themePath = themePath;
		this.doDeserializeIfNeeded();
		Element xml = getXml();
		Attribute attribute = xml.attribute(THEME_PATH_ATTRIBUTE_NAME);
		attribute.setValue(themePath);
		serializeProjectDefinition(xml);

		ProjectRuntimeCache.instance.removeFromCache(id);
	}

	public boolean removeDataSource(String dataSourceName) {
		doDeserializeIfNeeded();
		ListIterator<DataSource> iterator = dataSources.listIterator();
		while (iterator.hasNext()) {
			DataSource dataSource = iterator.next();
			if (dataSource.getName().equals(dataSourceName)) {
				iterator.remove();
				projectXmlDefinition = createProjectDefinitionForDataSources()
						.asXML();
				return true;
			}
		}
		return false;
	}

	public boolean isCustomizable() {
		return getCustomizerForm() != null;
	}

	public String getRandomFormName(Form form) {
		instantiateProperties();

		String randomFormName = properties
				.getProperty(FORM_NAME_TO_RANDOM_TOKEN_PREFIX + form.getName());
		if (randomFormName == null) {
			return form.getName();
		}
		if (randomFormName.equals(form.getName())) {
			return randomFormName;
		}
		return randomFormName + "." + makeURLSafeFormName(form);
	}

	public Set<String> getFormNamesSelectedInProjectManager() {
		instantiateProperties();

		String formNameList = properties
				.getProperty(SELECTED_FORM_NAMES_PROPERTY);
		if (formNameList == null) {
			return Collections.emptySet();
		}

		String[] names = formNameList.split(FORM_NAME_LIST_DELIMITER);
		Set<String> result = new HashSet<String>(names.length);
		for (int i = 0; i < names.length; i++) {
			result.add(names[i]);
		}

		return result;
	}

	public void selectFormInProjectManager(String formName) {
		Set<String> currentExcludedNames = getFormNamesSelectedInProjectManager();
		if (currentExcludedNames.contains(formName)) {
			return;
		}

		if (currentExcludedNames.size() == 0) {
			properties.put(SELECTED_FORM_NAMES_PROPERTY, formName);
		} else {
			properties.put(SELECTED_FORM_NAMES_PROPERTY, properties
					.getProperty(SELECTED_FORM_NAMES_PROPERTY)
					+ FORM_NAME_LIST_DELIMITER + formName);
		}

		updatePropertiesAsString();

		ProjectRuntimeCache.instance.removeFromCache(id);
	}

	public void unselectFormInProjectManager(String formName) {
		Set<String> currentSelectedNames = getFormNamesSelectedInProjectManager();
		if (!currentSelectedNames.contains(formName)) {
			return;
		}

		currentSelectedNames.remove(formName);

		StringBuilder updatedProperty = new StringBuilder();
		for (String nextFormName : currentSelectedNames) {
			if (updatedProperty.length() > 0) {
				updatedProperty.append(FORM_NAME_LIST_DELIMITER);
			}
			updatedProperty.append(nextFormName);
		}

		properties
				.put(SELECTED_FORM_NAMES_PROPERTY, updatedProperty.toString());

		updatePropertiesAsString();

		ProjectRuntimeCache.instance.removeFromCache(id);
	}

	public void showAllFormsByDefault() {
		instantiateProperties();
		properties.remove(SHOW_SELECTED_FORMS_BY_DEFAULT);
		updatePropertiesAsString();
		ProjectRuntimeCache.instance.removeFromCache(id);
	}

	public void showSelectedFormsByDefault() {
		instantiateProperties();
		properties.put(SHOW_SELECTED_FORMS_BY_DEFAULT, Boolean.TRUE.toString());
		updatePropertiesAsString();
		ProjectRuntimeCache.instance.removeFromCache(id);
	}

	public boolean isShowSelectedFormsByDefault() {
		instantiateProperties();
		return properties.containsKey(SHOW_SELECTED_FORMS_BY_DEFAULT);
	}

	private String makeURLSafeFormName(Form form) {
		String name = form.getName();
		StringBuilder result = new StringBuilder(name.length());
		for (int i = 0; i < name.length(); i++) {
			char c = name.charAt(i);
			switch (c) {
			case '\\':
			case '%':
			case '/':
				result.append('-');
				break;

			default:
				result.append(c);
				break;
			}

		}
		return result.toString();
	}

	public String getPropertiesAsString() {
		return propertiesAsString;
	}

	public void setPropertiesAsString(String projectPropertiesString) {
		this.propertiesAsString = projectPropertiesString;
		instantiatePropertiesWithLegacyFormNames = this.propertiesAsString == null;
	}

	public void generateRandomFormTokens() {
		instantiateProperties();

		boolean needsToBeUpdated = false;
		for (Form form : getForms()) {
			String key = FORM_NAME_TO_RANDOM_TOKEN_PREFIX + form.getName();
			String token = properties.getProperty(key);
			if (token != null) {
				continue;
			}

			token = RandomTokenGenerator.getRandomToken(7);
			properties.put(key, token);
			properties.put(RANDOM_TOKEN_TO_FORM_NAME_PREFIX + token, form
					.getName());

			needsToBeUpdated = true;
		}
		if (needsToBeUpdated) {
			updatePropertiesAsString();
		}
	}

	private void updatePropertiesAsString() {
		try {
			ByteArrayOutputStream stream = new ByteArrayOutputStream();
			try {
				properties.store(stream, null);
			} finally {
				stream.close();
			}
			propertiesAsString = stream.toString();
		} catch (IOException e) {
			throw new IllegalStateException(e);
		}
	}

	private void instantiateProperties() {
		if (properties != null) {
			return;
		}
		if (propertiesAsString == null) {
			properties = new Properties();
			if (instantiatePropertiesWithLegacyFormNames) {
				populateFormTokensWithFormNames();
			}
		} else {
			properties = new Properties();
			try {
				ByteArrayInputStream stream = new ByteArrayInputStream(
						propertiesAsString.getBytes());
				try {
					properties.load(stream);
				} finally {
					stream.close();
				}
			} catch (IOException e) {
				throw new IllegalStateException(e);
			}
		}
	}

	public void populateFormTokensWithFormNames() {
		if (properties == null) {
			properties = new Properties();
		}
		for (Form form : getForms()) {
			properties.put(FORM_NAME_TO_RANDOM_TOKEN_PREFIX + form.getName(),
					form.getName());
			properties.put(RANDOM_TOKEN_TO_FORM_NAME_PREFIX + form.getName(),
					form.getName());
		}
	}

	public Form getFormByRandomToken(String formId) {
		instantiateProperties();
		String[] tokens = formId.split("\\.", 2);

		String token = tokens[0];

		String formName = properties
				.getProperty(RANDOM_TOKEN_TO_FORM_NAME_PREFIX + token);
		if (formName == null) {
			return null;
		}

		return getForm(formName);
	}

	public PageHeader getPageHeader() {
		doDeserializeIfNeeded();
		return pageHeader;
	}

	public String getDesignerVersion() {
		return designerVersion;
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		doDeserializeIfNeeded();

		for (Document document : documents) {
			ResponseCreator result = document
					.getResponseCreatorForComponentId(componentId);
			if (result != null) {
				return result;
			}
		}

		for (Form form : forms) {
			for (FormSegment segment : form.getSegments()) {
				for (FormItem item : segment) {
					ResponseCreator result = item
							.getResponseCreatorForComponentId(componentId);
					if (result != null) {
						return result;
					}
				}
			}
		}

		return null;
	}
}
