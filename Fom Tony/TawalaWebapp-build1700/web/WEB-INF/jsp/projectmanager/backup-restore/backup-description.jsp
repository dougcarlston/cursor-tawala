<jsp:directive.page import="com.tawala.web.projectmanager.backup.BackupController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<c:url var="backupURL" value="${urls.projectManagerCreateBackup}">
	<c:param name="<%= BackupController.PROJECT_ID_PARAMETER %>" value="${userProject.id}" />
</c:url>
<script>
	parent.backupURL = '${backupURL}';
</script>


<div class="section" >
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
</div>
