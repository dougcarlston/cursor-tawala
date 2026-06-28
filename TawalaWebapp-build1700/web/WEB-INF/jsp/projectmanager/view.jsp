<%@ page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController" %>
<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

	<div id="projectManager" class="section" >
		<div class="status">
				<form>
				<div class="pageCtl">
					<input type="hidden" name="<%=ViewProjectManagerController.START_PARAMETER %>" value="0" />
					<select name="<%= ViewProjectManagerController.FILTER_PARAMETER %>">
					<c:forEach var="filter" items="${availableFilters}">
						<c:set var="filterSelected"><c:if test="${filter == projectPagingInfo.filter}"> selected</c:if></c:set>
						<option value="${filter}" ${filterSelected}><c:out value="${filter.description}"/></option>
					</c:forEach>
					</select> projects, 
					<input type="text" class="text" value="${projectPagingInfo.max}" size="3" name="<%= 
						ViewProjectManagerController.MAX_PROJECTS_PER_PAGE_PARAMETER %>" /> per page,
					sorted by 
					<select name="<%= ViewProjectManagerController.SORT_PARAMETER %>">
					<c:forEach var="sortOrder" items="${availableSortOrders}">
						<c:set var="sortOrderSelected"><c:if test="${sortOrder == projectPagingInfo.sortOrder}"> selected</c:if></c:set>
						<option value="${sortOrder}" ${sortOrderSelected}><c:out value="${sortOrder.description}"/></option>
					</c:forEach>
					</select>
					<input type="submit" value="View" />
				</div>
				</form>
		</div>

		<div class="paginationControls">
			<tawala:projectManagerPagination />
		</div>
		
        <table class="projectListing stripe ruler">
            <colgroup>
				<col/>
				<col class="info"/>
				<col class="created"/>
				<col class="updated"/>
				<col class="responses"/>
				<col class="accessed"/>
				<col class="controls"/>
            </colgroup>
            <thead>
                <tr>
					<th>&nbsp;</th>
					<th class="left">Name</th>
					<th>Created</th>
					<th>Updated</th>
					<th>Responses</th>
					<th>Access</th>
					<th></th>					
                </tr>
            </thead>
            <tbody>
				<c:forEach var="stats" items="${projectStatistics}" varStatus="status">
					<c:url var="projectDetailsLink" value="${urls.projectManagerProjectDetailView}">
						<c:param name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME %>"
										value="${stats.name}" />
					</c:url>
					<tr>
						<td><c:if test="${stats.offline}"><img src="/images/silk/stop.png" alt="Off-line"
											title="Project is off-line"
											class="smallIcon" /></c:if>
						</td>
						<td class="info left">
								<span class="title">
									<a	href="${projectDetailsLink}" id="projectDetailsLink${status.count}">
										<c:out value="${stats.name}" />
									</a>
								</span>
						</td>
						<td class="dateCreated">
							<fmt:formatDate value="${stats.created}" type="both" pattern="yyyy-MM-dd" /> 
						</td>
						<td class="dateCreated">
							<fmt:formatDate value="${stats.lastUpdated}" type="both" pattern="yyyy-MM-dd" /> 
						</td>
						<td class="responses">
							${stats.responseCount}
						</td>
						<td class="lastAccessed">
							<fmt:formatDate value="${stats.lastAccessed}" type="both" pattern="yyyy-MM-dd" /> 
						</td>
						<td>
							<div  class="controls">
								<form method="post" class="confirm">
									<input type="hidden" name="project" value="<c:out value="${stats.name}" />" /> 
									<input type="hidden" name="action" value="purge" /> 
									<input type="image" name="action" value="Purge" src="/images/silk/bin.gif" alt="Purge" title="Purge project data" class="smallIcon" />
								</form>
								<form method="post" class="confirm">
									<input type="hidden" name="project" value="<c:out value="${stats.name}" />" /> 
									<input type="hidden" name="action" value="delete" /> 
									<input type="image" name="action" value="Delete" src="/images/delete-icon.gif" alt="Delete" title="Delete project" class="smallIcon" />
								</form>
							</div>
						</td>
					</tr>
				</c:forEach>
            </tbody>
        </table>

		<div class="paginationControls">
			<tawala:projectManagerPagination />
		</div>
		<div class="status">
			<div class="stats" style="width: 50%;">Total Projects: ${projectCount}<c:if test="${inactiveProjectCount > 0}"> (${inactiveProjectCount} inactive)</c:if></div>
			<c:if test="${inactiveProjectCount > 0}">
				<div style="float: right;"><img src="/images/silk/stop.png" alt="Inactive"
												title="Project is off-line"
												class="smallIcon" style="vertical-align: middle"/> = Inactive Project
				</div>
			</c:if>
		</div>
		
		<br />
	</div>
