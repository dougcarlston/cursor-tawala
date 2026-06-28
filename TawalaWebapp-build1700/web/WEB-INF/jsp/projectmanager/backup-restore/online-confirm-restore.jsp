<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/backup-restore/header.jsp" />

<div class="section">
	
	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>

	<div class="details">
		<br /><br />
		<c:choose>
			<c:when test="${empty restoredProject}">
		<h3>Project <c:out value="${fileUpload.projectName}" /> has <strong>not</strong> been restored.</h3>
		<br />
		
		<p>An error occurred while restoring a project from the backup. We have recorded the failure and will try to resolve it as soon as possible.</p>
			</c:when>
			<c:otherwise>
		<h5>Project <c:out value="${fileUpload.projectName}" /> has been restored.</h5>
		
			</c:otherwise>
		</c:choose>

		<br />
		<br />
		<br />
		<div class="buttons">
			<button type="submit" name="done" onclick="parent.currentDialogObject.submit(); return false;">DONE</button>
		</div>
	</div>
</div>
