<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<c:if test="${! empty user}">
	<div class="block gradient">
		<div class="content">
			<h3>My Account</h3>
			<div>
				<c:set var="shownName" value="${user.id}" />
				<c:if test="${fn:startsWith(user.id, 'user_') and not empty user.firstName}">
					<c:set var="shownName" value="${user.firstName}" />
				</c:if>
				<span class="label">Name:</span><span><c:out value="${shownName}" /></span>
			</div>
			<div>
				<a href="/user/account">Edit Info</a> |
				<a href="/user/changepassword">Change Password</a>
			</div>
		</div>
	</div>
</c:if>
	
