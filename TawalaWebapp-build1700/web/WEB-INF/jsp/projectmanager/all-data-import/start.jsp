<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/all-data-import/header.jsp" />

<div class="section">
	<form:form method="post" name="uploadForm" enctype="multipart/form-data" commandName="fileUpload">

	<div class="details">
		<p>
		To import data into a project, use an Excel file created by the Project Data
		Export process. 
		</p>
		<br />
		<p>
		It is possible to modify the data in this file, but it is very important to
		keep columns and tabs unchanged.
		</p> 
		<br />
		<p>
		Select the file to import:
		</p>
		<br />
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="fileUpload" />
		</tiles:insert>
		
		<spring:bind path="fileUpload.data">
		<input class="text" type="file" name="${status.expression}" size="60" />
		</spring:bind>
	</div>

    <br />

	<jsp:include page="/WEB-INF/jsp/projectmanager/all-data-import/navigation.jsp" />
	</form:form>
</div>