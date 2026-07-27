<%@ page contentType="text/html"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>

<c:set var="labelStyle" value="" />
<c:if test="${fn:length(status.errorMessages) > 0}">
	<c:set var="labelStyle" value="error" />
</c:if>
