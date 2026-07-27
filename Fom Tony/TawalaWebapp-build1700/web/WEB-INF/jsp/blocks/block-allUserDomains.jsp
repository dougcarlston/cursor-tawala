<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<c:if test="${! empty domains}">
	<div class="block">
		<div class="content">
			<h3>Other Tawala Solutions</h3>
			<ul>
			<c:forEach var="currentDomain" items="${domains}">
				<c:url var="url" value="${urls.landingPagePrefix}/${currentDomain.name}" />
				<li><a href="${url}" id="linkToDomain${currentDomain.id}">Solutions for <c:out value="${currentDomain.displayName}" /></a></li>
			</c:forEach>
			</ul>
		</div>
	</div>
</c:if>
	
