<%@ page import="com.tawala.web.library.DeleteProjectController"%>
<%--<button type="button" onclick="window.location='${urls.libraryDeleteProject}?<%= DeleteProjectController.PARAMETER_PROJECT_ID %>=${project.id}'">--%>
<%--	Delete--%>
<%--</button>--%>
<form method="post" class="confirm" action="${urls.libraryDeleteProject}" onclick="return false;">
	<input type="hidden" name="<%= DeleteProjectController.PARAMETER_PROJECT_ID %>" value="${project.id}" />
	<input type="hidden" name="action" value="deleteproject" />
	<button type="button" name="action" value="deleteproject" alt="Delete" title="Delete this project">Delete Project</button>
</form>
