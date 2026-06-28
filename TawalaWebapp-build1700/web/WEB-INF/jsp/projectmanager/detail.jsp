<%@ page
	import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController"%>
<jsp:directive.page
	import="com.tawala.web.projectmanager.ProjectVersionDownloadController" />
<jsp:directive.page import="com.tawala.web.projectmanager.FormDataExportController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.ChangeProjectThemeController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.dataimport.ProjectDataImportController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.backup.DisplayBackupDescriptionController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.alldataimport.DisplayExportAllDataDescriptionController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.alldataimport.AllDataImportController"/>
<jsp:directive.page import="com.tawala.web.library.PublishProjectToLibraryController"/>
<jsp:directive.page import="com.tawala.web.library.AddVersionToUnrelatedProjectController" />
<jsp:directive.page import="com.tawala.web.projectmanager.backup.OnlineBackupRestoreController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.backup.DeleteOnlineBackupController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.backup.SetupDailyBackupController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.SaveFormSelectionController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.SaveHideSelectedFormsStateController"/>
<jsp:directive.page import="com.tawala.web.email.ViewAllUserProjectEmailsController"/>
<%@page import="com.tawala.web.projectmanager.UpdateProjectDetailsController"%>
<%@page import="com.tawala.jbpm.sportsdashboards.Constants"%>
<%@page import="com.tawala.web.projectmanager.integration.UpdateYourSportsLeagueDataController"%>
<%@page import="com.tawala.web.email.DeleteAllProjectEmailsController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags"%>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script>
	setPageTitle("Project Details");
</script>

<div class="section">
	<h2>${projectInfo.project.name}</h2>
</div>

<div class="section">
	<h3 class="sectionHeading">
		Project Actions
	</h3>
	<div class="buttons">
		<button type="submit" id="pmActionsExportAll" title="Export project data to Excel format">EXPORT</button>
		<button type="submit" id="pmActionsImportAll" title="Import data to project from Excel or CSV file">IMPORT</button>
		<button type="submit" id="pmActionsBackup" title="Backup project data">BACKUP</button>
		<button type="submit" id="pmActionsRestore" title="Restore project data">RESTORE</button>

		<img class="spacer" src="/images/spacer-line.gif" width="8" height="24" />

		<form name="projectActions" method="post" class="confirm">
			<input type="hidden" name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME%>"	value="<c:out value="${projectInfo.project.name}" />" /> 
			<input type="hidden" name="formName" value="<c:out value="${projectInfo.project.name}" />" /> 
			<input type="hidden" name="action" value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_PURGE%>" /> 
			<button type="submit" id="purgeProject" name="Pruge" value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_PURGE%>" title="Purge project data">PURGE</button>
		</form>
		
		<form name="projectActions" method="post" class="confirm" id="projectActions">
			<input type="hidden" name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME%>"	value="<c:out value="${projectInfo.project.name}" />" /> 
			<input type="hidden" name="action" value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE%>" /> 
			<button type="submit" id="deleteProject" name="Delete" value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE%>" title="Delete Project">DELETE</button>
		</form>
		
		<img  class="spacer" src="/images/spacer-line.gif" width="8" height="24" />

		<button type="submit" id="pmActionsPublish" title="Publish project to the Community Library">PUBLISH</button> 		
	</div>
	<br />
</div>

