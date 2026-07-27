package com.tawala.web.project;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeSet;

import javax.servlet.ServletContext;

import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.World;
import com.tawala.component.validator.FieldValidator;
import com.tawala.component.web.ResponseCreator;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.Document;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormRenderable;
import com.tawala.project.FormSegment;
import com.tawala.project.FormSubmission;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.ExecutionResult;
import com.tawala.project.theme.ProjectTheme;
import com.tawala.web.OldPageResponse;
import com.tawala.web.RedirectResponse;
import com.tawala.web.Request;
import com.tawala.web.Response;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.oldhtml.RenderingContext;

public abstract class DataCollectingProjectController extends ProjectController {
	public DataCollectingProjectController(ServletContext servletContext) {
		super(servletContext);
	}

	private static final Document DEFAULT_DOCUMENT;

	public static final String AUTHENTICATION_TOKEN_VARIABLE_NAME = "_InviteeID";
	public static final String REDIRECT_AFTER_POST_PARAMETER = "__";

	static {
		List<FormRenderable> items = new ArrayList<FormRenderable>();
		items.add(new DataCollectingProjectController.DefaultDocumentHtml());
		DEFAULT_DOCUMENT = new Document("default", items);
	}

	private static class DefaultDocumentHtml implements FormRenderable, Html {

		public Html toHtml(ExecutionContext executionContext) {
			return this;
		}

		public void render(PrintWriter out, RenderingContext renderingContext) {
			out.println("<div align=\"center\">Thank you!</div>");
			out.println("<br />");
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

		public ResponseCreator getResponseCreatorForComponentId(
				String componentId) {
			return null;
		}
	}

	protected LinkToUserProject fetchProject(World world, Request request)
			throws NotFoundException {
		LinkToUserProject linkToProject = world.domain().projects()
				.getWithProjectRuntime(getRandomProjectId(request));
		if (linkToProject == null)
			throw new NotFoundException("couldn't find project");
		return linkToProject;
	}

