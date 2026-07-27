<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<c:if test="${! empty user}">
	<div class="block">
		<div class="content">
			<h3>Shared Data</h3>
			<div>
				<c:choose>
					<c:when test="${empty sharedStorage || empty sharedStorage.dataSources}">
						No shared data sources.
					</c:when>
					<c:when test="${fn:length(sharedStorage.dataSources) == 1}">
						1 shared data source.
					</c:when>
					<c:otherwise>
						${fn:length(sharedStorage.dataSources)} shared data sources.
					</c:otherwise>
				</c:choose>
			</div>
			<div>
				<c:forEach var="dataSource" items="${sharedStorage.dataSources}" varStatus="status">
					<a id="linkToViewSharedData${status.count}" href="<c:url value="${urls.viewSharedDatasources}" />"><c:out value="${dataSource.name}" /></a><br />
				</c:forEach>
			</div>
			<br />
			<div>
				<a href="${urls.addNewDataSource}" id="addDataSourceLink">Create New Data Source</a>
			</div>
		</div>
	</div>
</c:if>
	
