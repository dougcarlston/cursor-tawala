<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/backup-restore/header.jsp" />

<script>
	function toggleProjectNameInputVisibility(checkbox) {
		if(checkbox.checked) {
			document.getElementById("projectNameInput").style.display = "block";
			document.getElementById("confirmRestoreCheckBox").disabled = true;
		} else {
			document.getElementById("projectNameInput").style.display = "none";
			document.getElementById("confirmRestoreCheckBox").disabled = false;
		}
	}
</script>

<div class="section">
	<form:form method="post" name="uploadForm" commandName="fileUpload">

	<div class="details">

	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>
	
		<c:if test="${! empty fileUpload.originalProject}">
		<p>
		The project <b><c:out value="${fileUpload.originalProject.name}"/></b> will be restored from the backup.
		</p>
		<br />
		<p>
		Tawala will now restore your project. It will replace all the data currently there and will restore the project version at the time of the backup. 
		Current data will be lost unless it has been backed up or exported.
		</p>
		<br />
		
		<form:checkbox path="confirmRestore" id="confirmRestoreCheckBox" /> 
				<label for="confirmRestoreCheckBox">I understand. Please proceed.</label>

		<br />
		<br />
		<form:checkbox path="restoreAsNewProject" id="restoreAsNewProjectCheckBox" onclick="toggleProjectNameInputVisibility(this);" /> 
			<label for="restoreAsNewProjectCheckBox">No, I would like to restore the project under a different name.</label>
		<br />

		<c:if test="${! fileUpload.restoreAsNewProject}">
		<c:set var="projectNameStyle">display: none;</c:set>
		</c:if>
		</c:if>

		<div id="projectNameInput" class="edit" style="${projectNameStyle}">
		<div class="line">
			<label>Restore the project under this name</label> 
			<form:input path="projectName" id="projectName" size="60"/>
		</div>
		<br />
		</div>
	</div>
		<jsp:include page="/WEB-INF/jsp/projectmanager/backup-restore/navigation.jsp" />
	</form:form>
</div>
