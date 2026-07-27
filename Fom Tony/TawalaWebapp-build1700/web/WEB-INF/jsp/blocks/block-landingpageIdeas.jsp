<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.userdomain.AddSuggestionController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<c:if test="${domain.showSuggestionsBlock}">	
	<div class="block">
		<div class="content">
			<h3>Got ideas for other solutions?</h3>
			<p>
				 If you would like to see other Web apps for ${domain.displayName} from Tawala 
				 please enter your suggestions here:
			</p>
			<br />
			<form action="${urls.addSuggestion}" id="ideasForm" name="ideasForm" accept-charset="utf-8">
				<input type="hidden" name="<%= AddSuggestionController.DOMAIN_PARAMETER %>" value="${domain.name}" id="landingpageDomain">
				<textarea id="ideasText" name="<%= AddSuggestionController.TEXT_PARAMETER %>" rows="3" cols="36"></textarea>
				<br /><br />
				<input type="button" name="ideasSubmit" value="Submit" id="ideasSubmit" onclick="ideasSubmission();">
			</form>
			<br /><br />
		</div>
	</div>
		
	<script type="text/javascript" charset="utf-8">
		var ideasClick = function(o) {
			alert("Thanks for the ideas!\n\n Feel free to enter more if you think of any.");
			document.getElementById('ideasText').value = "";
		};
		
		var ideasSubmission = function() {
			var ideasCallback = {
				success: ideasClick,
				failure: ideasClick,
				timeout: 5000
			};
	
			var formObject = document.getElementById('ideasForm');
			YAHOO.util.Connect.setForm(formObject);
			var cObj = YAHOO.util.Connect.asyncRequest('POST', '${urls.addSuggestion}', ideasCallback);
		};
	</script>
</c:if>	