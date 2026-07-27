package com.tawala.acceptance.user;

import java.io.IOException;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class PasswordChangeTest extends AcceptanceTestCase {
    private static final String USER_NAME = "joetester";
    
    public PasswordChangeTest() {
        setUserNamesToDelete(new String[] {USER_NAME});
    }

    public void testRedirectToPasswordChangeScreenOnLogin() throws Exception {
        bot.logOut();
        bot.go(WellKnown.urls.getUserPasswordChange());
        assertTrue(bot.getPath().localPart().startsWith(WellKnown.urls.getLogin()));
        
        User user = UserTest.aUser(USER_NAME, "123");
        user.setAdministrator(true);
        user.setRequirePasswordReset(true);
        WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);
        
        bot.logInAs(user.getId(), user.getPassword());

        // Testing a couple of URLs that required user authentication.
        bot.go(WellKnown.urls.getAdminManageUsers());
        assertEquals(WellKnown.urls.getUserPasswordChange(), bot.getPath().localPart());

        bot.go(WellKnown.urls.getLibraryDeleteProject());
        assertEquals(WellKnown.urls.getUserPasswordChange(), bot.getPath().localPart());

        validateScreenLogic(user);
    }

    public void testNormalResetLogic() throws Exception {
        bot.logOut();
        bot.go(WellKnown.urls.getUserPasswordChange());
        assertTrue(bot.getPath().localPart().startsWith(WellKnown.urls.getLogin()));
        
        User user = UserTest.aUser(USER_NAME, "123");
        user.setRequirePasswordReset(false);
        WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);
        
        bot.logInAs(user.getId(), user.getPassword());
        bot.go(WellKnown.urls.getUserPasswordChange());
        
        validateScreenLogic(user);
    }

    private void validateScreenLogic(User user) throws RobotException, IOException {
        // Test error handling
        WebForm form = bot.getForm("changePassword");
        form.setParameter("oldPassword", "wrong password");
        form.setParameter("password", "abc");
        form.setParameter("repeatedPassword", "dce");
        
        bot.submit(form);
        assertContains("The old password did not match the one on record.", bot.lastResponse().getText());
        assertContains("Passwords did not match or were not provided. Matching passwords are required.", bot.lastResponse().getText());

        // Submit a completed form
        form = bot.getForm("changePassword");
        form.setParameter("oldPassword", "123");
        form.setParameter("password", "xyz");
        form.setParameter("repeatedPassword", "xyz");

        bot.submit(form, "submit");
        
        assertContains("Your password has been changed!", bot
                .lastResponse().getText());

        user = WorldInitializer.getDefaultWorld().domain().users().get(user.getId());
        
        assertTrue(user.checkPassword("xyz"));
        assertFalse(user.isRequirePasswordReset());
    }
}