<div id="forms" class="section collapsible">
	<h3 class="sectionHeading">
		Project Data
	</h3>
	<div class="sectionContent">
		<c:choose>
			<c:when test="${showSelectedFormsOnly}">
				<c:set var="showAllFormsButtonStyle" value="display: inline" />
				<c:set var="showSelectedFormsOnlyButtonStyle" value="display: none" />
			</c:when>
			<c:otherwise>
				<c:set var="showAllFormsButtonStyle" value="display: none" />
				<c:set var="showSelectedFormsOnlyButtonStyle" value="display: inline" />
			</c:otherwise>
		</c:choose>

		<div class="buttons">
			<button type="submit" id="showAllFormsButton" title="Show All Forms" 
				style="${showAllFormsButtonStyle}" onclick="Tawala.ProjectManager.showSelectedFormsOnly(false);">SHOW ALL FORMS</button>
			<button type="submit" id="hideSelectedFormsButton" title="Show Selected Forms Only" 
				style="${showSelectedFormsOnlyButtonStyle}" onclick="Tawala.ProjectManager.showSelectedFormsOnly(true);">SHOW SELECTED FORMS ONLY</button>
		</div>
		<br style="clear: both;" />
		<table class="list sortable ruler">
			<colgroup>
				<col style="width: 30px;" />
				<col style="width: 410px;" />
				<col style="width: 70px;" />
				<col style="width: 140px;" />
			</colgroup>
			<thead>
				<tr>
					<th></th>
					<th class="left">
						Form
					</th>
					<th>
						Responses
					</th>
					<th>
						Actions
					</th>
				</tr>
			</thead>
			<tbody id="projectForms">
				<c:forEach var="formInfo" items="${formsInfo}" varStatus="status">
					<c:set var="rowDisplayStyle" value="table-row" />
					<c:choose>
						<c:when test="${tawala:contains(projectInfo.project.project.formNamesSelectedInProjectManager, formInfo.form.name)}">
							<c:set var="checkedAttribute" value="checked" />
						</c:when>
						<c:otherwise>
							<c:set var="checkedAttribute" value="" />
							<c:if test="${showSelectedFormsOnly}">
								<c:set var="rowDisplayStyle" value="none" />
							</c:if>
						</c:otherwise>
					</c:choose>
					<tr id="formSection${status.count}" style="display: ${rowDisplayStyle}" name="formSectionRow">
						<td>
							<div>
								<c:if test="${formInfo.startingPoint}">
									<a id="run${status.count}"
										href="<tawala:linkToRealProject 
												project="${projectInfo.project}"
												form="${formInfo.form}" />"
										target="<c:out value="${projectInfo.project.name}" />"> <img
											src="/images/silk/control_play.gif" alt="Run"
											title="Run project from this starting point"
											class="smallIcon" /> </a>
								</c:if>
							</div>
						</td>
						<td class="left">
							<c:url var="saveFormSelectionUrl" value="${urls.projectManagerHideFormFromView}">
								<c:param name="<%=SaveFormSelectionController.PROJECT_ID_PARAMETER%>" value="${projectInfo.project.id}" />
								<c:param name="<%=SaveFormSelectionController.FORM_NAME_PARAMETER%>" value="${formInfo.form.name}" />
								<c:param name="<%=SaveFormSelectionController.SELECTED_PARAMETER %>" value="" />
							</c:url>
							<div style="float: left; width: 90%;">
								<form style="display: inline">
									<input type="checkbox" ${checkedAttribute} id="hideFormCheckbox${status.count}" onchange="Tawala.ProjectManager.saveFormSelection('${saveFormSelectionUrl}' + this.checked)" />
								</form>
								<c:out value="${formInfo.form.name}" />
							</div>
						</td>
						<c:choose>
							<c:when test="${formInfo.form.sharedData}">
								<td colspan="2">
									This form's data is
									<a href="<c:url value="${urls.viewSharedDatasources}" />">shared</a>
									as "
									<c:out value="${formInfo.form.dataSourceName}" />
									".
								</td>
							</c:when>
							<c:otherwise>
								<td>
									${formInfo.responses}
								</td>
								<td>
									<div style="display: none">
										<a id="exportCSV${status.count}"
											href="<c:url value="${urls.projectManagerExportFormCSV}" >
											<c:param name="<%=FormDataExportController.PARAMETER_PROJECT_NAME%>" value="${projectInfo.project.name}" />
											<c:param name="<%=FormDataExportController.PARAMETER_FORM_NAME%>" value="${formInfo.form.name}" />
										</c:url>">Link to form data export in CSV</a>
										<a id="exportExcel${status.count}"
											href="<c:url value="${urls.projectManagerExportFormExcel}" >
											<c:param name="<%=FormDataExportController.PARAMETER_PROJECT_NAME%>" value="${projectInfo.project.name}" />
											<c:param name="<%=FormDataExportController.PARAMETER_FORM_NAME%>" value="${formInfo.form.name}" />
										</c:url>">Link to form data export in Excel</a>
									</div>
									<div class="controls">
										<a id="view${status.count}"
											href="<c:url value="/projectmanager/dataview">
											<c:param name="projectName" value="${projectInfo.project.name}" />
											<c:param name="formName" value="${formInfo.form.name}" />
										</c:url>">
											<img src="/images/silk/zoom.gif" alt="View"
												title="View data for form <c:out value="${formInfo.form.name}" />"
												class="smallIcon" /> </a>
										<a id="summary${status.count}"
											href="<c:url value="/projectmanager/summaryview">
											<c:param name="projectName" value="${projectInfo.project.name}" />
											<c:param name="formName" value="${formInfo.form.name}" />
										</c:url>">
											<img src="/images/silk/table.gif" alt="Summary"
												title="View summary of data for form <c:out value="${formInfo.form.name}" />"
												class="smallIcon" /> </a>
										<a href="#" onclick="Tawala.ProjectManager.Export.showExportFormDialog('<c:out value="${formInfo.form.name}" />', ${status.count}); return false;"><img src="/images/silk/page_white_go.gif" alt="Export"
												title="Export data from <c:out value="${formInfo.form.name}" />"
												class="smallIcon" /> </a>
										<c:if test="${user.status.allowedToViewDesigner}">
											<a id="import${status.count}"
												href="<c:url value="${urls.projectManagerImportData}" >
												<c:param name="<%=ProjectDataImportController.PARAMETER_PROJECT_NAME%>" value="${projectInfo.project.name}" />
												<c:param name="<%=ProjectDataImportController.PARAMETER_FORM_NAME%>" value="${formInfo.form.name}" />
											</c:url>">
												<img src="/images/silk/application_get.gif" alt="Import"
													title="Import data to <c:out value="${formInfo.form.name}" />"
													class="smallIcon" /> </a>
										</c:if>

										<form id="<c:out value="${formInfo.form.name}" />"
											class="confirm" onclick="return false;">
											<input type="hidden"
												name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME%>"
												value="<c:out value="${projectInfo.project.name}" />" />
											<input type="hidden"
												name="<%=ViewProjectManagerDetailsController.PARAMETER_FORM_NAME%>"
												value="<c:out value="${formInfo.form.name}" />" />
											<input type="hidden" name="action"
												value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_ERASE%>" />
											<input type="image"
												value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_ERASE%>"
												src="/images/silk/bin.gif" alt="Purge"
												title="Purge data for form <c:out value="${formInfo.form.name}" />"
												class="smallIcon" />
										</form>
									</div>
								</td>
							</c:otherwise>
						</c:choose>
					</tr>
				</c:forEach>
			</tbody>
		</table>
	</div>
