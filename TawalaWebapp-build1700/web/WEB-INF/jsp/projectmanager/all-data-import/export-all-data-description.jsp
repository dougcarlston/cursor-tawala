<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.alldataimport.AllProjectDataExportController;"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<c:url var="exportURL" value="${urls.projectManagerExportAllData}">
	<c:param name="<%= AllProjectDataExportController.PROJECT_ID_PARAMETER %>" value="${userProject.id}" />
</c:url>
<script>
	parent.exportURL = '${exportURL}';
</script>


<div class="section" >

	<p>	
	It is very useful to export the entire project data as a backup or for use
	in other applications.
	</p>
	<br />

	<p>
	The data contained in your project will be exported to an Excel File, and it
	will be downloaded to your computer.Click the EXPORT button below to start the download.
	</p>
	<br />
	
	<p>
	On some browsers, you might see a security message saying downloads have
	been blocked. Click on the message, and select "Download File".
	</p>
	<br />

	
<%--	<iframe id="file_download" width="0" height="0" scrolling="no" frameborder="0" --%>
<%--		src="${exportURL}"></iframe>--%>
</div>
