package com.tawala.acceptance;

import java.io.IOException;
import java.net.MalformedURLException;
import java.util.Date;

import org.xml.sax.SAXException;

import com.meterware.httpunit.GetMethodWebRequest;
import com.meterware.httpunit.WebClient;
import com.meterware.httpunit.WebResponse;
import com.scissor.webrobot.RobotException;
import com.scissor.webrobot.WebRobot;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.BaseAuthenticationInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.jforum.ForumIntegrationToken;
import com.tawala.web.user.UserAccessTicket;

public class AuthTest extends AcceptanceTestCase {

	public void testBasicLoginAndLogout() throws IOException, SAXException,
			RobotException {
		// visit home page
		bot.goHome();
		bot.followLink("linkToLogin");

		assertTrue(bot.hasCookie(ForumIntegrationToken.COOKIE_ID));
		assertEquals(ForumIntegrationToken.ANONYMOUS_FORUM_USER_ID,
				ForumIntegrationToken.extractUserId(bot
						.getCookieValue(ForumIntegrationToken.COOKIE_ID)));
		assertTrue(!bot
				.hasCookie(BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME));

		// log in
		bot.setParameter("userName", projectOwner.getId());
		bot.setParameter("password", projectOwner.getPassword());
		bot.submit();
		assertEquals(WellKnown.urls.getSportsHome(), bot.getPath().localPart());
		assertContains(projectOwner.getId(), bot.getPageText());
		assertTrue(bot.hasCookie("tawala_forum_info"));
		assertEquals(projectOwner.getId(), ForumIntegrationToken
				.extractUserId(bot
						.getCookieValue(ForumIntegrationToken.COOKIE_ID)));

		User retrieved = WorldInitializer.getDefaultWorld().domain().users()
				.get(projectOwner.getId());
		assertNotNull(retrieved.getLastLoggedInDate());

		// log out again
		bot.followLink("Logout");
		assertEquals(WellKnown.urls.getSportsHome(), bot.getPath().localPart());
		assertNotNull("login link", bot.getLink("linkToLogin"));

		assertEquals(ForumIntegrationToken.ANONYMOUS_FORUM_USER_ID,
				ForumIntegrationToken.extractUserId(bot
						.getCookieValue(ForumIntegrationToken.COOKIE_ID)));
	}

	public void testLoginAndLogoutWhenKeepingUserSignedIn() throws IOException,
			SAXException, RobotException {
		// visit home page
		bot.goHome();
		bot.followLink("linkToLogin");

		assertTrue(!bot
				.hasCookie(BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME));

		// log in
		bot.setParameter("userName", projectOwner.getId());
		bot.setParameter("password", projectOwner.getPassword());
		bot.setCheckbox("keepSignedIn", true);
		bot.submit();

		assertEquals(WellKnown.urls.getSportsHome(), bot.getPath().localPart());
		assertContains(projectOwner.getId(), bot.getPageText());
		assertTrue(bot.hasCookie("tawala_forum_info"));
		assertEquals(projectOwner.getId(), ForumIntegrationToken
				.extractUserId(bot
						.getCookieValue(ForumIntegrationToken.COOKIE_ID)));
		String accessToken = bot
				.getCookieValue(BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME);
		assertNotNull(accessToken);

		User retrieved = WorldInitializer.getDefaultWorld().domain().users()
				.get(projectOwner.getId());
		assertNotNull(retrieved.getLastLoggedInDate());

		UserAccessTicket accessTicket = UsersHibernateImpl
				.retrieveAccessTicket(accessToken);
		assertNotNull(accessTicket);
		assertEquals(retrieved, accessTicket.getUser());

		// log out
		bot.followLink("Logout");
		assertEquals(WellKnown.urls.getSportsHome(), bot.getPath().localPart());
		assertNotNull("login link", bot.getLink("linkToLogin"));

		assertEquals(ForumIntegrationToken.ANONYMOUS_FORUM_USER_ID,
				ForumIntegrationToken.extractUserId(bot
						.getCookieValue(ForumIntegrationToken.COOKIE_ID)));
		accessToken = bot
				.getCookieValue(BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME);
		assertTrue(accessToken == null || accessToken.length() == 0);

		accessTicket = UsersHibernateImpl.retrieveAccessTicket(accessToken);
		assertNull(accessTicket);

	}