</div>
<br />

<div id="projectAppearance" class="section collapsible">
	<h3 class="sectionHeading">
		Project Appearance
	</h3>
	<div class="sectionContent">
	<div style="float: right;" id="projectAppearanceStatusContainer"></div>
	<form:form id="themeChangeForm" action="${urls.changeProjectTheme}" commandName="projectInfo.project">
	<input type="hidden" name="<%= ChangeProjectThemeController.PARAMETER_PROJECT_ID %>" 
		value="${projectInfo.project.uniqueRandomId}"/>
	Choose a theme to change the appearance of your project: 
	<form:select path="project.theme.themeId" onchange="Tawala.ProjectManager.ChangeAppearance.saveTheme();">
		<c:if test="${! empty userDefinedThemes}">
		<optgroup label="My Themes">
			<form:options items="${userDefinedThemes}" itemLabel="name" itemValue="themeId" />
		</optgroup>
		</c:if>
		<optgroup label="Common Themes">
			<form:options items="${commonThemes}" itemLabel="name" itemValue="themeId" />
		</optgroup>
	</form:select>
	</form:form>
	</div>
</div>
<br />
<div id="formURLs" class="section collapsible">
	<h3 class="sectionHeading">
		Project Links
	</h3>
	<div class="sectionContent">
		<p>
			You can copy the links listed below to access the starting points in
			your project
		</p>
		<table class="list sortable ruler">
			<colgroup>
				<col style="width: 250px;" />
				<col style="" />
			</colgroup>
			<thead>
				<tr>
					<th class="left">
						Form
					</th>
					<th class="left">
						Link
					</th>
				</tr>
			</thead>
			<tbody>
				<c:forEach var="formInfo" items="${formsInfo}" varStatus="status">
					<c:if test="${formInfo.startingPoint}">
						<tr>
							<td class="left">
								<c:out value="${formInfo.form.name}" />
							</td>
							<td class="left">
								<div>
									<a id="run${status.count}"
										href="<tawala:linkToRealProject 
												project="${projectInfo.project}"
												form="${formInfo.form}" />"
										target="<c:out value="${projectInfo.project.name}" />"> <tawala:linkToRealProject
											project="${projectInfo.project}" form="${formInfo.form}" />
									</a>
								</div>
							</td>
						</tr>
					</c:if>
				</c:forEach>
			</tbody>
		</table>
	</div>
