package com.tawala.web.tags;

import javax.servlet.jsp.JspException;
import javax.servlet.jsp.JspTagException;
import javax.servlet.jsp.JspWriter;
import javax.servlet.jsp.tagext.TagSupport;

import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ProjectPagingInfo;
import com.tawala.web.projectmanager.ViewProjectManagerController;

public class ProjectManagerPaginationTag extends TagSupport {
	private static final long serialVersionUID = 1L;
	private static final int MAX_SECTIONS = 5;

	@Override
	public int doStartTag() throws JspException {
		try {
			ProjectPagingInfo pagingInfo = (ProjectPagingInfo) pageContext
					.getRequest()
					.getAttribute(
							ViewProjectManagerController.PROJECT_PAGING_INFO_ATTRIBUTE);
			long projectCount = (Long) pageContext.getRequest().getAttribute(
					UserInfoPreparationInterceptor.PROJECT_COUNT_ATTRIBUTE);

			if (projectCount <= pagingInfo.getMax() && pagingInfo.getStart() == 0) {
				// --- No need to show controls because all the projects are
				// shown on the page.
				return SKIP_BODY;
			}

			StringBuilder result = new StringBuilder();

			if (pagingInfo.getStart() > 0) {
				result.append(buildLink(Math.max(0, pagingInfo.getStart()
						- pagingInfo.getMax()), "Previous"));
				result.append(" | ");
			}

			for (int i = calculateFirstPreviousSegmentStart(pagingInfo
					.getStart(), pagingInfo.getMax(), MAX_SECTIONS); i < pagingInfo
					.getStart(); i += pagingInfo.getMax()) {
				result.append(buildLink(i, pagingInfo.getMax(), projectCount));
				result.append(" | ");
			}

			result.append(buildSegmentText(pagingInfo.getStart(), pagingInfo
					.getMax(), projectCount));
			result.append(" | ");

			long lastSegmentStartPosition = calculateLastSegmentStart(
					pagingInfo.getStart(), pagingInfo.getMax(), MAX_SECTIONS,
					projectCount);

			for (int i = pagingInfo.getStart() + pagingInfo.getMax(); i <= lastSegmentStartPosition; i += pagingInfo
					.getMax()) {
				result.append(buildLink(i, pagingInfo.getMax(), projectCount));
				result.append(" | ");
			}

			if (pagingInfo.getStart() + pagingInfo.getMax() < projectCount) {
				result.append(buildLink(pagingInfo.getStart()
						+ pagingInfo.getMax(), "Next"));
			}

			JspWriter out = pageContext.getOut();
			out.write(result.toString());
		} catch (Exception e) {
			throw new JspTagException(e.getMessage());
		}

		return SKIP_BODY;
	}

	private String buildLink(int start, String text) {
		return "<a href=\"" + WellKnown.urls.getProjectManagerView() + "?"
				+ ViewProjectManagerController.START_PARAMETER + "=" + start
				+ "\">" + text + "</a>";
	}

	private long calculateLastSegmentStart(int start, int max, int maxSegments,
			long projectCount) {
		long absolutelyLastSegmentStart = ((projectCount - 1) / max) * max;
		long lastSegmentStartForTheMaxNumberOfSegments = start + max
				* maxSegments;
		return Math.min(absolutelyLastSegmentStart,
				lastSegmentStartForTheMaxNumberOfSegments);
	}

	private String buildLink(int start, int max, long projectCount) {
		return buildLink(start, buildSegmentText(start, max, projectCount));
	}

	private String buildSegmentText(int start, int max, long projectCount) {
		return (start + 1) + "-" + Math.min(projectCount, start + max);
	}

	public static int calculateFirstPreviousSegmentStart(int currentStart,
			int elementsPerPage, int maxSegments) {
		return Math.max(0, currentStart - (elementsPerPage * maxSegments));
	}
}
