package com.tawala.web.user;

import java.util.Collections;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.util.StringUtils;
import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class AccountUpdateController extends BaseUserAccountEditController {
	public AccountUpdateController() {
		setFormView("user.account");
		setSuccessView("user.account");
	}

	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		return new UserForm(WorldInitializer.getDefaultWorld().domain().users()
				.get(
						UserInfoPreparationInterceptor.getSessionUser(request)
								.getId()));
	}

	protected void onBindAndValidate(HttpServletRequest request,
			Object command, BindException errors) throws Exception {

		UserForm form = (UserForm) command;
		commonUserValidation(errors, form);

		if (StringUtils.hasText(form.getPassword())) {
			validateNonEmptyPasswords(errors, form);
		}
	}

	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		UserForm form = (UserForm) command;
		User user = form.getUser();

		WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);

		Log.info(this, "Successfully updated user '" + form.getUser().getId()
				+ "'.");

		if (form.isSeeAdvancedFeatures()) {
			if (!user.getStatus().equals(Status.REGISTERED)) {
				user = WorldInitializer.getDefaultWorld().domain().users()
						.onUserUpgradeToFullyRegistered(user);
				EventService.createEvent(new Event(
						"RegistrationUpgradeFromEditAccount", request, user
								.getId()));
			}
		} else {
			if (!user.getStatus().equals(Status.REGISTERED_INITIAL)) {
				user = WorldInitializer.getDefaultWorld().domain().users()
						.onUserDowngradeToInitiallyRegistered(user);
				EventService.createEvent(new Event(
						"RegistrationDowngradeFromEditAccount", request, user
								.getId()));
			}
		}

		form.setUser(user);
		request.getSession().setAttribute("user", user);

		ModelAndView result = super
				.onSubmit(request, response, command, errors);
		result.addObject("updateSuccessful", true);

		return result;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see org.springframework.web.servlet.mvc.SimpleFormController#referenceData(javax.servlet.http.HttpServletRequest)
	 */
	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		return Collections.singletonMap("editUser", Boolean.TRUE);
	}

}