</div>
<br />
<div id="versions" class="section collapsible closed">
	<h3 class="sectionHeading">
		Project Versions
	</h3>
	<div class="sectionContent">
		<div>
			<span class="stats">Total Versions:
				${fn:length(projectInfo.project.versions)}</span>
		</div>
		<table class="list sortable ruler">
			<colgroup>
				<col style="width:28px;" />
				<col style="width:52px;" />
				<col style="width:56px;" />
				<col style="width:290px;" />
				<col style="width:110px;" />
				<col style="width:120px;" />
			</colgroup>
			<thead>
				<tr>
					<th></th>
					<th>
						Active
					</th>
					<th>
						Version
					</th>
					<th class="left">
						Description
					</th>
					<th class="left">
						Date
					</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
				<c:forEach var="version" items="${projectInfo.project.versions}"
					varStatus="status">
					<tr>
						<td>
							<c:if test="${projectInfo.project.deployedVersion != version}">
								<input type="checkbox" class="versionSelected"
									value="${version.id}" />
							</c:if>
						</td>
						<td>
							<c:if test="${projectInfo.project.deployedVersion == version}">
								<img alt="Currently deployed version"
									src="/images/silk/tick.gif" width="16" height="16"
									id="deployedVersion${version.id}" />
							</c:if>
						</td>
						<td>
							${version.versionNumber }
						</td>
						<td class="left"><c:out value="${version.description}" /></td>
						<td class="left">
							<fmt:formatDate value="${version.date}"
								pattern="HH:mm MM/dd/yyyy" />
						</td>
						<td>
							<div class="controls">
								<c:if test="${projectInfo.project.deployedVersion != version}">
									<c:url var="versionDeployURL"
										value="${urls.projectManagerProjectDetailView}">
										<c:param
											name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME%>"
											value="${projectInfo.project.name}" />
										<c:param name="action"
											value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_DEPLOY%>" />
										<c:param
											name="<%=ViewProjectManagerDetailsController.PROJECT_VERSION_PARAMETER%>"
											value="${version.id}" />
									</c:url>
									<c:url var="versionDeleteURL"
										value="${urls.projectManagerProjectDetailView}">
										<c:param
											name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME%>"
											value="${projectInfo.project.name}" />
										<c:param name="action"
											value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE_VERSION%>" />
										<c:param
											name="<%=ViewProjectManagerDetailsController.PROJECT_VERSION_PARAMETER%>"
											value="${version.id}" />
									</c:url>
									<a href="${versionDeployURL}" id="deploy${version.id}" class="textControl"
										title="Make this version the active version">Deploy</a>
									<form id="deleteVersion${version.id}" class="confirm"
										onclick="return false;">
										<input type="hidden"
											name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME%>"
											value="<c:out value="${projectInfo.project.name}" />" />
										<input type="hidden"
											name="<%=ViewProjectManagerDetailsController.PROJECT_VERSION_PARAMETER%>"
											value="${version.id}" />
										<input type="hidden" name="action"
											value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE_VERSION%>" />
										<input type="image"
											value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE_VERSION%>"
											src="/images/silk/delete.gif" alt="Purge"
											title="Delete this version" class="smallIcon" />
									</form>
								</c:if>

								<c:if test="${user.status.allowedToViewDesigner}">
									<c:url var="versionDownloadURL"
										value="${urls.projectManagerDownloadVersion}">
										<c:param
											name="<%=ProjectVersionDownloadController.PROJECT_VERSION_PARAMETER%>"
											value="${version.id}" />
									</c:url>
									<a href="${versionDownloadURL}" id="download${version.id}"><img
											src="/images/download-icon.gif"
											title="Download this version of the project"
											class="smallIcon" />
									</a>
								</c:if>
							</div>
						</td>
					</tr>
				</c:forEach>
			</tbody>
		</table>
		<div style="float: left;padding-top: .5em;">
			<a id="PMDeleteSelected" class="roundButton" href="#"><span>Delete
					Selected Items</span>
			</a>
		</div>
		<br />
	</div>
