package com.tawala.acceptance;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Form;
import com.tawala.project.FillInBlank;
import com.tawala.project.Blank;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.LetterCounter;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.TagBuilder;

public class FibBlankValidationTest extends AcceptanceTestCase {

    public void testPhoneNumberValidationGeneratesFormattedNumber() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        // add form that has a FIB with a Phone Number validated Blank
        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFibWithPhoneNumberValidatedBlank("Phone number:", 10);
        
        // add a second page to display the result
        form1.addBreak();
        form1.addTextWithFields("Number after validation: ",
                "<<Form 1:Q1:a>>");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testFibBlankField");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        // fill in blank with an unformatted number
        bot.setParameter("Q1:a", "4155551212");

        bot.submit();

        // verify formatted result on the second page
        assertContains("Number after validation: (415) 555-1212",
                bot.getPageText());
    }

    public void testDollarAmountValidationGeneratesValuesWithTwoDecimalPlaces() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        // add form that has a FIB with a Dollar Amount validated Blanks
        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFibWithDollarAmountValidatedBlank("Amount 1:", 10);
        form1.addFibWithDollarAmountValidatedBlank("Amount 2:", 10);
        form1.addFibWithDollarAmountValidatedBlank("Amount 3:", 10);
        form1.addFibWithDollarAmountValidatedBlank("Amount 4:", 10);
     
        // add a second page to display the results
        form1.addBreak();
        form1.addTextWithFields("Amount 1 after validation: ", "<<Form 1:Q1:a>>");
        form1.addTextWithFields("Amount 2 after validation: ", "<<Form 1:Q2:a>>");
        form1.addTextWithFields("Amount 3 after validation: ", "<<Form 1:Q3:a>>");
        form1.addTextWithFields("Amount 4 after validation: ", "<<Form 1:Q4:a>>");
 
        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testFibBlankField");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        // fill in blanks with various number formats
        bot.setParameter("Q1:a", "1");
        bot.setParameter("Q2:a", "2.");
        bot.setParameter("Q3:a", "3.0");
        bot.setParameter("Q4:a", "4.00");

        bot.submit();

        // verify formatted results on the second page
        assertContains("Amount 1 after validation: 1.00",
                bot.getPageText());
        assertContains("Amount 2 after validation: 2.00",
                bot.getPageText());
        assertContains("Amount 3 after validation: 3.00",
                bot.getPageText());
        assertContains("Amount 4 after validation: 4.00",
                bot.getPageText());
        
    }
}
