<%@ page import="com.tawala.web.library.RevertEventController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<c:if test="${! empty user && event.capableOfReverting}">
	<a href="${urls.libraryRevertEvent}?<%= RevertEventController.PARAMETER_EVENT_ID %>=${event.id}" title="Revert Changes"><img alt="Revert Changes" src="/images/silk/arrow_undo.gif" class="smallIcon" /></a>
</c:if>