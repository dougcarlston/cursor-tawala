package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.Set;

import javax.servlet.http.HttpSession;

import com.scissor.Log;
import com.scissor.xmlconfig.Format;
import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Document;
import com.tawala.project.Form;
import com.tawala.project.FormSegment;
import com.tawala.project.FormSubmission;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.VirtualDocument;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.project.fib.FillInBlankLayout;
import com.tawala.project.theme.CommonTheme;
import com.tawala.project.theme.ProjectTheme;
import com.tawala.web.Request;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.project.DataCollectingProjectController;

public class ExecutionContext {
	public static class Initializer {
		public void setRedirectAfterPost(boolean doRedirect) {
			ExecutionContext.REDIRECT_AFTER_POST = doRedirect;
		}

		public void setDetectBackButtonNavigation(boolean doDetect) {
			ExecutionContext.DETECT_BACK_BUTTON_NAVIGATION = doDetect;
		}
	}

	public static boolean REDIRECT_AFTER_POST = true;
	public static boolean DETECT_BACK_BUTTON_NAVIGATION = true;

	private static final String LAST_STORED_PAGE_ATTRIBUTE = "com.tawala.project.last.stored.page";
	private static final String LAST_STORED_FORM_NAME_ATTRIBUTE = "com.tawala.project.last.stored.form.name";

	public static final String OVERRIDE_THEME_PARAMETER = "_theme";
	public static final String SURPRESS_ADS_PARAMETER = "_a";
	public static final String INCLUDE_CUSTOMIZATION_MARKERS = "_include_customization_markers";
	public static final String TAWALA_STORED_CONTEXT_INFO = "tawala.stored.context";

	private static final String TAWALA_STORED_CONTEXT_INFO_FOR_PREVIEW = "tawala.stored.context.for.preview";
	private static final String MCQ_SELECTION_MARKER = "(selection)";
	private static final Format FORMAT_1_6 = new Format("1.6");

	private final Domain domain;
	private final LinkToUserProject link;
	private final UserProject project;
	private FormSubmission submission;
	private final FormSubmission variables;
	private final Request request;
	private final Map<String, VirtualDocument> documents;
	private Map<String, List<CompositeFormSubmission>> recordListMap = new HashMap<String, List<CompositeFormSubmission>>();
	private Map<String, CompositeFormSubmission> recordMap = new HashMap<String, CompositeFormSubmission>();
	private boolean evaluatingWhereClause;
	private CompositeFormSubmission currentWhereClauseCandidate;
	private UserProject.EntryPointType entryPointType = UserProject.EntryPointType.REAL_PROJECT;
	private final String overridingTheme;
	private Map<Object, Object> cachedObjects;
	private boolean includeCustomizationMarkers;
	private Map<String, String> selectionMap = new HashMap<String, String>();

	private boolean previewMode = false;
	private boolean generatingEmailContent = false;
	private FillInBlankLayout currentFillInBlankLayoutManager;
	private boolean allowAdsToAppear;
	private boolean adsExplicitlySurpressed;
	private String originalLink;

	public String getOriginalLink() {
		return originalLink;
	}

	public boolean isPreviewMode() {
		return previewMode;
	}

	public void setPreviewMode(boolean previewMode) {
		this.previewMode = previewMode;
	}

	public ExecutionContext(Domain domain, LinkToUserProject linkToProject,
			Form form, Request request, EntryPointType entryPointType) {
		this.domain = domain;
		this.link = linkToProject;
		this.project = linkToProject.getProject();
		this.request = request;
		this.overridingTheme = request.getParameter(OVERRIDE_THEME_PARAMETER);
		this.includeCustomizationMarkers = request
				.getParameter(INCLUDE_CUSTOMIZATION_MARKERS) != null;
		this.entryPointType = entryPointType;

		StoredContextInfo storedContextInfo = getStoredContextInfo();
		if (project.getId() != storedContextInfo.getUserProjectId()) {
			purgePreviousSessionData();
		}

		this.documents = getVirtualDocuments() == null ? new HashMap<String, VirtualDocument>()
				: getVirtualDocuments();
		this.variables = getPreviousVariables() == null ? new FormSubmission(
				true) : getPreviousVariables();

		this.submission = form == null ? null
				: extractSubmission(project, form);
		this.adsExplicitlySurpressed = request
				.getParameter(SURPRESS_ADS_PARAMETER) != null;

		originalLink = getStoredContextInfo().getOriginalLink();
		if (originalLink == null
				&& request
						.getParameter(DataCollectingProjectController.REDIRECT_AFTER_POST_PARAMETER) == null) {
			if (request.isPost()) {
				originalLink = request.getParameter(FormSegment.ORIGINAL_URL);
			} else {
				originalLink = request.getHttpRequest().getRequestURI();
			}
			getStoredContextInfo().setOriginalLink(originalLink);
		}
	}

