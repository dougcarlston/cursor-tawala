package com.tawala.project;

import com.tawala.component.web.ResponseCreator;

abstract public class FormRenderableNotHoldingActiveComponents implements FormRenderable {

	final public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		return null;
	}
}