	public void testKeepingSignedIn() throws IOException, SAXException,
			RobotException, InterruptedException {
		// visit home page
		bot.goHome();
		bot.followLink("linkToLogin");

		// log in
		bot.setParameter("userName", projectOwner.getId());
		bot.setParameter("password", projectOwner.getPassword());
		bot.setCheckbox("keepSignedIn", true);
		bot.submit();

		String accessToken = bot
				.getCookieValue(BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME);
		assertTrue(accessToken != null && accessToken.length() != 0);

		// --- Test various parts of the site that are covered by different
		// interseptors.
		validateNavigationCreatesSession(accessToken, WellKnown.urls.getHome());
		validateNavigationCreatesSession(accessToken, WellKnown.urls
				.getAdminManageUsers());
		validateNavigationCreatesSession(accessToken, WellKnown.urls
				.getProjectManagerView());
	}

	private void validateNavigationCreatesSession(String accessToken, String url)
			throws MalformedURLException, IOException, SAXException,
			InterruptedException {
		UserAccessTicket accessTicket = UsersHibernateImpl
				.retrieveAccessTicket(accessToken);
		Date lastLoggedInDate = accessTicket.getUser().getLastLoggedInDate();
		Date lastTicketUseDate = accessTicket.getLastUsed();

		// --- Simulate a new browser window.
		WebClient client = runner.newClient();
		client.putCookie(
				BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME,
				accessToken);

		WebResponse response = client.getResponse(new GetMethodWebRequest(
				WebRobot.URL_PREFIX + url));
		assertContains(projectOwner.getId(), response.getText());

		accessTicket = UsersHibernateImpl.retrieveAccessTicket(accessToken);

		// --- For some weird reason before() and after() didn't work here....
		assertEquals("Last login date changed", 1, accessTicket.getUser()
				.getLastLoggedInDate().compareTo(lastLoggedInDate));
		if (lastTicketUseDate == null) {
			assertNotNull(accessTicket.getLastUsed());
		} else {
			assertEquals("Last ticket use date", 1, accessTicket.getLastUsed()
					.compareTo(lastTicketUseDate));
		}
	}

	public void testBadUser() throws IOException, SAXException, RobotException {
		bot.goHome();
		bot.followLink("linkToLogin");

		bot.setParameter("userName", "fnord");
		bot.setParameter("password", projectOwner.getPassword());
		bot.submit();
		assertTrue(bot.getPath().localPart().startsWith(
				WellKnown.urls.getLogin()));
		assertDoesntContain("Hello, " + projectOwner.getId(), bot.getPageText());
		assertContains("Tawala Login", bot.getPageText());
		// TODO: error messages
	}

	public void testBadPassword() throws IOException, SAXException,
			RobotException {
		bot.goHome();
		bot.followLink("linkToLogin");

		bot.setParameter("userName", projectOwner.getId());
		bot.setParameter("password", "wrongpass");
		bot.submit();
		assertTrue(bot.getPath().localPart().startsWith(
				WellKnown.urls.getLogin()));
		assertDoesntContain("Hello, " + projectOwner.getId(), bot.getPageText());
		assertContains("Tawala Login", bot.getPageText());
		// TODO: error messages
	}

	public void testNavigationToThePreviousNonSecuredPageUponLogin()
			throws IOException, SAXException, RobotException {
		String originalPage = WellKnown.urls.getLibrarySearch();
		bot.go(originalPage);
		bot.followLink("linkToLogin");

		// log in
		bot.setParameter("userName", projectOwner.getId());
		bot.setParameter("password", projectOwner.getPassword());
		bot.submit();

		assertEquals(originalPage, bot.getPath().localPart());

	}

	public void testNavigationToThePreviousSecuredPageUponLogin()
			throws IOException, SAXException, RobotException {
		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);
		
		//--- Tests admin URLs
		validateNavigationToSecurePage(WellKnown.urls.getAdminManageUsers());
		//--- Tests regular user URLs
		validateNavigationToSecurePage(WellKnown.urls.getProjectManagerView());
	}

	private void validateNavigationToSecurePage(String originalPage) throws RobotException {
		bot.logOut();
		bot.go(originalPage);
		assertTrue(bot.getPath().localPart().startsWith(WellKnown.urls.getLogin()));

		// log in
		bot.setParameter("userName", projectOwner.getId());
		bot.setParameter("password", projectOwner.getPassword());
		bot.submit();

		assertEquals(originalPage, bot.getPath().localPart());
	}
}
