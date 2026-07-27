<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="section">
	<h2 class="sectionHeading">Count of Projects Cloned From the Library</h2>
	<div class="sectionContent">

		<table class="component outline">
			<thead>
			  <tr>
			    <th>Project</th>
			    <th>Number of Clones</th>
			    <th>Percentage</th>
			</thead>
			<tbody>
				<c:forEach var="row" items="${data}" varStatus="status">
				<c:choose>
					<c:when test="${status.count % 2 == 0}">
						<c:set var="rowClass" value="even" />
					</c:when>
					<c:otherwise>
						<c:set var="rowClass" value="odd" />
					</c:otherwise>
				</c:choose>
				<c:set var="colorNumber" value="${(status.count % 16) + 1}"/>
				<fmt:formatNumber var="percentage" value="${row.percentage}" pattern="##0.0" />
				
				<tr class="${rowClass}">
					<td class="left"><c:out value="${row.libraryProject.name}" /></td>
					<td><fmt:formatNumber value="${row.count}" pattern="###,##0"/></td>
					<td class="left"><div class="graph color${colorNumber}"><strong class="bar" style="width: ${percentage}%"><span>${percentage}%</span></strong></div></td>
				</tr>
				</c:forEach>
			</tbody>
		</table>
		
	</div>
</div>