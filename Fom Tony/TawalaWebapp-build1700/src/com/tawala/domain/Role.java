package com.tawala.domain;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

@Entity
@Table(name = "role")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "users")
public class Role {
	public static final List<Role> PREDEFINED_ROLES = new ArrayList<Role>();
	static {
		PREDEFINED_ROLES.add(new Role("sales", "Sales"));
		PREDEFINED_ROLES.add(new Role("customer service", "Customer Service"));
		PREDEFINED_ROLES.add(new Role("accounting", "Accounting"));
	}
	
	@Id
	@Column(name = "role_id", length=50)
	private String roleId;

	@Column(name = "description", length=255)
	private String description;

	Role() {
		// For Hibernate's use
	}

	public Role(String roleId, String description) {
		this.roleId = roleId;
		this.description = description;
	}

	public String getRoleId() {
		return roleId;
	}

	public String getDescription() {
		return description;
	}

	@Override
	public boolean equals(Object obj) {
		Role other = (Role) obj;
		return this.roleId.equals(other.roleId);
	}
	
	@Override
	public int hashCode() {
		return roleId.hashCode();
	}
	
	@Override
	public String toString() {
		return "Role[" + roleId + "]";
	}
}
