<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>

<script>
	setPageTitle("Summary: ${projectName}");
</script>

<div class="section">
	<h3>Form: ${formName}</h3>
	<p><a href="javascript:history.go(-1)">Return to project details</a></p>
	<div class="viewData">
		<table class="list sortable ruler">
			<thead>
				<tr>
					<th class="question">Question</th>
					<c:forEach var="columnName" items="${summaryColumns}" varStatus="status">
						<th><c:out value="${columnName}" /></th>
					</c:forEach>
				</tr>	
			</thead>
			<tbody>
				<c:forEach var="row" items="${summaryData}" varStatus="status">
					<tr class="${bgColor}">
						<c:forEach var="value" items="${row}" varStatus="status">
							<td><c:out value="${value}" /></td>
						</c:forEach>
					</tr>
				</c:forEach>
			</tbody>
		</table>	
	</div>
</div>
