<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.admin.ViewUserDetailController"/>
<jsp:directive.page import="com.tawala.web.admin.ProjectSearchController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="section">
	<h2 class="sectionHeading">Search Project</h2>
	<div class="sectionContent">

		<c:if test="${searchFailed}"><font color="red">Unable to find a project with this id!</font></c:if>
		<form id="searchByIdForm">
			<label for="projectId">Type the project random id: </label><input type="text" id="projectId" name="<%= ProjectSearchController.PARAMETER_PROJECT_ID %>" value="<c:out value="${projectId}" />" size="20" />
		    <input type="submit" value="Search"/> 
		</form>
		
		<c:if test="${! empty link}">
		<br />
		<c:url var="viewUserDetailUrl" value="${urls.adminViewUserInfo}">
			<c:param name="<%= ViewUserDetailController.USER_ID_PARAMETER %>" value="${link.project.user.databaseId}" />
		</c:url>
		User: <a href="${viewUserDetailUrl}" id="viewUserDetailLink"><c:out value="${link.project.user.id}"/></a> (<c:out value="${link.project.user.firstName}"/> <c:out value="${link.project.user.lastName}"/>, 
			email: <c:out value="${link.project.user.email}"/>).
		<br />
		
		Project: <b><c:out value="${link.project.name}"/></b>
		</c:if>
		
	</div>
</div>