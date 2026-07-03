package com.tawala.acceptance;

import org.apache.log4j.Level;

import com.scissor.webrobot.RobotException;

public class StandaloneErrorTest extends AcceptanceTestCase {

    public void testUnexpectedExceptionHandling() throws RobotException {
        bot.go("/test_exception_throwing");

        assertContains("We are very sorry!", bot.getPageText());
        logs.containsMessage(Level.ERROR,
                "Failed to process page: /test_exception_throwing");
    }
}
