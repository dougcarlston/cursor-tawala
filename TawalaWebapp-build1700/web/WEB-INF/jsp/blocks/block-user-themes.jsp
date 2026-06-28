<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.project.theme.EditThemeController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<c:if test="${user.administrator || originalUser.administrator}">
	<div class="block">
		<div class="content">
			<h3>Custom Themes</h3>
			<div>
				<c:forEach var="userTheme" items="${userThemes}" varStatus="status">
					<c:url var="editThemeURL" value="${urls.editTheme}">
						<c:param name="<%=EditThemeController.THEME_ID_PARAMETER %>" value="${userTheme.id}" />
					</c:url>
					<a id="linkToEditTheme${status.count}" href="${editThemeURL}"><c:out value="${userTheme.name}" /></a><br />
				</c:forEach>
			</div>
			<br />
			<div>
				<a href="${urls.editTheme}" id="createUserTheme">Create New Custom Theme</a>
			</div>
		</div>
	</div>
</c:if>
	
