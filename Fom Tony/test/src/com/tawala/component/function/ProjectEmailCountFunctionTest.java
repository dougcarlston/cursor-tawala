package com.tawala.component.function;

import java.io.IOException;

import org.springframework.mock.web.MockServletConfig;

import com.tawala.World;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.email.Email;
import com.tawala.email.EmailService;
import com.tawala.email.UniqueBodyEmail;
import com.tawala.email.UserProjectEmail;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.Form;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.library.ProjectLibraryTestCase;
import com.tawala.web.FakeRequest;
import com.tawala.web.Request;
import com.tawala.web.WorldInitializer;

public class ProjectEmailCountFunctionTest extends com.tawala.TestCase {
	private static final String TEST_USER_ID = "testUser";
	public static final String MAIN_FORM = "Main";
	private User owner;
	private Project project;
	private UserProject userProject;
	private ExecutionContext executionContext;

	public ProjectEmailCountFunctionTest() {
		addUserNameToDelete(TEST_USER_ID);
	}

	@Override
	protected void setUp() throws Exception {
		new HibernateTestSetup().onSetUp();
		ProjectLibraryTestCase.setupLuceneForUnitTest();
		new WorldInitializer().init(new MockServletConfig());
		World world = WorldInitializer.getDefaultWorld();

		super.setUp();

		owner = UserTest.aUser(TEST_USER_ID);

		world.domain().users().addOrSave(owner);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addForm(MAIN_FORM);

		project = projectBuilder.build();

		userProject = new UserProject(project, owner, "test");
		world.domain().projects().put(userProject);

		Request request = new FakeRequest(true);

		Form form = project.getForm(MAIN_FORM);
		executionContext = new ExecutionContext(world.domain(),
				LinkToUserProject.createUnauthenticatedLink(userProject), form,
				request, EntryPointType.REAL_PROJECT);
	}

	public void testFunction() throws IOException {
		ProjectEmailCountFunction.Runtime function = new ProjectEmailCountFunction.Runtime();

		for (int i = 0; i < 5; i++) {
			Value value = function.execute(executionContext);
			assertNotNull(value);
			assertEquals(i, value.asNumber().intValue());

			addEmail();
		}
	}

	private void addEmail() throws IOException {
		UserProjectEmail email = new UserProjectEmail(userProject, MAIN_FORM,
				"test@tawala.com", null, "Test Email",
				UniqueBodyEmail.Type.TEXT, "Just a test.");
		email.setState(Email.State.SENT);
		EmailService.saveEmail(email);
	}
}
