<%@page import="com.tawala.web.projectmanager.projectgroup.GroupRosterReportController"%><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@page import="com.tawala.web.projectmanager.projectgroup.ViewGroupMenuController"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<h2>Player Report</h2>
	<h3><c:out value="${group.name}"/></h3>
	
	<p>It might take a while because it requires a large amount of data aggregation. Please don't hit Run Report multiple times.</p>
	
	<!-- TODO: Would be nice to add a spinner image here. -->
	
	<div class="buttons">
		<button type="submit" onclick="runReport();" id="runButton">RUN REPORT</button>

		<c:url var="groupMenuURL" value="${urls.projectGroupMenu}">
			<c:param name="<%=ViewGroupMenuController.GROUP_ID %>" value="${group.id}" />
		</c:url>
		<a id="linkToEditGroup${status.count}" href="${groupMenuURL}">CANCEL</a>

	</div>	
	<br /><br />		
</div>

<c:url var="runReportURL" value="${urls.projectGroupRosterReport}">
	<c:param name="<%= GroupRosterReportController.GROUP_ID_PARAMETER %>" value="${group.id}" />
	<c:param name="<%= GroupRosterReportController.DO_RUN %>" value="yes" />
</c:url>

<script type="text/javascript">
	function runReport(){
		window.location = '${runReportURL}';
	}
</script> 
