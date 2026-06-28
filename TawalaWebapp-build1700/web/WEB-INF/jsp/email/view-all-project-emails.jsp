<jsp:directive.page import="com.tawala.web.email.ViewUserProjectEmail"/>
<%@page import="com.tawala.web.email.ExportAllProjectEmailController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags"%>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script>
	setPageTitle("Project Emails");
</script>

<div class="section">
	<h2><c:out value="${project.name}" /></h2>
</div>

<p><a href="javascript:history.go(-1)">&lt;&lt; Return to Project Details</a></p><br />

<c:url var="exportAllEmailURL" value="${urls.exportAllProjectEmail}">
	<c:param name="<%= ExportAllProjectEmailController.PROJECT_ID_PARAMETER %>" value="${project.id}"/>
</c:url>
<a id="exportEmailsLink" href="${exportAllEmailURL}">Export this report to Excel</a>

<div class="sectionContent yui-skin-sam">
	<div id="email-paging-top"></div>
	<div id="email-data-table"></div>
	<div id="email-paging-bottom"></div>
</div>

<c:url var="viewEmailURL" value="${urls.viewUserProjectEmail}">
	<c:param name="<%= ViewUserProjectEmail.EMAIL_ID_PARAMETER %>" value=""/>
</c:url>

<script type="text/javascript">
<!--
var userProjectId = ${project.id};
var viewEmailURL = '${viewEmailURL}';

YAHOO.util.Event.onDOMReady(initializeEmailDataTable);
-->
</script>

