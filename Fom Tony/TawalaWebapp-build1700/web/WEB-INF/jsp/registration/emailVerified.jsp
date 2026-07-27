<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="contentHeading">Registration Complete</div>

<c:choose>
<c:when test="${! confirmedUser.status.allowedToLogOn}">
	<h4>Registration Email Confirmed</h4>
	
	<p>
		Thanks for confirming your email address. You will be notified by email 
		when your account is activated.
	</p>
</c:when>
<c:otherwise>
	<h4>Welcome To Tawala!</h4>

	<p>
		Thanks for registering as a Tawala Designer. We hope you enjoy using the product. To get started
		you may Login in the menu above using your choosen username and password.
	</p>
	<br /><br />
	<p>May you create many wonderful projects!</p>
</c:otherwise>
</c:choose>

<br /><br />
<p>
	Thanks!
	<br /><br />
	The Tawala Team
</p>
