<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<jsp:directive.page import="com.tawala.web.library.ModifyProjectController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<c:set var="project" value="${form.project}" scope="request" />
<script>
	setPageTitle("Edit Project Information: <c:out value="${project.name}" />");
</script>

<c:set var="targetId" value="projectEdit" scope="request" />

<div id="projectEdit" class="section">
	<div>
		<%@ include file="/WEB-INF/jsp/library/projectDeleteLink.jsp" %>
	</div>
	
	<form:form method="POST" id="editProjectForm" commandName="form">
		<c:set var="projectIdParameter"><%= ModifyProjectController.PARAMETER_PROJECT_ID %></c:set>
		<input type="hidden" name="${projectIdParameter}" value="${param[projectIdParameter]}" />
		<jsp:include page="/WEB-INF/jsp/library/editProjectBody.jsp" flush="true"/>
		<div class="editActions">
			<input type="image" name="save" value="Save" src="/images/submit-button.gif" /> 
			<a href="${urls.libraryProjectDetailView}?<%= ViewProjectDetailsController.PARAMETER_ID %>=${form.project.id}">
				<img src="/images/cancel-button.gif" alt="Cancel" /></a>
		</div>
	</form:form>
</div>
<jsp:include page="/WEB-INF/jsp/library/newCategory.jsp" flush="true"/>