</div>

<c:if test="${user.administrator || originalUser.administrator}">
<div id="backup-schedules" class="section collapsible closed">
	<h3 class="sectionHeading">
		Scheduled Project Backups
	</h3>
	<div class="sectionContent">
		<div>
			<c:choose>
				<c:when test="${empty backupSchedules}">
					<form:form commandName="dailyBackupSchedule" action="${urls.projectManagerCreateBackupSchedule}" id="setupDailyBackupScheduleForm">
						<div style="float:left; padding-bottom: .8em; padding-right: 1em;">
						<input type="hidden" name="projectName" value="<c:out value="${projectInfo.project.name}" />" />
						Run daily backup at <form:select path="hour" items="${hours}" itemValue="number" itemLabel="name" />
						</div>  
						<div class="buttons">
							<button type="submit" value="Schedule Backup" >SCHEDULE BACKUP</button>
						</div>
					</form:form>

					<div class="status">
						No backups are currently scheduled to run
					</div>
				</c:when>
				<c:otherwise>
					<c:forEach items="${backupSchedules}" var="backupSchedule">
						<c:set var="dailyBackupSchedule" value="${backupSchedule}" scope="request"/>
						<form:form commandName="dailyBackupSchedule" action="${urls.projectManagerCreateBackupSchedule}" id="updateDailyBackupScheduleForm">
							<div style="float:left; padding-bottom: .8em; padding-right: 1em;">
							<input type="hidden" name="<%= SetupDailyBackupController.PROJECT_NAME_PARAMETER %>" value="<c:out value="${projectInfo.project.name}" />" />
							Run daily backup at <form:select path="<%= SetupDailyBackupController.HOUR_PARAMETER %>" items="${hours}" itemValue="number" itemLabel="name" />
							</div>
							<div class="buttons">  
								<button type="submit" value="Change Backup">CHANGE BACKUP</button> 
								<button type="submit" value="Stop Backups" name="<%= SetupDailyBackupController.STOP_PARAMETER %>">CANCEL BACKUP</button>
							</div>
						</form:form>
						<div class="status">
							Next backup will run on <fmt:formatDate value="${backupSchedule.nextBackupDate}" pattern="MMMM d"/> around <fmt:formatDate value="${backupSchedule.nextBackupDate}" pattern="h:mma z"/>
						</div>
					</c:forEach>
				</c:otherwise>
			</c:choose>
			<br />
			<c:choose>
			<c:when test="${empty onlineBackups}">
				<p>There are no online backups available to restore for this project.</p>
				<br />
			</c:when>
			<c:otherwise>
			Available backups:<br />
			<table class="list sortable ruler">
				<thead>
					<tr>
						<th>Date</th>
						<th>Project Version</th>
						<th>Number of records</th>
						<th>&nbsp;</th>
					</tr>
				</thead>
				<tbody>
					<c:forEach var="onlineBackup" items="${onlineBackups}" varStatus="backupIteration">
					<tr>
						<td align="left"><fmt:formatDate value="${onlineBackup.created}" pattern="yyyy/MM/dd hh:mma"/></td>
						<td>${onlineBackup.projectVersionNumber}</td>
						<td>${onlineBackup.recordCount}</td>
						<td><div class="controls" style="width: auto;">
								<div class="buttons">
									<button type="submit" id="restoreOnlineBackup${backupIteration.count}" value="${onlineBackup.id}" name="pmActionsRestoreFromOnlineBackup" title="Restore Project from This Backup">RESTORE</button> 		
									<form class="confirm" id="deleteOnlineBackup${backupIteration.count}" action="${urls.projectManagerDeleteOnlineBackup}">
										<input type="hidden" name="<%= DeleteOnlineBackupController.PARAMETER_BACKUP_ID %>" value="${onlineBackup.id}">
										<input type="hidden" name="<%= DeleteOnlineBackupController.PARAMETER_PROJECT_NAME %>" value="<c:out value="${projectInfo.project.name}" />">
										<input type="hidden" name="action" value="deleteBackup">
										<!-- button type="submit" name="deleteButton">Delete</button -->
										<input type="image" src="/images/silk/delete.gif" alt="Delete this backup"
													title="Delete this backup" class="smallIcon" />
									</form>
								</div>
							</div>
						</td>
					</tr>
					</c:forEach>
				</tbody>
			</table>
			<br />
			<form class="confirm" id="deleteOnlineBackup" action="${urls.projectManagerDeleteOnlineBackup}">
				<input type="hidden" name="<%= DeleteOnlineBackupController.PARAMETER_BACKUP_ID %>" value="<%= DeleteOnlineBackupController.DELETE_ALL %>">
				<input type="hidden" name="<%= DeleteOnlineBackupController.PARAMETER_PROJECT_NAME %>" value="<c:out value="${projectInfo.project.name}" />">
				<input type="hidden" name="<%= DeleteOnlineBackupController.PARAMETER_PROJECT_ID %>" value="<c:out value="${projectInfo.project.id}" />">
				<input type="hidden" name="action" value="deleteAllProjectBackups">
				<div class="buttons">
					<button type="submit" name="deleteButton">DELETE ALL PROJECT BACKUPS</button>
				</div>
			</form>
			
			</c:otherwise>
		</c:choose>
			

		</div>
	</div>
