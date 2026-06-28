<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.ListProjectWorkflowsInAParticularStateController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.ViewProjectWorkflowDetailsController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.task.DefaultViewTaskController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<h2>Project Task Management</h2>
<div class="section collapsible">
	<h3 class="sectionHeading">My Tasks</h3>
	<div class="sectionContent">
		<table class="list sortable ruler left">
			<colgroup>
				<col style="width: 25%;" />
				<col style="width: 25%;" />
				<col style="width: 25%;" />
				<col style="width: 25%;" />
			</colgroup>
			<thead>
				<tr>
					<th>User</th>
					<th>Project</th>
					<th>Task</th>
					<th>Since</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
			<c:if test="${empty currentUserTasks}">
				<tr>
					<td colspan="4">There are no pending tasks.</td>
				</tr>
			</c:if>
			<c:forEach var="projectTask" items="${currentUserTasks}">
				<c:url var="viewTaskURL" value="${projectTask.viewTaskURL}">
					<c:param name="<%= DefaultViewTaskController.TASK_ID_PARAMETER %>" value="${projectTask.taskInstance.id}"/>
				</c:url>
				<c:url var="viewProcessDetailsURL" value="${urls.adminViewProjectWorkflowDetails}">
					<c:param name="<%= ViewProjectWorkflowDetailsController.PROCESS_INSTANCE_ID_PARAMETER %>" 
							value="${projectTask.taskInstance.processInstance.id}" />
				</c:url>
				<tr class="task">
					<td><c:out value="${projectTask.userProject.user.id}" /></td>
					<td><a href="${viewProcessDetailsURL}"><c:out value="${projectTask.userProject.name}" /></a></td>
					<td class=""><c:out value="${projectTask.taskInstance.description}" /></td>
					<td><fmt:formatDate value="${projectTask.taskInstance.create}" pattern="MM/dd/yy"/></td>
					<td><a href="${viewTaskURL}">Perform Action</a></td>
				</tr>
			</c:forEach>
			</tbody>
		</table>
	</div>
</div>
<p />

<div class="section collapsible">
	<h3 class="sectionHeading">Unassigned Tasks in Functional Group (e.g. support)</h3>
	<div class="sectionContent">
		<table class="list sortable ruler left">
			<thead>
				<tr>
					<th>User</th>
					<th>Project</th>
					<th>Task</th>
					<th>Since</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
			<c:if test="${empty unassignedTasks}">
				<tr>
					<td colspan="4">There are no unassigned tasks.</td>
				</tr>
			</c:if>
			<c:forEach var="projectTask" items="${unassignedTasks}">
				<c:url var="assignTaskURL" value="${urls.adminAssignTask}">
					<c:param name="<%= AssignTaskController.TASK_ID_PARAMETER %>" value="${projectTask.taskInstance.id}" />
				</c:url>
				<c:url var="viewProcessDetailsURL" value="${urls.adminViewProjectWorkflowDetails}">
					<c:param name="<%= ViewProjectWorkflowDetailsController.PROCESS_INSTANCE_ID_PARAMETER %>" 
							value="${projectTask.taskInstance.processInstance.id}" />
				</c:url>
				<tr class="task">
					<td><c:out value="${projectTask.userProject.user.id}" /></td>
					<td><a href="${viewProcessDetailsURL}"><c:out value="${projectTask.userProject.name}" /></a></td>
					<td class=""><c:out value="${projectTask.taskInstance.description}" /></td>
					<td><fmt:formatDate value="${projectTask.taskInstance.create}" pattern="MM/dd/yy"/></td>
					<td><a href="${assignTaskURL}">Assign this task to myself</a></td>
				</tr>
			</c:forEach>
			</tbody>
		</table>
	</div>
</div>
