package com.tawala.acceptance.user;

import org.springframework.mail.javamail.JavaMailSenderImpl;
import org.springframework.mock.web.MockHttpServletRequest;
import org.springframework.mock.web.MockHttpServletResponse;
import org.springframework.web.servlet.ModelAndView;

import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.email.Emailer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.user.EmailVerificationController;

import fake.smtp.FakeSmtpServer;

public class EmailVerificationTest extends AcceptanceTestCase {
    private static final String USER_NAME = "tester1";
    private User user;
    private FakeSmtpServer server;
    
    public EmailVerificationTest() {
        setUserNamesToDelete(new String[] {USER_NAME});
    }

    @Override
    protected void setUp() throws Exception {
        super.setUp();
        
        user = UserTest.aUser(USER_NAME);
        user.setStatus(Status.EMAIL_UNVALIDATED);
        user.generateEmailValidationToken();
        
        world.domain().users().addOrSave(user);
        
        server = new FakeSmtpServer();

        JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
        senderImpl.setPort(server.getPort());
        senderImpl.setHost("127.0.0.1");
        new Emailer().setSender(senderImpl);
    }
    
    @Override
    protected void tearDown() throws Exception {
        if(server != null)
            server.shutDown();
        
        super.tearDown();
    }

    public void testFailedValidationLogic() throws Exception {
        MockHttpServletRequest request = new MockHttpServletRequest("POST",
                WellKnown.urls.getEmailConfirmation());
        MockHttpServletResponse response = new MockHttpServletResponse();

        ModelAndView modelAndView = new EmailVerificationController()
                .handleRequest(request, response);
        assertNotNull(modelAndView);

        assertEquals("Sent to error screen",
                "registration.email.verification.error", modelAndView.getViewName());
    }

    
    public void testGoRightLogicForUnvalidatedUsers() throws Exception {
        MockHttpServletRequest request = prepareCorrectRequest();
        MockHttpServletResponse response = new MockHttpServletResponse();
        
        ModelAndView modelAndView = new EmailVerificationController()
                .handleRequest(request, response);
        assertNotNull(modelAndView);

        assertEquals("Sent to success screen",
                "registration.email.verified", modelAndView.getViewName());
        
        user = world.domain().users().get(USER_NAME);
        //assertEquals("new status", Status.EMAIL_VALIDATED, user.getStatus());
        assertEquals("new status", Status.REGISTERED, user.getStatus());
    }
        
    public void testGoRightLogicForRegisteredUsers() throws Exception {
        user.setStatus(Status.REGISTERED);
        world.domain().users().addOrSave(user);

        MockHttpServletRequest request = prepareCorrectRequest();
        MockHttpServletResponse response = new MockHttpServletResponse();
        
        ModelAndView modelAndView = new EmailVerificationController()
                .handleRequest(request, response);
        assertNotNull(modelAndView);

        assertEquals("Sent to success screen",
                "registration.email.verified", modelAndView.getViewName());
        
        user = world.domain().users().get(USER_NAME);
        assertEquals("status unchanged", Status.REGISTERED, user.getStatus());
    }

    /**
     * @return
     */
    private MockHttpServletRequest prepareCorrectRequest() {
        MockHttpServletRequest request = new MockHttpServletRequest("POST",
                WellKnown.urls.getEmailConfirmation());
        request.setParameter(EmailVerificationController.PARAMETER_ID, user.getId());
        request.setParameter(EmailVerificationController.PARAMETER_VALIDATION_TOKEN, user.getEmailValidationToken());
        return request;
    }
    
    public void testWebInterfaceForUnvalidatedUsers() throws Exception {
        String URI = user.constructEmailValidationURI();
        bot.go(URI);

        assertContains("Thanks for registering", bot.lastResponse().getText());
//        assertEquals("new status", Status.EMAIL_VALIDATED, world.domain().users().get(user.getId()).getStatus());
        assertEquals("new status", Status.REGISTERED, world.domain().users().get(user.getId()).getStatus());
    }

    public void testWebInterfaceForRegisteredUsers() throws Exception {
        user.setStatus(Status.REGISTERED);
        world.domain().users().addOrSave(user);

        String URI = user.constructEmailValidationURI();
        bot.go(URI);

        assertContains("May you create many wonderful projects!", bot.lastResponse().getText());
        assertEquals("new status", Status.REGISTERED, user.getStatus());
    }

    public void testWebInterfaceForValidationErrors() throws Exception {
        String URI = user.constructEmailValidationURI();

        world.domain().users().delete(user);

        bot.go(URI);

        assertContains("Registration Error", bot.lastResponse().getText());
    }
}
