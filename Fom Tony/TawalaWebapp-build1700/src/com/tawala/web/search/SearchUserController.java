package com.tawala.web.search;

import java.io.Serializable;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.lucene.queryParser.ParseException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

public class SearchUserController extends UserSearchSupport implements
		Controller {
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String query = readObject(request);
		try {
			List<Long> result = getUserIndexer().searchForUser(query);
			writeObject(response, (Serializable) result);
		} catch (ParseException e) {
			writeObject(response, (Serializable)e);
		}

		return null;
	}
}
