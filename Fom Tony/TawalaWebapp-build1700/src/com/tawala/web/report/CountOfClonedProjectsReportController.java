package com.tawala.web.report;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;

public class CountOfClonedProjectsReportController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		ModelAndView result = new ModelAndView(
				"report.count.of.cloned.projects");
		List<LibraryProject> projects = ProjectLibraryService.getClonedProjects();
		
		List<GraphData> data = new ArrayList<GraphData>(projects.size());
		float total = 0;
		for (LibraryProject project : projects) {
			GraphData graphData = new GraphData();
			graphData.libraryProject = project;
			graphData.count = project.getCloneCount();
			data.add(graphData);
			
			total += graphData.count;
		}
		
		for (GraphData element : data) {
			element.percentage = element.count *100/total;
		}
		
		Collections.sort(data);
		
		result.addObject("data", data);
		return result;
	}

	public static class GraphData implements Comparable<GraphData> {
		private LibraryProject libraryProject;
		private long count;
		private float percentage;
		
		public int compareTo(GraphData o) {
			return (int)(o.percentage - this.percentage);
		}
		public long getCount() {
			return count;
		}
		public LibraryProject getLibraryProject() {
			return libraryProject;
		}
		public float getPercentage() {
			return percentage;
		}
	}
}
