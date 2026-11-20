<jsp:directive.page import="com.tawala.web.projectmanager.backup.BackupController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<c:url var="backupURL" value="${urls.projectManagerCreateBackup}">
	<c:param name="<%= BackupController.PROJECT_ID_PARAMETER %>" value="${userProject.id}" />
</c:url>
<c:url var="backToProject" value="/app/mytawala-project.html">
	<c:param name="name" value="${userProject.name}" />
</c:url>
<script>
	if (parent && parent !== window) parent.backupURL = '${backupURL}';
</script>


<div class="section" style="padding: 16px 20px; font: 13px/1.4 Tahoma, Verdana, sans-serif; color: #222;">
	<p>	
		You can backup your entire application and any data it contains at the time of backup. 
		It is a complete snapshot of your web application and data.<br/>
	</p>
	<br />

	<p>
		To continue downloading the backup file press the BACKUP button below.
	</p>
	<br />
	
	<p>
		On some browsers, you might see a security message saying downloads have been blocked. 
		Click on the message, and select "Download File".	
	</p>
	<br />

	<p id="backupActions">
		<a href="${backupURL}" style="display:inline-block;padding:6px 18px;background:#1a3a5c;color:#fff;font-weight:700;text-decoration:none;">BACKUP</a>
	</p>
	<p style="margin-top:16px;">
		<a href="${backToProject}">Back to the project</a>
	</p>
</div>
<script>
	if (parent && parent !== window) {
		var actions = document.getElementById("backupActions");
		if (actions) actions.style.display = "none";
	}
</script>
