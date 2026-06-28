
<jsp:directive.page import="com.tawala.web.userdomain.EditUserDomainController"/>
<jsp:directive.page import="com.tawala.web.userdomain.DeleteUserDomainController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<div class="section">
	<h2 class="sectionHeading">Manage User Domains</h2>
	<div class="sectionContent">
		<c:choose>
			<c:when test="${empty domains}">
				There are no user domains currently defined.
			</c:when>
			<c:otherwise>
				<table class="list ruler">
					<col style="" />
					<col style="width: 150px;" />
					<thead>
						<tr>
							<th>Domain</th><th>Action</th>
						</tr>
					</thead>
					<tbody>
						<c:forEach var="domain" items="${domains}">
							<c:url var="editUrl" value="${urls.editUserDomain}">
								<c:param name="<%= EditUserDomainController.DOMAIN_ID_PARAMETER %>" value="${domain.id}" />
							</c:url>
							<c:url var="deleteUrl" value="${urls.deleteUserDomain}">
								<c:param name="<%= DeleteUserDomainController.DOMAIN_ID_PARAMETER %>" value="${domain.id}" />
							</c:url>
							<tr>
								<td class="left">${domain.name}</td>
								<td class="left"><a href="${editUrl}">Edit</a>&nbsp;&nbsp;&nbsp;&nbsp; <a href="${deleteUrl}">Delete</a></td>
							</tr>
						</c:forEach>
					</tbody>
				</table>
			</c:otherwise>
		</c:choose>
		
		<br />
		<a href="${urls.editUserDomain}">Add another domain</a>
	</div>
</div>	