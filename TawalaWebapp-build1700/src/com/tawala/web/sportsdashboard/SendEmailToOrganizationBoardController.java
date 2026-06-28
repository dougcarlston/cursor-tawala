package com.tawala.web.sportsdashboard;

import java.util.ArrayList;
import java.util.List;

import javax.mail.internet.InternetAddress;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.scissor.Log;
import com.tawala.email.EmailService;
import com.tawala.email.SportsDashboardLetterToOrgBoardEmail;
import com.tawala.web.controller.WellKnown;

public class SendEmailToOrganizationBoardController extends
		SimpleFormController {
	public static class Form {
		private String name;
		private String email;
		private String message;
		private String spamTrap;
		private List<String> recipients = new ArrayList<String>(6);
		{
			for (int i = 0; i < 6; i++) {
				recipients.add("");
			}
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

		public String getSpamTrap() {
			return spamTrap;
		}

		public void setSpamTrap(String spamTrap) {
			this.spamTrap = spamTrap;
		}

		public String getMessage() {
			return message;
		}

		public void setMessage(String message) {
			this.message = message;
		}

		public List<String> getRecipients() {
			return recipients;
		}
	}

	public static class FormValidator implements Validator {

		public boolean supports(Class clazz) {
			return Form.class.equals(clazz);
		}

		public void validate(Object target, Errors errors) {
			Form form = (Form) target;
			if (!isValidEmail(form.getEmail())) {
				errors.rejectValue("email", "email.address.invalid");
			}

			int index = 0;
			boolean atLeastOneAddress = false;
			for (String recipient : form.getRecipients()) {
				index++;
				if (recipient.trim().length() == 0) {
					continue;
				}

				atLeastOneAddress = true;
				if (!isValidEmail(recipient)) {
					errors.rejectValue("recipients[" + (index - 1) + "]",
							"email.address.invalid");
				}
			}

			if (!atLeastOneAddress) {
				errors.rejectValue("recipients[0]",
						"sports-dashboard.email.at.least.one.address.required");
			}
		}

		public static boolean isValidEmail(String email) {
			if (email.indexOf('@') < 0) {
				return false;
			}

			try {
				new InternetAddress(email);
				return true;
			} catch (Exception e) {
				return false;
			}
		}
	}

	public SendEmailToOrganizationBoardController() {
		setFormView("sportsdashboards.emailboard");
		setSuccessView("redirect:"
				+ WellKnown.urls.getSportsEmailConfirmation());
		setCommandClass(Form.class);
		setCommandName("form");
		setValidator(new FormValidator());
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
			SportsDashboardLetterToOrgBoardEmail email = new SportsDashboardLetterToOrgBoardEmail(
					composeFromName(form), form.getRecipients(), form
							.getMessage());

			EmailService.sendAndStoreEmail(email);
		}

		return new ModelAndView(getSuccessView());
	}

	private static String composeFromName(Form form) {
		StringBuilder fromBuilder = new StringBuilder();
		if (form.getName().trim().length() > 0) {
			fromBuilder.append(form.getName());
			fromBuilder.append('<').append(form.getEmail()).append('>');
		} else {
			fromBuilder.append(form.getEmail());
		}

		return fromBuilder.toString();
	}
}