</div>
</c:if>
<c:url var="viewAllProjectEmailsURL" value="${urls.viewAllUserProjectEmails}">
	<c:param name="<%= ViewAllUserProjectEmailsController.PROJECT_ID_PARAMETER %>" value="${projectInfo.project.id}" />
</c:url>
<div class="section collapsible closed">
	<h3 class="sectionHeading">Project Emails</h3>
	<div class="sectionContent">
		<p><a id="viewAllProjectEmailsLink" href="${viewAllProjectEmailsURL}">View all project emails</a></p>
<c:if test="${user.administrator || originalUser.administrator}">
<c:url var="deleteAllProjectEmailsURL" value="${urls.deleteAllUserProjectEmails}">
	<c:param name="<%= DeleteAllProjectEmailsController.USER_PROJECT_ID_PARAMETER %>" value="${projectInfo.project.id}" />
</c:url>
		<p>
		<form class="confirm" name="deleteAllEmails">
			<input type="hidden" name="<%=ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME%>"	value="<c:out value="${projectInfo.project.name}" />" /> 
			<input type="hidden" name="formName" value="<c:out value="${projectInfo.project.name}" />" /> 
			<input type="hidden" name="action" value="<%=ViewProjectManagerDetailsController.PARAMETER_ACTION_DELETE_ALL_EMAILS%>" /> 
			<button type="submit" id="deleteAllEmails" name="pmActionsDeleteAllEmails" title="Delete All Project Emails">DELETE ALL PROJECT EMAILS</button>
		</form> 		
</c:if>	
	</div>
</div>

<c:if test="${! empty updateProjectDetailsForm.userProject.YSLLeagueId}">
<div class="section collapsible closed">
	<h3 class="sectionHeading">Integration with YourSportsLeague.com</h3>
	<div class="sectionContent">
		<p>Your YourSportsLeague.com's league id is <b><c:out value="${updateProjectDetailsForm.userProject.YSLLeagueId}"/></b>.</p>
		<c:choose>
			<c:when test="${empty updateProjectDetailsForm.userProject.YSLLastUpdated}">
				This project has never been synchronized.
			</c:when>
			<c:otherwise>
				This project was last synchronized on <fmt:formatDate pattern="MMMM dd, yyyy 'at' hh:mma z" value="${updateProjectDetailsForm.userProject.YSLLastUpdated}"/>.
			</c:otherwise>
		</c:choose>
		<p>
		
		<form action="${urls.updateYourSportsLeagueData}">
			<input type="hidden" name="<%= UpdateYourSportsLeagueDataController.PROJECT_ID_PARAMETER %>" value="${updateProjectDetailsForm.userProject.id}"/>
			<div class="buttons"> 
				<button type="submit">Update YourSportsLeague.com with teams and coaches data from this project</button>
			</div>
		</form>
		</p>
	</div>
