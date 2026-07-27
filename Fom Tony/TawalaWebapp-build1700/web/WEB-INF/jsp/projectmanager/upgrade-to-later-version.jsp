
<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<script>
	setPageTitle("Upgrade Project");
</script>

<h1><c:out value="${project.name}" /></h1>

<div class="section">
	<div id="upgradeProjectDiv" class="details">

		<form:form method="post" id="upgradeToNewerVersionForm" commandName="upgradeForm">

			<form:errors path="newLibraryVersionNumber" cssClass="error"/><br />
			
			Select the version from the library to upgrade to: <br />
			<c:forEach var="version" items="${newerVersions}" varStatus="iterationStatus">
				<form:radiobutton path="newLibraryVersionNumber" value="${version.versionNumber}" label=" Version ${version.versionNumber}: ${version.text}" />
				<br />
			</c:forEach>

			<br />

			Describe the new project version you are about to add:<br/>
			<form:textarea path="versionDescription" rows="3" cols="60" />

			<div class="buttons">
				<button type="submit" name="submit" value="Submit">UPGRADE</button>
				<c:url var="projectDetailURL" value="${urls.projectManagerProjectDetailView}">
					<c:param name="<%= ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME %>" value="${project.name}" />
				</c:url>
				<button type="submit" name="cancel" onclick="window.location.href='${projectDetailURL}'; return false;">CANCEL</button>
			</div>
			
		</form:form>	
	</div>
</div>