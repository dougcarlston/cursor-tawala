package com.tawala.web.admin;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.component.repository.Repository;

public class DisplayComponentRepositoryController implements Controller {
	public static final String SIGNATURE_PARAMETER = "signature";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String clientSignature = request.getParameter(SIGNATURE_PARAMETER);
		response.setContentType("text/xml");
		response.getOutputStream().print(
				Repository.getXMLPresentation(clientSignature));

		return null;
	}
}
