package com.tawala.project;

import java.io.UnsupportedEncodingException;
import java.math.BigDecimal;
import java.net.URLEncoder;
import java.rmi.server.UID;
import java.util.ArrayList;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.OneToMany;
import javax.persistence.OneToOne;
import javax.persistence.OrderBy;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.persistence.UniqueConstraint;
import javax.persistence.Version;

import org.hibernate.annotations.AccessType;
import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

import com.scissor.ImpossibleException;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.DomainMetadata;
import com.tawala.domain.User;
import com.tawala.project.caching.ProjectRuntimeCache;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.project.ProjectController;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_user_project_id")
@Entity
@Table(name = "user_project", uniqueConstraints = { @UniqueConstraint(columnNames = {
		"user_id", "project_id" }) })
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "userprojects")
public class UserProject {
	public static final String SETUP_FORM_THEME = "plain";

	private static String WEB_SITE_HOST_NAME = "www.tawala.com";

	@Id
	@Column(name = "user_project_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@ManyToOne(fetch = FetchType.EAGER)
	@JoinColumn(name = "user_id", nullable = false)
	private User user;

	@Column(name = "name", length = 100, nullable = false)
	private String name;

	@Column(name = "lib_project_id", nullable = true)
	private Long libraryProjectId;

	@Column(name = "lib_version_number", nullable = true)
	private Integer libraryVersionNumber;

	@OneToOne(fetch = FetchType.LAZY, cascade = { CascadeType.ALL })
	@JoinColumn(name = "project_id", nullable = false)
	private Project project;

	@SuppressWarnings("unused")
	@Column(name = "created_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date createdDate = new Date();

	@Column(name = "last_updated_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date lastUpdatedDate = new Date();

	@Column(name = "next_version", nullable = false)
	private int nextVersionNumber = 1;

	@OneToMany(fetch = FetchType.LAZY, cascade = { CascadeType.ALL })
	@JoinColumn(name = "user_project_id")
	@OrderBy("versionNumber DESC")
	@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "userprojects.versions")
	private List<ProjectVersion> versions = new ArrayList<ProjectVersion>();

	@OneToOne(fetch = FetchType.LAZY, cascade = { CascadeType.ALL })
	@JoinColumn(name = "deployed_version_id", nullable = true)
	private ProjectVersion deployedVersion;

	@SuppressWarnings("unused")
	@Version
	@AccessType("field")
	@Column(name = "version", nullable = false)
	private int version;

	@Column(name = "original_lib_project_version_id", nullable = true)
	private Long originalLibraryProjectVersionId;

	@Column(name = "unique_random_id", nullable = false, length = 20)
	private String uniqueRandomId;

	@Column(name = "offline", nullable = false)
	private boolean offline = false;

	@Column(name = "reg_start_dt", nullable = true)
	@Temporal(TemporalType.DATE)
	private Date registrationStartDate;

	@Column(name = "reg_close_dt", nullable = true)
	@Temporal(TemporalType.DATE)
	private Date registrationCloseDate;
	
	@Column(name = "reg_closed", nullable = true)
	private Boolean registrationClosed;

	@Column(name = "reg_fee", nullable = true)
	private BigDecimal registrationFee;

	@Column(name = "reg_invoice_dt", nullable = true)
	@Temporal(TemporalType.DATE)
	private Date registrationInvoiceDate;

	@Column(name = "project_type", nullable = true)
	private String projectType;

	@Column(name = "po_number", nullable = true)
	private String purchaseOrderNumber;
	
	@Column(name = "invoice_number", nullable = true)
	private String invoiceNumber;
	
	@Column(name = "ysl_league_id", nullable = true, length=20)
	private String YSLLeagueId;

	@Column(name = "roster_template_id", nullable = true, length=40)
	private String teamRosterTemplateId;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "ysl_last_updated", nullable = true)
	private Date YSLLastUpdated;

	@Column(name = "require_ssl", nullable = false)
	private boolean requireSSL = false;

	UserProject() {
		// For Hibernate's use.
	}

	public UserProject(ConfigElement projectElement, User user) {
		this(new Project(projectElement), user, projectElement
				.attribute("name").stringValue());
	}

	public String getUniqueRandomId() {
		return uniqueRandomId;
	}

	public void setUniqueRandomId(String uniqueRandomId) {
		this.uniqueRandomId = uniqueRandomId;
	}

	public UserProject(Project project, User user, String name) {
		this.user = user;
		this.project = project;
		this.name = name;

		ProjectVersion version = new ProjectVersion(this, project.makeCopy());
		version.setDescription("Original version");
		addVersion(version);
	}

	public String getName() {
		return name;
	}

	public Long getLibraryProjectId() {
		return libraryProjectId;
	}

	public void setLibraryProjectId(Long libraryProjectId) {
		this.libraryProjectId = libraryProjectId;
	}

	public Integer getLibraryVersionNumber() {
		return libraryVersionNumber;
	}

	public void setLibraryVersionNumber(Integer libraryVersionNumber) {
		this.libraryVersionNumber = libraryVersionNumber;
	}

	public User getUser() {
		return user;
	}

	public void setUser(User user) {
		this.user = user;
	}

	@Override
	public boolean equals(Object obj) {
		UserProject other = (UserProject) obj;
		return this.id == other.id;
	}

	public String toString() {
		return "UserProject #" + id;
	}

	public Project getProject() {
		return project;
	}

	/**
	 * @return Returns the id.
	 */
	public long getId() {
		return id;
	}

	/**
	 * @param id
	 *            The id to set.
	 */
	public void setId(long id) {
		this.id = id;
	}

	public void addVersion(ProjectVersion version) {
		version.setVersionNumber(nextVersionNumber++);
		version.setParent(this);
		versions.add(0, version);
	}

	public List<ProjectVersion> getVersions() {
		return versions;
	}

	public void deployVersion(ProjectVersion projectVersion,
			boolean preserveCurrentTheme) {
		ProjectRuntimeCache.instance.removeFromCache(getProject().getId());

		this.deployedVersion = projectVersion;
		getProject().setProjectXmlDefinition(
				projectVersion.getProject().getProjectXmlDefinition());
		if (preserveCurrentTheme) {
			projectVersion.getProject().setThemePath(getProject().getTheme().getThemeId());
		} else {
			getProject().setThemePath(
					projectVersion.getProject().getTheme().getThemeId());
		}
	}

	public ProjectVersion getDeployedVersion() {
		return deployedVersion;
	}

	public Map<Form, String> getEntryPointURLs() {
		return getEntryPointURLs(EntryPointType.REAL_PROJECT);
	}

	public Map<Form, String> getEntryPointURLsWithoutCustomizerForm() {

		Map<Form, String> result = new LinkedHashMap<Form, String>();
		for (Form form : project.getUserStartingPoints()) {
			result.put(form, getUrlToForm(EntryPointType.REAL_PROJECT, form));
		}
		return result;
	}

	public Map<Form, String> getEntryPointURLsSuitableForSavingDuringCustomization() {

		Map<Form, String> result = new LinkedHashMap<Form, String>();
		for (Form form : project.getFormsSuitableForSavingDuringCustomization()) {
			result.put(form, getUrlToForm(EntryPointType.REAL_PROJECT, form));
		}
		return result;
	}

	public String getAdminURL() {
		for (Form form : project.getFormsSuitableForSavingDuringCustomization()) {
			if (Project.ADMINISTRATION_FORM_NAMES.contains(form.getName())) {
				return getUrlToForm(EntryPointType.REAL_PROJECT, form);
			}
		}
		return null;
	}

	public String getSetupURL() {
		for (Form form : project.getFormsSuitableForSavingDuringCustomization()) {
			if (Project.SETUP_WIZARD_FORM_NAMES.contains(form.getName())) {
				return getUrlToForm(EntryPointType.REAL_PROJECT, form);
			}
		}
		throw new IllegalStateException(
				"Unable to find setup form in user project #" + getId());
	}

	public String getMainEntryPointURL() {
		for (Form form : project.getFormsSuitableForSavingDuringCustomization()) {
			if (!Project.SETUP_WIZARD_FORM_NAMES.contains(form.getName())
					&& !Project.ADMINISTRATION_FORM_NAMES.contains(form
							.getName())
					&& !Project.CUSTOMIZATION_PREVIEW_FORM_NAMES.contains(form
							.getName())) {
				return getUrlToForm(EntryPointType.REAL_PROJECT, form);
			}
		}
		throw new IllegalStateException(
				"Unable to find main entry point form in user project #"
						+ getId());
	}

	public String getCustomizationPreviewEntryPointURL() {
		return getUrlToForm(EntryPointType.REAL_PROJECT, project
				.getCustomizationPreviewForm());
	}

	public Map<Form, String> getEntryPointURLs(EntryPointType entryPointType) {
		Map<Form, String> result = new LinkedHashMap<Form, String>();

		for (Form form : getProject().getForms()) {
			if (form.isStartingPoint()) {
				result.put(form, getUrlToForm(entryPointType, form));
			}
		}

		return result;
	}

	public String getUrlToForm(EntryPointType entryPointType, Form form) {
		return getUrlToForm(getUniqueRandomId(), entryPointType, form, null,
				false);
	}

	public String getUrlToForm(String uniqueId, EntryPointType entryPointType,
			Form form, String overridingTheme, boolean suppressAds) {
		if (uniqueId == null)
			throw new IllegalStateException("Project's unique id is null");

		return getUrl(entryPointType, form, overridingTheme, uniqueId,
				suppressAds);
	}

	public String getUrlToForm(LinkToUserProject link,
			EntryPointType entryPointType, Form form) {
		String uniqueId = link.getId();
		if (uniqueId == null)
			throw new IllegalStateException("Project's unique id is null");

		return getUrl(entryPointType, form, null, uniqueId, false);
	}

	private String getUrl(EntryPointType entryPointType, Form form,
			String overridingTheme, String uniqueId, boolean suppressAds) {
		StringBuilder url = new StringBuilder();
		url.append(getRequiredRequestProtocol()).append("://").append(WEB_SITE_HOST_NAME).append(
				entryPointType.getPath());
		url.append('/').append(uniqueId).append('/').append(
				urlEncode(project.getRandomFormName(form)));

		Map<String, String> parameters = null;
		if (overridingTheme != null) {
			if (parameters == null) {
				parameters = new LinkedHashMap<String, String>();
			}
			parameters.put(ExecutionContext.OVERRIDE_THEME_PARAMETER,
					urlEncode(overridingTheme));
		}
		if (suppressAds) {
			if (parameters == null) {
				parameters = new LinkedHashMap<String, String>();
			}
			parameters.put(ExecutionContext.SURPRESS_ADS_PARAMETER, "y");
		}

		if (parameters != null) {
			StringBuilder parameterString = new StringBuilder();
			for (Map.Entry<String, String> parameter : parameters.entrySet()) {
				if (parameterString.length() > 0) {
					parameterString.append('&');
				}
				parameterString.append(parameter.getKey()).append('=').append(
						parameter.getValue());
			}
			url.append('?').append(parameterString);
		}

		return url.toString();
	}

	private String getRequiredRequestProtocol() {
		return isRequireSSL() ? "https" : "http";
	}

	public String getImageUrl(boolean createFullyQualifiedURL, String uniqueId,
			String imageName) {
		String url = (createFullyQualifiedURL ? getProjectUrlPrefix(uniqueId)
				: "")
				+ ProjectController.IMAGE_PATH
				+ urlEncode(imageName)
				+ Project.IMAGE_SUFFIX;

		return url;
	}

	public String getProjectUrlPrefix(String uniqueId) {
		return getRequiredRequestProtocol() + "://" + WEB_SITE_HOST_NAME
				+ EntryPointType.REAL_PROJECT.getPath() + '/' + uniqueId + '/';
	}

	public String getProjectComponentUrl(ExecutionContext context,
			boolean createFullyQualifiedURL, String componentId) {
		String result = (createFullyQualifiedURL ? getRequiredRequestProtocol() + "://"
				+ WEB_SITE_HOST_NAME + context.getEntryPointType().getPath()
				+ '/' + context.getLink().getId() + '/' : "")
				+ ProjectController.COMPONENT_PATH + urlEncode(componentId);

		return result;
	}

	private static String urlEncode(String stringToEncode) {
		try {
			return URLEncoder.encode(stringToEncode, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			throw new ImpossibleException(e);
		}
	}

	// TODO: setWebSiteHostName is called by Spring on container initialization.
	// The static version of the method is used by the acceptance tests.
	// Ideally, only one method should exist.
	public void setWebSiteHostName(String hostName) {
		setWebsiteHostName(hostName);
	}

	public static void setWebsiteHostName(String hostName) {
		WEB_SITE_HOST_NAME = hostName;
	}

	public static String getWebsiteHostName() {
		return WEB_SITE_HOST_NAME;
	}

	public void removeVersion(ProjectVersion projectVersion) {
		versions.remove(projectVersion);
	}

	public void makeProjectNameUnique() {
		// --- It's unique enough for our purposes. To make it into a GUID host
		// name would need to be added.
		String uniqueId = new UID().toString();
		if (name.length() + uniqueId.length() + 1 > DomainMetadata.instance
				.getUserProjectNameMaxLength()) {
			name = name.substring(0, DomainMetadata.instance
					.getUserProjectNameMaxLength()
					- uniqueId.length() - 1);
		}

		name = name + ' ' + uniqueId;
	}

	public enum EntryPointType {
		REAL_PROJECT {
			String getPath() {
				return WellKnown.urls.getProjectRunUrlPrefix();
			}
		},
		TEST_DRIVE {
			String getPath() {
				return WellKnown.urls.getLibraryTestDriveProjectUrlPrefix();
			}
		},
		SAMPLE_COLLECTION {
			String getPath() {
				return WellKnown.urls.getLibraryRecordSampleDataUrlPrefix();
			}
		},
		FORM_PREVIEW {
			String getPath() {
				return WellKnown.urls.getFormPreviewUrlPrefix();
			}
		};

		abstract String getPath();
	}

	void setName(String name) {
		this.name = name;
	}

	public Long getOriginalLibraryProjectVersionId() {
		return originalLibraryProjectVersionId;
	}

	public void setOriginalLibraryProjectVersionId(
			Long originalLibraryProjectVersionId) {
		this.originalLibraryProjectVersionId = originalLibraryProjectVersionId;
	}

	public Date getLastUpdatedDate() {
		return lastUpdatedDate;
	}

	public void setLastUpdatedDate(Date lastUpdatedDate) {
		this.lastUpdatedDate = lastUpdatedDate;
	}

	public boolean isOffline() {
		return offline;
	}

	public void setOffline(boolean offline) {
		this.offline = offline;
	}

	public void setProject(Project cachedProject) {
		project = cachedProject;
	}

	public Date getRegistrationStartDate() {
		return registrationStartDate;
	}

	public void setRegistrationStartDate(Date registrationStartDate) {
		this.registrationStartDate = registrationStartDate;
	}

	public Date getRegistrationCloseDate() {
		return registrationCloseDate;
	}

	public void setRegistrationCloseDate(Date registrationCloseDate) {
		this.registrationCloseDate = registrationCloseDate;
	}

	public Boolean getRegistrationClosed() {
		return registrationClosed;
	}

	public void setRegistrationClosed(Boolean registrationClosed) {
		this.registrationClosed = registrationClosed;
	}

	public BigDecimal getRegistrationFee() {
		return registrationFee;
	}

	public void setRegistrationFee(BigDecimal registrationFee) {
		this.registrationFee = registrationFee;
	}

	public Date getRegistrationInvoiceDate() {
		return registrationInvoiceDate;
	}

	public void setRegistrationInvoiceDate(Date registrationInvoiceDate) {
		this.registrationInvoiceDate = registrationInvoiceDate;
	}

	public String getProjectType() {
		return projectType;
	}

	public void setProjectType(String projectType) {
		this.projectType = projectType;
	}

	public String getPurchaseOrderNumber() {
		return purchaseOrderNumber;
	}

	public void setPurchaseOrderNumber(String purchaseOrderNumber) {
		this.purchaseOrderNumber = purchaseOrderNumber;
	}

	public String getInvoiceNumber() {
		return invoiceNumber;
	}

	public void setInvoiceNumber(String invoiceNumber) {
		this.invoiceNumber = invoiceNumber;
	}

	public String getYSLLeagueId() {
		return YSLLeagueId;
	}

	public void setYSLLeagueId(String leagueId) {
		if(leagueId != null && leagueId.trim().length() == 0) {
			YSLLeagueId = null;
		} else {
			YSLLeagueId = leagueId;
		}
	}

	public Date getYSLLastUpdated() {
		return YSLLastUpdated;
	}

	public void setYSLLastUpdated(Date lastUpdated) {
		YSLLastUpdated = lastUpdated;
	}

	public String getTeamRosterTemplateId() {
		return teamRosterTemplateId;
	}

	public void setTeamRosterTemplateId(String teamRosterTemplateId) {
		this.teamRosterTemplateId = teamRosterTemplateId;
	}

	public boolean isRequireSSL() {
		return requireSSL;
	}

	public void setRequireSSL(boolean requireSSL) {
		this.requireSSL = requireSSL;
	}
}
