package com.tawala.project.formatting;

import java.util.ArrayList;
import java.util.List;

import com.tawala.component.web.ResponseCreator;
import com.tawala.project.FormRenderable;
import com.tawala.project.commands.ExecutionContext;

public class ContainerElement {
	private final List<FormRenderable> contents = new ArrayList<FormRenderable>();
	
	public List<FormRenderable> getContents() {
		return contents;
	}
	
	public ContainerElement(List<FormRenderable> contents) {
		this.contents.addAll(contents);
	}

	public ContainerElement() {
	}

	public void addElement(FormRenderable element) {
		contents.add(element);
	}
	
	public void addElements(List<FormRenderable> otherElements) {
		contents.addAll(otherElements);
	}

	public boolean isEmpty(ExecutionContext context) {
		for (FormRenderable element : contents) {
			if(! element.isEmpty(context)) {
				return false;
			}
		}
		return true;
	}

	//--- See FormRenderable interface
	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		for (FormRenderable formRenderable : getContents()) {
			ResponseCreator responseCreator = formRenderable.getResponseCreatorForComponentId(componentId);
			if(responseCreator != null) {
				return responseCreator;
			}
		}
		return null;
	}
}
