<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/all-data-import/header.jsp" />

<div class="section">
	
	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>

	<div class="details">
		<br /><br />
<c:choose>
	<c:when test="${empty importResults}">
		<h5 class="error" >Data has NOT been restored.</h5>
		<br />
		An error occured while reading the backup file. Please try to go through the restore process again.
	</c:when>
	<c:otherwise>
		<h5>Data has been imported.</h5>
		<br />				
		
		<c:if test="${problemsDuringImport}">
			<p class="error">
			Some rows were skipped while importing data. Please review the statistics section below for more details.
			</p>
			<br />
		</c:if>

		<div class="buttons">
			<button type="submit" name="done" onclick="parent.currentDialogObject.submit(); return false;">DONE</button>
		</div>
		<br /><br /><br />
		
		<div class="section collapsible closed">
			<h3 title="Click title bar to hide or view" class="sectionHeading arrowRight">
				Import Statistics
			</h3>
			<div class="sectionContent" style="height: 150px; overflow: auto;">
				<table class="list sortable ruler">
					<thead>
						<tr>
							<th>Form Name</th>
							<th class="right">Records Inserted</th>
							<th class="right">Number of Skipped Rows</th>
							<th class="right">Skipped Row Numbers</th>
						</tr>
					</thead>
					<tbody>
					<c:forEach var="resultEntry" items="${importResults}">
						<c:set var="formName" value="${resultEntry.key}"/>
						<c:set var="stats" value="${resultEntry.value}"/>
						<tr>
							<td align="left"><c:out value="${formName}" /></td>
							<td align="right"><fmt:formatNumber groupingUsed="true" value="${stats.addedRecords}"/></td>
							<td align="right">${fn:length(stats.skippedRows)}</td>
							<td><c:forEach var="rowNumber" items="${stats.skippedRows}" varStatus="status"><c:if test="${status.count > 1}">, </c:if>${rowNumber}</c:forEach></td>
						</tr>
					</c:forEach>
					</tbody>
				</table>
			</div>
		</div>
	</c:otherwise>
</c:choose>
	</div>
</div>