	@Override
	protected Response generateResponse(final Request request,
			final World world, final LinkToUserProject link, final Form form) {
		long startTime = System.currentTimeMillis();

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		Response result = (Response) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						try {
							return handleResponse(request, world, link, form);
						} catch (IOException e) {
							throw new IllegalStateException(
									"Unexpected exception: ", e);
						}
					}
				});
		Log
				.info(this, request.getMethod() + " to project "
						+ link.getProject() + " in "
						+ (System.currentTimeMillis() - startTime) + " msecs.");
		return result;
	}

	private Response handleResponse(final Request request, final World world,
			final LinkToUserProject link, final Form postedToForm)
			throws IOException {
		Project project = link.getProject().getProject();
		final ExecutionContext context = new ExecutionContext(world.domain(),
				link, postedToForm, request, getEntryPointType());
		context.setAllowAdsToAppear(getAllowAdsToAppear());

		if (link.isAuthenticated()) {
			context.setValue(AUTHENTICATION_TOKEN_VARIABLE_NAME, link
					.getAuthenticationToken());
		} else {
			context.removeVariable(AUTHENTICATION_TOKEN_VARIABLE_NAME);
		}

		if (request.getParameter(REDIRECT_AFTER_POST_PARAMETER) != null) {
			OldPage page = context.getStoredPage(postedToForm);
			if (page == null) {
				// --- Probably session has expired.
				Log.warn(this, "Unable to find stored page for form '"
						+ postedToForm.getName() + "'");
				OldPage errorPage = new OldPage(new BackButtonNavigationPage(
						context.getOriginalLink()));
				addHeaderAndStyleSheets(project, context, errorPage);
				return new OldPageResponse(errorPage);
			} else {
				return new OldPageResponse(page);
			}
		}

		if (request.isPost() && context.isDetectBackButtonNavigation()) {
			// --- Verify that the token is allowed.
			String receivedToken = request
					.getParameter(FormSegment.NEXT_EXPECTED_TOKEN);
			if (receivedToken != null
					&& !context.isPostTokenAllowed(receivedToken)) {
				Log.warn(this, "Received unexpected token: " + receivedToken);
				OldPage page = new OldPage(new BackButtonNavigationPage(context
						.getOriginalLink()));
				addHeaderAndStyleSheets(project, context, page);
				return new OldPageResponse(page);
			}
		}

		context.clearExpectedPostTokens();

		FormSubmission submission = context.getSubmission();

		String expectedFormSubmissionId = request
				.getParameter(FormSegment.EDITED_SUBMISSION_ID);
		if (expectedFormSubmissionId != null
				&& Long.parseLong(expectedFormSubmissionId) != submission
						.getDatabaseId()) {
			Log.warn(this,
					"Detected session expiration or back button navigation.");
			OldPage page = new OldPage(new EditedRecordNotFoundPage(context
					.getOriginalLink()));
			addHeaderAndStyleSheets(project, context, page);
			return new OldPageResponse(page);
		}

		final FormSegment previousSegment = postedToForm
				.getPreviousSegment(request);
		if (previousSegment != null) {
			OldPage page = validateSubmission(context, postedToForm,
					previousSegment);

			if (page != null) {
				addHeaderAndStyleSheets(project, context, page);
				return new OldPageResponse(page);
			}
		}

		OldPage page = new OldPage();
		RedirectResponse redirectResponse = null;

		int loopCount = 0;
		Form form = postedToForm;
		String newFormName = previousSegment == null ? postedToForm.getName()
				: null;

		Set<String> formsParticipatingInTheLoop = new TreeSet<String>();
		
		boolean needToRedirect = false;
		boolean inTheMiddleOfMultiSegmentForm = false;
		while (true) {
			if(newFormName != null) {
				formsParticipatingInTheLoop.add(newFormName);
			}
			if (++loopCount >= 1000) {
				Log.error(this, "Process loop exceeded " + loopCount
						+ " iterations for project link \"" + link.getId() + "\". " +
								"Forms participating in the loop: " + formsParticipatingInTheLoop + ".");
				break;
			}

			if (newFormName != null) {
				// --- Execute pre-process
				form = project.getForm(newFormName);
				if (form == null) {
					Log.error(this, "Unable to find form '" + newFormName
							+ "' for project #" + link.getId() + ".");
					break;
				}
				if (!context.getSubmission().getFormName().equals(
						form.getName())) {
					submission = new FormSubmission(context.getUserProject(),
							form);
					context.setSubmission(submission);
				}

				// Run the pre-process
				ExecutionResult result = project.executeProcess(context, form
						.getPreProcessName());
				if (submission != context.getSubmission()) {
					// --- Case of Edit command.
					submission = context.getSubmission();
				}
				if (result.requireRedirect()) {
					redirectResponse = new RedirectResponse(result
							.getRedirectURL());
					break;
				}

				if (result.hasOutput()) {
					page.addAll(result.getHtml());
				}

				if (result.hasNextForm()
				// This is the case where Edit Record or
						// Show Form is issued in preprocess.
						&& !newFormName.equals(result.getNextForm())) {
					request.clearParameters();
					newFormName = result.getNextForm();

					continue;
				}
				newFormName = null;
			}

			FormSegment nextSegment = form.findNextSegment(context);

			request.clearParameters();

			if (nextSegment == null) {
				// We reached implicit SAVE point at the end of form.

				// -- Redirect would be needed if we were trying to POST. If it
				// was a GET - it wasn't a form post.
				needToRedirect = request.isPost();

				ExecutionResult result = project.executeProcess(context, form
						.getPostProcessName());
				if (result.hasOutput()) {
					page.addAll(result.getHtml());
				}

				if (submission.isAlreadyStored()) {
					world.domain().storedData().update(submission);
				} else {
					world.domain().storedData().record(submission);
				}
				submission.setBeingEdited(false);
				context.storePreviousSubmission(submission);

				// There is a chance that the context.submission got changed in
				// post-process.
				if (submission == context.getSubmission()) {
					// Resets the data in the current submission to
					// prevent any side effects of subsequent SETs.
					submission = new FormSubmission(context.getUserProject(),
							form);
					context.setSubmission(submission);
				} else {
					submission = context.getSubmission();
				}

				if (result.requireRedirect()) {
					redirectResponse = new RedirectResponse(result
							.getRedirectURL());
					break;
				}

				if (result.hasNextForm()) {
					newFormName = result.getNextForm();
					continue;
				}
			} else {
				context.storePreviousSubmission(submission);
				inTheMiddleOfMultiSegmentForm = true;
			}

			if (nextSegment != null) {
				OldPage nextPage = nextSegment.toPage(context);
				page.addContents(nextPage);
			}
			break;
		}

		context.storeExecutionContextSnapshot();

		if (redirectResponse != null) {
			return redirectResponse;
		}

		if (page.isEmpty()) {
			page.add(DataCollectingProjectController.DEFAULT_DOCUMENT
					.toHtml(context));
		}

		addHeaderAndStyleSheets(project, context, page);

		// --- Determine if session timeout is needed on this page
		if ((context.getSubmission() != null && context.getSubmission()
				.isBeingEdited())
				|| (request.isPost() && inTheMiddleOfMultiSegmentForm)) {
			int timeout = request.getSession().getMaxInactiveInterval() / 60;
			page.setSessionExpirationTimeout(timeout);
		}

		if (needToRedirect && context.isRedirectAfterPost()) {
			context.storeLastPage(postedToForm, page);
			String urlToForm = context.getUserProject().getUrlToForm(
					context.getLink().getId(), context.getEntryPointType(),
					postedToForm, context.getOverridingTheme(),
					context.isAdsExplicitlySurpressed());
			String url = urlToForm + (urlToForm.contains("?") ? '&' : '?')
					+ REDIRECT_AFTER_POST_PARAMETER;
			return new RedirectResponse(url);
		} else {
			return new OldPageResponse(page);
		}
	}

	protected abstract EntryPointType getEntryPointType();

	protected abstract boolean getAllowAdsToAppear();

	private static OldPage validateSubmission(ExecutionContext context,
			Form form, FormSegment currentSegment) {
		if (!context.expectPostedData()) {
			return null;
		}

		Map<Field, FieldValidator> invalidFields = currentSegment.validate(context.getSubmission(), context);
		
		if (invalidFields.size() > 0) {
			return currentSegment
					.toPage(
							context,
							 invalidFields);
		}
		return null;
	}

	public static void addHeaderAndStyleSheets(Project project,
			final ExecutionContext context, OldPage page) {
		ProjectTheme theme = context.getTheme();
		page.addPrintStylesheets(theme.getPrintStylesheetURLs());
		page.addScreenStylesheets(theme.getScreenStylesheetURLs());

		page.addScript("/scripts/project/default.js");

		if (project.getPageHeader() != null) {
			page.addToFront(project.getPageHeader().toHtml(context));
		}

		if (context.shouldDisplayAds()) {
			page.setGoogleAdSenseCode(theme.getGoogleAdSenseCode());
		}
	}

	@Override
	protected Response generateCustomComponentResponse(
			final Request requestObject, final World world,
			final LinkToUserProject link, final String componentId) {
		final ResponseCreator responseCreator = link.getProject().getProject()
				.getResponseCreatorForComponentId(componentId);
		if (responseCreator == null) {
			throw new IllegalStateException(
					"Unable to find a response creator for id '" + componentId
							+ "'");
		}

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Response) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus status) {
						ExecutionContext context = new ExecutionContext(world
								.domain(), link, null, requestObject,
								getEntryPointType());

						return responseCreator.createResponse(context);
					}
				});
	}
}
