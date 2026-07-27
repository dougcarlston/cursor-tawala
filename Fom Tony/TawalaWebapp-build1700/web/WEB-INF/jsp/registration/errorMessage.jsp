<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<c:if test="${fn:length(status.errorMessages) > 0}">
	<c:forEach var="error" items="${status.errorMessages}">
		<div class="error"><c:out value="${error}" /></div>
	</c:forEach>
</c:if>
