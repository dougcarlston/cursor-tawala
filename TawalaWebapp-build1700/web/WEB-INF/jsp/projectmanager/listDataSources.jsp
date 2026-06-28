<jsp:directive.page import="com.tawala.web.projectmanager.ViewSharedDataSourcesController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.ViewProjectManagerDataController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.FormDataExportController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.dataimport.SharedDataImportController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

<script>
	setPageTitle("Shared Data Sources");
</script>

		<div class="sectionContent">
			<table class="list sortable ruler" >
		        <colgroup>
					<col style="width: 380px;"/>
					<col style="width: 70px;"/>
					<col style="width: 150px;"/>
		        </colgroup>
				<thead>
					<tr>
						<th class="left">Data Source</th>
						<th>Responses</th>
						<th>Actions</th>
					</tr>
				</thead>
				<tbody>
					<c:if test="${empty sharedData.dataSources}">
					<tr>
						<td colspan="3" align="center">No shared data sources are defined</td>
					</tr>
					</c:if>
					<c:forEach var="dataSource" items="${sharedData.dataSources}" varStatus="status">
						<tr>
							<td class="left"><c:out value="${dataSource.name}" /></td>	
							<td>${responseCounts[dataSource]}</td>
							<td>
								<div style="display: none;">
									<a id="exportCSV${status.count}" href="<c:url value="${urls.projectManagerExportFormCSV}" >
											<c:param name="<%=FormDataExportController.PARAMETER_SHARED_DATA%>" value="yes" />
											<c:param name="<%=FormDataExportController.PARAMETER_FORM_NAME%>" value="${dataSource.name}" />
										</c:url>"> Link to export in CSV </a>
									<a id="exportExcel${status.count}" href="<c:url value="${urls.projectManagerExportFormExcel}" >
											<c:param name="<%=FormDataExportController.PARAMETER_SHARED_DATA%>" value="yes" />
											<c:param name="<%=FormDataExportController.PARAMETER_FORM_NAME%>" value="${dataSource.name}" />
										</c:url>"> Link to export in Excel </a>
								</div>
								<div class="controls">
									<a id="view${status.count}" href="<c:url value="${urls.projectManagerDataView}">
											<c:param name="<%=ViewProjectManagerDataController.SHARED_DATA%>" value="yes" />
											<c:param name="<%=ViewProjectManagerDataController.FORM_NAME%>" value="${dataSource.name}" />
										</c:url>">
										<img src="/images/silk/zoom.gif" alt="View" title="View data for form <c:out value="${dataSource.name}" />" class="smallIcon" />
									</a>
									<a href="#" onclick="Tawala.ProjectManager.Export.showExportDataSourceDialog('<c:out value="${dataSource.name}" />', ${status.count}); return false;">
										<img src="/images/silk/page_white_go.gif" alt="Export" title="Export data from <c:out value="${dataSource.name}" />" class="smallIcon" />
									</a>

									<a id="import${status.count}" href="<c:url value="${urls.projectManagerImportSharedData}" >
											<c:param name="<%=SharedDataImportController.PARAMETER_DATASOURCE_NAME%>" value="${dataSource.name}" />
										</c:url>">
										<img src="/images/silk/application_get.gif" alt="Import" title="Import data to <c:out value="${dataSource.name}" />" class="smallIcon" />
									</a>
									
									<form id="confirmDataEraseForm<c:out value="${status.count}" />" class="confirm" onclick="return false;">
										<input type="hidden" name="<%= ViewSharedDataSourcesController.PARAMETER_FORM_NAME %>" value="<c:out value="${dataSource.name}" />" /> 
										<input type="hidden" name="action" value="<%= ViewSharedDataSourcesController.PARAMETER_ACTION_ERASE %>" />
										<input type="image" value="<%= ViewSharedDataSourcesController.PARAMETER_ACTION_ERASE %>" src="/images/silk/bin.gif" alt="Purge" title="Purge data from <c:out value="${dataSource.name}" />" class="smallIcon" />
									</form>
									<form id="confirmDeleteForm<c:out value="${status.count}" />" method="post" class="confirm">
										<input type="hidden" name="<%= ViewSharedDataSourcesController.PARAMETER_FORM_NAME %>" value="<c:out value="${dataSource.name}" />" /> 
										<input type="hidden" name="action" value="<%= ViewSharedDataSourcesController.PARAMETER_ACTION_DELETE %>" /> 
										<input type="image" name="action" value="Delete" src="/images/delete-icon.gif" alt="Delete" title="Delete datasource" class="smallIcon" />
									</form>
								</div>
							</td>
						</tr>
					</c:forEach>	
				</tbody>
			</table>
		</div>
