<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@page import="com.tawala.web.admin.SwitchUserController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<h2 class="sectionHeading">View Task</h2>

<c:url value="${urls.switchUser}" var="projectDetailsURL">
	<c:param name="<%= SwitchUserController.USER_ID_PARAMETER %>" value="${command.processTask.userProject.user.databaseId}"/>
	<c:param name="<%= SwitchUserController.PROJECT_NAME_PARAMETER %>" value="${command.processTask.userProject.name}" />
</c:url>

<div class="section">
	<h3><c:out value="${command.processTask.taskInstance.description}" /></h3>
	<div class="sectionContent">
		<p>
			<b>User:</b> <c:out value="${command.processTask.userProject.user.id}" />
			<br />
			<b>Project:</b> <c:out value="${command.processTask.userProject.name}" /> <a href="${projectDetailsURL}" />view in Project Manager</a>
		</p>
	</div>
</div>
		
<div class="section">
		<form:form>
			<tiles:insert attribute="secondaryContent" />
<c:if test="${fn:length(transitions) > 1}">		                    		                    
	<h3>Choose the Next Step:</h3>
	<div class="sectionContent">
		    <c:forEach var="transition" items="${transitions}">
				<p>
					<input class="radio" type="radio" id="transition${transition.id}" name="transitionName" value="${transition.name}" />
					<label class="choice" for="transition${transition.id}">${transition.description}</label>
				</p>
			</c:forEach>
			<br />
	</div>
</c:if>
	<div class="sectionContent">
			<div class="buttons">
				<button type="submit" name="Complete Task" value="Complete Task">MARK TASK COMPLETED</button>
				<a href="${urls.adminSportsDashboardProjectTaskManagement}">CANCEL</a>
			</div>

	</div>
		</form:form>
</div>