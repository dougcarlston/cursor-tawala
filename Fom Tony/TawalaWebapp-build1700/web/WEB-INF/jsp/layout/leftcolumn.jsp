<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName" />
<tiles:importAttribute name="blockList" />

	<c:choose>
		<c:when test="${! empty user}" >
			<c:forEach var="block" items="${blockList}" varStatus="status">
				<tiles:insert name="${block}" />
			</c:forEach>
		</c:when>
		<c:otherwise>
			<tiles:insert name="block-login" />
		</c:otherwise>
	</c:choose>
	