package com.tawala.web;

import java.io.IOException;
import java.util.ArrayList;

import mock.javax.servlet.http.FakeHttpServletRequest;
import mock.javax.servlet.http.FakeHttpServletResponse;

import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.controller.WellKnown;

public class DeploymentQueryResponseTest extends TestCase {
	private User projectOwner = UserTest.aUser("tester");

	public void testBasics() throws IOException {
		ArrayList<UserProject> projects = new ArrayList<UserProject>();
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("Form 1", true);
		UserProject hello = new UserProject(builder.build(), projectOwner,
				"Hello World");
		hello.setUniqueRandomId("hello123");
		projects.add(hello);

		builder = new ProjectBuilder();
		builder.addForm("survey", true);
		UserProject fruit = new UserProject(builder.build(), projectOwner,
				"Fruit Survey");
		fruit.setUniqueRandomId("fruit123");
		fruit.setWebSiteHostName("localhost:8080");

		projects.add(fruit);
		ApiResponse response = new DeploymentQueryResponse(
				projectOwner.getId(), projects);
		FakeHttpServletRequest request = new FakeHttpServletRequest();
		request.setRequestUrl("http://www.tawala.com/client");
		FakeHttpServletResponse httpResponse = new FakeHttpServletResponse();
		response.handle(request, httpResponse, null);
		assertEquals("text/xml", httpResponse.getContentType());
		assertEquals(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
						+ "<response status=\"success\">\n"
						+ "  <deployments user=\"tester\">\n"
						+ "    <deployment project=\"Hello World\">\n"
						+ "      <startpoint form=\"Form 1\" url=\"http://localhost:8080"
						+ WellKnown.urls.getProjectRunUrlPrefix()
						+ "/hello123/Form+1\"/>\n"
						+ "    </deployment>\n"
						+ "    <deployment project=\"Fruit Survey\">\n"
						+ "      <startpoint form=\"survey\" url=\"http://localhost:8080"
						+ WellKnown.urls.getProjectRunUrlPrefix()
						+ "/fruit123/survey\"/>\n" + "    </deployment>\n"
						+ "  </deployments>\n" + "</response>\n", httpResponse
						.getOutput());
	}
}
