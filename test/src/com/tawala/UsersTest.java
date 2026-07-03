package com.tawala;

import com.tawala.domain.EmailAddress;
import com.tawala.domain.User;

public class UsersTest extends TestCase {
    private static final User BOB = new User("bob", "Bob", "Smith",
            new EmailAddress("bob@example.org"), "letmein");
    private static final User JANE = new User("jane", "Jane", "Doe",
            new EmailAddress("jdoe@example.com"), "letmeout");

    public void testStoreAndFetch() {
        Users users = new UsersPersistentBunchImpl();
        assertEquals(0, users.size());
        assertNull(users.get("bob"));

        users.addOrSave(BOB);
        assertEquals(1, users.size());
        assertSame(BOB, users.get("bob"));
        assertNull(users.get("jane"));

        users.addOrSave(JANE);
        assertEquals(2, users.size());
        assertSame(BOB, users.get("bob"));
        assertSame(JANE, users.get("jane"));
    }

    public void testInstancesIndependent() {
        Users one = new UsersPersistentBunchImpl();
        one.addOrSave(BOB);
        Users two = new UsersPersistentBunchImpl();
        one.addOrSave(JANE);
        assertEquals(0, two.size());
        assertNull(two.get("bob"));
        assertNull(two.get("jane"));
    }
}