	public ExecutionContext(Domain domain, UserProject project) {
		this.link = null;
		this.domain = domain;
		this.project = project;
		this.overridingTheme = null;
		this.variables = null;
		this.request = null;
		this.documents = null;
		this.previewMode = true;
	}

	private FormSubmission extractSubmission(UserProject project, Form form) {
		FormSubmission previousSubmission = getStoredContextInfo()
				.getPreviousSubmission(form.getName());
		if (previousSubmission != null && previousSubmission.isBeingEdited()) {
			previousSubmission.extractPostedData(this);
			return previousSubmission;
		} else {
			if (expectPostedData())
				return new FormSubmission(project, form, this);
			else
				return new FormSubmission(project, form);
		}
	}

	public boolean expectPostedData() {
		return request.isPost()
				&& request.getParameter(FormSegment.SEGMENT_ID) != null;
	}

	public Domain getDomain() {
		return domain;
	}

	public Project getProject() {
		return project.getProject();
	}

	public String getProjectName() {
		return project.getName();
	}

	public FormSubmission getSubmission() {
		return submission;
	}

	public Request getRequest() {
		return request;
	}

	public Value getValue(String name) {
		Reference reference = new Reference(name, this);
		FormSubmission referencedSubmission = getSubmission(reference);
		return referencedSubmission == null ? Value.NULL : referencedSubmission
				.getValue(reference);
	}

	public List<Value> getValues(String name) {
		Reference reference = new Reference(name, this);
		FormSubmission submission = getSubmission(reference);
		return submission == null ? null : submission.getValues(reference);
	}

	public String resolveMCQSelectionIfNecessary(String name) {
		if (name.equals(MCQ_SELECTION_MARKER)) {
			name = selectionMap.get(name);
		}
		return name;
	}

	public void setValue(final String fieldId, String value) {
		Reference reference = new Reference(fieldId, this);
		if (reference.isVariable()) {
			// The variable is set to the current submission to preserve
			// backward compatibility with
			// older projects.
			if (!getProject().getFormat().isAtLeast(FORMAT_1_6)) {
				submission.setValue(fieldId, value);
			}
			variables.setValue(fieldId, value);
		} else {
			FormSubmission applicableSubmission = getSubmission(reference);
			if (applicableSubmission == null) {
				Log.warn(this, "Unable to find submission for field '"
						+ fieldId + "'");
			} else {
				applicableSubmission.setValue(reference.getFieldName(), value);
			}
		}
	}

	public void setRecordList(String recordListName,
			List<CompositeFormSubmission> recordList) {
		this.recordListMap.put(recordListName, recordList);
	}

	public void removeRecordList(String recordListName) {
		this.recordListMap.remove(recordListName);
	}

	public List<CompositeFormSubmission> getRecordList(String listId) {
		List<CompositeFormSubmission> result = this.recordListMap.get(listId);
		return result != null ? result
				: new ArrayList<CompositeFormSubmission>();
	}

	public void mapRecord(String recordId, CompositeFormSubmission record) {
		this.recordMap.put(recordId, record);
	}

	public void removeRecordMapping(String recordId) {
		this.recordMap.remove(recordId);
	}

