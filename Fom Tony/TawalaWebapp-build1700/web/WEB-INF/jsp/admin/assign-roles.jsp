<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>


<form:form>

	<div class="section">
		<h2 class="sectionHeading">Assign Roles</h2>
		<div class="sectionContent">
			<table class="list">
			<c:forEach var="currentUserRolesEntry" items="${command.rolesByUserIdMap}">
				<c:set var="userId" value="${currentUserRolesEntry.key}" />
				<c:set var="currentUser" value="${command.usersByUserIdMap[userId]}" />
				<tr>
					<td><c:out value="${currentUser.id}" /></td>
					<td><c:out value="${currentUser.firstName}" /> <c:out value="${currentUser.lastName}" /></td>
					<td align="left" style="padding: 2px;"><form:checkboxes items="${roles}" path="rolesByUserIdMap[${userId}]" itemLabel="description" itemValue="roleId" delimiter="<br />"/></td>
				</tr>
			</c:forEach>
			</table>
			<br />
					
			<div class="buttons">
				<button type="submit">SAVE</button>
			</div>
			
		</div>
	</div>

</form:form>