package com.tawala.web.userdomain;

import java.util.Date;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.util.StringUtils;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.userdomain.Suggestion;
import com.tawala.userdomain.UserDomainStorage;

public class AddSuggestionController implements Controller {
	public static final String DOMAIN_PARAMETER = "domain";
	public static final String TEXT_PARAMETER = "text";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		Suggestion suggestion = new Suggestion();
		suggestion.setCreatedDate(new Date());
		suggestion.setDomainName(request.getParameter(DOMAIN_PARAMETER));
		suggestion.setSuggestion(request.getParameter(TEXT_PARAMETER));

		boolean requestIsValid = true;
		if (!StringUtils.hasText(suggestion.getDomainName())) {
			Log.error(this, "Domain name is empty.");
			requestIsValid = false;
		}
		if (!StringUtils.hasText(suggestion.getSuggestion())) {
			Log.error(this, "Suggestion is empty.");
			requestIsValid = false;
		}

		if (requestIsValid) {
			UserDomainStorage.createSuggestion(suggestion);
		}

		return null;
	}
}
