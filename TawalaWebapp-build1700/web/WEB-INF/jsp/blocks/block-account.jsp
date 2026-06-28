<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<c:if test="${! empty user}">
	<div class="block gradient">
		<div class="content">
			<h3>My Account</h3>
			<div>
				<span class="label">Name:</span><span>${user.id}</span>
			</div>
			<div>
				<a href="/user/account">Edit Info</a> |
				<a href="/user/changepassword">Change Password</a>
			</div>
		</div>
	</div>
</c:if>
	
