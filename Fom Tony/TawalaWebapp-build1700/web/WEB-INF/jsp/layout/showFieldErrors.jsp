<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<c:if test="${status.error}">
	<span class="error">
		<c:forEach var="error" items="${status.errorMessages}">
			<c:out value="${error}"/><br />
		</c:forEach>
	</span>
</c:if>
