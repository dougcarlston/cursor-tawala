package com.tawala.web.project;

import java.io.IOException;
import java.io.PrintWriter;

import javax.servlet.ServletContext;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.World;
import com.tawala.project.Form;
import com.tawala.project.FormSegment;
import com.tawala.project.Image;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.ImageResponse;
import com.tawala.web.OldPageResponse;
import com.tawala.web.PathElement;
import com.tawala.web.RedirectResponse;
import com.tawala.web.Request;
import com.tawala.web.Response;
import com.tawala.web.WorldInitializer;
import com.tawala.web.admin.UrgentMessage;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.oldhtml.RenderingContext;

abstract public class ProjectController implements Controller {
	private static final String IMAGE_PATH_WITHOUT_SLASH = "___image";
	public static final String IMAGE_PATH = IMAGE_PATH_WITHOUT_SLASH + "/";
	private static final String COMPONENT_PATH_WITHOUT_SLASH = "__component";
	public static final String COMPONENT_PATH = COMPONENT_PATH_WITHOUT_SLASH
			+ "/";

	private ServletContext servletContext;
	
	public ProjectController(ServletContext servletContext) {
		this.servletContext = servletContext;
	}
	
	public final ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		Request requestObject = new Request(request);
		try {
			if (requestObject.path().size() < 4)
				throw new NotFoundException("project path too short: '"
						+ request.getRequestURI() + "'");

			World world = getWorld(request);
			if (world == null) {
				// --- Case of expired session on the test drive.
				// --- TODO: special message.
				Log
						.warn(this,
								"Unable to create a world from request. Most likely cause - expired session.");
				return new ModelAndView("project.not.found");
			}

			Response responseObject = null;

			LinkToUserProject link = fetchProject(world, requestObject);

			if (requestObject.path().size() >= 4
					&& getFormName(requestObject).equals(
							IMAGE_PATH_WITHOUT_SLASH)) {
				responseObject = fetchImage(link.getProject().getProject(),
						requestObject.path().element(4));
			} else if (requestObject.path().size() >= 4
					&& getFormName(requestObject).equals(
							COMPONENT_PATH_WITHOUT_SLASH)) {
				responseObject = generateCustomComponentResponse(requestObject,
						world, link, requestObject.path().element(4).getName());
			} else if (link.getProject().isOffline()) {
				responseObject = generateOffLinePage(link.getProject());
			} else if(link.getProject().isRequireSSL() && ! request.isSecure()) {
				responseObject = createRedirectRequest(requestObject);
			} else {
				
				Form form = fetchForm(link.getProject().getProject(),
						requestObject);

				responseObject = generateResponse(requestObject, world, link,
						form);
			}
			
			UrgentMessage urgentMessage = UrgentMessage.get(servletContext);
			if(urgentMessage != null) {
				responseObject.handleUrgentNotificationMessage(urgentMessage);
			}

			responseObject.process(request, response, world);
		} catch (NotFoundException e) {
			Log.warn(this, "Error looking up project or form", e);
			return new ModelAndView("project.not.found");
		}

		return null;
	}

	private Response createRedirectRequest(Request requestObject) {
		if(requestObject.isPost()) {
			throw new IllegalStateException("Attempt to post data to a secure project through an unsecure connection.");
		}
		String url = "https://" + requestObject.getHttpRequest().getRequestURL().substring("http://".length());
		return new RedirectResponse(url);
	}

	private Response generateOffLinePage(UserProject project) {
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), project);

		OldPage page = new OldPage();
		page.add(new ProjectOfflineDocumentHtml());

		DataCollectingProjectController.addHeaderAndStyleSheets(project
				.getProject(), context, page);

		return new OldPageResponse(page);
	}

	abstract protected World getWorld(HttpServletRequest request);

	abstract protected LinkToUserProject fetchProject(World world,
			Request request) throws NotFoundException;

	abstract protected Response generateResponse(Request request, World world,
			LinkToUserProject link, Form form) throws IOException;

	abstract protected Response generateCustomComponentResponse(
			Request requestObject, World world, LinkToUserProject link,
			String name);

	private Response fetchImage(Project project, PathElement element)
			throws NotFoundException {
		Image image = project.getImage(element.getName());
		if (image == null)
			throw new NotFoundException("Unable to find image '"
					+ element.getName() + "' in project '" + project.getId()
					+ "'.");

		Image.Data data = image.getTheBestDataForWeb();
		if (data == null)
			throw new IllegalStateException("Unable to find image '"
					+ element.getName() + "' in project '" + project.getId()
					+ "'.");

		return new ImageResponse(data);
	}

	private Form fetchForm(Project project, Request request)
			throws NotFoundException {
		String formId = request.getParameter(FormSegment.FORM_ID);
		if (formId == null)
			formId = getFormName(request);
		Form form = project.getFormByRandomToken(formId);
		if (form == null)
			throw new NotFoundException("couldn't find form '" + formId + "'.");
		return form;
	}

	protected String getRandomProjectId(Request request) {
		return request.path().element(2).getName();
	}

	protected String getFormName(Request request) {
		return request.path().element(3).getName();
	}

	private static class ProjectOfflineDocumentHtml implements Html {

		public void render(PrintWriter out, RenderingContext renderingContext) {
			out
					.println("<h1 class=\"heading\">Project Is Not Available</h1>"
							+ "<div class=\"text error\">You can't access the project at this time.<br />The project owner took this project off-line.</div>");

			out.println("<br />");
			out.println("<br />");
			out.println("<div class=\"produced-by\">");
			out.println("<a href=\"http://www.tawala.com\">");
			out.println("Produced using the Tawala Designer<br />");
			out.println("Tawala Systems, Inc.<br />");
			out.println("www.tawala.com</a>");
			out.println("</div>");
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}
	}
}
