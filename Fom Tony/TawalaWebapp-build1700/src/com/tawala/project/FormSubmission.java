package com.tawala.project;

import java.io.IOException;
import java.io.StringReader;
import java.io.StringWriter;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.Lob;
import javax.persistence.ManyToOne;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.persistence.Transient;

import org.hibernate.annotations.AccessType;
import org.json.JSONObject;
import org.springframework.web.multipart.MultipartFile;

import com.tawala.UsersHibernateImpl;
import com.tawala.component.validator.FieldValidator;
import com.tawala.domain.User;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.DataSource;
import com.tawala.project.data.FieldSetter;
import com.tawala.project.data.StoredField;
import com.tawala.project.theme.UserUploadedFile;
import com.thoughtworks.xstream.XStream;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_submission_id")
@Entity
@Table(name = "submission")
public class FormSubmission {
	private static XStream xstream = new XStream();

	public static final FormSubmission NULL = new FormSubmission(null,
			Collections.unmodifiableMap(new HashMap<String, String[]>()));

	@Id
	@Column(name = "submission_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long databaseId;

	@Transient
	protected Map<String, String[]> fieldContents;

	@Lob
	@Column(name = "contents", nullable = false)
	@AccessType("property")
	private String contents;

	@Column(name = "form", nullable = false, length = 120)
	private String storedFormName;

	@Transient
	private String formName;

	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "project_id", nullable = false)
	private Project project;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "created_dt", nullable = false)
	private Date creationDate = new Date();

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "updated_dt", nullable = true)
	private Date updatedDate;

	@Transient
	private boolean isDirty = false;

	@Transient
	private boolean isBeingEdited;

	FormSubmission() {
		// --- For Hibernate's use.
	}

	// --- Used to create variables.
	public FormSubmission(boolean unused) {
		this(null, new LinkedHashMap<String, String[]>());
	}

	// --- Copy constructor
	public FormSubmission(FormSubmission anotherOne) {
		this.formName = anotherOne.formName;
		this.isDirty = true;
		this.project = anotherOne.project;
		this.storedFormName = anotherOne.storedFormName;

		fieldContents = new LinkedHashMap<String, String[]>();
		for (Map.Entry<String, String[]> anotherEntry : anotherOne.fieldContents
				.entrySet()) {
			// -- Values are not being "deep-copied" because by design the array
			// it holds it is immutable.
			fieldContents.put(anotherEntry.getKey(), anotherEntry.getValue());
		}
	}

	public FormSubmission(UserProject userProject, Form form) {
		this(form, new LinkedHashMap<String, String[]>());
		this.project = getProject(form, userProject);
	}

	public FormSubmission(UserProject userProject, Form form,
			ExecutionContext context) {
		this(form, makeFieldContents(context, form));
		this.project = getProject(form, userProject);
	}

	public FormSubmission(User user, DataSource dataSource) {
		this.fieldContents = new LinkedHashMap<String, String[]>();
		this.isDirty = false;
		this.project = user.getSharedStorage();
		this.storedFormName = dataSource.getName();
	}

	private Project getProject(Form form, UserProject userProject) {
		return form.isSharedData() ? userProject.getUser().getSharedStorage()
				: userProject.getProject();
	}

	protected FormSubmission(Form form, Map<String, String[]> fieldContents) {
		this.fieldContents = fieldContents;
		this.formName = form == null ? null : form.getName();
		this.storedFormName = form == null ? null : (form.isSharedData() ? form
				.getDataSourceName() : form.getName());
		this.isDirty = true;
	}

	private static Map<String, String[]> makeFieldContents(
			ExecutionContext context, Form form) {
		Map<String, String[]> result = new LinkedHashMap<String, String[]>();

		FormSubmission old = context.getPreviousSubmission(form.getName());
		if (old != null)
			result.putAll(old.fieldContents);

		populateWithValuesFromPreviousRequest(result, context, form);

		return result;
	}

	private static void populateWithValuesFromPreviousRequest(
			Map<String, String[]> result, ExecutionContext context, Form form) {
		FormSegment previousSegment = form.getPreviousSegment(context
				.getRequest());
		if (previousSegment == null) {
			return;
		}
		for (Field field : previousSegment.fields()) {
			if (field.expectsFileUpload()) {
				MultipartFile file = context.getRequest().getUploadedFile(field.getHtmlId());
				if(file != null && file.getSize() != 0) {
					String fileURL = storeFile(context.getProjectOwner(), context.getUserProject(), file);
					result.put(field.getHtmlId(), new String[] {fileURL});
				} else {
					result.remove(field.getHtmlId());
				}
			} else {
				List<String> parameterValues = context.getRequest()
						.getParameterValues(field.getHtmlId());
				result.put(field.getHtmlId(), copyToArray(parameterValues));
			}
		}
	}
	
	private static String storeFile(User user, UserProject userProject, MultipartFile file) {
		try {
			UserUploadedFile savedFile = new UserUploadedFile(user, userProject,
					file.getBytes(), file.getContentType(), file
							.getOriginalFilename(), (int) file.getSize());
			UsersHibernateImpl.saveUserUploadedFile(savedFile);
			return savedFile.getFileURL();
		} catch (IOException e) {
			throw new IllegalStateException("Error reading uploaded file:", e);
		}
	}


	private void setValue(Field field, List<String> fieldValues) {
		fieldContents.put(field.getHtmlId(), copyToArray(fieldValues));
		isDirty = true;
	}

	private static String[] copyToArray(List<String> values) {
		if (values == null)
			return new String[0];

		String[] result = new String[values.size()];
		int i = 0;
		for (String value : values) {
			result[i++] = value;
		}
		return result;
	}

	public Value getValue(Reference reference) {
		return getValue(reference.getFieldName());
	}

	public Value getValue(String fieldName) {
		String[] values = fieldContents.get(fieldName);
		Value result = (values == null || values.length == 0) ? Value.NULL
				: new Value(values[0]);
		return result;
	}

	public boolean isFieldSet(Reference reference) {
		String[] values = fieldContents.get(reference.getFieldName());
		return values != null && values.length > 0;
	}

	public List<Value> getValues(Reference reference) {
		return getValues(reference.getFieldName());
	}

	public List<Value> getValues(String fieldName) {
		String[] values = fieldContents.get(fieldName);
		if (values == null)
			return Collections.emptyList();

		List<Value> result = new ArrayList<Value>(values.length);
		for (String value : values) {
			result.add(new Value(value));
		}

		return result;
	}

	public Collection<String> getFieldIds() {
		return fieldContents.keySet();
	}

	public void setValue(final String fieldId, String value) {
		String[] parts = fieldId.split(":", 2);
		String id = parts.length == 2 && getFormName().equals(parts[0]) ? parts[1]
				: fieldId;

		setValue(new DynamicField(id), Collections.singletonList(value));
		isDirty = true;
	}

	public void clearValue(final String fieldId) {
		Field field = new DynamicField(fieldId);
		fieldContents.remove(field.getHtmlId());
		isDirty = true;
	}

	public String toString() {
		StringBuilder result = new StringBuilder();
		result.append("FormSubmission{id=");
		result.append(databaseId);
		result.append(", form=").append(formName);
		result.append(", fieldContents={");

		for (Map.Entry<String, String[]> mapEntry : fieldContents.entrySet()) {
			result.append(" ").append(mapEntry.getKey()).append("=[");
			boolean firstValue = true;
			for (String value : mapEntry.getValue()) {
				if (firstValue) {
					firstValue = false;
				} else {
					result.append(", ");
				}
				result.append(value);
			}
			result.append(']');
		}

		result.append("}}");
		return result.toString();
	}

	public boolean contains(Reference reference, String value) {
		return getValues(reference).contains(new Value(value));
	}

	public void clearItems(List<String> itemIds) {
		for (String itemId : itemIds) {
			clearValue(itemId);
		}
		isDirty = true;
	}

	public void clearAllData() {
		fieldContents.clear();
		isDirty = true;
	}

	/**
	 * A hack to get dynamic fields working. TODO: refactor dynamic fields more
	 * sensibly.
	 */
	private static class DynamicField implements Field {
		private final String fieldId;

		public DynamicField(String fieldId) {
			this.fieldId = fieldId;
		}

		public String getId() {
			return fieldId;
		}

		public String getHtmlId() {
			return fieldId;
		}

		public boolean isRequired() {
			return false;
		}

		public StoredField getStoredFieldDefinition() {
			throw new IllegalStateException(this.getClass().getName()
					+ " doesn't support getStoredFieldDefinition()");
		}

		public boolean canBeUsedToReplaceText() {
			return false;
		}

		public FieldSetter getFieldSetter() {
			throw new IllegalStateException(
					"This method should never be called for this class.");
		}

		public JSONObject getDataTableEditorInfo(UserProject userProject) {
			throw new IllegalStateException(
					"Method getDataTableEditorInfo is not implemented");
		}

		public boolean expectsFileUpload() {
			return false;
		}

		public List<FieldValidator> getFieldValidators() {
			return null;
		}

		public boolean wasDisplayedOnPreviousPage(ExecutionContext context) {
			throw new IllegalStateException("Unexpected call to " + this.getClass());
		}
	}

	String getContents() {
		if (contents != null && !isDirty) {
			return contents;
		}

		StringWriter writer = new StringWriter();
		try {
			writer.write(xstream.toXML(this.fieldContents));
		} finally {
			try {
				writer.close();
			} catch (IOException e) {
				throw new IllegalStateException(
						"Unable to close StringWriter:", e);
			}
		}

		return writer.toString();
	}

	@SuppressWarnings("unchecked")
	void setContents(String contents) {
		this.contents = contents;
		StringReader reader = new StringReader(contents);
		try {
			fieldContents = (Map<String, String[]>) xstream.fromXML(reader);
		} catch (Exception e) {
			throw new IllegalArgumentException(
					"Failed to convert contents from CLob:", e);
		}
		isDirty = false;
	}

	/**
	 * @return Returns the creationDate.
	 */
	public Date getCreationDate() {
		return creationDate;
	}

	/**
	 * @param creationDate
	 *            The creationDate to set.
	 */
	public void setCreationDate(Date creationDate) {
		this.creationDate = creationDate;
	}

	/**
	 * @return Returns the formName.
	 */
	public String getFormName() {
		return formName == null ? storedFormName : formName;
	}

	/**
	 * @return Returns the project.
	 */
	public Project getProject() {
		return project;
	}

	/**
	 * @param project
	 *            The project to set.
	 */
	public void setProject(Project project) {
		this.project = project;
	}

	public long getDatabaseId() {
		return databaseId;
	}

	/*
	 * It's a copy by reference. In theory it's not quite correct, but this
	 * method only applies to one case and one of the submission ceases to be
	 * used right after that.
	 */
	public void copyFieldsFrom(FormSubmission submission) {
		this.fieldContents = submission.fieldContents;
		isDirty = true;
	}

	public void setFormName(String formName) {
		this.formName = formName;
	}

	public void copyDeclaredFieldsTo(FormSubmission submission) {
		Form form = project.getForm(this.getFormName());
		if (form == null) {
			return;
		}

		List<DeclaredField> declaredFields = form.getDeclaredFields();
		if (declaredFields != null) {
			for (DeclaredField field : declaredFields) {
				Value value = getValue(new Reference(field.getId()));
				submission.setValue(field.getId(), value.toString());
			}
		}

	}

	public void setValues(String fieldId, List<String> values) {
		fieldContents.put(fieldId, copyToArray(values));
		isDirty = true;
	}

	public void extractPostedData(ExecutionContext context) {
		populateWithValuesFromPreviousRequest(fieldContents, context, context
				.getProject().getForm(getFormName()));
		isDirty = true;
	}

	public boolean isBeingEdited() {
		return isBeingEdited;
	}

	public void setBeingEdited(boolean isBeingEdited) {
		this.isBeingEdited = isBeingEdited;
	}

	public boolean isAlreadyStored() {
		return databaseId != 0;
	}

	public Date getUpdatedDate() {
		return updatedDate;
	}

	public void setUpdatedDate(Date updatedDate) {
		this.updatedDate = updatedDate;
	}
}
