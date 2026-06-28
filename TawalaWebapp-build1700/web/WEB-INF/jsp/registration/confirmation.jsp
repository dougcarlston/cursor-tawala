<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="section">
	<c:if test="${false}">
		<h4>Thank you for submitting your registration!</h4>
		
		<p>
		You're almost done! An email message has been sent to <c:out value="${userForm.emailAddress}" />. The message contains a link that you must
		follow to complete the registration process. Either click the link in the message or copy it and paste
		it into the address bar of a browser to complete the process.
		</p>
	</c:if>
	
	<h4>Welcome To Tawala!</h4>

	<p>
		Thanks for registering as a Tawala Designer. We hope you enjoy using the product. To get started
		you may Login in the menu above using your chosen username and password.
	</p>
	<br /><br />
	<p>May you create many wonderful projects!</p>


	<br />
	<p>The Tawala Team</p>
</div>
