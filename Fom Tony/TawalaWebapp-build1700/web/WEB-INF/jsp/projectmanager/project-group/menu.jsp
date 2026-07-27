
<%@page import="com.tawala.web.projectmanager.projectgroup.GroupRosterReportController"%>
<%@page import="com.tawala.web.projectmanager.projectgroup.GroupCoachReportController"%>
<%@page import="com.tawala.web.projectmanager.projectgroup.SelectCoachController"%>
<%@page import="com.tawala.web.projectmanager.projectgroup.UpdateLateRegistrationRecordController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<h2><c:out value="${group.name}"/></h2>
	
	<c:url var="runRosterReportURL" value="${urls.projectGroupRosterReport}">
		<c:param name="<%= GroupRosterReportController.GROUP_ID_PARAMETER %>" value="${group.id}" />
	</c:url>
	
	<c:url var="runCoachReportURL" value="${urls.projectGroupCoachReport}">
		<c:param name="<%= GroupCoachReportController.GROUP_ID_PARAMETER %>" value="${group.id}" />
	</c:url>
	
	<c:url var="manageCoachesURL" value="${urls.projectGroupManageCoaches}">
		<c:param name="<%= SelectCoachController.GROUP_ID %>"  value="${group.id}"/>
	</c:url>

	<ul class="hMenu" style="float: right;">
		<li><a href="${runRosterReportURL}">RUN PLAYER REPORT</a></li>
		<li><a href="${runCoachReportURL}">RUN COACH REPORT</a></li>
		<li><a href="${manageCoachesURL}">MANAGE COACHES</a></li>
	</ul>
	
	<div id="loadStatusContainer" style="height: 15px;"></div>
	
	<h3>Projects</h3> 

	<table class="list left">
		<thead class="dark">
		<tr>
			<th>User</th>
			<th>Project Name</th>
			<th>Notify on Late Assignments</th>
			<th>&nbsp;</th>
		</tr>
		</thead>
		<tbody>
	<c:if test="${empty group.userProjects}">
			<tr>
				<td colspan="4">No projects are added yet.</td>
			</tr>
	</c:if>
	<c:forEach var="userProject" items="${group.userProjects}">
			<tr>
				<td><c:out value="${userProject.user.id}" /></td>
				<td>
					<a href="${additionalProjectData[userProject].adminDashURL}" target="_admindash">			
						<c:out value="${userProject.name}" />
					</a>
				</td>
				<td id="lateAssignmentDisplay">
					<c:set var="linkToChangeOption"><tawala:linkToRealProject 
												project="${userProject}"
												formName="LateAssignmentNotificationSetup" /></c:set>
					<c:choose>
					<c:when test="${additionalProjectData[userProject].lateAssignmentNotificationFlag}">
						Yes (<c:out value="${additionalProjectData[userProject].lateAssignmentNotificationEmail}"/>)
						<c:if test="${! empty linkToChangeOption}">
							<a id="${userProject.id}" class="notificationLink" href="${linkToChangeOption}" target="_change_notification_option">disable</a>
						</c:if>
					</c:when>
					<c:otherwise>No 
						<c:if test="${! empty linkToChangeOption}">
							<a id="${userProject.id}" class="notificationLink" href="${linkToChangeOption}" target="_change_notification_option">enable</a>
						</c:if>
					</c:otherwise>					
					</c:choose>
				</td>
			</tr>
	</c:forEach>
		</tbody>
	</table>
			
</div>


<script>
	Tawala.ProjectGroup.PROJECT_ID = "<%= UpdateLateRegistrationRecordController.PROJECT_ID %>";
	Tawala.ProjectGroup.URL_LoadCoaches = '<c:url value="${urls.projectGroupLoadCoachesForProject}" />';
	Tawala.ProjectGroup.URL_UpdateCoachStatus = '<c:url value="${urls.projectGroupUpdateCoachStatus}" />';

	Tawala.ProjectGroup.groupId = ${group.id};
	
</script>
