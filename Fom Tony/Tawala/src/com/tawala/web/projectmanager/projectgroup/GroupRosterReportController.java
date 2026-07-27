package com.tawala.web.projectmanager.projectgroup;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.UsersHibernateImpl;
import com.tawala.domain.ProjectGroup;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class GroupRosterReportController implements Controller {
	public static final String GROUP_ID_PARAMETER = "group_id";
	public static final String DO_RUN = "x";

	public ModelAndView handleRequest(final HttpServletRequest request,
			final HttpServletResponse response) throws Exception {

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (ModelAndView)transactionTemplate.execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				String groupId = request.getParameter(GROUP_ID_PARAMETER);
				ProjectGroup group = UsersHibernateImpl.getProjectGroup(
						UserInfoPreparationInterceptor.getSessionUser(request), Long
								.parseLong(groupId));
				if (group == null) {
					try {
						response.sendRedirect(WellKnown.urls.getProjectManagerView());
					} catch (IOException e) {
						throw new IllegalStateException("Error redirecting: ", e);
					}
					return null;
				}

				if (request.getParameter(DO_RUN) == null) {
					ModelAndView result = new ModelAndView(
							"projectmanager.project.group.roster.report.info");
					result.addObject("group", group);
					return result;
				}

				HSSFWorkbook workbook = new RosterReportGenerator()
						.generateReport(group);

				response.setContentType("application/vnd.ms-excel");
				response.setHeader("Content-Disposition", "attachment; filename=\""
						+ group.getName() + " Roster Report" + ".xls\";");
				try {
					workbook.write(response.getOutputStream());
				} catch (IOException e) {
					throw new IllegalStateException("Error writing the worksheet to the output stream:", e);
				}

				return null;
			}} );
		
	}
}
