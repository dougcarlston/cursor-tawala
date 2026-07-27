package com.tawala.web.sportsdashboard;


import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.scissor.Log;
import com.tawala.email.EmailService;
import com.tawala.email.SportsDashboardContactUsEmail;
import com.tawala.web.controller.WellKnown;

public class ContactUsController extends SimpleFormController {
	public static class Form {
		private String name;
		private String phone;
		private String URL;
		private String email;
		private String sport;
		private String additionalInfo;
		private String spamTrap;

		public String getAdditionalInfo() {
			return additionalInfo;
		}

		public void setAdditionalInfo(String additionalInfo) {
			this.additionalInfo = additionalInfo;
		}

		public String getEmail() {
			return email;
		}

		public void setEmail(String email) {
			this.email = email;
		}

		public String getName() {
			return name;
		}

		public void setName(String name) {
			this.name = name;
		}

		public String getURL() {
			return URL;
		}

		public void setURL(String URL) {
			this.URL = URL;
		}

		public String getPhone() {
			return phone;
		}

		public void setPhone(String phone) {
			this.phone = phone;
		}

		public String getSport() {
			return sport;
		}

		public void setSport(String sport) {
			this.sport = sport;
		}

		public String getSpamTrap() {
			return spamTrap;
		}

		public void setSpamTrap(String spamTrap) {
			this.spamTrap = spamTrap;
		}

	}

	public ContactUsController() {
		setFormView("sportsdashboards.contactus");
		setSuccessView("redirect:" + WellKnown.urls.getSportsEmailConfirmation());
		setCommandClass(Form.class);
		setCommandName("form");
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;
		if (form.getSpamTrap().length() > 0) {
			Log.error(this, "Attempt to spam - caught typed invisible field: '"
					+ form.getSpamTrap() + "'");
		} else {
			SportsDashboardContactUsEmail email = new SportsDashboardContactUsEmail(
					form.getName(), form.getURL(), form.getEmail(), form.getSport());
			
			EmailService.sendAndStoreEmail(email);
		}

		return new ModelAndView(getSuccessView());
	}
}
