package com.tawala.web.library;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;

import com.tawala.domain.EmailAddress;
import com.tawala.domain.User;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.UserProject;
import com.tawala.project.commands.Send;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.web.WorldInitializer;

public class ViewProjectVersionSampleData extends ProjectVersionController {

	public static final String SAMPLE_COLLECTING_PROJECT_ATTRIBUTE = "sampleCollectingProject";

	@Override
	public ModelAndView doHandle(HttpServletRequest request,
			HttpServletResponse response, LibraryProject project,
			LibraryProjectVersion version) throws Exception {

		Map<Form, List<FormSubmission>> submissions = new LinkedHashMap<Form, List<FormSubmission>>();
		for (Form form : version.getProject().getForms()) {
			submissions.put(form, WorldInitializer.getDefaultWorld().domain()
					.storedData().responsesFor(version.getProject(), form));
		}

		ModelAndView result = new ModelAndView("library.view.sample.data");
		result.addObject("userProject", setupProjectForRecordingSampleData(
				project, version, request.getSession()));
		result.addObject("project", project);
		result.addObject("version", version);
		result.addObject("submissions", submissions);

		return result;
	}

	private UserProject setupProjectForRecordingSampleData(
			LibraryProject project, LibraryProjectVersion version,
			HttpSession session) {
		UserProject runnableProject = new UserProject(version.getProject(),
				new User("test", "test", "test", new EmailAddress(Send.DEFAULT_FROM_ADDRESS),
						"none"), project.getName());
		runnableProject.setUniqueRandomId(String.valueOf(version.getId()));
		LinkToUserProject linkToUserProject = LinkToUserProject
				.createUnauthenticatedLink(runnableProject, runnableProject
						.getUniqueRandomId());

		linkToUserProject.getProject().getProject()
				.populateFormTokensWithFormNames();

		session.setAttribute(SAMPLE_COLLECTING_PROJECT_ATTRIBUTE,
				linkToUserProject);

		return runnableProject;
	}
}
