<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<jsp:include page="/WEB-INF/jsp/projectmanager/dataimport/header.jsp" />

<div class="section">
	
	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="fileUpload" />
	</tiles:insert>

	<div class="details">
		<br /><br />
		<h3>Data has been imported.</h3>
		<br />
		Created <fmt:formatNumber groupingUsed="true" value="${statistics.addedRecords}"/> records.
		
		<c:if test="${fn:length(statistics.skippedRows) > 0}">
		<div>
		Skipped the following lines because they contained unexpected number of columns:</br>
		<c:forEach var="rowNumber" items="${statistics.skippedRows}" varStatus="status"><c:if test="${status.count > 1}">, </c:if>${rowNumber}</c:forEach>
		</div>
		</c:if>

		<c:if test="${fn:length(statistics.emptyRows) > 0}">
		<div>
		Skipped the following lines because they are empty:</br>
		<c:forEach var="rowNumber" items="${statistics.emptyRows}" varStatus="status"><c:if test="${status.count > 1}">, </c:if>${rowNumber}</c:forEach>
		</div>
		</c:if>
		
		<br />
		<br />
		<p>
			<a href="<c:url value="${fileUpload.returnURL}" />">Click here</a> to return to the <c:out value="${fileUpload.dataSourceDescription}" /> details page.
		</p>
		<br />
	</div>
</div>
