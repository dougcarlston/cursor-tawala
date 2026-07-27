<jsp:directive.page import="com.tawala.web.email.ViewUserProjectEmailBody"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<script>
	setPageTitle("View Email");
</script>
<p><a href="javascript:history.go(-1)">&lt;&lt; Return to Email List</a></p><br />

<table style="width: auto">
	<tr>
		<th class="left" style="padding: 2px">Status: </th>
		<td class="left" style="padding: 2px"> ${email.state.shortDescription}</td>
	</tr>
	<tr>
		<th class="left" style="padding: 2px">To: </th>
		<td class="left" style="padding: 2px"> <c:out value="${email.to}"/></td>
	</tr>
	<c:if test="${! empty email.cc}">
	<tr>
		<th class="left" style="padding: 2px">Cc: </th>
		<td class="left" style="padding: 2px"> <c:out value="${email.cc}"/></td>
	</tr>
	</c:if>
	<tr>
		<th class="left" style="padding: 2px">From: </th>
		<td class="left" style="padding: 2px"> <c:out value="${email.from}"/></td>
	</tr>
	<tr>
		<th class="left" style="padding: 2px">Subject: </th>
		<td class="left" style="padding: 2px"> <c:out value="${email.subject}"/></td>
	</tr>
	<tr>
		<th class="left" style="padding: 2px">Date Created: </th>
		<td class="left" style="padding: 2px"> <fmt:formatDate value="${email.createdDate}" pattern="MM/dd/yyyy HH:mm" /></td>
	</tr>
	<c:if test="${! empty email.sentDate}">
	<tr>
		<th class="left" style="padding: 2px">Date Sent: </th>
		<td class="left" style="padding: 2px"> <fmt:formatDate value="${email.sentDate}" pattern="MM/dd/yyyy HH:mm" /></td>
	</tr>
	</c:if>
	<c:if test="${! empty email.customerErrorReason}">
	<tr>
		<th class="left" style="padding: 2px">Error Reason: </th>
		<td class="left" style="padding: 2px"> <pre><c:out value="${email.customerErrorReason}"/></pre></td>
	</tr>
	</c:if>
</table>
<c:url var="viewEmailBodyURL" value="${urls.viewUserProjectEmailBody}">
	<c:param name="<%= ViewUserProjectEmailBody.EMAIL_ID_PARAMETER %>" value="${email.id}"/>
</c:url>
<iframe frameborder="0" height="500px" id="email-body" name="email-body" src="${viewEmailBodyURL}" width="100%"></iframe>
