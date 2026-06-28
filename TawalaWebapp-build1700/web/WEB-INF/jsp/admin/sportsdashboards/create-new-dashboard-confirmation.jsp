<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.ViewUserDetailController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<h2>Create New Dashboard</h2>

<p><b>New project is created</b></p>

<c:url var="viewUserDetailUrl" value="${urls.adminViewUserInfo}">
	<c:param name="<%= ViewUserDetailController.USER_ID_PARAMETER %>" value="${user.databaseId}" />
</c:url>

<p>
Successfully cloned <b><c:out value="${libraryProject.name}" /></b> to <b><a href="${viewUserDetailUrl}"><c:out value="${user.id}" /></a></b> account.
</p>

<p>
	Here are the project links:
	<c:forEach var="urlEntry" items="${userProject.entryPointURLs}" >
		<c:set var="form" value="${urlEntry.key}"/>
		<c:set var="url" value="${urlEntry.value}"/>
		<p><c:out value="${form.name}" /></p>
		<a href="${url}" target="_blank">${url}</a>
		<br /><br />
	</c:forEach>
</p>
