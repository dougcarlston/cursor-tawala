<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<%@ page import="com.tawala.web.library.ViewRecentLibraryChangesController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>


<div class="section" id="library">
	<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>

	<div id="search">
		<h3 style="float: left;">Recent Changes</h3>
		<form style="float: right;">
			Show changes made in the last 
			<input type="text" name="<%= ViewRecentLibraryChangesController.PARAMETER_DAY_COUNT %>" value="${days}" 
					size="4" /> day(s). 
			<input type="image" value="Refresh" src="/images/silk/arrow_refresh.gif" title="Refresh list" class="smallIcon" />
		</form>
	</div>
	
	<table class="list sortable ruler">
		<col style="width: 160px;" />
		<col style="" />
		<col style="width: 80px;" />
		<col style="width: 110px;" />
		<col style="width: 40px;" />		
		<thead>
			<tr>
				<th class="left">Project</th>
				<th class="left">Description</th>
				<th class="left">User</th>
				<th class="left">Date</th>
				<th>&nbsp;</th>
			</tr>
		</thead>

		<tfoot>
			<tr>
				<td></td>
				<td></td>
				<td></td>
				<td></td>
				<td></td>
			</tr>
		</tfoot>
		
		<tbody>
		<c:forEach items="${events}" var="event">
			<tr>
				<c:choose>
					<c:when test="${event.projectRelated}">
						<c:set var="project" value="${projectMap[event.projectId]}" />
					</c:when>
					<c:otherwise>
						<c:remove var="project" />
					</c:otherwise>
				</c:choose>
				<td class="left">
					<c:choose>
						<c:when test="${empty project}">&nbsp;</c:when>
						<c:when test="${project.deleted }"><c:out value="${project.name}" /></c:when>
						<c:otherwise>
							<a href="${urls.libraryProjectDetailView}?<%= ViewProjectDetailsController.PARAMETER_ID %>=${project.id}"><c:out value="${project.name}" /></a>
						</c:otherwise>
					</c:choose>
				</td>
				<td class="left"><spring:message message="${event.description}" /></td>
				<td class="left"><c:out value="${event.userId}" /></td>
				<td class="left"><fmt:formatDate value="${event.date}" pattern="HH:mm MM/dd/yyyy" /></td>
				<td><%@ include file="/WEB-INF/jsp/library/revertChangesLink.jsp" %></td>
			</tr>
		</c:forEach>
		</tbody>
	</table>
</div>