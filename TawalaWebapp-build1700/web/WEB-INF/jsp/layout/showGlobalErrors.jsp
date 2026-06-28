<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<tiles:importAttribute name="commandName" />
<spring:hasBindErrors name="${commandName}">
	<div class="error">
		<c:forEach var="error" items="${errors.globalErrors}">
			<spring:message code="${error.code}" arguments="${error.arguments}"/><br />
		</c:forEach>
	</div>
</spring:hasBindErrors>