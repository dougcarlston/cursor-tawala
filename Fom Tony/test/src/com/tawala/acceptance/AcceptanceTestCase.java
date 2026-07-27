package com.tawala.acceptance;

import com.meterware.httpunit.HttpUnitOptions;
import com.meterware.httpunit.parsing.HTMLParserFactory;
import com.scissor.webrobot.RobotException;
import com.tawala.World;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.email.EmailService;
import com.tawala.project.UserProject;
import com.tawala.web.ServletTestCase;
import com.tawala.web.SiteRobot;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public abstract class AcceptanceTestCase extends ServletTestCase {
    protected SiteRobot bot;
    protected World world;
    protected User projectOwner;
    
    public AcceptanceTestCase() {
        setUserNamesToDelete(new String[] {"bob"});
    }

    protected void setUp() throws Exception {
        super.setUp();
        bot = newBot();
        HTMLParserFactory.useJTidyParser();
        HttpUnitOptions.setScriptingEnabled( false );
        
        world = WorldInitializer.getDefaultWorld();
        projectOwner = UserTest.aUser("bob");
        world.domain().users().addOrSave(projectOwner);
        UserProject.setWebsiteHostName("ignored");
		EmailService.setSendImmediately(true);
    }

    @Override
    protected void tearDown() throws Exception {
    	bot.clearResponses();
    	super.tearDown();
    }

    protected SiteRobot newBot() {
        return new SiteRobot(client);
    }

    protected void checkPageContains(String... lookFor) throws RobotException {
        checkPageContains(bot, lookFor);
    }

    protected void checkPageContains(SiteRobot bot, String... lookFor) throws RobotException {
        String pageText = bot.getPageText();
        for (String s : lookFor) {
            assertContains(s, pageText);

        }
    }

    protected void checkAccessRejected(String... paths) throws RobotException {
        for (String path : paths) {
            bot.go(path);
            assertTrue(bot.getPath().localPart().startsWith(WellKnown.urls.getLogin()));
        }
    }
}
