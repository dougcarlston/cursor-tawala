package com.tawala.web.search;

import java.io.Serializable;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

public class SearchProjectController extends ProjectSearchSupport implements
		Controller {
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		Object[] parameters = readObjects(request, 2);
		String query = (String) parameters[0];
		Integer libraryId = (Integer)parameters[1];
		
		List<Long> result = getProjectIndexer().search(libraryId.intValue(), query);
		writeObject(response, (Serializable) result);

		return null;
	}
}
