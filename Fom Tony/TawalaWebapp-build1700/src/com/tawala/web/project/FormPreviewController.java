package com.tawala.web.project;

import java.io.IOException;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import com.scissor.Log;
import com.tawala.UsersPersistentBunchImpl;
import com.tawala.World;
import com.tawala.project.Document;
import com.tawala.project.Form;
import com.tawala.project.FormSegment;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.theme.ProjectTheme;
import com.tawala.web.OldPageResponse;
import com.tawala.web.Request;
import com.tawala.web.Response;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.OldPage;

public class FormPreviewController extends ProjectController {
	public FormPreviewController(ServletContext servletContext) {
		super(servletContext);
	}

	private static final Document DEFAULT_DOCUMENT = new Document("notFound",
			"This form didn't produce any output!");

	@Override
	protected World getWorld(HttpServletRequest request) {
		HttpSession session = request.getSession(true);
		if (session == null) {
			throw new IllegalStateException("Unable to create a new session");
		}

		World testDriveWorld = new World(WorldInitializer.getRealPath(),
				new UsersPersistentBunchImpl());
		session.setAttribute(
				ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME,
				testDriveWorld);

		return testDriveWorld;
	}

	@Override
	protected LinkToUserProject fetchProject(World world, Request request)
			throws NotFoundException {
		LinkToUserProject linkToUserProject = WorldInitializer
				.getDefaultWorld().domain().formPreviews().getProject(
						getRandomProjectId(request));
		if (linkToUserProject == null)
			throw new NotFoundException("couldn't find project");
		return linkToUserProject;
	}

	@Override
	protected Response generateResponse(Request request, World world,
			LinkToUserProject link, Form form) throws IOException {
		ExecutionContext executionContext = new ExecutionContext(
				world.domain(), link, form, request,
				EntryPointType.REAL_PROJECT);
		executionContext.setPreviewMode(true);

		boolean firstSegment = true;
		OldPage completePage = new OldPage();

		if (link.getProject().getProject().getPageHeader() != null) {
			completePage.addToFront(link.getProject().getProject()
					.getPageHeader().toHtml(executionContext));
		}

		for (FormSegment segment : form.getSegments()) {
			if (firstSegment) {
				firstSegment = false;
			} else {
				completePage
						.add(new HtmlReadyString(
								"<div align=\"center\" style=\"color: #ffaaaa; border-bottom: 1px solid #ffaaaa;\">Page Break</div>"));
			}
			if (!segment.isEmpty()) {
				completePage.addContents(segment.toPage(executionContext));
			}
		}

		if (completePage.isEmpty()) {
			completePage = new OldPage(DEFAULT_DOCUMENT.toHtml(null));
		}

		ProjectTheme theme = executionContext.getTheme();
		completePage.addPrintStylesheets(theme.getPrintStylesheetURLs());
		completePage.addScreenStylesheets(theme.getScreenStylesheetURLs());

		Log.info(this, request.getMethod() + " to form preview " + form);
		return new OldPageResponse(completePage);
	}

	@Override
	protected Response generateCustomComponentResponse(Request requestObject,
			World world, LinkToUserProject link, String name) {
		throw new IllegalStateException(
				"Custom component response is not implemented for the form previews.");
	}
}
