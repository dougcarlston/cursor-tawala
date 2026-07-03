package com.tawala.project.data;

import java.util.Map;

import com.tawala.TestCase;
import com.tawala.domain.UserTest;
import com.tawala.project.ProjectVersion;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;

public class ProjectToExcelExporterTest extends TestCase {

	public void testMappingOfFormNamesToSheetNames() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		String normalFormName = "NormalForm";
		projectBuilder.addForm(normalFormName);
		String veryLongFormName = "Form with a very long name that should be truncated because it's too long.";
		projectBuilder.addForm(veryLongFormName);
		String specialCharactersFormName = "Form with special characters ? / \\ [ ]";
		projectBuilder.addForm(specialCharactersFormName);
		
		//--- These two will translate into the same sheet name and the de-duping process should resolve the problem.
		String duplicateFormName1 = "Duplicate Form ?";
		projectBuilder.addForm(duplicateFormName1);
		String duplicateFormName2 = "Duplicate Form [";
		projectBuilder.addForm(duplicateFormName2);
		String duplicateFormName3 = "Duplicate Form ]";
		projectBuilder.addForm(duplicateFormName3);
		
		UserProject userProject = new UserProject(projectBuilder.build(), UserTest.aUser(), "test");
		ProjectVersion projectVersion = new ProjectVersion(userProject, userProject.getProject());
		userProject.deployVersion(projectVersion, false);
		
		ProjectToExcelExporter exporter = new ProjectToExcelExporter();
		exporter.addCoverSheet(userProject);
		
		Map<String, String > map = exporter.getFormNameToSheetNameMap();
		assertEquals(normalFormName, map.get(normalFormName));
		assertEquals(veryLongFormName.substring(0, 31), map.get(veryLongFormName));
		assertEquals(duplicateFormName1.replace('?', '-'), map.get(duplicateFormName1));
		assertEquals(duplicateFormName2.replace('[', '-') + '1', map.get(duplicateFormName2));
		assertEquals(duplicateFormName3.replace(']', '-') + '2', map.get(duplicateFormName3));
	}
	
	public void testQuotingMultivalueStrings() {
		assertEquals("plain string", ProjectToExcelExporter.quoteIfRequiredByCSVFormat("plain string"));
		assertEquals("\"string with , comma inside\"", ProjectToExcelExporter.quoteIfRequiredByCSVFormat("string with , comma inside"));
		assertEquals("\"\"\"Good job, my dear!\"\" said Jack.\"", ProjectToExcelExporter.quoteIfRequiredByCSVFormat("\"Good job, my dear!\" said Jack."));
		assertEquals("\"Line one\nLine two separated by new line\"", ProjectToExcelExporter.quoteIfRequiredByCSVFormat("Line one\nLine two separated by new line"));
		assertEquals("\"Line one\rLine two separated by CR\"", ProjectToExcelExporter.quoteIfRequiredByCSVFormat("Line one\rLine two separated by CR"));
	}
}
