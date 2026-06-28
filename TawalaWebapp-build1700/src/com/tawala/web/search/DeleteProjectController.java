package com.tawala.web.search;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

public class DeleteProjectController extends ProjectSearchSupport implements
		Controller {
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		Long projectId = readObject(request);
		getProjectIndexer().delete(projectId);

		return null;
	}
}
