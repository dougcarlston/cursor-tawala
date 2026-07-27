<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<c:if test="${ ! empty messages }">
	<div class="message" id="messages">
		<c:forEach var="message" items="${messages}">
		<spring:message message="${message}" /><br />
		</c:forEach>
	</div>
</c:if>
