package com.tawala.acceptance.user;

import org.springframework.mock.web.MockHttpServletRequest;
import org.springframework.mock.web.MockHttpServletResponse;
import org.springframework.mock.web.MockHttpSession;
import org.springframework.web.servlet.ModelAndView;

import com.meterware.httpunit.WebForm;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.EmailAddress;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.user.AccountUpdateController;

public class AccountUpdateTest extends AcceptanceTestCase {
    private static final String USER_NAME = "joetester";
    private User user;
    
    public AccountUpdateTest() {
        setUserNamesToDelete(new String[] {USER_NAME});
    }

    @Override
    protected void setUp() throws Exception {
        super.setUp();

        user = UserTest.aUser(USER_NAME, "abc");
        user.setFirstName("UnusualFirstName");
        user.setFirstName("UnusualLastName");
        user.setEmail(new EmailAddress("joe@example.com"));
        world.domain().users().addOrSave(user);
    }

    public void testFailedValidationLogic() throws Exception {
        MockHttpSession session = new MockHttpSession();
        MockHttpServletRequest request = prepareRequest(session);
        request.setParameter("emailAddress", "some garbage");

        ModelAndView modelAndView = new AccountUpdateController()
                .handleRequest(request, new MockHttpServletResponse());

        assertNotNull(modelAndView);
        assertEquals("View redirected back to the view screen", "user.account",
                modelAndView.getViewName());

        assertEquals("Email hasn't been changed", "joe@example.com",
                ((User) session.getAttribute("user")).getEmail().toString());
    }

    /**
     * @param session
     * @return
     */
    private MockHttpServletRequest prepareRequest(MockHttpSession session) {
        MockHttpServletRequest request = new MockHttpServletRequest("POST",
                WellKnown.urls.getUserAccountUpdate());

        session.setAttribute("user", user);
        request.setSession(session);
        return request;
    }

    public void testGoRightLogicWithoutPasswordUpdate() throws Exception {
        MockHttpSession session = new MockHttpSession();
        MockHttpServletRequest request = prepareRequest(session);
        
        request.setParameter("user.firstName", "Joe");
        request.setParameter("user.lastName", "Smith");
        request.setParameter("emailAddress", "newemail@example.com");
        request.setParameter("password", "");
        request.setParameter("repeatedPassword", "");

        ModelAndView modelAndView = new AccountUpdateController()
                .handleRequest(request, new MockHttpServletResponse());
        assertNotNull(modelAndView);

        assertEquals("Sent back to the same screen",
                "user.account", modelAndView.getViewName());
        
        assertEquals("Email has been changed", "newemail@example.com",
                ((User) session.getAttribute("user")).getEmail().toString());
        assertTrue("Password is unchanged", ((User) session.getAttribute("user")).checkPassword("abc"));
    }

    public void testGoRightLogicWithPasswordUpdate() throws Exception {
        MockHttpSession session = new MockHttpSession();
        MockHttpServletRequest request = prepareRequest(session);
        
        request.setParameter("user.firstName", "Joe");
        request.setParameter("user.lastName", "Smith");
        request.setParameter("emailAddress", "newemail@example.com");
        request.setParameter("password", "newpassword");
        request.setParameter("repeatedPassword", "newpassword");

        ModelAndView modelAndView = new AccountUpdateController()
                .handleRequest(request, new MockHttpServletResponse());
        assertNotNull(modelAndView);

        assertEquals("Sent back to the same screen",
                "user.account", modelAndView.getViewName());
        
        assertEquals("Email has been changed", "newemail@example.com",
                ((User) session.getAttribute("user")).getEmail().toString());
        assertEquals("Password has been changed", "newpassword",
                ((User) session.getAttribute("user")).getPassword());
    }

    public void testOnlyLoggedInUpdate() throws Exception {
        bot.go(WellKnown.urls.getUserAccountUpdate());

        assertTrue(bot.getPath().localPart().startsWith(WellKnown.urls.getLogin()));
    }
    
    public void testWebInterface() throws Exception {
        bot.logInAs(user.getId(), user.getPassword());
        
        bot.go(WellKnown.urls.getUserAccountUpdate());
        assertContains("First Name:", bot.lastResponse().getText());

        WebForm form = bot.getForm("editUser");
        assertTrue("Form has parameter user.firstName", form
                .hasParameterNamed("user.firstName"));

        // Verify correct display of original data
        bot.submit(form, "submit");
        assertContains(user.getFirstName(), bot.lastResponse().getText());
        assertContains(user.getLastName(), bot.lastResponse().getText());
        assertContains(user.getEmail().toString(), bot.lastResponse().getText());

        // Submit a completed form
        form = bot.getForm("editUser");
        form.setParameter("user.firstName", "Joe");
        form.setParameter("user.lastName", "Smith");
        form.setParameter("emailAddress", "joe@example.com");

        bot.submit(form, "submit");
        
        User updatedUser = world.domain().users().get(USER_NAME);
        
        assertNotNull(updatedUser);
        assertEquals("Joe", updatedUser.getFirstName());
        assertEquals("Smith", updatedUser.getLastName());
        assertEquals("joe@example.com", updatedUser.getEmail().toString());
        
        assertContains("Your account has been updated!", bot.lastResponse().getText());
    }
}
