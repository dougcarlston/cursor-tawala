package com.tawala.project.commands;

import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Process;

public class MathCommandsTest extends TestCase {
    private User projectOwner = UserTest.aUser("tester");

    public void testNewFieldCreated() {
        assertEquals("3.50", executeWithLiteralValue("addTo", "3.50", null));
    }

    public void testMixedAddition() {
        assertEquals("5.00", executeWithLiteralValue("addTo", "3.50", "1.5"));
    }

    public void testFieldAddition() {
        assertEquals("5.00", executeWithFieldValue("addTo", "3.50", "1.5"));
    }

    public void testSubtraction() {
        assertEquals("5", executeWithFieldValue("subtractFrom", "3", "8"));
    }

    public void testMultiplication() {
        assertEquals("24", executeWithFieldValue("multiplyBy", "3", "8"));
    }

    public void testDivision() {
        assertEquals("4", executeWithFieldValue("divideBy", "2", "8"));
    }

    //--- TODO: commented out until JDK is upgraded until 1.6
    public void XXXtestDivisionRounding() {
        assertEquals("0.63", executeWithFieldValue("divideBy", "16", "10"));
    }

    private String executeWithLiteralValue(String command, String literalValue,
            String fieldValue) {
        Process process = mathProcessFor(command, "<operand value='"
                + literalValue + "'/>");
        ExecutionContext context = FakeExecutionContext
                .basicContext(projectOwner);
        if (fieldValue != null)
            context.setValue("total", fieldValue);
        process.execute(context);
        return context.getValue("total").toString();
    }

    private String executeWithFieldValue(String command, String unchangedValue,
            String changedValue) {
        Process process = mathProcessFor(command, "<operand field='other'/>");
        ExecutionContext context = FakeExecutionContext
                .basicContext(projectOwner);
        context.setValue("other", unchangedValue);
        context.setValue("total", changedValue);
        process.execute(context);
        assertEquals(unchangedValue, context.getValue("other").toString());
        return context.getValue("total").toString();
    }

    private static Process mathProcessFor(String command, String operand) {
        return new Process(parseConfig("<process name=\"proc1\">\n" + "<"
                + command + " field='total'>" + operand + "</" + command + ">"
                + "</process>\n"));
    }

}
