package com.tawala.project;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.SkipExecutionResult;
import com.tawala.project.data.DataSource;
import com.tawala.web.Request;
import com.tawala.web.oldhtml.OldPage;

public class Form {
	private static final int SEGMENT_ID_NOT_FOUND = -1;

	protected static final Factory<FormItem> FACTORY = new Factory<FormItem>();

	private static final Factory<DeclaredField> DECLARED_FIELD_FACTORY = new Factory<DeclaredField>();

	static {
		FACTORY.register("text", TextBlock.class);
		FACTORY.register("heading", "type", "Main", HeadingItem.class);
		FACTORY.register("heading", "type", "Sub", SubheadingItem.class);
		FACTORY.register("fib", FillInBlank.class);
		FACTORY.register("file", FillInBlank.class);
		FACTORY.register("mc", MultipleChoice.class);
		FACTORY.ignore("field");

		DECLARED_FIELD_FACTORY.register("field", DeclaredField.class);
	}

	private Project project;
	private final String name;
	private final FormSegments segments;
	private boolean isStartingPoint;
	private boolean isDataEntryOnly;
	private String themePath;
	private final String postProcessName;
	private final String preProcessName;
	private final String dataSourceName;
	private List<DeclaredField> declaredFields;
	private final boolean detectBackButtonNavigation;

	public Form(ConfigElement config) {
		this(config.attribute("name").stringValue(), config
				.attribute("process").stringValue(), config.attribute(
				"preProcess").stringValue(), new FormSegments(config.attribute(
				"name").stringValue(), config.child("items")), config
				.attribute("dataSourceName").stringValue(), config.attribute(
				"blockBackButton").booleanValue());

		if (config.hasAttribute("startPoint")) {
			this.isStartingPoint = config.attribute("startPoint")
					.booleanValue();
		}

		if (config.hasAttribute("dataEntryOnly")) {
			this.isDataEntryOnly = config.attribute("dataEntryOnly")
					.booleanValue();
		}

		if (config.hasAttribute("themePath")) {
			this.themePath = config.attribute("themePath").stringValue();
		}
		ConfigElement items = config.child("items");
		this.declaredFields = items == null ? null : DECLARED_FIELD_FACTORY
				.makeChildren(items.children("field"));
	}

	private Form(String name, String postProcessName, String preProcessName,
			FormSegments segments, String dataSourceName,
			boolean detectBackButtonNavigation) {
		assert name != null && !name.equals("");
		this.name = name;
		this.segments = segments;
		this.isStartingPoint = true;
		this.isDataEntryOnly = false;
		this.themePath = "default";

		this.preProcessName = preProcessName;
		this.postProcessName = postProcessName;
		this.dataSourceName = dataSourceName;
		this.detectBackButtonNavigation = detectBackButtonNavigation;
	}

	public String getPreProcessName() {
		return preProcessName;
	}

	public String getPostProcessName() {
		return postProcessName;
	}

	public Form(String name) {
		this(name, null, null, new FormSegments(name), null, false);
	}

	public Form(String name, String postProcessName) {
		this(name, postProcessName, null, new FormSegments(name), null, false);
	}

	public String getName() {
		return name;
	}

	public boolean isStartingPoint() {
		return this.isStartingPoint;
	}

	public boolean isDataEntryOnly() {
		// return this.isDataEntryOnly || name.equalsIgnoreCase("customize") ||
		// name.equalsIgnoreCase("setup");
		return this.isDataEntryOnly;
	}

	public String getThemePath() {
		return this.themePath;
	}

	public List<FormItem> getItems() {
		List<FormItem> allItems = new ArrayList<FormItem>();

		for (FormSegment segment : segments) {
			allItems.addAll(segment);
		}

		return Collections.unmodifiableList(allItems);
	}

	// Returns the Ids of all input items (blanks and MCQs)
	public List<String> getInputItemIds() {
		List<String> itemIds = new ArrayList<String>();

		for (FormSegment segment : segments) {
			for (FormItem item : segment) {
				if (item instanceof FillInBlank) {
					for (Blank blank : ((FillInBlank) item).getBlanks()) {
						itemIds.add(blank.getHtmlId());
					}
				} else if (item instanceof MultipleChoice) {
					itemIds.add(((MultipleChoice) item).getHtmlId());
				}
			}
		}

		return itemIds;
	}

	// Returns the Ids of all multiple choice items
	public List<String> getMcItemIds() {
		List<String> itemIds = new ArrayList<String>();

		for (FormSegment segment : segments) {
			for (FormItem item : segment) {
				if (item instanceof MultipleChoice) {
					itemIds.add(((MultipleChoice) item).getHtmlId());
				}
			}
		}

		return itemIds;
	}

