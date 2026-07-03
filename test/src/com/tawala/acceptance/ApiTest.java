package com.tawala.acceptance;

import java.io.ByteArrayInputStream;
import java.io.IOException;

import org.xml.sax.SAXException;

import com.meterware.httpunit.PostMethodWebRequest;
import com.meterware.httpunit.WebRequest;
import com.meterware.httpunit.WebResponse;
import com.meterware.servletunit.ServletUnitClient;
import com.scissor.Log;
import com.tawala.World;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.ServletTestCase;
import com.tawala.web.WorldInitializer;

public class ApiTest extends ServletTestCase {
	private World world;
	private User user;
	private static final String API_USER = "bob";
	private static final String API_PASSWORD = "asdf;lkj";

	public ApiTest() {
		setUserNamesToDelete(new String[] { API_USER });
	}

	protected void setUp() throws Exception {
		super.setUp();

		user = UserTest.aUser(API_USER, API_PASSWORD);
		world = WorldInitializer.getDefaultWorld();
		world.domain().users().addOrSave(user);
		world.domain().projects().deleteAllProjectsForUser(world, user);
		UserProject.setWebsiteHostName("ignored");
	}

	protected void tearDown() throws Exception {
		super.tearDown();
		logs.dumpUnseen(this);
	}

	public void testUnknownCommand() throws IOException, SAXException {
		requestApi("mxylplyk");
		assertEquals(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
						+ "<response status=\"failure\">\n"
						+ "  <error id=\"command.unknown\" message=\"Unknown command 'mxylplyk'.\"/>\n"
						+ "</response>\n", response.getText());
	}

