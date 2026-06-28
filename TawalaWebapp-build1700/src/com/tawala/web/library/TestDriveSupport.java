package com.tawala.web.library;

import java.util.List;

import com.tawala.World;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.web.WorldInitializer;

public class TestDriveSupport {

	public static void copyExistingSubmissionsToTestWorld(Project project,
			World testDriveWorld) {
		for (Form form : project.getForms()) {
			List<FormSubmission> submissions = WorldInitializer
					.getDefaultWorld().domain().storedData()
					.fullyInitializedResponsesFor(project, form);
			for (FormSubmission submission : submissions) {
				testDriveWorld.domain().storedData().record(submission);
			}
		}
	}
}
