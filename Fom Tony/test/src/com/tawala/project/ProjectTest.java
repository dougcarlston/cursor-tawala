package com.tawala.project;

import java.io.IOException;
import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Format;
import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.Html;

// TODO: convert all the hardcoded XML to use ProjectBuilder.

public class ProjectTest extends TestCase {
	private User owner = UserTest.aUser("tester");

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	public ProjectTest() {
		addUserNameToDelete(owner.getId());
	}

	public void testMinimalConstruction() {
		Project project = ProjectBuilder.buildMinimalisticProject();
		checkProjectHas(project, 1, 0, 0);
	}

	public void testSingleForm() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addForm("Form 1");

		Project project = projectBuilder.build();
		checkProjectHas(project, 1, 0, 0);
	}

	public void testSingleDocument() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addDocument("Document 1");

		Project project = projectBuilder.build();

		checkProjectHas(project, 0, 1, 0);
	}

	public void testSingleProcess() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addProcess("Process 1");

		Project project = projectBuilder.build();
		checkProjectHas(project, 0, 0, 1);
	}

	public void testNextDocument() throws IOException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("doc1", "hello");
		com.tawala.project.builder.ProcessBlockBuilder proc1 = builder
				.addProcess("proc1");
		proc1.addShow(doc1);
		builder.addForm("form1", proc1);

		Project project = builder.build();
		Form form = project.defaultForm();

		List<Html> result = project.executeProcess(
				new ExecutionContext(WorldInitializer.getDefaultWorld()
						.domain(), LinkToUserProject
						.createUnauthenticatedLink(new UserProject(project,
								owner, "test")), form, new FakeRequest(true),
						EntryPointType.REAL_PROJECT), "proc1").getHtml();
		assertEquals(1, result.size());
		assertContains("hello", result.get(0).toString());
	}

	public void testGetFormByName() {
		Project project = twoForms();
		Form form = project.getForm("form2");
		assertEquals("form2", form.getName());
		assertNull(project.getForm("nonexistent"));
	}

	public void testMultipleExecutionsOfSetProjectXMLDefinitions() {
		Project project = ProjectBuilder.buildMinimalisticProject();

		String xmlDefinition = project.getProjectXmlDefinition();
		project = new Project(new ConfigElement(xmlDefinition));

		checkProjectHas(project, 1, 0, 0);

		project.setProjectXmlDefinition(xmlDefinition);
		checkProjectHas(project, 1, 0, 0);
	}

	private void checkProjectHas(Project project, int forms, int documents,
			int processes) {
		assertEquals(forms, project.getForms().size());
		assertEquals(documents, project.getDocuments().size());
		assertEquals(processes, project.getProcesses().size());
	}

	public static String justOneFormXml(String projectName, String contents,
			Format format, Boolean preventBackButtonNavigation) {
		return "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n"
				+ "<project name=\""
				+ projectName
				+ "\" themePath=\"default\" format=\""
				+ format.toString()
				+ "\">\n"
				+ "    <forms>\n"
				+ "        <form name=\"Hello\""
				+ (preventBackButtonNavigation != null
						&& preventBackButtonNavigation ? " blockBackButton=\"true\""
						: "") + ">\n" + "            <items>\n"
				+ "                <text>" + contents + "</text>\n"
				+ "            </items>\n" + "        </form>\n"
				+ "    </forms>\n" + "</project>";
	}

	public static String projectFibXml(String projectName) {
		return "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n"
				+ "<project name=\""
				+ projectName
				+ "\">\n"
				+ "    <forms>\n"
				+ "        <form name=\"Hello\">\n"
				+ "            <items>\n"
				+ "              <fib label=\"Q1\">Date: <blank label=\"a\" length=\"2\"/>/<blank label=\"b\" length=\"2\"/>/20<blank label=\"c\" length=\"2\"/></fib>\n"
				+ "            </items>\n" + "        </form>\n"
				+ "    </forms>\n" + "</project>";
	}

	public static Project projectHelloBob() {
		ProjectBuilder project = new ProjectBuilder();
		FormBuilder form = project.addForm("getName");
		form.addFib("What's your name?", 20);

		DocumentBuilder document = project.addDocument("hi, bob");
		document.addText("Hi, ");
		document.addField("getName:Q1:a");
		document.addText(".");

		com.tawala.project.builder.ProcessBlockBuilder process = project
				.addProcess("say hello");
		process.addShow(document);
		form.setPostProcess(process);
		return project.build();
	}

	public static Project projecIfFillInBlank() {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder doc1 = projectBuilder.addDocument("glad");
		doc1.addText("I'm glad to hear that.");

		DocumentBuilder doc2 = projectBuilder.addDocument("sorry");
		doc2.addText("I'm sorry to hear that.");

		ProcessBlockBuilder processBuilder = projectBuilder.addProcess("if");
		IfBuilder ifBuilder = processBuilder.addIf();
		ifBuilder.conditions().addComparison("stringEquals", "form:question",
				"value", "yes");
		ifBuilder.trueSet().addShow(doc1);
		ifBuilder.falseSet().addShow(doc2);

		FormBuilder formBuilder = projectBuilder
				.addForm("form", processBuilder);
		formBuilder.addFib("Are you happy?", "question", 3);

		return projectBuilder.build();
	}

	public static Project twoForms() {
		Project project = projecIfFillInBlank();
		String formXml = "<form name=\"form2\" process=\"if\">\n"
				+ "<items>\n"
				+ "<fib label=\"Q1\">Are you two? <blank label=\"a\" length=\"3\"/></fib>\n"
				+ "</items>\n" + "</form>\n";

		project = ProjectBuilder.addForm(project, formXml);
		return project;
	}

	public static Project projectIfMultipleChoice() {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder doc1 = projectBuilder.addDocument("bananas");
		doc1.addText("You like bananas.");

		DocumentBuilder doc2 = projectBuilder.addDocument("otherwise");
		doc2.addText("You don't like bananas");

		ProcessBlockBuilder processBuilder = projectBuilder.addProcess("if");
		IfBuilder ifBuilder = processBuilder.addIf();
		ifBuilder.conditions().addMCComparisonAdvancedIfStyle("form:Q1",
				"contains", "b");
		ifBuilder.trueSet().addShow(doc1);
		ifBuilder.falseSet().addShow(doc2);

		FormBuilder formBuilder = projectBuilder
				.addForm("form", processBuilder);
		formBuilder.addMc("What is your favorite fruit?", "Blueberries",
				"Bananas", "Strawberries");

		return projectBuilder.build();
	}

	public static Project addition() {
		return ProjectBuilder
				.newProject("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n"
						+ "<project name=\"addition\" format=\"1.3\">\n"
						+ "<forms>\n"
						+ "<form name=\"enter\" process=\"add\">\n"
						+ "<items>\n"
						+ "<fib label=\"Q1\"><blank label=\"a\" length=\"3\"/> + <blank label=\"b\" length=\"3\"/></fib>\n"
						+ "</items>\n"
						+ "</form>\n"
						+ "</forms>\n"
						+ "<processes>\n"
						+ "<process name=\"add\">\n"
						+ "<set field=\"total\"><add><operand field='Q1:a'/><operand field='Q1:b'/></add></set>\n"
						+ "<if>\n"
						+ "<conditions>\n"
						+ "<isGreaterThan field=\"total\"><string value=\"10\"/></isGreaterThan>\n"
						+ "</conditions>\n"
						+ "<trueSet><set field=\"comment\"><string value=\"that's big!\"/></set></trueSet>\n"
						+ "</if>\n" + "</process>\n" + "</processes>\n"
						+ "</project>");
	}

	public static Project aProject() {
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("form");
		Project project = builder.build();
		return project;
	}

	public static Project projectFib() {
		return ProjectBuilder.newProject(projectFibXml("two"));
	}

	public static Project justOneForm(Format format,
			Boolean preventBackButtonNavigation) {
		String xml = justOneFormXml("two", "Hello, World!", format,
				preventBackButtonNavigation);
		return ProjectBuilder.newProject(xml);
	}
}
