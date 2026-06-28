package com.tawala.web.jforum;

import java.net.URLEncoder;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

import javax.servlet.http.Cookie;

// ONLY EDIT IN THE TAWALA PROJECT. Don't edit the JForum copy. TODO: make this a library.

public class ForumIntegrationToken {
    public static final String ANONYMOUS_FORUM_USER_ID = "tawala";
    public static final String COOKIE_ID = "tawala_forum_info";

    private final String userId;

    public ForumIntegrationToken(String userId) {
        this.userId = userId;
    }

    public Cookie asCookie() {
        String value = userId;
        Cookie cookie = new Cookie(COOKIE_ID, value + "|" + sign(value));
        cookie.setPath("/");
		return cookie;
    }

    @SuppressWarnings({"deprecation"})
    private static String sign(String s) {
        String contents = s + "-tawala-chibbolex";
        MessageDigest md = null;
        try {
            md = MessageDigest.getInstance("SHA");
        } catch (NoSuchAlgorithmException e) {
            System.err.println("unexpected failure finding algorithm SHA");
            e.printStackTrace(System.err);
            return e.toString();
        }
        byte[] raw = md.digest(contents.getBytes());
        return URLEncoder.encode(new String(raw)).replaceAll("%", "");
    }

    public static String extractUserId(String cookieValue) {
        try {
            String[] strings = cookieValue.split("\\|");
            String name = strings[0];
//            System.out.println("name = " + name);
            String maybeSignature = strings[1];
//            System.out.println("maybeSignature  = " + maybeSignature);
            String reallySignature = sign(name);
//            System.out.println("reallySignature = " + reallySignature);
            if (reallySignature.equals(maybeSignature)) {
//                System.out.println("Yes, it's " + name);
                return name;
            }
            return null;
        } catch (Exception e) {
            System.err.println("Weird exception during SSO decrypt");
            e.printStackTrace(System.err);
            return null;
        }
    }
}
