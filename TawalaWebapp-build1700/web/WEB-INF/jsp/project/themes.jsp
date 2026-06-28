<%@ page contentType="application/xml" %><%
%><jsp:directive.page import="com.tawala.project.theme.CommonTheme"/><%
%><%
%><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %><%
%><?xml version="1.0" encoding="utf-8" ?>
<projectThemes path="css/project">
<c:forEach var="theme" items="<%= CommonTheme.ALL_THEMES %>">
	<theme>
		<name><c:out value="${theme.name}"/></name>
		<path><c:out value="${theme.path}"/></path>
	</theme>
</c:forEach>
</projectThemes>
