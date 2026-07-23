package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.VALUE;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.scissor.xmlconfig.Format;
import com.tawala.domain.User;
import com.tawala.project.FormSegment;
import com.tawala.project.Project;
import com.tawala.project.ProjectTest;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;
import com.tawala.web.WorldInitializer;

public class ViewProjectTest extends AcceptanceTestCase {

	public void testViewFormBeforeExplicitBackButtonNavigationPrevention()
			throws RobotException {
		Format format = FormSegment.FORMAT_WITH_EXPLICIT_BACK_BUTTON_DETECTION_FOR_FORMS;
		Project project = ProjectTest.justOneForm(new Format(format.major(),
				format.minor() - 1), false);
		checkOutput(project, false);
	}

	public void testViewFormAfterExplicitBackButtonNavigationPrevention()
			throws RobotException {
		Project project = ProjectTest
				.justOneForm(
						FormSegment.FORMAT_WITH_EXPLICIT_BACK_BUTTON_DETECTION_FOR_FORMS,
						true);
		checkOutput(project, true);
	}

	private void checkOutput(Project project, boolean checkBackButtonPrevention)
			throws RobotException {
		UserProject userProject = new UserProject(project, projectOwner, "two");
		world.domain().projects().put(userProject);

		bot.go(userProject, "Hello");
		assertEquals("text/html", bot.getContentType());

		Pattern pattern = Pattern.compile("<input type=\"hidden\" name=\""
				+ FormSegment.NEXT_EXPECTED_TOKEN + "\" value=\"" + "([^\"]+)"
				+ "\">", Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());
		String nextToken = matcher.group(1);

		String urlToForm = userProject.getUrlToForm(
				EntryPointType.REAL_PROJECT, userProject.getProject().getForm(
						"Hello"));
		String urlToFormWithoutSiteName = urlToForm.substring("http://ignored"
				.length());

		String buildNumber = WorldInitializer.getDefaultWorld()
				.getBuildNumber();

		String html = "<!DOCTYPE html>\n"
				+ "<html>\n"
				+ "<head>\n"
				+ "<title>SportsDashboards</title>\n<meta name=\"robots\" content=\"noarchive\">\n"
				+ "<link rel=\"stylesheet\" href=\"/css/project/print.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"print\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/scripts/yui-2.9.0/build/fonts/fonts-min.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/scripts/yui-2.9.0/build/button/assets/skins/sam/button.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/scripts/yui-2.9.0/build/container/assets/container.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/scripts/yui-2.9.0/build/container/assets/skins/sam/container.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/scripts/yui-2.9.0/build/datatable/assets/skins/sam/datatable.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/scripts/yui-2.9.0/build/paginator/assets/skins/sam/paginator.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/css/project/default.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/css/project/default/project.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<link rel=\"stylesheet\" href=\"/css/project/form-layout-core.css?x="
				+ buildNumber
				+ "\" type=\"text/css\" media=\"screen\" />\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/utilities/utilities.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/json/json-min.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/container/container-min.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/datasource/datasource-min.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/datatable/datatable-min.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/paginator/paginator-min.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/animation/animation-min.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "<script type=\"text/javascript\" src=\"/scripts/yui-2.9.0/build/cookie/cookie-min.js?x=" 
				+ buildNumber
				+ "\" ></script>\n"

				+ "<script type=\"text/javascript\" src=\"/scripts/tiny_mce/tiny_mce.js?x="
				+ buildNumber
				+ "\" ></script>\n"

				+ "<script type=\"text/javascript\" src=\"/scripts/project/default.js?x="
				+ buildNumber
				+ "\" ></script>\n"
				+ "</head>\n"
				+ "<body class=\"yui-skin-sam\">\n"
				+ "<div id=\"outerContainer\">\n"
				+ "<div id=\"tawalaProjectContainer\">\n"
				+ "<div id=\"form\">"
				+ "<form method=\"POST\" id=\"tawalaProjectForm\" onSubmit=\"return onSubmit(this);\" action=\""
				+ urlToForm
				+ "\">\n"
				+ "<div class=\"text\">Hello, World!</div>"
				+ "<input type=\"hidden\" name=\"_form\" value=\""
				+ userProject.getProject().getRandomFormName(
						userProject.getProject().getForm("Hello"))
				+ "\"></input>\n"
				+ "<input type=\"hidden\" name=\"_segment\" value=\"0\"></input>\n"
				+ "<input type=\"hidden\" name=\""
				+ FormSegment.NEXT_EXPECTED_TOKEN
				+ "\" value=\""
				+ nextToken
				+ "\"></input>\n"
				+ (checkBackButtonPrevention ? "<script>confirmNavigationAwayFromPage = true;</script>\n"
						: "")
				+ "<input type=\"hidden\" name=\""
				+ FormSegment.ORIGINAL_URL
				+ "\" value=\""
				+ urlToFormWithoutSiteName
				+ "\"></input>\n"
				+ "<div class=\"submitButton\"><input class=\"button\" type=\"submit\" value=\"submit\""
				+ "></input>\n"
				+ // TODO: should be single tag?
				"</div>\n" +
				"<div id=\"wait.panel\"></div>\n"
				+ "</form>\n"
				+ "</div>\n"
				+ "<div id=\"innerTawalaFooter\"><div id=\"tawalaFooterLogo\" onclick=\"location.href=\'http://www.tawala.com\'\"></div></div>\n"
				+ "</div><!-- end tawalaProjectContainer -->\n"
				+ "<div id=\"tawalaFooter\" onclick=\"location.href='http://www.tawala.com'\"></div>\n"
				+ "</div><!-- end outerContainer -->\n"
				+ "</body>\n" + "</html>\n";
		assertEquals(html, bot.getPageText().replace("\r\n", "\n"));
	}

