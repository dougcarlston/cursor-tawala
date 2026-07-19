package com.tawala.web.client;

import java.io.IOException;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.dom4j.Element;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.World;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.Form;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.email.EmailRuntimeConfig;
import com.tawala.email.EmailService;
import com.tawala.email.UniqueBodyEmail;
import com.tawala.email.UserProjectEmail;
import com.tawala.web.ApiErrorResponse;
import com.tawala.web.ApiRequest;
import com.tawala.web.ApiResponse;
import com.tawala.web.DataSourceQueryResponse;
import com.tawala.web.DeploymentQueryResponse;
import com.tawala.web.FormPreviewResponse;
import com.tawala.web.Request;
import com.tawala.web.Response;
import com.tawala.web.WorldInitializer;

public class ClientApiController implements Controller {

	private static final long TEST_EMAIL_MIN_INTERVAL_MS = 15000L;
	private static volatile long lastTestEmailAt = 0L;

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		Request requestObject = new Request(request);
		Response responseObject = handle(WorldInitializer.getDefaultWorld(),
				requestObject);
		responseObject.process(request, response, WorldInitializer
				.getDefaultWorld());
		return null;
	}

	private Response handle(World world, Request request) throws Exception {
		ApiResponse response;
		ApiRequest apiRequest = new ApiRequest(new ConfigElement(request
				.getContentAsString()));
		User user = apiRequest.retrieveUserAndValidateCredentials(world);
		if (user == null) {
			response = new ApiErrorResponse("auth.failed",
					"unknown userid or password");
		} else if (apiRequest.getType().equals("queryDeployments")) {
			response = queryDeployments(apiRequest);
		} else if (apiRequest.getType().equals("queryDataSources")) {
			response = queryDataSources(user);
		} else if (apiRequest.getType().equals("uploadProject")) {
			response = uploadProject(apiRequest, request, world, user);
		} else if (apiRequest.getType().equals("previewForm")) {
			response = previewForm(apiRequest, request, world, user);
		} else if (apiRequest.getType().equals("queryEmailStatus")) {
			response = new EmailStatusResponse();
		} else if (apiRequest.getType().equals("sendTestEmail")) {
			response = sendTestEmail(apiRequest);
		} else {
			response = new ApiErrorResponse("command.unknown",
					"Unknown command '" + apiRequest.getType() + "'.");
		}
		// apiRequest.addUnusedXmlWarnings(response);
		return response;
	}

	private ApiResponse queryDataSources(User user) {
		Project sharedStorage = WorldInitializer.getDefaultWorld().domain()
				.users().getSharedStorageForUser(user);
		return new DataSourceQueryResponse(sharedStorage == null ? null
				: sharedStorage.getDataSources());
	}

	private ApiResponse queryDeployments(ApiRequest apiRequest) {
		String userId = apiRequest.getUserId();
		return new DeploymentQueryResponse(userId, allProjectsFor(userId));
	}

	private ApiResponse uploadProject(ApiRequest apiRequest, Request request,
			World world, User user) {
		String userId = apiRequest.getUserId();
		ConfigElement projectElement = apiRequest.getXml().child("project");
		UserProject project = new UserProject(projectElement, user);

		project = world.domain().projects().put(project);

		Event event = new Event("ProjectUploadThroughDesigner", project
				.getName());
		event.setUserId(user.getDatabaseId());
		EventService.createEvent(event);

		return new UploadProjectResponse(userId, allProjectsFor(userId),
				project);
	}

	private ApiResponse previewForm(ApiRequest apiRequest, Request request,
			World world, User user) throws Exception {
		final UserProject project = new UserProject(apiRequest.getXml().child(
				"project"), user);
		world.domain().formPreviews().put(user.getId(), project);
		return new FormPreviewResponse(project, apiRequest.getFormName());
	}

	private ApiResponse sendTestEmail(ApiRequest apiRequest) {
		long now = System.currentTimeMillis();
		if (now - lastTestEmailAt < TEST_EMAIL_MIN_INTERVAL_MS) {
			return new ApiErrorResponse("email.rateLimited",
					"Wait a few seconds before sending another test email.");
		}
		EmailRuntimeConfig config = EmailRuntimeConfig.get();
		if (!config.isDeliveryReady()) {
			return new ApiErrorResponse("email.disabled",
					"Outbound email is not configured on this server.");
		}
		String to;
		try {
			if (!apiRequest.getXml().hasChild("testEmail")) {
				return new ApiErrorResponse("email.invalid",
						"testEmail/@to is required");
			}
			to = apiRequest.getXml().child("testEmail").attribute("to").stringValue();
		} catch (RuntimeException e) {
			return new ApiErrorResponse("email.invalid",
					"testEmail/@to is required");
		}
		if (to == null || to.trim().length() == 0 || to.indexOf('@') < 0) {
			return new ApiErrorResponse("email.invalid",
					"A valid test recipient address is required.");
		}
		try {
			String from = config.getFromName() + " <" + config.getFromAddress() + ">";
			UserProjectEmail email = new UserProjectEmail(null, from, to.trim(),
					null, "Tawala email delivery test",
					UniqueBodyEmail.Type.TEXT,
					"This is a test message from the Tawala Designer Email Delivery dialog.\n"
							+ "If you received it, SMTP delivery is working.\n");
			// Send immediately so the Designer dialog can report success/failure.
			boolean previous = EmailService.isSendImmediately();
			EmailService.setSendImmediately(true);
			try {
				EmailService.enqueueForDelivery(email);
			} finally {
				EmailService.setSendImmediately(previous);
			}
			if (email.getState() != com.tawala.email.Email.State.SENT) {
				String reason = email.getCustomerErrorReason();
				if (reason == null || reason.length() == 0) {
					reason = email.getErrorReason();
				}
				if (reason == null || reason.length() == 0) {
					reason = "Test email failed";
				}
				return new ApiErrorResponse("email.sendFailed", reason);
			}
			lastTestEmailAt = now;
			return new EmailStatusResponse("Test email sent to " + to.trim());
		} catch (Exception e) {
			Log.error(this, "Test email failed", e);
			config.recordError(e.getMessage());
			return new ApiErrorResponse("email.sendFailed",
					e.getMessage() == null ? "Test email failed" : e.getMessage());
		}
	}

	private List<UserProject> allProjectsFor(String userId) {
		return WorldInitializer.getDefaultWorld().domain().projects()
				.getAllForUserId(userId);
	}

	private static class UploadProjectResponse extends DeploymentQueryResponse {
		private final UserProject project;

		public UploadProjectResponse(String userId, List<UserProject> projects,
				UserProject project) {
			super(userId, projects);
			this.project = project;
		}

		protected void addContents(Element root, HttpServletRequest request) {
			Element deployments = root.addElement("deployments");
			deployments.addAttribute("user", userId);
			for (UserProject project : projects) {
				Element deployment = deployments.addElement("deployment");
				deployment.addAttribute("project", project.getName());
				for (Form form : project.getProject().getForms()) {
					Element startpoint = deployment.addElement("startpoint");
					startpoint.addAttribute("form", form.getName());
					startpoint.addAttribute("url", project.getUrlToForm(
							EntryPointType.REAL_PROJECT, form));
				}
			}
		}

		public void handle(HttpServletRequest request11,
				HttpServletResponse response, World world1) throws IOException {
			super.handle(request11, response, world1);
			Log.info(this, "successful upload from "
					+ request11.getRemoteAddr() + " for " + project);
			if (!project.getProject().getFormat().isAtLeast(
					Project.MINIMUM_SAFE)) {
				Log.warn(this, "uploaded project version "
						+ project.getProject().getFormat()
						+ "is older than minimum " + Project.MINIMUM_SAFE);
			}
		}
	}
}
