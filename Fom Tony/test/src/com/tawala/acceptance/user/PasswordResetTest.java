package com.tawala.acceptance.user;

import java.io.UnsupportedEncodingException;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.springframework.mail.javamail.JavaMailSenderImpl;

import com.meterware.httpunit.WebForm;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.email.Emailer;
import com.tawala.web.WorldInitializer;

import fake.smtp.FakeSmtpMessage;
import fake.smtp.FakeSmtpServer;

public class PasswordResetTest extends AcceptanceTestCase {
    private static final String USER_NAME = "joetester";
    private FakeSmtpServer server;

    public PasswordResetTest() {
        setUserNamesToDelete(new String[] { USER_NAME });
    }

    @Override
    protected void setUp() throws Exception {
        super.setUp();
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

    public void testWebInterface() throws Exception {
        bot.logOut();

        User user = UserTest.aUser(USER_NAME, "123");
        WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);

        bot.logInAs(user.getId(), "wrong password");

        assertContains(
                "Please check your user name and password and try again.", bot
                        .lastResponse().getText());

        WebForm form = bot.getForm("passwordResetForm");

        assertEquals(user.getId(), form.getParameterValue("userName"));

        // Submit a completed form
        bot.submit(form);
        assertContains("Your password has been reset", bot.lastResponse()
                .getText());

        server.waitForAllConnectionsToClose();
        assertEquals(1, server.getMessageCount());

        validateEmail(world.domain().users().get(USER_NAME), server
                .getMessage(0));
    }

    private void validateEmail(User user, FakeSmtpMessage message)
            throws UnsupportedEncodingException {
        assertNotNull("user", user);
        assertNotNull("message", message);

        String body = message.getBody();
        assertContains("Dear " + user.getFirstName(), body);

        Pattern pattern = Pattern
                .compile(
                        "\\s*Your temporary password is:\\s*<br /><br />\\s*([^\\s]+)\\s*<br /><br />",
                        Pattern.MULTILINE);
        Matcher matcher = pattern.matcher(body);

        matcher.find();
        String newPassword = matcher.group(1);
        assertTrue(user.checkPassword(newPassword));
    }
}