	public FormSegment getSegmentAfter(Request request) {
		int segmentId = extractSegmentId(request);
		int nextSegmentId = segmentId == SEGMENT_ID_NOT_FOUND ? 0
				: segmentId + 1;
		return getSegment(nextSegmentId);
	}

	public FormSegment getPreviousSegment(Request request) {
		int segmentId = extractSegmentId(request);
		return segmentId == SEGMENT_ID_NOT_FOUND ? null : getSegment(segmentId);
	}

	public FormSegment getSegment(int index) {
		return segments.size() > index ? segments.get(index) : null;
	}

	private int extractSegmentId(Request request) {
		String segmentIdParameter = request
				.getParameter(FormSegment.SEGMENT_ID);
		return segmentIdParameter == null ? SEGMENT_ID_NOT_FOUND : Integer
				.parseInt(segmentIdParameter);
	}

	int segmentCount() {
		return segments.size();
	}

	public FormSegment getSegmentForSkip(String skipTo) {
		for (FormSegment segment : segments) {
			if (segment.containsItemWithId(skipTo)) {
				return segment.segmentStaringWithId(skipTo);
			}
		}
		return null;
	}

	public OldPage firstPage(ExecutionContext context) {
		if (segments.hasInitialSkip()) {
			SkipExecutionResult result = segments.getInitialSkip().execute(
					this, context);
			if (result.hasSkipTo()) {
				FormSegment destinationSegment = result
						.getDestinationSegment(this);
				if (destinationSegment != null) {
					return destinationSegment.toPage(context);
				} else {
					return null;
				}
			}
		}
		return getFirstSegment().toPage(context);
	}

	public FormSegment getFirstSegment() {
		return getSegment(0);
	}

	public FormSegment findNextSegment(ExecutionContext context) {
		FormSegment previousSegment = getPreviousSegment(context.getRequest());
		if (previousSegment == null) {
			if (segments.hasInitialSkip()) {
				SkipExecutionResult result = segments.getInitialSkip().execute(
						this, context);
				if (result.hasSkipTo()) {
					return result.getDestinationSegment(this);
				}
			}

			return getFirstSegment();
		}

		SkipBlocks skipBlocks = previousSegment.getSkipBlocks();
		SkipExecutionResult result = skipBlocks.execute(context);
		if (result.hasSkipTo())
			return result.getDestinationSegment(this, context.getRequest());
		else
			return getSegmentAfter(context.getRequest());
	}

	public FormSegments getSegments() {
		return segments;
	}

	public Collection<Field> getAllFields() {
		Collection<Field> result = new ArrayList<Field>();

		for (FormSegment segment : getSegments()) {
			for (Field field : segment.fields()) {
				result.add(field);
			}
		}

		if (declaredFields != null) {
			result.addAll(declaredFields);
		}

		return result;
	}

	public Field getFieldById(String fieldId) {
		for (FormSegment segment : getSegments()) {
			for (Field field : segment.fields()) {
				// TODO: this is really odd way of doing this. The alternate
				// labels should be "presentation only" and should translate to
				// the canonical ids for all internal processing.
				if (field.getId().equals(fieldId)
						|| field.getHtmlId().equals(fieldId))
					return field;
			}
		}
		if (declaredFields != null) {
			for (DeclaredField field : declaredFields) {
				if (field.getId().equals(fieldId)) {
					return field;
				}
			}
		}
		throw new IllegalArgumentException("Unable to find field '" + fieldId
				+ "' in form " + this);
	}

	public String toString() {
		return "Form \"" + name + "\"";
	}

	public DataSource extractDataSource() {
		if (this.dataSourceName == null) {
			return null;
		}

		DataSource result = new DataSource(this.dataSourceName);
		for (Field field : getAllFields()) {
			result.addField(field.getStoredFieldDefinition());
		}
		return result;
	}

	public boolean isSharedData() {
		return dataSourceName != null;
	}

	public String getDataSourceName() {
		return dataSourceName;
	}

	public List<DeclaredField> getDeclaredFields() {
		return declaredFields;
	}

	public String getDescription() {
		if (Project.SETUP_WIZARD_FORM_NAMES.contains(getName())) {
			return "URL link to change customization settings.";
		} else if (Project.ADMINISTRATION_FORM_NAMES.contains(getName())) {
			return "URL link to web app administration page.";
		} else if (Project.CUSTOMIZATION_PREVIEW_FORM_NAMES.contains(getName())) {
			return "URL link to view customization settings.";
		} else {
			return "Main URL link for your users.";
		}
	}

	public boolean isDetectBackButtonNavigation() {
		return detectBackButtonNavigation;
	}

	public Project getProject() {
		return project;
	}

	void setProject(Project project) {
		this.project = project;
	}
}
