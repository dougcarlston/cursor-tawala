<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.domain.Status"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<%
	pageContext.setAttribute("initialRegistrationStatus", Status.REGISTERED_INITIAL);
%>

<c:choose>
	<c:when test="${empty user}">
		<div class="block">
			<div class="levelLink">
            	<a href="${urls.userInitialRegistration}" id="linkToInitialSetup">
            		<img src="/images/homepage/nextlevel-button.gif" alt="Save your customized apps" />
            		<br /><span>Save your customized apps</span>
            	</a>
			</div>
		</div>
	</c:when>
	<c:when test="${user.status == initialRegistrationStatus}">
		<div class="block">
			<div class="levelLink">
				<a href="${urls.userDisplayNextLevel}" id="linkToUpgradeToFullyRegistered"> 
					<img src="/images/homepage/nextlevel-button.gif" alt="Create your own Tawala apps" />
					<br />
					<span>Create your own Tawala apps</span>
				</a>
			</div>
		</div>
	</c:when>
</c:choose>	
	
