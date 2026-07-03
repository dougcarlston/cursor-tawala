package com.tawala.domain;

import java.util.Date;

import junit.framework.TestCase;


public class UserTest extends TestCase {

    public void testEmailValidationTokenGeneration() {
        User user = new User(Status.EMAIL_UNVALIDATED);
        user.generateEmailValidationToken();
        
        for (int i = 0; i < 100; i++) {
            String previousToken = user.getEmailValidationToken();
            assertNotSame("", previousToken);

            user.generateEmailValidationToken();
            assertNotSame(previousToken, user.getEmailValidationToken());
        }
    }
    
    public static User aUser() {
        String userId = "aUser";
        return aUser(userId);
    }

    public static User aUser(String userId) {
        String password = "ignored";
        return aUser(userId, password);
    }

    public static User aUser(String userId, String password) {
        User user = new User(userId, "Some", "User", new EmailAddress("userid@example.com"), password);
        user.setEmail(new EmailAddress("test@example.com"));
        user.setAdministrator(false);
        user.setPassword(password);
        user.setId(userId);
        user.setRegistrationDate(new Date());

        return user;
    }

	public static User aRegisteredUser() {
		User result = aUser();
		result.setStatus(Status.REGISTERED);
		return result;
	}
}
