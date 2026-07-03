package com.tawala.acceptance.user;

import java.io.UnsupportedEncodingException;

import org.springframework.mail.javamail.JavaMailSenderImpl;
import org.springframework.mock.web.MockHttpServletRequest;
import org.springframework.mock.web.MockHttpServletResponse;
import org.springframework.web.servlet.ModelAndView;

import com.meterware.httpunit.WebForm;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.DomainMetadata;
import com.tawala.domain.User;
import com.tawala.email.Emailer;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.user.UserRegistrationController;

import fake.smtp.FakeSmtpMessage;
import fake.smtp.FakeSmtpServer;

public class UserRegistrationTest extends AcceptanceTestCase {
	private static final String USER_NAME = "joetester";
	private FakeSmtpServer server;

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		User user = world.domain().users().get(USER_NAME);
		if (user != null) {
			world.domain().users().delete(user);
		}

		server = new FakeSmtpServer();

		JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
		senderImpl.setPort(server.getPort());
		senderImpl.setHost("127.0.0.1");
		new Emailer().setSender(senderImpl);
	}

	@Override
	protected void tearDown() throws Exception {
		if (server != null)
			server.shutDown();
		super.tearDown();
	}

	public void testFailedValidationLogic() throws Exception {
		MockHttpServletRequest request = new MockHttpServletRequest("POST",
				WellKnown.urls.getUserRegistration());
		MockHttpServletResponse response = new MockHttpServletResponse();

		ModelAndView modelAndView = new UserRegistrationController()
				.handleRequest(request, response);
		assertNotNull(modelAndView);

		assertEquals("View redirected back to the view screen", "registration",
				modelAndView.getViewName());
	}

	public void testGoRightLogic() throws Exception {
		MockHttpServletRequest request = new MockHttpServletRequest("POST",
				WellKnown.urls.getUserRegistration());
		MockHttpServletResponse response = new MockHttpServletResponse();
		request.setParameter("user.firstName", "Joe");
		request.setParameter("user.lastName", "Smith");
		request.setParameter("emailAddress", "joe@example.com");
		request.setParameter("user.id", USER_NAME);
		request.setParameter("password", "abc");
		request.setParameter("repeatedPassword", "abc");

		ModelAndView modelAndView = new UserRegistrationController()
				.handleRequest(request, response);
		assertNotNull(modelAndView);

		assertEquals("Sent to confirmation screen",
				"registration.confirmation", modelAndView.getViewName());

		/*
		 * TODO: Until we change the logic of registration
		 * server.waitForAllConnectionsToClose(); assertEquals(1,
		 * server.getMessageCount()); validateEmail(world.domain().users().get(
		 * USER_NAME), server.getMessage(0));
		 */
	}

	public void testWebInterface() throws Exception {
		bot.go(WellKnown.urls.getUserRegistration());
		assertContains("First Name:", bot.lastResponse().getText());

		WebForm form = bot.getForm(0);
		assertTrue("Form has parameter user.firstName", form
				.hasParameterNamed("user.firstName"));

		// Submit a completed form
		form = bot.getForm(0);
		form.setParameter("user.firstName", "Joe");
		form.setParameter("user.lastName", "Smith");
		form.setParameter("emailAddress", "joe@example.com");
		form.setParameter("user.id", USER_NAME);
		form.setParameter("password", "abc");
		form.setParameter("repeatedPassword", "abc");

		bot.submit(form, "submit");
		assertContains("Thanks for registering as a Tawala Designer.", bot
				.lastResponse().getText());

		/*
		 * TODO: Until we change the logic of registration
		 * server.waitForAllConnectionsToClose(); assertEquals(1,
		 * server.getMessageCount()); validateEmail(world.domain().users().get(
		 * USER_NAME), server.getMessage(0));
		 */
	}

	public void testLongEmailAddress() throws Exception {
		bot.go(WellKnown.urls.getUserRegistration());

		WebForm form = bot.getForm(0);

		StringBuilder emailAddress = new StringBuilder("joe@");
		String suffix = ".com";
		for (int i = emailAddress.length(); i < DomainMetadata.instance
				.getUserEmailAddressMaxLength()
				- suffix.length(); i++) {
			emailAddress.append('x');
		}
		emailAddress.append(suffix);

		form = bot.getForm(0);
		form.setParameter("user.firstName", "Joe");
		form.setParameter("user.lastName", "Smith");
		form.setParameter("emailAddress", emailAddress.toString());
		form.setParameter("user.id", USER_NAME);
		form.setParameter("password", "abc");
		form.setParameter("repeatedPassword", "abc");

		bot.submit(form, "submit");
		assertContains("Thanks for registering as a Tawala Designer.", bot
				.lastResponse().getText());

		User savedUser = WorldInitializer.getDefaultWorld().domain().users()
				.get(USER_NAME);
		assertEquals(emailAddress.toString(), savedUser.getEmail().toString());

	}

	@SuppressWarnings("unused")
	private void validateEmail(User user, FakeSmtpMessage message)
			throws UnsupportedEncodingException {
		assertNotNull("user", user);
		assertNotNull("message", message);

		String body = message.getBody();
		assertContains("Dear Joe", body);
		assertContains(WellKnown.urls.getEmailConfirmation(), body);
		assertContains(user.constructEmailValidationURI(), body);

		assertEquals(user.getEmail().toString(), message.getRecipients().get(0));
	}
}
