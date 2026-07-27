<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/all-data-import/header.jsp" />

<div class="section">
	<form:form method="post" name="uploadForm" commandName="fileUpload">

	<div class="details">
		<c:if test="${fn:length(fileUpload.dataImporter.warnings) > 0}">
		<span style="color: maroon">WARNINGS:</span><br />
		<ul>
		<c:forEach var="message" items="${fileUpload.dataImporter.warnings}">
			<li style="list-style: circle; margin-left: 15pt;"><spring:message code="${message.code}" arguments="${message.arguments}"/></li>
		</c:forEach>
		</ul>
		<br />
		<br />
		</c:if>

	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>
		<p>
		Tawala will now import your data into the project. It will replace all the
		data currently there. Current data will be lost unless it has been backed up
		or exported.
		</p>
		<br />
		
		<form:checkbox path="confirmDataDeletion" id="confirmDataDeletionCheckBox" /> 
				<label for="confirmDataDeletionCheckBox">I understand. Please proceed.</label>
		<br />
	</div>
		<jsp:include page="/WEB-INF/jsp/projectmanager/all-data-import/navigation.jsp" />
	</form:form>
</div>
