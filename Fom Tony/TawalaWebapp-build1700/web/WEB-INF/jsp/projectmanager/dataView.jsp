
<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDataController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.ChangeSubmissionFieldValueController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

<c:choose>
	<c:when test="${sharedData}">
<c:url var="changeFieldValueURL" value="${urls.projectManagerChangeFieldValue}">
	<c:param name="<%= ChangeSubmissionFieldValueController.SHARED_DATA_PARAMETER %>" value="${sharedData}" />
</c:url>
	</c:when>
	<c:otherwise>
<c:url var="changeFieldValueURL" value="${urls.projectManagerChangeFieldValue}">
	<c:param name="<%= ChangeSubmissionFieldValueController.USER_PROJECT_ID_PARAMETER %>" value="${userProject.id}"/>
	<c:param name="<%= ChangeSubmissionFieldValueController.SHARED_DATA_PARAMETER %>" value="${sharedData}" />
</c:url>
	</c:otherwise>
</c:choose>
<script>
	setPageTitle("View Form Data");
	var changeFieldValueURL = '${changeFieldValueURL}';
	var records = ${dataPresentation.data};
	var responseSchema = ${dataPresentation.responseSchema};
	var columnDefinitions = ${dataPresentation.columnDefinitions};
	var recordCount = ${fn:length(formSubmissions)};
</script>

<div class="section">
	<h2>${projectName}</h2>
	<h3>Form: ${formName}</h3>
	<p><a href="/projectmanager/projectdetail?projectName=${projectName}">Return to project details</a></p>

    <div id="dt-options">
		<a id="dt-options-link" href="">Show/Hide Columns</a>
		<div id="dt-columnSettings">
			<p>Select columns to show</p>
			<div id="dt-columnList" >
			</div>
			<div class="buttons right">
				<button id="buttonDone">DONE</button>
				<button id="buttonShowAll">SHOW ALL</button>
				<button id="buttonHideChecked">SHOW SELECTED</button>
			</div>
			<br style="clear: both"/>
		</div>
	</div>	

	<form method="post" action="/projectmanager/dataview?projectName=${projectName}&formName=${formName}" id="deleteSubmissionForm">
		<c:if test="${sharedData}">
			<input type="hidden" name="<%= ViewProjectManagerDataController.SHARED_DATA %>" value="${sharedData}" />
		</c:if>
				
		<div id="records" style="width: 960px; overflow: hidden;"></div>
	</form>

	<div class="buttons"  style="clear: both;">
		<a class="button" id="deleteSelectedRecords" href="#"><span>Delete Selected Records</span></a>
	</div>
	<br />
</div>