</div>
</c:if>

 
<c:if test="${user.administrator || originalUser.administrator}">
<div id="project-properties" class="section collapsible closed">
	<h3 class="sectionHeading">
		Additional Project Details
	</h3>
	<div class="sectionContent">
		<div>
		<c:url var="updateProjectURL" value="${urls.projectManagerUpdateProjectDetails}">
			<c:param name="<%=UpdateProjectDetailsController.PROJECT_ID_PARAMETER %>" value="${projectInfo.project.id}" />
		</c:url>
		<form:form action="${updateProjectURL}" commandName="updateProjectDetailsForm" method="POST">
			<div class="note">Make sure to use mm/dd/yyyy format for dates.</div>
			<p>
			<label class="bold block">Require SSL</label>
			<form:checkbox path="userProject.requireSSL" /> 
			<p>
			<label class="bold block">Project type</label> 
			<form:select path="userProject.projectType">
            	<form:option value="">Not a Sports Dashboard</form:option>
            	<form:option value="<%= Constants.PROJECT_TYPE %>">Sports Dashboard</form:option>
          	</form:select>
			</p>
			<p>
			<label class="bold block">Registration Start Date</label> 
			<input type="text" name="userProject.registrationStartDate" 
				value="<fmt:formatDate value="${updateProjectDetailsForm.userProject.registrationStartDate}" pattern="MM/dd/yyyy"/>" />
			</p>
			
			<p>
			<label class="bold block">Registration End Date</label> 
			<input type="text" name="userProject.registrationCloseDate" 
				value="<fmt:formatDate value="${updateProjectDetailsForm.userProject.registrationCloseDate}" pattern="MM/dd/yyyy"/>" />
			</p>

			<p>				
			<label class="bold block">Purchase Order Number</label>
			<input type="text" name="userProject.purchaseOrderNumber" 
				value="<c:out value="${updateProjectDetailsForm.userProject.purchaseOrderNumber}"/>" />
			</p>

			<p>				
			<label class="bold block">Invoice Number</label>
			<input type="text" name="userProject.invoiceNumber" 
				value="<c:out value="${updateProjectDetailsForm.userProject.invoiceNumber}"/>" />
			</p>
			
			<p>				
			<label class="bold block">Invoice Date</label>
			<input type="text" name="userProject.registrationInvoiceDate" 
				value="<fmt:formatDate value="${updateProjectDetailsForm.userProject.registrationInvoiceDate}" pattern="MM/dd/yyyy"/>" />
			</p>
				
			<p>
			<label class="bold block">Cost per player</label>
			<input type="text" name="userProject.registrationFee" 
				value="<fmt:formatNumber value="${updateProjectDetailsForm.userProject.registrationFee}" pattern="###0.00"/>" />
			</p>

			<p>				
			<label class="bold block">YourSportsLeague.com League Id</label>
			<input type="text" name="userProject.YSLLeagueId" 
				value="<c:out value="${updateProjectDetailsForm.userProject.YSLLeagueId}"/>" />
			</p>

			<p>
			<label class="bold block">Roster Export Template</label>
			<form:select path="userProject.teamRosterTemplateId">
            	<form:option value="">No Template (link will not appear)</form:option>
            	<form:options items="${teamRosterTemplateIds}"  itemValue="id" itemLabel="description" />
          	</form:select>
          	</p>

			<div class="buttons">
				<button type="submit">UPDATE</button>
			</div>
		</form:form>
		</div>
	</div>
</div>
</c:if>

