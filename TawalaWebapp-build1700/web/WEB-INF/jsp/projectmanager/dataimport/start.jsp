<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/dataimport/header.jsp" />

<div class="section">
	<form:form method="post" name="uploadForm" enctype="multipart/form-data" commandName="fileUpload">

	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>
	
	<div class="details">
		<h3>Please select a file that contains the data to import.</h3>
		<br />
		
		<spring:bind path="fileUpload.data">
		<input type="file" name="${status.expression}" size="60" /> <br />
		</spring:bind>

		<br />
		<p>File format:</p>
		<form:radiobutton path="excelSpreadsheet" value="true" /> Excel Spreadsheet
		<br />
		<form:radiobutton path="excelSpreadsheet" value="false" /> Comma Separated Value (CSV)
							
	</div>
	<jsp:include page="/WEB-INF/jsp/projectmanager/dataimport/navigation.jsp" />
	</form:form>
</div>