	public void mapLastRecord(String recordId, String formId) {

		String recordListId = "AllRecords";

		List<FormDataProvider> formDataSelectors = Collections
				.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
						formId));

		RecordSelector recordSelector = new RecordSelector(formDataSelectors,
				null, recordListId);
		List<CompositeFormSubmission> records = recordSelector.getRecords(this);

		if (records != null) {
			mapRecord(recordId, records.get(records.size() - 1));
		}
	}

	public boolean containsRecordNamed(String recordName) {
		return recordMap.containsKey(recordName);
	}

	public void mapSelection(String mcItemId) {
		this.selectionMap.put(MCQ_SELECTION_MARKER, mcItemId);
	}

	public FormSubmission getSubmission(Reference reference) {
		if (reference.isVariable())
			return variables;

		if (evaluatingWhereClause
				&& currentWhereClauseCandidate.canbeReferedBy(reference))
			return currentWhereClauseCandidate.getFormSubmission(reference);

		if (reference.isRecordReference()) {
			CompositeFormSubmission record = recordMap.get(reference
					.getRecordName());
			return record == null ? null : record.getFormSubmission(reference);
		}

		if (submission != null
				&& (submission.getFormName() != null
						&& submission.getFormName().equals(
								reference.getFormName()) || MCQ_SELECTION_MARKER
						.equals(reference.getFieldName()))) {
			return submission;
		}

		return getPreviousSubmission(reference.getFormName());
	}

	protected String getQualifier(String fieldId) {
		String[] parts = fieldId.split(":", 2);
		return parts.length == 2 ? parts[0] : "";
	}

	public User getProjectOwner() {
		return project.getUser();
	}

	public Document getDocument(String name) {
		if (documents.containsKey(name)) {
			return getVirtualDocument(name);
		}
		return project.getProject().getDocument(name);
	}

	public VirtualDocument getVirtualDocument(String name) {
		return documents.get(name);
	}

	public void add(VirtualDocument document) {
		documents.put(document.getName(), document);
	}

	public boolean isEvaluatingWhereClause() {
		return evaluatingWhereClause;
	}

	public void setEvaluatingWhereClause(boolean evaluatingWhereClause) {
		this.evaluatingWhereClause = evaluatingWhereClause;
	}

	public void setCurrentWhereClauseCandidate(
			CompositeFormSubmission currentWhereClauseCandidate) {
		this.currentWhereClauseCandidate = currentWhereClauseCandidate;
	}

	public Map<String, VirtualDocument> getDocuments() {
		return documents;
	}

	public FormSubmission getVariables() {
		return variables;
	}

	public void setSubmission(FormSubmission submission) {
		this.submission = submission;
	}

	public UserProject getUserProject() {
		return project;
	}

	public EntryPointType getEntryPointType() {
		return entryPointType;
	}

	public void setEntryPointType(UserProject.EntryPointType entryPointType) {
		this.entryPointType = entryPointType;
	}

	public String getOverridingTheme() {
		return overridingTheme;
	}

	public Locale getLocale() {
		// --- Hardcoded until a better day...
		return Locale.US;
	}

	public Object cacheObject(Object key, Object value) {
		if (cachedObjects == null) {
			cachedObjects = new HashMap<Object, Object>();
		}
		return cachedObjects.put(key, value);
	}

	public Object getCachedObject(Object key) {
		return cachedObjects == null ? null : cachedObjects.get(key);
	}

	public Form getForm() {
		return project.getProject().getForm(getSubmission().getFormName());
	}

	public boolean isIncludeCustomizationMarkers() {
		return includeCustomizationMarkers;
	}

	public StoredContextInfo getStoredContextInfo() {
		HttpSession session = request.getSession();
		String attributeName = getStorageAttribute();
		ExecutionContext.StoredContextInfo result = (ExecutionContext.StoredContextInfo) session
				.getAttribute(attributeName);
		if (result == null) {
			result = new ExecutionContext.StoredContextInfo(getUserProject());
			session.setAttribute(attributeName, result);
		}
		return result;
	}

	public FormSubmission getPreviousSubmission(String formName) {
		return getStoredContextInfo().getPreviousSubmission(formName);
	}

	public void storePreviousSubmission(FormSubmission submission) {
		getStoredContextInfo().addPreviousSubmission(submission);
	}

	public void purgePreviousSessionData() {
		request.getSession().removeAttribute(getStorageAttribute());
	}

	/*
	 * Storage attribute is going to be different for different cases. The main
	 * reason is the customization process where several page flows get started
	 * at the same time and we need to preserve the states of different flows.
	 * 
	 */
	private String getStorageAttribute() {
		return (includeCustomizationMarkers ? TAWALA_STORED_CONTEXT_INFO_FOR_PREVIEW
				: TAWALA_STORED_CONTEXT_INFO)
				+ getEntryPointType().toString();
	}

	public void storeExecutionContextSnapshot() {
		getStoredContextInfo().setVirtualDocuments(getDocuments());
		getStoredContextInfo().setVariables(getVariables());
	}

	public Map<String, VirtualDocument> getVirtualDocuments() {
		return getStoredContextInfo().getVirtualDocuments();
	}

	public FormSubmission getPreviousVariables() {
		return getStoredContextInfo().getVariables();
	}

	// Currently it's pure storage. It doesn't attempt to reset itself in
	// the HttpSession object to indicate that the values changed. This is done
	// on purpose; the values are not "replicatable" across containers.
	public static class StoredContextInfo {
		private final long userProjectId;
		private FormSubmission variables;
		private Map<String, VirtualDocument> virtualDocuments;
		private Map<String, FormSubmission> sessionSubmissions;
		private Set<String> allowedPostTokens;
		private String originalLink;

		public String getOriginalLink() {
			return originalLink;
		}

		public void setOriginalLink(String originalLink) {
			this.originalLink = originalLink;
		}

		public StoredContextInfo(UserProject userProject) {
			this.userProjectId = userProject.getId();
		}

		public void addAllowedPostToken(String token) {
			if (allowedPostTokens == null) {
				allowedPostTokens = new HashSet<String>();
			}
			allowedPostTokens.add(token);
		}

		public Set<String> getAllowedPostTokens() {
			return allowedPostTokens;
		}

		public void clearExpectedPostTokens() {
			this.allowedPostTokens = null;
		}

		public FormSubmission getPreviousSubmission(String formName) {
			return sessionSubmissions == null ? null : sessionSubmissions
					.get(formName);
		}

		public void addPreviousSubmission(FormSubmission submission) {
			if (sessionSubmissions == null) {
				sessionSubmissions = Collections
						.synchronizedMap(new HashMap<String, FormSubmission>());
			}
			sessionSubmissions.put(submission.getFormName(), submission);
		}

		public Map<String, VirtualDocument> getVirtualDocuments() {
			return virtualDocuments;
		}

		public void setVirtualDocuments(
				Map<String, VirtualDocument> virtualDocuments) {
			this.virtualDocuments = virtualDocuments;
		}

		public FormSubmission getVariables() {
			return variables;
		}

		public void setVariables(FormSubmission variables) {
			this.variables = variables;
		}

		public long getUserProjectId() {
			return userProjectId;
		}
	}

	public void setCurrentFillInBlankLayoutManager(
			FillInBlankLayout layoutManager) {
		this.currentFillInBlankLayoutManager = layoutManager;
	}

	public FillInBlankLayout getCurrentFillInBlankLayoutManager() {
		return currentFillInBlankLayoutManager;
	}

	public ProjectTheme getTheme() {
		if (overridingTheme != null) {
			CommonTheme theme = CommonTheme.getThemeByPath(overridingTheme);
			if (theme == null) {
				Log.error(this, "Unable to find overriding theme: '"
						+ overridingTheme + "'");
				theme = CommonTheme.DEFAULT_THEME;
			}
			return theme;
		}
		return getProject().getTheme();
	}

	public boolean shouldDisplayAds() {
		return !adsExplicitlySurpressed && allowAdsToAppear
				&& project.getUser().isEnableAds();
	}

	public void setAllowAdsToAppear(boolean allowAdsToAppear) {
		this.allowAdsToAppear = allowAdsToAppear;
	}

	public boolean isAdsExplicitlySurpressed() {
		return adsExplicitlySurpressed;
	}

	public LinkToUserProject getLink() {
		return link;
	}

	public CompositeFormSubmission getMappedSubmission(String recordListName) {
		return this.recordMap.get(recordListName);
	}

	public void removeVariable(String variableName) {
		this.variables.clearValue(variableName);
	}

	public void storeLastPage(Form postedToForm, OldPage page) {
		HttpSession session = getRequest().getSession();
		session.setAttribute(LAST_STORED_PAGE_ATTRIBUTE, page);
		session.setAttribute(LAST_STORED_FORM_NAME_ATTRIBUTE, postedToForm
				.getName());
	}

	public OldPage getStoredPage(Form postedToForm) {
		HttpSession session = getRequest().getSession();
		String formName = (String) session
				.getAttribute(LAST_STORED_FORM_NAME_ATTRIBUTE);
		if (formName == null || !formName.equals(postedToForm.getName())) {
			return null;
		} else {
			return (OldPage) session.getAttribute(LAST_STORED_PAGE_ATTRIBUTE);
		}
	}

	public String generateNextExpectedPostToken() {
		String result = String.valueOf(new Date().getTime());
		getStoredContextInfo().addAllowedPostToken(result);
		return result;
	}

	public void clearExpectedPostTokens() {
		getStoredContextInfo().clearExpectedPostTokens();
	}

	public boolean isPostTokenAllowed(String token) {
		Set<String> allowedTokens = getStoredContextInfo()
				.getAllowedPostTokens();
		return allowedTokens != null && allowedTokens.contains(token);
	}

	public boolean isDetectBackButtonNavigation() {
		return DETECT_BACK_BUTTON_NAVIGATION;
	}

	public boolean isRedirectAfterPost() {
		return REDIRECT_AFTER_POST;
	}

	public boolean isGeneratingEmailContent() {
		return generatingEmailContent;
	}

	public void setGeneratingEmailContent(boolean generatingEmailContent) {
		this.generatingEmailContent = generatingEmailContent;
	}
}
