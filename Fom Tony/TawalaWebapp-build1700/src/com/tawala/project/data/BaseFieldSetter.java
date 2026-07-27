package com.tawala.project.data;


public abstract class BaseFieldSetter implements FieldSetter {
	private final String fieldName;
	
	public BaseFieldSetter(String fieldName) {
		this.fieldName = fieldName;
	}

	public String getFieldName() {
		return fieldName;
	}
}
