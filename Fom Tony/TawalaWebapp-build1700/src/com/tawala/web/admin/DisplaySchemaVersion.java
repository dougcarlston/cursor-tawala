package com.tawala.web.admin;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.hibernate.TawalaSessionFactory;

public class DisplaySchemaVersion implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		response.getOutputStream().println(
				"Current default database schema version is "
						+ TawalaSessionFactory.MAIN.getDatabaseSchemaVersion()
								.getVersion()
						+ "."
						+ "\n"
						+ "Current backup database schema version is "
						+ TawalaSessionFactory.BACKUP
								.getDatabaseSchemaVersion().getVersion() + ".");
		return null;
	}

}
