<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script>
//	setPageTitle("Publish New Version: ${form.libraryProject.name}");
</script>
 
<div class="section">

	<h3>Publish Project As New Version</h3> 
	<p>
		Project <a href="<c:url value="${urls.libraryProjectDetailView}">
		<c:param name="<%= ViewProjectDetailsController.PARAMETER_ID %>" value="${form.libraryProject.id}"/></c:url>">
		<c:out value="${form.libraryProject.name}" /></a> 
		
		has been updated with a new version. 
	</p>
	<br /><br />
	<p>
	Thank you!
	</p>
	<br /><br />
  
<%--	<a href="${urls.projectManagerProjectDetailView}?projectName=${form.libraryProject.name}">Click here to return to the ${form.libraryProject.name} project</a>--%>

	<div class="buttons">
		<button type="submit" name="cancel" onclick="parent.currentDialogObject.cancel(); return false;">DONE</button>
	</div>
</div>
