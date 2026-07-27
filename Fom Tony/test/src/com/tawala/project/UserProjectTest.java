package com.tawala.project;

import java.util.Map;

import junit.framework.TestCase;

import com.tawala.domain.DomainMetadata;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.controller.WellKnown;

public class UserProjectTest extends TestCase {

	public void testEntryPointURLs() {
		Project project = ProjectBuilder.buildMinimalisticProject();
		User user = UserTest.aUser("joe");

		UserProject userProject = new UserProject(project, user, "test project");
		userProject.setUniqueRandomId("123abc");

		userProject.setWebSiteHostName("www.tawala.com");

		Map<Form, String> urls = userProject.getEntryPointURLs();
		assertEquals(1, urls.size());
		assertEquals("http://www.tawala.com"
				+ WellKnown.urls.getProjectRunUrlPrefix() + "/123abc/"
				+ project.getForms().get(0).getName(), urls.values().iterator()
				.next());

		userProject.setWebSiteHostName("loadbalancer.tawala.com:5577");

		urls = userProject.getEntryPointURLs();
		assertEquals("http://loadbalancer.tawala.com:5577"
				+ WellKnown.urls.getProjectRunUrlPrefix() + "/123abc/"
				+ project.getForms().get(0).getName(), urls.values().iterator()
				.next());
	}

	public void testUniqueNameGeneration() {
		Project project = ProjectBuilder.buildMinimalisticProject();
		User user = UserTest.aUser("joe");

		UserProject userProject = new UserProject(project, user, "test project");
		userProject.makeProjectNameUnique();

		assertFalse("test project".equals(userProject.getName()));

		UserProject anotherProject = new UserProject(project, user,
				"test project");
		anotherProject.makeProjectNameUnique();

		assertFalse(userProject.getName().equals(anotherProject.getName()));

		StringBuilder longName = new StringBuilder();
		for (int i = 0; i < 20; i++) {
			longName
					.append("Long chunk of text to create a name that exceeds the maximum size of the field");
		}
		userProject = new UserProject(project, user, longName.toString());
		userProject.makeProjectNameUnique();

		assertTrue(userProject.getName().length() <= DomainMetadata.instance
				.getUserProjectNameMaxLength());
	}

}
