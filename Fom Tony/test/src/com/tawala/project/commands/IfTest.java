package com.tawala.project.commands;

import java.util.Iterator;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.domain.UserTest;
import com.tawala.project.Document;
import com.tawala.project.Form;
import com.tawala.project.Process;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.oldhtml.Html;

public class IfTest extends TestCase {
    public void testFunctioning() {
        Project project = ProjectBuilder.buildMinimalisticProject();
        Form form = new Form(
                parseConfig("<form name=\"form1\" process=\"proc1\">\n"
                        + "<items>\n"
                        + "<fib label=\"Q1\">Name?<blank label=\"a\" length=\"20\"/></fib>\n"
                        + "</items>\n" + "</form>\n"));
        project.add(form);

        project.add(new Document("doc1", "doc1"));
        project.add(new Document("doc2", "doc2"));

        If anIf = new If(new Equals("form1:Q1:a", "foo"), new ShowDocument("doc1"),
                new ShowDocument("doc2"));
        Process process = new Process("proc1");
        process.add(anIf);
        project.add(process);

        executeAndExpect("form1", "doc1", project, anIf, "foo");
        executeAndExpect("form1", "doc2", project, anIf, "bar");
    }

    public void testBasicXmlParsing() {
        ConfigElement xml = parseConfig("<if>\n" + "  <conditions>\n"
                + "    <stringEquals  field=\"Q1:a\">\n"
                + "      <string value=\"foo\" />\n" + "    </stringEquals>\n"
                + "  </conditions>\n" + "  <trueSet>\n"
                + "    <show document=\"a\" />\n" + "  </trueSet>\n"
                + "  <falseSet>\n" + "    <show document=\"b\" />\n"
                + "  </falseSet>\n" + "</if>");
        If anIf = new If(xml);

        Equals condition = (Equals) anIf.condition();
        assertEquals(new ReferenceOperator("Q1:a"), condition.left());
        assertEquals(new LiteralOperator("foo"), condition.right());

        ProcessCommandList trueList = (ProcessCommandList) anIf.getThen();
        assertEquals(1, trueList.size());
        ShowDocument trueShow = (ShowDocument) trueList.get(0);
        assertEquals("a", trueShow.getDocumentName());

        ProcessCommandList falseList = (ProcessCommandList) anIf.getOtherwise();
        assertEquals(1, falseList.size());
        ShowDocument falseShow = (ShowDocument) falseList.get(0);
        assertEquals("b", falseShow.getDocumentName());

        assertFalse(xml.hasUnused());
    }

    // TODO: test XML with multiple conditions

    public void testAnd() {
        ConfigElement xml = parseConfig("<if>\n" + "  <conditions>\n"
                + "    <and>\n" + "      <mcEquals field=\"Q1\">\n"
                + "        <string value=\"a\"/>\n" + "      </mcEquals>\n"
                + "      <mcEquals field=\"Q2\">\n"
                + "        <string value=\"a\"/>\n" + "      </mcEquals>\n"
                + "    </and>\n" + "  </conditions>\n" + "  <trueSet>\n"
                + "  </trueSet>\n" + "</if>");

        If anIf = new If(xml);

        BooleanExpression condition = anIf.condition();
        assertTrue(condition instanceof ConditionList);

        Iterator<BooleanExpression> i = ((ConditionList) condition).contents()
                .iterator();
        MultipleChoiceEquals condition1 = (MultipleChoiceEquals) i.next();
        MultipleChoiceEquals condition2 = (MultipleChoiceEquals) i.next();
        assertEquals("Q1", condition1.fieldId());
        assertEquals("Literal 'a'", condition1.operator().toString());
        assertEquals("Q2", condition2.fieldId());
        assertEquals("Literal 'a'", condition2.operator().toString());

        assertFalse(xml.hasUnused());
    }

    private void executeAndExpect(String formName, String output,
            Project project, If anIf, String value) {
        Form form = project.getForm(formName);
        assertNotNull("form", form);
        FakeExecutionContext context = new FakeExecutionContext(
                new UserProject(project, UserTest.aUser(), "test"), form,
                "Q1:a", value);
        ExecutionResult result = anIf.execute(context);
        List<Html> html = result.getHtml();
        assertEquals(1, html.size());
        assertContains(output, html.get(0).toString());
    }

}
