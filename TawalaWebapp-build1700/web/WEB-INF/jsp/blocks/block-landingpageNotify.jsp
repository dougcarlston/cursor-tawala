<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.userdomain.AddNotificationRequestController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<c:if test="${domain.showNotifyBlock}">	
	<div class="block">
		<div class="content">
			<h3>Notify Me!</h3>
			<div id="blockContentNotifyInput">
				<p>
					If you would like to be notified when this Tawala domain goes live please enter
					your email below (email addresses will not be used for any other purpose).
				</p>
				<br />
				<form action="${urls.addNotificationRequest}" id="notifyForm" name="notifyForm" accept-charset="utf-8">
					<input type="text" name="<%= AddNotificationRequestController.EMAIL_PARAMETER %>" value="" id="notifyEmail">
					<br /><br />
					<input type="hidden" name="<%= AddNotificationRequestController.DOMAIN_PARAMETER %>" value="${domain.name}" id="landingpageDomain">
					<input type="button" name="notifySubmit" value="Submit" id="notfySubmit" onclick="notifySubmission();">
				</form>
			</div>
			<div id="blockContentNotifyThanks" style="display: none;">
				<p>
					<b>Thanks!</b>
					<br /><br />
					We'll notify you as soon as the domain is ready.
				</p>
			</div>
		</div>
	</div>
	
	<script type="text/javascript" charset="utf-8">
		var notifyClick = function(o) {
			document.getElementById("blockContentNotifyInput").style.display = "none";
			document.getElementById("blockContentNotifyThanks").style.display = "block";												
		};
	
		var notifySubmission = function() {
			var notifyCallback = {
				success: notifyClick,
				failure: notifyClick,
				timeout: 5000
			};
	
			var formObject = document.getElementById('notifyForm');
			YAHOO.util.Connect.setForm(formObject);
			var cObj = YAHOO.util.Connect.asyncRequest('POST', '${urls.addNotificationRequest}', notifyCallback);
		};
	</script>
</c:if>	