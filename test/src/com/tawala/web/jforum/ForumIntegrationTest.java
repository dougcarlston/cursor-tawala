package com.tawala.web.jforum;

import javax.servlet.http.Cookie;

import com.tawala.TestCase;

public class ForumIntegrationTest extends TestCase {

    public void testCookieBasics() {
        ForumIntegrationToken token = new ForumIntegrationToken("bob");
        Cookie cookie = token.asCookie();
        assertEquals("tawala_forum_info", cookie.getName());
        assertEquals("bob", ForumIntegrationToken.extractUserId(cookie.getValue()));

        assertNull(ForumIntegrationToken.extractUserId("not" + cookie.getValue()));
        assertNull(ForumIntegrationToken.extractUserId(cookie.getValue() + "x"));


    }
}
