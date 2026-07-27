package com.tawala.component;

import com.scissor.xmlconfig.ConfigElement;

public abstract class ComponentRuntimeSupport {
	private Integer version;
	
	public ComponentRuntimeSupport(ConfigElement configElement) {
		this.version = configElement.attribute("version").intValue();
	}
	
	public ComponentRuntimeSupport() {
		this.version = null;
	}

	public int getVersion() {
		if(version == null) {
			throw new IllegalStateException("Version has never been extracted.");
		}
		return version;
	}
}
