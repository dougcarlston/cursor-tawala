<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/dataimport/header.jsp" />

<div class="section">
	<form:form method="post" name="uploadForm" commandName="fileUpload">

	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>

	<div class="details">
		<h3>Confirm data import mapping</h3>
		
		<c:if test="${fileUpload.skipFirstRow}">
		<div>First row will be skipped.</div>
		<br />
		</c:if>
		
		<p>
			<form:checkbox path="deleteOldData" id="deleteOldDataCheckBox" /> 
				<label for="deleteOldDataCheckBox">Delete previous data before importing? <strong>Caution:</strong> You won't be able to restore previous data if you haven't exported it earlier!</label>
		</p>
		<br />
		
		
		<p>If you need to correct it use the Previous Button to revise the mappings.</p>
		<br />

		<div class="data viewData" style="height:400px;">
	
			<table class="list ruler">
				<thead>
					<tr>
				<c:forEach var="columnNumber" begin="0" end="${fileUpload.columnCount - 1}">
					<th class="left">
						<spring:bind path="fileUpload.mapping[${columnNumber}]">
						<c:choose>
							<c:when test="${empty status.value}">Skip this column</c:when>
							<c:otherwise><c:out value="${status.value}" /></c:otherwise>
						</c:choose>
						</spring:bind>
					</th>
				</c:forEach>
					</tr>
				</thead>
				<tbody>
					<c:forEach var="row" items="${fileUpload.sampleData}" varStatus="sampleDataStatus">
					<c:choose>
						<c:when test="${sampleDataStatus.count == 1 && fileUpload.skipFirstRow}">
							<c:set var="rowAttributes">id="firstDataRow" class="skipped"</c:set>
						</c:when>
						<c:when test="${sampleDataStatus.count == 1}">
							<c:set var="rowAttributes">id="firstDataRow"</c:set>
						</c:when>
						<c:otherwise>
							<c:set var="rowAttributes"></c:set>
						</c:otherwise>
					</c:choose>
					<tr ${rowAttributes}>
						<c:forEach var="column" items="${row}" varStatus="columnIterationStatus">
							<c:set var="mappingKey">${columnIterationStatus.count - 1}</c:set>
							<c:choose>
								<c:when test="${empty fileUpload.mapping[mappingKey]}"><c:set var="cellClass" value="left skipped" /></c:when>
								<c:otherwise><c:set var="cellClass" value="left" /></c:otherwise>
							</c:choose>
							<td class="${cellClass}"><c:out value="${column}" /></td>
						</c:forEach>
					</tr>
					</c:forEach>
				</tbody>
			</table>
			</div>
		</div>
		<jsp:include page="/WEB-INF/jsp/projectmanager/dataimport/navigation.jsp" />
	</form:form>
</div>