<!-- New IFRAME based dialog -->
<div id="dialogIFrame" style="text-align: left;" class="yui-skin-sam">
	<div class="hd"><div class="tl"></div><span id="dialogTitle">Publish Project to the Community Library</span><div class="tr"></div></div>
	<div class="bd">
		<div id="dialogContentIFrame" class="content">
			<div id="publishSelectContentIF" style="display:none;">
				<div class="section">
					<p>
					You have choosen to publish the project <b>${projectInfo.project.name}</b> to the Community Library.
					<br /><br />
					You may either publish this project as a new project in the Library or update a project that already exist.
					</p>
				</div>
				<br />
				<div class="buttons">
					<button 
						value="" id="publishNewIFrame"
						title="Copy this app to the library to make it available to other designers">
						Publish as a New Project to the library
					</button>
						
					<button value="" id="publishAsVersionIFrame" 
							title ="Update an app in the library with this version">
						Update an Existing Library Project
					</button>
				</div>
			</div>

			<div id="restoreOnlineBackupIF" style="display:none;">
				<div id="restoreOnlineBackupIFrame"></div>
			</div>

			<div id="publishNewContentIF" style="display:none;">
				<div id="publishNewContentIFrame"></div>
			</div>

			<div id="publishAsVersionContentIF" style="display:none;">
				<div id="publishAsVersionContentIFrame"></div>
			</div>

			<div id="addNewCategoryContentIF" style="display:none;">
				<div id="addNewCategoryContentIFrame"></div>
			</div>

			<div id="exportAllContentIF" style="display:none;">
				<div id="exportAllContentIFrame"></div>
			</div>

			<div id="importAllContentIF" style="display:none;">
				<div id="importAllContentIFrame"></div>
			</div>

			<div id="backupContentIF" style="display:none;">
				<div id="backupContentIFrame"></div>
			</div>

			<div id="restoreContentIF" style="display:none;">
				<div id="restoreContentIFrame"></div>
			</div>

			<div id="deleteAllEmailsIF" style="display:none;">
				<div id="deleteAllEmailsIFrame"></div>
			</div>
		</div>
	</div>
	<div class="ft"></div>
</div>

<c:url var="linkToPublishAsNewProject" value="${urls.projectManagerPublish}">
		<c:param name="<%=PublishProjectToLibraryController.PARAMETER_PROJECT_ID%>" 
				value="${projectInfo.project.name}" />
</c:url>
<c:url var="linkToPublishAsVersion" value="${urls.libraryAddProjectVersionFromUnreleatedProject}">
		<c:param name="<%=AddVersionToUnrelatedProjectController.PARAMETER_PROJECT_ID%>" 
				value="${projectInfo.project.name}" />
</c:url>
<c:url  var="exportAllURL" value="${urls.projectManagerExportAllDataDescription}">
	<c:param name="<%=DisplayExportAllDataDescriptionController.PROJECT_ID_PARAMETER%>" value="${projectInfo.project.id}"/>
</c:url>
<c:url var="backupURL" value="${urls.projectManagerBackupDescription}" >
	<c:param name="<%=DisplayBackupDescriptionController.PROJECT_ID_PARAMETER%>" value="${projectInfo.project.id}"/>
</c:url>
<c:url var="restoreAllDataURL" value="${urls.projectManagerRestoreAllData}">
	<c:param name="<%=AllDataImportController.PROJECT_ID_PARAMETER%>" value="${projectInfo.project.id}"/>
</c:url>
<c:url var="restoreURL" value="${urls.projectManagerRestoreProjectFromBackup}">
</c:url>
<c:url var="restoreOnlineBackupURL" value="${urls.projectManagerRestoreProjectFromOnlineBackup}">
	<c:param name="<%=OnlineBackupRestoreController.PARAMETER_PROJECT_ID %>" value="${projectInfo.project.id}" />
	<c:param name="<%=OnlineBackupRestoreController.PARAMETER_BACKUP_ID %>" value="" />
</c:url>
<c:url var="storeShowFormsStateURL" value="${urls.saveHideSelectedFormsState}" >
	<c:param name="<%= SaveHideSelectedFormsStateController.PROJECT_ID_PARAMETER %>" value="${projectInfo.project.id}"/>
	<c:param name="<%= SaveHideSelectedFormsStateController.STATUS_PARAMETER %>" value="" />
</c:url>

<script type="text/javascript">
<!--
var linkToPublishAsNew = '<spring:escapeBody javaScriptEscape="true">${linkToPublishAsNewProject}</spring:escapeBody>';
var linkToPublishAsVersion = '<spring:escapeBody javaScriptEscape="true">${linkToPublishAsVersion}</spring:escapeBody>';
var linkToExportAll = '${exportAllURL}';
var linkToImportAll = '${restoreAllDataURL}';
var linkToBackup = '${backupURL}';
var linkToRestore = '${restoreURL}';
var linkToRestoreOnlineBackupURL = '${restoreOnlineBackupURL}';

var isAdmin = ${user.administrator};
var userProjectId = ${projectInfo.project.id};
var showSelectedFormsOnlyVar = ${showSelectedFormsOnly};
var storeShowFormsStateURL = '${storeShowFormsStateURL}';
-->
</script>

