package com.tawala.web.library;

import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.project.library.LibraryChangeEvent;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectChangeEvent;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.util.DateUtil;

public class ViewRecentLibraryChangesController implements Controller {
	public static final String PARAMETER_DAY_COUNT = "days";
	public static final int DEFAULT_DAY_COUNT = 1;

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		int days = DEFAULT_DAY_COUNT;
		try {
			String parameter = request.getParameter(PARAMETER_DAY_COUNT);
			if (parameter != null)
				days = Integer.parseInt(request
						.getParameter(PARAMETER_DAY_COUNT));
		} catch (Exception e) {
			Log.warn(this, "Error parsing the number of days:", e);
		}

		Date startTime = DateUtil.dateEarlierStartingAt12am(days);
		
		Collection<LibraryChangeEvent> events = ProjectLibraryService
				.getChangesSince(startTime);

		ModelAndView result = new ModelAndView("library.recent.changes");

		populateModelBeanWithEvents(result, events, days);
		
		return result;
	}

	public static void populateModelBeanWithEvents(ModelAndView result, Collection<LibraryChangeEvent> events, int days) {
		Map<Long, LibraryProject> projectMap = new HashMap<Long, LibraryProject>();
		for (LibraryChangeEvent event : events) {
			if (ProjectChangeEvent.class.isAssignableFrom(event.getClass())) {
				ProjectChangeEvent projectChangeEvent = (ProjectChangeEvent) event;
				if (projectMap.get(projectChangeEvent.getProjectId()) == null) {
					projectMap.put(projectChangeEvent.getProjectId(),
							ProjectLibraryService.findProjectById(projectChangeEvent
									.getProjectId()));
				}
			}
		}

		result.addObject("events", events);
		result.addObject("projectMap", projectMap);
		result.addObject("days", days);
	}
}