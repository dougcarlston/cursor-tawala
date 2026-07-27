package com.tawala.acceptance.user;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.web.controller.VisitorTrackerInterceptor;
import com.tawala.web.controller.WellKnown;

public class VisitorTrackingTest extends AcceptanceTestCase {

	public void testCookieHandling() throws RobotException {
		//--- Fresh start - visitor's cookie is set.
		bot.go(WellKnown.urls.getLibrarySearch());
		String cookieValue = bot
				.getCookieValue(VisitorTrackerInterceptor.TAWALA_TOKEN_COOKIE_NAME);
		assertNotNull(cookieValue);
		assertTrue(cookieValue
				.startsWith(VisitorTrackerInterceptor.VISITOR_PREFIX));

		//--- Upon login user's cookie is set
		bot.logInAs(projectOwner);
		cookieValue = bot
				.getCookieValue(VisitorTrackerInterceptor.TAWALA_TOKEN_COOKIE_NAME);
		assertNotNull(cookieValue);
		assertTrue(cookieValue
				.startsWith(VisitorTrackerInterceptor.USER_PREFIX));
		
		//--- User's cookie persists after the logout.
		bot.logOut();
		bot.go(WellKnown.urls.getLibrarySearch());
		cookieValue = bot
				.getCookieValue(VisitorTrackerInterceptor.TAWALA_TOKEN_COOKIE_NAME);
		assertNotNull(cookieValue);
		assertTrue(cookieValue
				.startsWith(VisitorTrackerInterceptor.USER_PREFIX));

	}
}