	public void testQueryDeployments() throws IOException, SAXException {
		requestApi("queryDeployments");

		assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
				+ "<response status=\"success\">\n"
				+ "  <deployments user=\"bob\"/>\n" + "</response>\n", response
				.getText());
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("f1", true);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, user, "p1");
		world.domain().projects().put(userProject);

		requestApi("queryDeployments");
		assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
				+ "<response status=\"success\">\n"
				+ "  <deployments user=\"bob\">\n"
				+ "    <deployment project=\"p1\">\n"
				+ "      <startpoint form=\"f1\" url=\""
				+ userProject.getUrlToForm(EntryPointType.REAL_PROJECT,
						userProject.getProject().getForm("f1")) + "\"/>\n"
				+ "    </deployment>\n" + "  </deployments>\n"
				+ "</response>\n", response.getText());
	}

	public void testStartingPoints() throws IOException, SAXException {
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("f1");
		builder.addForm("f2", false);
		builder.addForm("f3", true);
		UserProject userProject = new UserProject(builder.build(), user, "p1");
		world.domain().projects().put(userProject);

		requestApi("queryDeployments");
		assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
				+ "<response status=\"success\">\n"
				+ "  <deployments user=\"bob\">\n"
				+ "    <deployment project=\"p1\">\n"
				+ "      <startpoint form=\"f1\" url=\""
				+ userProject.getUrlToForm(EntryPointType.REAL_PROJECT,
						userProject.getProject().getForm("f1"))
				+ "\"/>\n"
				+ "      <startpoint form=\"f3\" url=\""
				+ userProject.getUrlToForm(EntryPointType.REAL_PROJECT,
						userProject.getProject().getForm("f3")) + "\"/>\n"
				+ "    </deployment>\n" + "  </deployments>\n"
				+ "</response>\n", response.getText());
	}

	public void testQueryDataSources() throws IOException, SAXException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("f1");
		formBuilder.setExternalDataSource("ClientInfo");
		formBuilder.addFib("My fib:", "fibfield", 20);
		formBuilder.addMcWithAlternateLabel("mcqfield", "My MCQ:",
				new String[] { "1", "2", "3" });

		UserProject userProject = new UserProject(builder.build(), user, "p1");
		world.domain().projects().put(userProject);

		requestApi("queryDataSources");
		assertEquals(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
						+ "<response status=\"success\">\n"
						+ "  <datasources>\n"
						+ "    <datasource name=\"ClientInfo\">\n"
						+ "      <field name=\"fibfield\" type=\"string\"/>\n"
						+ "      <field name=\"mcqfield\" type=\"mcq\" choices=\"3\" onlyone=\"false\"/>\n"
						+ "    </datasource>\n" + "  </datasources>\n"
						+ "</response>\n", response.getText());
	}

	// --- TODO: shall we reenable it?
	public void UNUSEDtestUnknownXmlWarning() throws IOException, SAXException {
		requestApi("queryDeployments", "<thingy />");
		assertContains("<debug message=\"Unused xml: /request/thingy\"/>",
				response.getText());

	}

	public void testUploadProject() throws IOException, SAXException {
		world.domain().projects().deleteAllProjectsForUser(world, user);
		int projectCount = world.domain().projects().size();
		String project = "<project name=\"Hello World\" themePath=\"basicblue\" format=\"1.3\">\n"
				+ "    <forms>\n"
				+ "        <form name=\"Hello\">\n"
				+ "            <items>\n"
				+ "                <text>Hello, World!</text>\n"
				+ "            </items>\n"
				+ "        </form>\n"
				+ "        <form name=\"Not a Starting Point\" startPoint=\"false\">\n"
				+ "            <items>\n"
				+ "                <text>Hello, World!</text>\n"
				+ "            </items>\n"
				+ "        </form>\n"
				+ "    </forms>\n" + "</project>";
		requestApi("uploadProject", project);

		UserProject userProject = world.domain().projects()
				.getWithProjectRuntime(user.getId(), "Hello World");
		assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
				+ "<response status=\"success\">\n"
				+ "  <deployments user=\"bob\">\n"
				+ "    <deployment project=\"Hello World\">\n"
				+ "      <startpoint form=\"Hello\" url=\""
				+ userProject.getUrlToForm(EntryPointType.REAL_PROJECT,
						userProject.getProject().getForm("Hello"))
				+ "\"/>\n"
				+ "      <startpoint form=\"Not a Starting Point\" url=\""
				+ userProject.getUrlToForm(EntryPointType.REAL_PROJECT,
						userProject.getProject()
								.getForm("Not a Starting Point")) + "\"/>\n"
				+ "    </deployment>\n" + "  </deployments>\n"
				+ "</response>\n", response.getText());
		assertEquals(projectCount + 1, world.domain().projects().size());
		assertEquals(1, world.domain().projects().getAllForUserId(API_USER)
				.size());

		assertEquals("basicblue", userProject.getProject().getTheme().getThemeId());
		logs
				.containsMessage(Log.INFO,
						"successful upload from 127.0.0.1 for Project(bob, Hello World)");

		// --- Test theme change on the subsequent uploads
		project = "<project name=\"Hello World\" themePath=\"yellow\" format=\"1.3\">\n"
				+ "    <forms>\n"
				+ "        <form name=\"Hello\">\n"
				+ "            <items>\n"
				+ "                <text>Hello, World!</text>\n"
				+ "            </items>\n"
				+ "        </form>\n"
				+ "        <form name=\"Not a Starting Point\" startPoint=\"false\">\n"
				+ "            <items>\n"
				+ "                <text>Hello, World!</text>\n"
				+ "            </items>\n"
				+ "        </form>\n"
				+ "    </forms>\n" + "</project>";
		requestApi("uploadProject", project);

		userProject = world.domain().projects().getWithProjectRuntime(
				user.getId(), "Hello World");
		assertEquals("yellow", userProject.getProject().getTheme().getThemeId());
	}

	public void testAuthentication() throws IOException, SAXException {
		requestApi("queryDeployments", "", API_USER, API_PASSWORD);
		assertContains("success", response.getText());

		requestApi("queryDeployments", "", API_USER, "wrong password");
		String text = response.getText();
		assertContains("failure", text);
		assertContains("auth.failed", text);

		requestApi("queryDeployments", "", "unknown_user", API_PASSWORD);
		text = response.getText();
		assertContains("failure", text);
		assertContains("auth.failed", text);
	}

	private void requestApi(String command, String contents)
			throws IOException, SAXException {
		requestApi(command, contents, API_USER, API_PASSWORD);
	}

	private void requestApi(String command, String contents, String userId,
			String password) throws IOException, SAXException {
		getResponse(postXml("<request type=\"" + command
				+ "\" protocol=\"1.0\">\n" + "<credentials user=\"" + userId
				+ "\" password=\"" + password + "\" />\n" + contents
				+ "</request>"), client);
		assertEquals(200, response.getResponseCode());
	}

	private void requestApi(String command) throws IOException, SAXException {
		requestApi(command, "");
	}

	protected WebResponse getResponse(WebRequest request,
			ServletUnitClient client) throws IOException, SAXException {
		response = client.getResponse(request);
		return response;
	}

	public static PostMethodWebRequest postXml(String xml) {
		return new PostMethodWebRequest("http://ignored/client",
				new ByteArrayInputStream(xml.getBytes()), "text/xml");
	}
}
