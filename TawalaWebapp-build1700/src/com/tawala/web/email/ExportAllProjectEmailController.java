package com.tawala.web.email;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;

import javax.servlet.ServletOutputStream;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.email.EmailService;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ExportAllProjectEmailController implements Controller {
	public static final String PROJECT_ID_PARAMETER = "id";

	public ModelAndView handleRequest(final HttpServletRequest request,
			final HttpServletResponse response) throws Exception {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				try {
					return doHandle(request, response);
				} catch (IOException e) {
					throw new RuntimeException(e);
				}
			}

		});
		return null;
	}

	private String createFileName(UserProject project) {
		String projectName = project.getName();
		projectName = projectName.replace(':', '-');
		projectName = projectName.replace('/', '-');
		projectName = projectName.replace('\\', '-');
		projectName = projectName.replace(' ', '_');

		String timestamp = new SimpleDateFormat("yyyyMMdd_HHmm")
				.format(new Date());

		return "emails-for-" + projectName + '-' + timestamp + ".xls";
	}

	private Object doHandle(final HttpServletRequest request,
			final HttpServletResponse response) throws IOException {
		String projectId = request.getParameter(PROJECT_ID_PARAMETER);
		final UserProject userProject = ProjectsHibernateImpl
				.getUserProjectById(UserInfoPreparationInterceptor
						.getSessionUser(request), Long.parseLong(projectId));
		if (userProject == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		User sessionUser = UserInfoPreparationInterceptor
				.getSessionUser(request);
		if (!userProject.getUser().equals(sessionUser)) {
			Log
					.error(
							this,
							"User "
									+ sessionUser.getDatabaseId()
									+ " attempted to export the emails the user doesn't own (project id="
									+ userProject.getId() + ")");
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		HSSFWorkbook workbook = EmailService
				.createProjectEmailReport(userProject);

		response.setContentType("application/vnd.ms-excel");
		response.setHeader("Content-Disposition", "attachment; filename=\""
				+ createFileName(userProject) + "\";");

		ServletOutputStream outputStream = response.getOutputStream();
		workbook.write(outputStream);

		outputStream.flush();
		outputStream.close();

		return null;
	}
}
