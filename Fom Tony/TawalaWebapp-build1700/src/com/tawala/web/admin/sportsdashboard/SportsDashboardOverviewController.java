package com.tawala.web.admin.sportsdashboard;

import java.math.BigDecimal;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.Constants;
import com.tawala.jbpm.sportsdashboards.SportsDashboardStatistics;
import com.tawala.project.ProjectsHibernateImpl;

public class SportsDashboardOverviewController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		ModelAndView result = new ModelAndView(
				"admin.sports-dashboards.overview");

		long currentTime = System.currentTimeMillis();
		result.addObject("projectStatesStats", JbpmService
				.getOpenProcessStatistics(Constants.PROCESS_NAME));
		Log.info(this, "Took " + (System.currentTimeMillis() - currentTime) + "msecs to get project state statistics.");

		currentTime = System.currentTimeMillis();
		List<SportsDashboardStatistics> projectStats = ProjectsHibernateImpl
				.getSportsDashboardOpenProjectsStats();
		Log.info(this, "Took " + (System.currentTimeMillis() - currentTime) + "msecs to get open project statistics.");
		
		BigDecimal totalBillable = new BigDecimal(0);
		long totalRegistrations = 0;
		for (SportsDashboardStatistics sportsDashboardStatistics : projectStats) {
			BigDecimal projectBillable = sportsDashboardStatistics
					.getBillableAmount();
			if (projectBillable != null) {
				totalBillable = totalBillable.add(projectBillable);
			}

			totalRegistrations += sportsDashboardStatistics
					.getRegistrationCount();
		}
		result.addObject("projectStats", projectStats);
		result.addObject("totalBillable", totalBillable);
		result.addObject("totalRegistrations", totalRegistrations);
		
		currentTime = System.currentTimeMillis();
		result.addObject("registrationTrend", ProjectsHibernateImpl.getSportsDashboardOpenProjectsRegistrationTrend(7));
		Log.info(this, "Took " + (System.currentTimeMillis() - currentTime) + "msecs to get open project registration trend.");

		currentTime = System.currentTimeMillis();
		result.addObject("emailTrend", ProjectsHibernateImpl.getSportsDashboardOpenProjectsEmailTrend(7));
		Log.info(this, "Took " + (System.currentTimeMillis() - currentTime) + "msecs to get open project email trend.");

		return result;
	}
}
