<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script>
	setPageTitle("${project.name}");
</script>

<h3>Project History</h3>

<div class="details" id="projectHistory">

<table class="list sortable ruler">
	<col class="col1"/>
	<col class="col2"/>
	<col class="col3"/>
	<thead>
		<tr>
			<th class="left">Description</th>
			<th class="left">Date</th>
			<th class="left">User</th>
			<th>&nbsp;</th>
		</tr>
	</thead>
	<tbody>
<c:forEach items="${events}" var="event">
	<tr>
		<td class="left"><spring:message message="${event.description}" /></td>
		<td class="left"><fmt:formatDate value="${event.date}" pattern="HH:mm MM/dd/yyyy" /></td>
		<td class="left"><c:out value="${event.userId}" /></td>
		<td><%@ include file="/WEB-INF/jsp/library/revertChangesLink.jsp" %></td>
	</tr>
</c:forEach>
	</tbody>
</table>
<br />
</div>