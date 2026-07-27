package com.tawala.component.web;

import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.Response;

public interface ResponseCreator {
	Response createResponse(ExecutionContext context);
}
