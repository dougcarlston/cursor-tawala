<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.ViewProjectWorkflowDetailsController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>


<h2>Projects in "<c:out value="${processStateName}" />" state</h2>
<div class="section">
	<div class="sectionContent">

		<table class="list left">
			<thead>
				<tr>
					<th>User</th>
					<th>Project</th>
				</tr>
			</thead>
			<tbody>
		<c:forEach var="userProjectProcess" items="${processes}">
			<c:url var="viewProcessDetailsURL" value="${urls.adminViewProjectWorkflowDetails}">
				<c:param name="<%= ViewProjectWorkflowDetailsController.PROCESS_INSTANCE_ID_PARAMETER %>" value="${userProjectProcess.processInstance.id}" />
			</c:url>
				<tr>
					<td><c:out value="${userProjectProcess.userProject.user.id}"/></td>
					<td><a href="${viewProcessDetailsURL}"><c:out value="${userProjectProcess.userProject.name}"/></a></td>
				</tr>
		</c:forEach>
			<tbody>
		</table>
		
	</div>
</div>