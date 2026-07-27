<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/backup-restore/header.jsp" />

<div class="section">
	<form:form method="post" name="uploadForm" enctype="multipart/form-data" commandName="fileUpload">

	<div class="details">
		To restore a project, use a backup file created by the Project Backup process. 
		
		<br />
		<br />

		Select the file:
		<br />

	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>
		
		<spring:bind path="fileUpload.data">
		<input type="file" name="${status.expression}" size="60" />
		</spring:bind>
	</div>

    <br />

	<jsp:include page="/WEB-INF/jsp/projectmanager/backup-restore/navigation.jsp" />
	</form:form>
</div>