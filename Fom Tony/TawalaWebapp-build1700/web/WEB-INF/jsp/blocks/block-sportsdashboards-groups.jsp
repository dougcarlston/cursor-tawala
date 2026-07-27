<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.project.theme.EditThemeController"/>
<%@page import="com.tawala.web.projectmanager.projectgroup.ManageProjectGroupController"%>
<%@page import="com.tawala.web.projectmanager.projectgroup.ViewGroupMenuController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

	<div class="block">
		<div class="content">
			<h3>League Groups</h3>
			<div>
				<table class="list left">
					<tbody>
				<c:forEach var="group" items="${sportsdashboardsGroups}" varStatus="status">
					<c:url var="groupMenuURL" value="${urls.projectGroupMenu}">
						<c:param name="<%=ViewGroupMenuController.GROUP_ID %>" value="${group.id}" />
					</c:url>
					
					<c:url var="manageGroupURL" value="${urls.manageProjectGroup}">
						<c:param name="<%=ManageProjectGroupController.GROUP_ID_PARAMETER %>" value="${group.id}" />
					</c:url>
					<tr>
						<td>
							<a id="linkToEditGroup${status.count}" href="${groupMenuURL}"><c:out value="${group.name}" /></a>
						</td>
						<td>
							<c:if test="${user.administrator || originalUser.administrator}">
								<a id="linkToEditGroup${status.count}" href="${manageGroupURL}">Manage</a>
							</c:if>
						</td>
					</tr>					
				</c:forEach>
					</tbody>
				</table>
			</div>
			
<c:if test="${user.administrator || originalUser.administrator}">
			<br />
			<div>
				<a href="${urls.manageProjectGroup}" id="addNewGroupLink">Create New Group</a>
			</div>
</c:if>
		</div>
	</div>
