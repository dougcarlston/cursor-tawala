package com.tawala.project.commands;

public class Reference {
	private final String stringPresentation;
	private final String recordName;
	private final String formName;
	private final String fieldName;

	public Reference(String stringPresentation, ExecutionContext context) {
		this.stringPresentation = stringPresentation;
		String[] pieces = tokenize(stringPresentation);

		String fieldNameCandidate;
		if (pieces.length == 1) {
			recordName = null;
			formName = null;
			fieldNameCandidate = pieces[0];
		} else if (pieces.length == 3 && context.containsRecordNamed(pieces[0])) {
			recordName = pieces[0];
			formName = pieces[1];
			fieldNameCandidate = pieces[2];
		} else if (pieces.length == 3
				&& context.getProject().getForm(pieces[0]) != null) {
			recordName = null;
			formName = pieces[0];
			fieldNameCandidate = pieces[1] + ':' + pieces[2];
		} else if (pieces.length == 2
				&& context.getProject().getForm(pieces[0]) != null) {
			recordName = null;
			formName = pieces[0];
			fieldNameCandidate = pieces[1];
		} else {
			recordName = null;
			formName = null;
			fieldNameCandidate = stringPresentation;
		}

		fieldName = context.resolveMCQSelectionIfNecessary(fieldNameCandidate);
	}

	private static String[] tokenize(String stringPresentation) {
		String[] pieces = stringPresentation.split(":", 3);
		return pieces;
	}

	public Reference(String fieldName) {
		this.stringPresentation = fieldName;
		this.recordName = null;
		this.formName = null;
		this.fieldName = fieldName;
	}
	
	public Reference(String fieldName, boolean forceBreaking) {
		this.stringPresentation = fieldName;
		String [] tokens = tokenize(stringPresentation);

		switch (tokens.length) {
		case 1:
			this.recordName = null;
			this.formName = null;
			this.fieldName = tokens[0];
			break;
			
		case 2:
			this.recordName = null;
			this.formName = tokens[0];
			this.fieldName = tokens[1];
			break;

		case 3:
			this.recordName = tokens[0];
			this.formName = tokens[1];
			this.fieldName = tokens[2];
			break;

		default:
			throw new IllegalStateException("Unexpected number of tokens: " + stringPresentation);
		}
	}

	public Reference(String formName, String fieldName) {
		this.stringPresentation = formName + ":" + fieldName;
		this.recordName = null;
		this.formName = formName;
		this.fieldName = fieldName;
	}

	public String getFieldName() {
		return fieldName;
	}

	public String getFormName() {
		return formName;
	}

	public String getRecordName() {
		return recordName;
	}

	public String getStringPresentation() {
		return stringPresentation;
	}

	public boolean isVariable() {
		return formName == null;
	}

	public boolean isRecordReference() {
		return recordName != null;
	}

	@Override
	public String toString() {
		return stringPresentation;
	}

	@Override
	public int hashCode() {
		final int PRIME = 31;
		int result = 1;
		result = PRIME * result + ((stringPresentation == null) ? 0 : stringPresentation.hashCode());
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		final Reference other = (Reference) obj;
		if (stringPresentation == null) {
			if (other.stringPresentation != null)
				return false;
		} else if (!stringPresentation.equals(other.stringPresentation))
			return false;
		return true;
	}
}
