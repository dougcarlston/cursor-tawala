package com.tawala.web.admin;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.web.controller.WellKnown;

public class UrgentMessageController extends SimpleFormController {
	private final ServletContext servletContext;
	
	public UrgentMessageController(ServletContext context) {
		setFormView("admin.manage.urgent.message");
		this.servletContext = context;
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;
		if (form.isRemoveMessage()) {
			UrgentMessage.remove(servletContext);
		} else {
			UrgentMessage.set(servletContext, form.getMessage());
		}

		response.sendRedirect(WellKnown.urls.getAdminManageUrgentMessage());
		return null;
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		UrgentMessage result = UrgentMessage.get(servletContext);
		if (result == null) {
			result = new UrgentMessage();
		} else {
			result = (UrgentMessage) result.clone();
		}
		
		return new Form(result);
	}

	public static class Form {
		private UrgentMessage message;
		private boolean removeMessage;

		public Form(UrgentMessage message) {
			this.message = message;
		}
		
		public UrgentMessage getMessage() {
			return message;
		}

		public boolean isRemoveMessage() {
			return removeMessage;
		}

		public void setRemoveMessage(boolean removeMessage) {
			this.removeMessage = removeMessage;
		}
	}
}
