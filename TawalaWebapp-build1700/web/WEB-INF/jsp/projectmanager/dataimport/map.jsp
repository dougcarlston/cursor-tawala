<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/dataimport/header.jsp" />

<div class="section">
	<form:form method="post" id="uploadForm" commandName="fileUpload">

	<div class="details">
		<h3>Map data to fields.</h3>

		<c:if test="${fileUpload.firstRowIsLikelyHeaders}">
		<p>It appears that the first row is the field names. We tried to automatically match the columns to the fields. 
		Please review the mappings carefully!
		</p>
		<br />
		</c:if>

		<p>
			<form:checkbox path="skipFirstRow" id="skipFirstRowCheckBox" onclick="markFirstRowSkipped(this);"/> 
				<label for="skipFirstRowCheckBox">Skip the first row? (Usually the first row consists of field names)</label>
		</p>
		<br />

	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>

		
		<p>For each column of data in the import file select the field it will populate from the drop-down menu:
		</p>
		<br />

		<div class="data viewData" style="height:400px;">
			<table class="list ruler" style="width:600px;">
				<thead>
					<tr>
				<c:forEach var="columnNumber" begin="0" end="${fileUpload.columnCount - 1}">
					<th class="left">
						<spring:bind path="fileUpload.mapping[${columnNumber}]">
						<select name="${status.expression}">
							<option value="">Skip this column</option>
							<optgroup label="Fields">
						<c:forEach var="field" items="${fileUpload.fields}">
							<option value="<c:out value="${field.name}" />"<c:if test="${field.name == status.value}"> selected="selected"</c:if> ><c:out value="${field.name}"/></option>
						</c:forEach>
							</optgroup>
						</select>
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
							<c:set var="rowAttributes" value="" />
						</c:otherwise>
					</c:choose>
					<tr ${rowAttributes}>
						<c:forEach var="column" items="${row}">
						<td class="left"><c:out value="${column}" /></td>
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
