package com.tawala.web.projectmanager.alldataimport;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;
import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.web.bind.ServletRequestDataBinder;
import org.springframework.web.multipart.support.StringMultipartFileEditor;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.AbstractWizardFormController;

import com.scissor.Log;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.message.Message;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.data.ImportStatistics;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class AllDataImportController extends AbstractWizardFormController {
	public static final String PROJECT_ID_PARAMETER = "id";

	private static final String COMMAND_NAME = "fileUpload";

	public static final int PAGE_COUNT = 2;

	public AllDataImportController() {
		setAllowDirtyBack(true);
		setAllowDirtyForward(false);
		setPages(new String[] { "projectmanager.all.data.import.start",
				"projectmanager.all.data.import.approve" });
		setCommandName(COMMAND_NAME);
		setPageAttribute("pageNumber");
	}

	@Override
	protected Object formBackingObject(final HttpServletRequest request)
			throws Exception {
		final String projectId = request.getParameter(PROJECT_ID_PARAMETER);
		if (projectId == null) {
			throw new IllegalArgumentException(
					"Unable to find the project id in request.");
		}

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		UserProject project = (UserProject) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus status) {
						UserProject project = ProjectsHibernateImpl
								.getUserProjectById(
										UserInfoPreparationInterceptor
												.getSessionUser(request), Long
												.parseLong(projectId));
						if (project == null) {
							throw new IllegalArgumentException(
									"Unable to find project by id: "
											+ projectId);
						}

						// The following statements cause loading of
						// various related objects by Hibernate.
						project.getDeployedVersion().getVersionNumber();
						project.getProject().getForms();

						return project;
					}
				});

		EventService.createEvent(new Event("AttemptToRestoreAllData", request,
				project.getName()));
		return new FileUploadBean(project);
	}

	protected void initBinder(HttpServletRequest request,
			ServletRequestDataBinder binder) throws ServletException {
		binder.registerCustomEditor(String.class,
				new StringMultipartFileEditor());
	}

	@SuppressWarnings("unchecked")
	@Override
	protected ModelAndView processFinish(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		final FileUploadBean fileUploadBean = (FileUploadBean) command;

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		Map<String, ImportStatistics> importResults = (Map<String, ImportStatistics>) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus status) {
						WorldInitializer.getDefaultWorld().domain()
								.storedData().purgeProjectResponses(
										fileUploadBean.getUserProject()
												.getProject());

						try {
							return fileUploadBean.getDataImporter().restore();
						} catch (IOException e) {
							Log
									.error(
											this,
											"Exception occured while processing the file:",
											e);
							status.setRollbackOnly();
							return null;
						}
					}
				});

		boolean problemsDuringImport = false;
		for (ImportStatistics statistics : importResults.values()) {
			if (statistics.getSkippedRows().size() > 0) {
				problemsDuringImport = true;
				break;
			}
		}

		ModelAndView result = new ModelAndView(
				"projectmanager.all.data.import.confirm");
		result.addObject(COMMAND_NAME, fileUploadBean);
		result.addObject("importResults", importResults);
		result.addObject("problemsDuringImport", problemsDuringImport);

		EventService.createEvent(new Event("AllDataRestoreComplete", request,
				fileUploadBean.getUserProject().getName()));

		return result;
	}

	@Override
	protected ModelAndView processCancel(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		response.sendRedirect(WellKnown.urls.getProjectManagerView());
		return null;
	}

	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors, int page) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		result.put("totalPageCount", PAGE_COUNT);
		return result;
	}

	@Override
	protected void validatePage(Object command, Errors errors, int page) {
		FileUploadBean fileUploadBean = (FileUploadBean) command;
		try {
			switch (page) {
			case 0:
				if (fileUploadBean.isEmptyFile()) {
					errors.reject("data.backup-restore.no.data");
					return;
				}
				try {
					fileUploadBean.getWorkbook();
				} catch (Exception e) {
					errors.reject("data.all-data-import.incorrect.excel.file");
					return;
				}

				for (Message message : fileUploadBean.getDataImporter()
						.getErrors()) {
					errors.reject(message.getCode(), message.getArguments(),
							message.getDefaultMessage());
				}
				break;

			case 1:
				if (!fileUploadBean.isConfirmDataDeletion()) {
					errors
							.reject("data.all-data-import.confirm.current.data.deletion");
				}
				break;
			}
		} catch (Exception e) {
			throw new IllegalArgumentException("Error parsing data", e);
		}

		return;
	}
}
