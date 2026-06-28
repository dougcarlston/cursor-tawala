
<%@page import="com.tawala.web.projectmanager.projectgroup.AddProjectToGroupController"%>
<%@page import="com.tawala.web.projectmanager.projectgroup.DeleteProjectFromGroupController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<c:choose>
		<c:when test="${empty command.projectGroup.id}">
			<h2>Create New Project Group</h2>
		</c:when>
		<c:otherwise>
			<h2>Manage Project Group</h2>
		</c:otherwise>
	</c:choose>		

	<form:form method="POST" id="manageProjectGroupForm">
		<div class="title">Group Name: <form:errors path="projectGroup.name" cssClass="error"/></div>
		<form:input path="projectGroup.name" cssStyle="width: 50%;" /> <br />
		<br />
		
		<c:choose>
			<c:when test="${empty command.projectGroup.id}">
				<div class="buttons">
					<button type="submit" value="CREATE NEW PROJECT GROUP" />CREATE NEW PROJECT GROUP</button>
					<a href="/projectmanager/view">CANCEL</a>
				</div>				
			</c:when>
		<c:otherwise>
			<c:url var="addProjectToGroupURL" value="${urls.addProjectToGroup}">
				<c:param name="<%= AddProjectToGroupController.GROUP_ID_PARAMETER %>" value="${command.projectGroup.id}" />
			</c:url>
			<br />
			<div style="float: right;"><a href="${addProjectToGroupURL}">Add Project</a></div>
			<h3>Projects</h3> 
			<table class="list left">
				<thead class="dark">
				<tr>
					<th>User</th>
					<th>Project Name</th>
					<th>&nbsp;</th>
				</tr>
				</thead>
				<tbody>
			<c:if test="${empty command.projectGroup.userProjects}">
					<tr>
						<td colspan="3">No projects are added yet.</td>
					</tr>
			</c:if>
			<c:forEach var="userProject" items="${command.projectGroup.userProjects}">
				<c:url var="deleteProjectURL" value="${urls.deleteProjectFromGroup}">
					<c:param name="<%= DeleteProjectFromGroupController.GROUP_ID %>" value="${command.projectGroup.id}"/>
					<c:param name="<%= DeleteProjectFromGroupController.PROJECT_ID %>" value="${userProject.id}"/>
				</c:url>
					<tr>
						<td><c:out value="${userProject.user.id}" /></td>
						<td><c:out value="${userProject.name}" /></td>
						<td><a href="${deleteProjectURL}"><img src="/images/silk/delete.gif"/></a></td>
					</tr>
			</c:forEach>
				</tbody>
			</table>
			
			<br />
			<div class="buttons">
				<button type="submit">SAVE</button>
				<button type="submit" name="delete" value="true">DELETE</button>
				<a href="/projectmanager/view">CANCEL</a>
			</div>
		</c:otherwise>
		</c:choose>
	</form:form>

</div>