	public void testViewSpecificForm() throws RobotException {
		UserProject userProject = new UserProject(ProjectTest.twoForms(),
				projectOwner, "if");
		world.domain().projects().put(userProject);

		bot.go(userProject, "form");
		assertDoesntContain("two", bot.getPageText());

		bot.go(userProject, "form2");
		assertContains("two", bot.getPageText());
	}

	public void testPosting() throws RobotException {
		UserProject userProject = new UserProject(ProjectTest.aProject(),
				projectOwner, "three");
		world.domain().projects().put(userProject);

		bot.go(userProject, "form");
		bot.submit();

		assertContains("Thank you!", bot.getPageText());
	}

	public void testPostingWithEmptyProcess() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		builder.addDocument("doc1", "never seen");
		builder.addForm("form1", builder.addProcess("empty"));
		UserProject userProject = new UserProject(builder.build(),
				projectOwner, "four");
		world.domain().projects().put(userProject);

		bot.go(userProject, "form1");
		bot.submit();
		assertContains("Thank you!", bot.getPageText());
	}

	public void testReplaceProjects() throws RobotException {
		UserProject userProject = registerProject(projectOwner, "proj", "one");
		bot.go(userProject, "Hello");
		assertMatches("one", bot.getPageText());

		userProject = registerProject(projectOwner, "proj", "two");
		bot.go(userProject, "Hello");
		assertMatches("two", bot.getPageText());
	}

	public void testFormWithFollowingDocument() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("doc1", "we win!");
		ProcessBlockBuilder proc1 = builder.addProcess("proc1");
		proc1.addShow(doc1);
		FormBuilder form1 = builder.addForm("form1", proc1);
		form1.addText("hi!");
		UserProject userProject = new UserProject(builder.build(),
				projectOwner, "proj");
		world.domain().projects().put(userProject);

		bot.go(userProject, "form1");
		assertMatches("hi!", bot.getPageText());
		bot.submit();
		assertMatches("we win!", bot.getPageText());
	}

	public void testFormWithFollowingDocuments() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("doc1", "hello");
		DocumentBuilder doc2 = builder.addDocument("doc2", "world");
		ProcessBlockBuilder proc1 = builder.addProcess("proc1");
		proc1.addShow(doc1);
		proc1.addShow(doc2);
		FormBuilder form1 = builder.addForm("form1", proc1);
		form1.addText("hi!");
		UserProject userProject = new UserProject(builder.build(),
				projectOwner, "proj");
		world.domain().projects().put(userProject);

		bot.go(userProject, "form1");
		assertMatches("hi!", bot.getPageText());
		bot.submit();
		assertMatches("<div class=\"document\">.*hello.*</div>" + NEWLINE
				+ "<div class=\"document\">.*world.*</div>", bot.getPageText());
	}

	public void testDocumentWithFields() throws RobotException {
		final Project project = ProjectTest.projectHelloBob();
		UserProject userProject = new UserProject(project, projectOwner,
				"hellobob");
		world.domain().projects().put(userProject);
		bot.go(userProject);
		assertMatches("What's your name?", bot.getPageText());
		bot.setParameter("Q1:a", "Bob");
		bot.submit();
		assertMatches("Hi, Bob.", bot.getPageText());
	}

	public void testProjectWithFibIf() throws RobotException {
		UserProject userProject = new UserProject(ProjectTest
				.projecIfFillInBlank(), projectOwner, "if");
		world.domain().projects().put(userProject);

		checkIfResult(userProject, "question", "Are you happy?",
				"I'm glad to hear that.", "yes");
		checkIfResult(userProject, "question", "Are you happy?",
				"I'm sorry to hear that.", "no");
	}

	public void testProjectWithMcIf() throws RobotException {
		UserProject userProject = new UserProject(ProjectTest
				.projectIfMultipleChoice(), projectOwner, "if");
		world.domain().projects().put(userProject);

		checkIfResult(userProject, "Q1", "What is your favorite fruit?",
				"You like bananas.", "b");
		checkIfResult(userProject, "Q1", "What is your favorite fruit?",
				"You don't like bananas.", "a");
	}

	public void testCompoundIf() throws RobotException {
		ProjectBuilder projectWithAnd = new ProjectBuilder();
		ConditionsBuilder conditions = createAgeQueryShell(projectWithAnd,
				"teenager", "normal");
		conditions.startAnd();
		conditions.addComparison("isGreaterThan", "Form 1:Q1:a", "value", "12");
		conditions.addComparison("isLessThan", "Form 1:Q1:a", "value", "20");
		conditions.endAnd();

		checkTeenagerDetection(projectWithAnd);

		ProjectBuilder projectWithOr = new ProjectBuilder();
		conditions = createAgeQueryShell(projectWithOr, "normal", "teenager");
		conditions.startOr();
		conditions.addComparison("isLessThan", "Form 1:Q1:a", "value", "13");
		conditions.addComparison("isGreaterThan", "Form 1:Q1:a", "value", "19");
		conditions.endOr();

		checkTeenagerDetection(projectWithOr);
	}

	public void testSessionPreservedOnSubsequentGETRequestsToTheSameProject()
			throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder form = builder.addForm("Form1");
		form.addFib().addBlank("name");
		form.addTextWithFields("Your name from existing session: ",
				"<<PreviousName>>");

		ProcessBlockBuilder postProcess = builder.addProcess("Post Process");
		postProcess.addSet("PreviousName", OperandType.FIELD, "Form1:name");
		form.setPostProcess(postProcess);

		UserProject project = new UserProject(builder.build(), projectOwner,
				"test");
		world.domain().projects().put(project);

		// --- First GET doesn't return anything
		bot.go(project);
		WebForm webForm = bot.getForm(0);
		webForm.setParameter("name", "Joe");

		bot.submit(webForm);

		// --- Second GET should see the variable from the first session.
		bot.go(project);
		assertContains("Joe", bot.getPageText());
		webForm = bot.getForm(0);
		webForm.setParameter("name", "Jack");
		bot.submit(webForm);

		// --- Third GET
		bot.go(project);
		assertContains("Jack", bot.getPageText());
	}

	private void checkTeenagerDetection(ProjectBuilder builder)
			throws RobotException {
		Project project = builder.build();
		UserProject userProject = new UserProject(project, projectOwner, "abc");
		world.domain().projects().put(userProject);
		userProject = world.domain().projects().getWithProjectRuntime(
				projectOwner.getId(), "abc");

		bot.go(userProject);
		bot.setParameter("Q1:a", "12");
		bot.submit();
		assertContains("normal", bot.getPageText());

		bot.go(userProject);
		bot.setParameter("Q1:a", "13");
		bot.submit();
		assertContains("teenager", bot.getPageText());

		bot.go(userProject);
		bot.setParameter("Q1:a", "20");
		bot.submit();
		assertContains("normal", bot.getPageText());
	}

	private ConditionsBuilder createAgeQueryShell(ProjectBuilder builder,
			String trueResult, String falseResult) {
		DocumentBuilder doc = builder.addDocument("Doc 1");
		doc.addField("Result");
		ProcessBlockBuilder proc = builder.addProcess("Proc 1");
		IfBuilder ifBuilder = proc.addIf();
		ConditionsBuilder conditions = ifBuilder.conditions();
		ifBuilder.trueSet().addSet("Result", VALUE, trueResult);
		ifBuilder.falseSet().addSet("Result", VALUE, falseResult);
		proc.addShow(doc);
		FormBuilder form = builder.addForm("Form 1", proc);
		form.addFib("Age?", 3);
		return conditions;
	}

	private void checkIfResult(UserProject userProject, String questionId,
			String question, String result, String... answer)
			throws RobotException {
		bot.go(userProject, "form");
		assertMatches(question, bot.getPageText());
		bot.setParameters(questionId, answer);
		bot.submit();
		assertMatches(result, bot.getPageText());
	}

	private UserProject registerProject(User owner, String name, String contents) {
		UserProject userProject = new UserProject(
				parseConfig(ProjectTest.justOneFormXml(name, contents,
						Project.CURRENT_VERSION, null)), owner);
		world.domain().projects().put(userProject);

		return world.domain().projects().getWithProjectRuntime(owner.getId(),
				name);
	}
}
