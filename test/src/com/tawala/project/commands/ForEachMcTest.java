package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.Form;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.FakeRequest;
import com.tawala.web.Request;

public class ForEachMcTest extends TestCase {
	private static final String USER_NAME = "testuser";

	private ConfigElement getXml;
	private ConfigElement forEachXml;
	private Domain domain;
	private User owner;
	private HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();

	public ForEachMcTest() {
		setUserNamesToDelete(new String[] { USER_NAME });
	}

	public void setUp() throws Exception {
		super.setUp();

		hibernateTestSetup.onSetUp();

		getXml = parseConfig("<get recordList=\"RecordList\">\n" + "<forms>\n"
				+ "<form name=\"Form 1\" />\n" + "</forms>\n"
				+ "<conditions>\n" + "</conditions>\n" + "</get>");

		domain = new Domain(new UsersHibernateImpl());

		owner = UserTest.aUser(USER_NAME);
		domain.users().addOrSave(owner);
	}

	@Override
	protected void tearDown() throws Exception {
		hibernateTestSetup.onTearDown();
		super.tearDown();
	}

	public void testCommandCreation() {

		forEachXml = parseConfig("<forEachMc>\n" + "</forEachMc>");

		ForEachMc forEach = new ForEachMc(forEachXml);
		assertEquals(0, forEach.commands().size());
	}

	public void testEqualsValue() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "  <forEachMc>\n"
				+ "    <if>\n"
				+ "      <conditions>\n"
				+ "        <mcEquals field=\"Record:Form 1:(selection)\" value=\"a\"/>\n"
				+ "      </conditions>\n"
				+ "      <trueSet>\n"
				+ "        <addTo field=\"Score\">\n"
				+ "           <operand value=\"10\"/>\n"
				+ "        </addTo>\n"
				+ "      </trueSet>\n"
				+ "      <falseSet>\n"
				+ "        <addTo field=\"Score\">\n"
				+ "           <operand value=\"1\"/>\n"
				+ "        </addTo>\n"
				+ "      </falseSet>\n"
				+ "    </if>"
				+ "  </forEachMc>"
				+ "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);
		assertEquals(1, forEach.commands().size());

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addMc("MC Item 1", "Choice A", "Choice B");

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testExecution");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Q1", "a");
		record(userProject, form, "Q1", "b");

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);

		forEach.execute(context);
		assertEquals("11", context.getValue("Score").toString());
	}

	public void testEqualsQualifiedMc() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "  <forEachMc>\n"
				+ "    <if>\n"
				+ "      <conditions>\n"
				+ "        <mcEquals field=\"Form 1:(selection)\">\n"
				+ "          <string field=\"Record:Form 1:(selection)\" />\n"
				+ "        </mcEquals>\n"
				+ "      </conditions>\n"
				+ "      <trueSet>\n"
				+ "        <addTo field=\"Score\">\n"
				+ "           <operand value=\"10\"/>\n"
				+ "        </addTo>\n"
				+ "      </trueSet>\n"
				+ "      <falseSet>\n"
				+ "        <addTo field=\"Score\">\n"
				+ "           <operand value=\"1\"/>\n"
				+ "        </addTo>\n"
				+ "      </falseSet>\n"
				+ "    </if>"
				+ "  </forEachMc>"
				+ "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);
		assertEquals(1, forEach.commands().size());

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addMc("MC Item 1", "Choice A", "Choice B");

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testEqualsQualifiedMc");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Q1", "a");
		record(userProject, form, "Q1", "b");

		Request request = makeCurrentSubmission(userProject, form, "Q1", "a");
		FakeExecutionContext context = new FakeExecutionContext(domain,
				new UserProject(project, owner, "test"), form, request);

		get.execute(context);

		forEach.execute(context);
		assertEquals("11", context.getValue("Score").toString());
	}

	private FakeExecutionContext makeContext(Project project, Form form) {
		return new FakeExecutionContext(domain, new UserProject(project, owner,
				"test"), form);
	}

	private Request makeCurrentSubmission(UserProject project, Form form,
			String q, String... qSelections) {

		LinkedHashMap<String, List<String>> parameters = new LinkedHashMap<String, List<String>>();

		parameters.put(q, valueList(qSelections));

		return new FakeRequest(true, parameters);
	}

	private List<String> valueList(String[] valueArray) {

		List<String> valueList = new ArrayList<String>();

		for (int i = 0; i < valueArray.length; i++) {
			valueList.add(valueArray[i]);
		}

		return valueList;
	}

	private void record(UserProject project, Form form, String... data) {
		ExecutionContext context = new FakeExecutionContext(project, form,
				new FakeRequest(true, data));
		domain.storedData().record(context.getSubmission());
	}

}
