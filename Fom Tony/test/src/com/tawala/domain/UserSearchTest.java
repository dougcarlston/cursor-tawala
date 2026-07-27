package com.tawala.domain;

import java.io.IOException;
import java.util.Collection;

import org.apache.lucene.queryParser.ParseException;
import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.web.WorldInitializer;

public class UserSearchTest extends TestCase {

    private static final String USER_NAME = "tester1";

    public UserSearchTest() {
        setUserNamesToDelete(new String[] { USER_NAME });
    }

    @Override
    protected void setUp() throws Exception {
        super.setUp();
        new WorldInitializer().init(new MockServletConfig(
                new MockServletContext()));
    }
    
    public void testBasics() throws ParseException, IOException {
        User user = new User(USER_NAME, "FirstName A.", "LastName", new EmailAddress(
                "tester@example.com"), "123");

        WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);
        
        verifySearchSuccessful(user, "firstname");
        verifySearchSuccessful(user, "lastname");
        verifySearchSuccessful(user, "tester@example.com");
        verifySearchSuccessful(user, "firstname lastname");
        verifySearchSuccessful(user, "a");
        verifySearchSuccessful(user, USER_NAME);

        verifySearchUnsuccessful("Jim");
        verifySearchUnsuccessful("123");    
    }

    private void verifySearchSuccessful(User original, String query)
            throws ParseException, IOException {
        Collection<User> foundUsers = WorldInitializer.getDefaultWorld()
                .domain().users().search(query);
        assertEquals("found count for '" + query + "'", 1, foundUsers.size());

        User found = foundUsers.iterator().next();
        assertEquals(original, found);
    }

    private void verifySearchUnsuccessful(String query) throws ParseException,
            IOException {
        Collection<User> foundUsers = WorldInitializer.getDefaultWorld()
                .domain().users().search(query);
        assertEquals("found count for '" + query + "'", 0, foundUsers.size());
    }

}
