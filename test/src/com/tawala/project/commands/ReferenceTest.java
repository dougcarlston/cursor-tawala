package com.tawala.project.commands;

import junit.framework.TestCase;

import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class ReferenceTest extends TestCase {
	
	public void testVariable() {
		FakeExecutionContext fakeExecutionContext = createContext();
		assertReference(new Reference("Variable", fakeExecutionContext), null, null, "Variable");
	}

	public void testFormReference() {
		FakeExecutionContext fakeExecutionContext = createContext();
		assertReference(new Reference("Form1:Field1", fakeExecutionContext), null, "Form1", "Field1");
		assertReference(new Reference("Form1:Field1:a", fakeExecutionContext), null, "Form1", "Field1:a");
		assertReference(new Reference("FormXXX:Field1", fakeExecutionContext), null, null, "FormXXX:Field1");
	}

	public void testRecordReference() {
		FakeExecutionContext fakeExecutionContext = createContext();
		
		Reference reference = new Reference("RecordList:Form1:Field1", fakeExecutionContext);
		assertNull("Record", reference.getRecordName());

		fakeExecutionContext.mapRecord("RecordList", new CompositeFormSubmission("Records"));

		reference = new Reference("RecordList:Form1:Field1", fakeExecutionContext);
		assertReference(reference, "RecordList", "Form1", "Field1");

		reference = new Reference("RecordList:Form1:Field1:a", fakeExecutionContext);
		assertReference(reference, "RecordList", "Form1", "Field1:a");
	}

	private void assertReference(Reference reference, String recordName, String formName, String fieldName) {
		if(recordName == null) {
			assertNull("Record Name", reference.getRecordName());
		} else {
			assertNotNull("Record Name", reference.getRecordName());
			assertEquals("Record Name", recordName, reference.getRecordName());
		}
		if(formName == null) {
			assertNull("Form Name", reference.getFormName());
		} else {
			assertNotNull("Form Name", reference.getFormName());
			assertEquals("Form Name", formName, reference.getFormName());
		}
		assertNotNull("Field Name", reference.getFieldName());
		assertEquals("Field Name", fieldName, reference.getFieldName());
	}

	private FakeExecutionContext createContext() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form1");
		formBuilder.addFib().addBlank("Field1");
		
		FakeExecutionContext fakeExecutionContext = new FakeExecutionContext(projectBuilder.build());
		return fakeExecutionContext;
	}
}
