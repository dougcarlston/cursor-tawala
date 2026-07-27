package com.tawala.domain;

import java.beans.PropertyEditorSupport;

import com.tawala.UsersHibernateImpl;

public class RoleEditor extends PropertyEditorSupport {
	@Override
	public String getAsText() {
		return ((Role)getValue()).getRoleId();
	}
	
	@Override
	public void setAsText(String text) throws IllegalArgumentException {
		Role role = UsersHibernateImpl.getRoleById(text);
		if(role == null) {
			throw new IllegalStateException("Unable to find role by id: '" + text + "'");
		}
		setValue(role);
	}
}
