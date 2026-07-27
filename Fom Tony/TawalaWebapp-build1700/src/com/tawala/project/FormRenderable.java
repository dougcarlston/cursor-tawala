package com.tawala.project;

import com.tawala.component.web.ResponseCreator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;

public interface FormRenderable {
    Html toHtml(ExecutionContext context);
	boolean isEmpty(ExecutionContext context);
	ResponseCreator getResponseCreatorForComponentId(String componentId);
}
