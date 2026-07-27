package com.tawala.component.web;

import com.tawala.component.ComponentMetadata;
import com.tawala.project.FormRenderable;

public interface WebComponentMetadata extends ComponentMetadata {
	Class<FormRenderable> getRuntimeProcessingClass();
}
