<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.alldataimport.AllProjectDataExportController;"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<c:url var="exportURL" value="${urls.projectManagerExportAllData}">
	<c:param name="<%= AllProjectDataExportController.PROJECT_ID_PARAMETER %>" value="${userProject.id}" />
</c:url>
<c:url var="backToProject" value="/app/mytawala-project.html">
	<c:param name="name" value="${userProject.name}" />
</c:url>
<script>
	if (parent && parent !== window) parent.exportURL = '${exportURL}';
</script>


<div class="section" style="padding: 16px 20px; font: 13px/1.4 Tahoma, Verdana, sans-serif; color: #222;">

	<p>	
	It is very useful to export the entire project data as a backup or for use
	in other applications.
	</p>
	<br />

	<p>
	The data contained in your project will be exported to an Excel File, and it
	will be downloaded to your computer. Click the EXPORT button below to start the download.
	</p>
	<br />
	
	<p>
	On some browsers, you might see a security message saying downloads have
	been blocked. Click on the message, and select "Download File".
	</p>
	<br />

	<p id="exportActions">
		<a href="${exportURL}" style="display:inline-block;padding:6px 18px;background:#1a3a5c;color:#fff;font-weight:700;text-decoration:none;">EXPORT</a>
	</p>
	<p style="margin-top:16px;">
		<a href="${backToProject}">Back to the project</a>
	</p>
</div>
<script>
	if (parent && parent !== window) {
		var actions = document.getElementById("exportActions");
		if (actions) actions.style.display = "none";
	}
</script>
