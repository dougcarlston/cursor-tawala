package com.tawala.domain;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.util.Collection;
import java.util.Date;
import java.util.List;

import org.apache.lucene.queryParser.ParseException;
import org.springframework.beans.factory.BeanInitializationException;
import org.springframework.beans.factory.config.BeanDefinition;
import org.springframework.beans.factory.config.ConfigurableListableBeanFactory;
import org.springframework.beans.factory.xml.XmlBeanFactory;
import org.springframework.core.io.FileSystemResource;
import org.springframework.core.io.Resource;
import org.springframework.mail.MailException;
import org.springframework.mail.javamail.JavaMailSenderImpl;
import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.UsersHibernateImpl;
import com.tawala.email.Emailer;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.util.DateUtil;
import com.tawala.web.WorldInitializer;
import com.tawala.web.user.UserAccessTicket;

import fake.smtp.FakeSmtpServer;

public class UsersHibernateImplTest extends TestCase {
	private static String[] USER_NAMES = new String[] { "joethetester",
			"tester2", "tester3", "tester4" };

	private HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();
	private UsersHibernateImpl users;

	private FakeSmtpServer server;

	public UsersHibernateImplTest() {
		setUserNamesToDelete(USER_NAMES);
	}

	@Override
	protected void setUp() throws Exception {
		hibernateTestSetup.onSetUp();
		super.setUp();
		users = new UsersHibernateImpl();
		Resource res = new FileSystemResource(
				"web/WEB-INF/notification-config.xml");
		XmlBeanFactory factory = new XmlBeanFactory(res);

		factory.getBean("emailer");
		factory.getBean("emailVerificationMessage");
		factory.getBean("userApprovalNotification");
		factory.getBean("userPutOnHoldNotification");
		factory.getBean("passwordResetNotification");

		server = new FakeSmtpServer();

		JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
		senderImpl.setPort(server.getPort());
		senderImpl.setHost("127.0.0.1");
		new Emailer().setSender(senderImpl);

		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	@SuppressWarnings("unused")
	private static void replaceWithPrefixedValue(
			ConfigurableListableBeanFactory factory, String[] propertiesNames,
			String prefix) {
		for (int i = 0; i < propertiesNames.length; i++) {
			int separatorIndex = propertiesNames[i].indexOf('.');
			if (separatorIndex == -1) {
				throw new BeanInitializationException("Invalid key '"
						+ propertiesNames[i]
						+ "': expected 'beanName.property'");
			}
			String beanName = propertiesNames[i].substring(0, separatorIndex);
			String beanProperty = propertiesNames[i]
					.substring(separatorIndex + 1);

			BeanDefinition bd = factory.getBeanDefinition(beanName);
			String value = (String) bd.getPropertyValues().getPropertyValue(
					beanProperty).getValue();
			bd.getPropertyValues().addPropertyValue(beanProperty,
					prefix + value);
		}
	}

	@Override
	protected void tearDown() throws Exception {
		if (server != null)
			server.shutDown();

		hibernateTestSetup.onTearDown();

		super.tearDown();
	}

	public void testUserCreationAndRetrieval() {
		User user = UserTest.aUser(USER_NAMES[0], "123");

		users.addOrSave(user);

		User retrieved = users.get(user.getDatabaseId());
		assertNotNull(retrieved);

		retrieved = users.get(USER_NAMES[0]);
		assertNotNull(retrieved);

		retrieved.setLastName("NewLastName");
		users.addOrSave(retrieved);

		retrieved = users.get(user.getDatabaseId());
		assertEquals("NewLastName", retrieved.getLastName());
	}

	public void testOnUserRegistration() throws MailException,
			UnsupportedEncodingException, InterruptedException {
		User user = UserTest.aUser(USER_NAMES[0], "123");

		users.onUserRegistration(user);

		User retrieved = users.get(user.getDatabaseId());
		assertEquals(Status.REGISTERED, retrieved.getStatus());

		/*
		 * TODO: until the email is working well.
		 * server.waitForAllConnectionsToClose(); assertEquals(1,
		 * server.getMessageCount());
		 * 
		 * FakeSmtpMessage message = server.getMessage(0);
		 * assertContains(retrieved.getEmailValidationToken(),
		 * message.getBody());
		 */
	}

	public void testOnEmailValidation() throws MailException,
			UnsupportedEncodingException, InterruptedException {
		User user = UserTest.aUser(USER_NAMES[0], "123");

		users.onUserRegistration(user);
		users.onUserEmailValidation(users.get(user.getDatabaseId()));

		User retrieved = users.get(user.getDatabaseId());
		assertEquals(Status.REGISTERED, retrieved.getStatus());
	}

	public void testOnUserApproval() throws MailException,
			UnsupportedEncodingException, InterruptedException {
		User user = UserTest.aUser(USER_NAMES[0], "123");

		users.onUserRegistration(user);
		users.onUserEmailValidation(users.get(user.getDatabaseId()));
		users.onUserApproval(users.get(user.getDatabaseId()));

		User retrieved = users.get(user.getDatabaseId());
		assertEquals(Status.REGISTERED, retrieved.getStatus());

		/*
		 * TODO: Until we change the logic of registration
		 * server.waitForAllConnectionsToClose(); assertEquals(2,
		 * server.getMessageCount());
		 * 
		 * assertContains("Dear " + user.getFirstName(), server.getMessage(1)
		 * .getBody());
		 */
	}

	public void testSize() {
		int previousUserSize = users.size();

		for (int i = 0; i < USER_NAMES.length; i++) {
			users.addOrSave(UserTest.aUser(USER_NAMES[i], "123"));
		}

		assertEquals(previousUserSize + USER_NAMES.length, users.size());
	}

	public void testGetAllUsersWithValidateEmail() {
		List<User> expectedUsers = users
				.findUsersWithStatus(Status.EMAIL_VALIDATED);
		for (int i = 0; i < USER_NAMES.length; i++) {
			User user = UserTest.aUser(USER_NAMES[i], "123");
			user.setStatus(Status.EMAIL_UNVALIDATED);
			users.addOrSave(user);
			if (i % 2 == 0) {
				users.onUserEmailValidation(users.get(user.getDatabaseId()));
				expectedUsers.add(user);

			}
		}

		List<User> retrievedUsers = users
				.findUsersWithStatus(Status.EMAIL_VALIDATED);
		assertTrue(expectedUsers.containsAll(retrievedUsers));
		assertTrue(retrievedUsers.containsAll(expectedUsers));
	}

	public void testSearch() throws ParseException, IOException {
		User user = UserTest.aUser(USER_NAMES[1]);
		user.setFirstName("UnusualFirstName");
		user.setLastName("UnusualLastName");
		user.setEmail(new EmailAddress("unusualaddress@example.com"));

		users.addOrSave(user);

		searchAndValidatePresense(user.getFirstName(), user);
		searchAndValidatePresense(user.getLastName(), user);
		searchAndValidatePresense(user.getEmail().toString(), user);
		searchAndValidatePresense(user.getId(), user);
	}

	private void searchAndValidatePresense(String query, User user)
			throws ParseException, IOException {
		Collection<User> searchResults = users.search(query);

		assertTrue("Search for '" + query + "' contains user", searchResults
				.contains(user));
	}

	public void testPasswordReset() throws MailException,
			UnsupportedEncodingException, InterruptedException {
		User user = UserTest.aUser(USER_NAMES[0], "123");

		users.onUserRegistration(user);

		users.resetPassword(user);

		User retrieved = users.get(user.getDatabaseId());
		assertTrue(retrieved.isRequirePasswordReset());

		assertFalse(retrieved.checkPassword("123"));

		server.waitForAllConnectionsToClose();
		assertEquals(1, server.getMessageCount());

		assertContains("Dear " + user.getFirstName(), server.getMessage(0)
				.getBody());
	}

	public void testOnLogin() {
		Date now = new Date(System.currentTimeMillis() - 1000);
		User user = UserTest.aUser(USER_NAMES[0], "123");

		users.addOrSave(user);

		User retrieved = users.get(user.getDatabaseId());
		assertNull(retrieved.getLastLoggedInDate());

		users.onLogin(user);

		retrieved = users.get(user.getDatabaseId());
		assertNotNull(retrieved.getLastLoggedInDate());
		assertTrue(now.before(retrieved.getLastLoggedInDate()));
	}

	public void testFindUsersRegisteredSince() {
		User user = UserTest.aUser(USER_NAMES[0], "123");

		users.addOrSave(user);

		List<User> expectedUsers = users.findUsersRegisteredSince(DateUtil
				.dateEarlierStartingAt12am(2));

		assertTrue(expectedUsers.size() >= 1);
		assertEquals(user, expectedUsers.get(0));
	}

	public void testOnUserUpgradeToFullyRegistered() {
		User user = UserTest.aUser(USER_NAMES[0], "123");
		users.addOrSave(user);

		user = users.get(user.getDatabaseId());
		assertEquals(Status.REGISTERED_INITIAL, user.getStatus());

		user = users.onUserUpgradeToFullyRegistered(user);
		assertEquals(Status.REGISTERED, user.getStatus());
	}

	public void testUserAccessTokenCreation() {
		User user = UserTest.aUser(USER_NAMES[0], "123");
		users.addOrSave(user);

		UserAccessTicket ticket = UsersHibernateImpl.generateUserAccessTicket(user);
		assertEquals(user, ticket.getUser());
	}

	public void testSearchingCaseInsensitiveUserNames() {
		String userProvidedUserName = "JohnSmith";
		addUserNameToDelete(userProvidedUserName);
		
		User user = UserTest.aUser(userProvidedUserName, "123");
		users.addOrSave(user);
		
		validateUserCanBeFound(user, "johnsmith");
		validateUserCanBeFound(user, "JOHNSMITH");
		validateUserCanBeFound(user, "JOhnSMith");
	}

	public void testUniquenessOfCaseInsensitiveUserNames() {
		String userProvidedUserName = "JohnSmith";
		addUserNameToDelete(userProvidedUserName);
		
		User user = UserTest.aUser(userProvidedUserName, "123");
		users.addOrSave(user);
		
		validateUserCanBeFound(user, "johnsmith");
		
		userProvidedUserName = "JOHNSMITH";
		User another = UserTest.aUser(userProvidedUserName);
		addUserNameToDelete(userProvidedUserName);
		try {
			users.addOrSave(another);
			fail("Was able to create another user.");
		} catch (Throwable e) {
			//--- To be expected.
		}
	}

	public void testProjectGroupPersistence() {
		User user = UserTest.aUser("tester");
		addUserNameToDelete(user.getId());
		users.addOrSave(user);
		
		ProjectGroup firstGroup = new ProjectGroup(user, "First Group");
		UsersHibernateImpl.saveProjectGroup(firstGroup);
		
		List<ProjectGroup> retrievedGroups = UsersHibernateImpl.getAllUserSportsDashboardGroups(user);
		assertNotNull(retrievedGroups);
		assertEquals(1, retrievedGroups.size());
		assertEquals("First Group", retrievedGroups.get(0).getName());
		
		UsersHibernateImpl.deleteProjectGroup(user, retrievedGroups.get(0).getId());
		retrievedGroups = UsersHibernateImpl.getAllUserSportsDashboardGroups(user);
		assertNotNull(retrievedGroups);
		assertEquals(0, retrievedGroups.size());
	}
		
	private void validateUserCanBeFound(User user, String userName) {
		User loadedUser = users.get(userName);
		assertNotNull("Loaded User", loadedUser);
		assertEquals(user, loadedUser);
	}

}
