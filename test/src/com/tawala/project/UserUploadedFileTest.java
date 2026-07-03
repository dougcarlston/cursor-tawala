package com.tawala.project;

import javax.servlet.ServletException;

import org.springframework.mock.web.MockServletConfig;

import com.tawala.TestCase;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.theme.UserUploadedFile;
import com.tawala.web.WorldInitializer;

public class UserUploadedFileTest extends TestCase {
	private User user;
	private byte[] data = new byte[] { 44, 23, 77, 23, 11, 4, 56 };

	@Override
	protected void setUp() throws Exception {
		new HibernateTestSetup().onSetUp();
		setUserNamesToDelete("testuser");
		super.setUp();
		user = UserTest.aUser("testuser");
		new UsersHibernateImpl().addOrSave(user);
	}

	public void testCreateUserFileWithoutProjectDependencies() {
		UserUploadedFile file = new UserUploadedFile(user, data, "image/jpeg",
				"test.jpg", data.length);
		try {
			file.getFileURL();
			fail("An exception should have been thrown.");
		} catch (IllegalStateException e) {
			// Ignore - it's expected.
		}

		UsersHibernateImpl.saveUserUploadedFile(file);

		assertMatches("/i/[a-z0-9]{15}/test.jpg", file.getFileURL());
		
		UserUploadedFile reloadedFile = UsersHibernateImpl.getUserUploadedFileById(file.getId());
		assertNotNull(reloadedFile);
		
		new UsersHibernateImpl().delete(user);
		clearHibernateSecondLevelCache();
		
		reloadedFile = UsersHibernateImpl.getUserUploadedFileById(file.getId());
		assertNull(reloadedFile);
	}

	public void testCreateUserFileWithProjectDependencies() throws ServletException {
		new WorldInitializer().init(new MockServletConfig());
		
		UserProject userProject = new UserProject(ProjectBuilder.buildMinimalisticProject(), user, "test project");
		ProjectsHibernateImpl projectsHibernateImpl = new ProjectsHibernateImpl();
		projectsHibernateImpl.put(userProject);
		
		UserUploadedFile file = new UserUploadedFile(user, userProject, data, "image/jpeg",
				"test.jpg", data.length);

		UsersHibernateImpl.saveUserUploadedFile(file);

		assertMatches("/i/[a-z0-9]{15}/test.jpg", file.getFileURL());
		
		UserUploadedFile reloadedFile = UsersHibernateImpl.getUserUploadedFileById(file.getId());
		assertNotNull(reloadedFile);
		
		projectsHibernateImpl.delete(userProject, null);
		clearHibernateSecondLevelCache();
		
		reloadedFile = UsersHibernateImpl.getUserUploadedFileById(file.getId());
		assertNull(reloadedFile);
	}
}
