package com.tawala.web.payment.paypal;

import java.math.BigDecimal;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.payment.paypal.InstantPaymentNotification;
import com.tawala.project.ProjectsHibernateImpl;

public class InstantPaymentNotificationController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		InstantPaymentNotification notification = new InstantPaymentNotification(request);
		ProjectsHibernateImpl.processPaypalNotification(notification);

		Log.info(this, "Received PayPal's instant payment notification.");
		BigDecimal paymentAmount = notification.getPaymentAmount();
		EventService.createEvent(new Event("Received Paypal IPN", paymentAmount == null ? "not specified" : paymentAmount.toPlainString()));
		
		return null;
	}
}
