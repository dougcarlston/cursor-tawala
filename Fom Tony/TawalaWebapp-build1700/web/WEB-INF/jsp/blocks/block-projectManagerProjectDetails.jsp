<%@ page contentType="text/html" %>
<%@ page import="com.tawala.web.library.PublishProjectToLibraryController" %>
<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<%@ page import="com.tawala.web.library.AddVersionToRelatedProjectController" %>
<%@ page import="com.tawala.web.library.AddVersionToUnrelatedProjectController" %>
<jsp:directive.page import="com.tawala.web.projectmanager.HowToIncludeInAnotherWebSiteController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.DisplayInvitationController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.ExistingProjectCustomizationController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.ChangeOnlineStatusController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.UpgradeWithNewerVersionController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

	<div class="block">
		<div class="content">
			<div>
				<span class="label">Current Deployed Version:</span>
				<span>${projectInfo.project.deployedVersion.versionNumber}</span>
			</div>
			<div>
				<span class="label">Last Deployed:</span>
				<span><fmt:formatDate value="${projectInfo.project.deployedVersion.date}" pattern="yyyy-MM-dd" /></span>
			</div>
			<div>
				<span class="label">Last Updated:</span>
				<span><fmt:formatDate value="${projectInfo.project.lastUpdatedDate}" pattern="yyyy-MM-dd" /></span>
			</div>
			<div>
				<span class="label">Last Used:</span>
				<span>
					<c:choose>
						<c:when test="${empty lastDataRecordedDate}">never</c:when>
						<c:otherwise><fmt:formatDate value="${lastDataRecordedDate}" pattern="yyyy-MM-dd hh:mm" /></c:otherwise>
					</c:choose>
				</span>
			</div>

			<c:choose>
				<c:when test="${user.status.allowedToUpdateLibraryProjects}">
					<c:set var="divDisplayStyle" value="block" />
				</c:when>
				<c:otherwise>
					<c:set var="divDisplayStyle" value="none" />
				</c:otherwise>
			</c:choose>

			<div style="display:${divDisplayStyle};">
				<span class="label">Published:</span>
				<c:url var="linkToProjectLibraryDetails" value="${urls.libraryProjectDetailView}">
						<c:param name="<%=ViewProjectDetailsController.PARAMETER_ID%>" value="${projectInfo.project.libraryProjectId}" />
				</c:url>
				<c:choose>
					<c:when test="${! empty projectInfo.submittedProject}">
						<span>Yes</span><br />
						<div class="publish">
							<span class="label">Published as:</span>
							<br /> 
							<a class="action" href="${linkToProjectLibraryDetails}" title="View this project in the Library"><b><c:out value="${projectInfo.submittedProject.name}" /></b></a>
							<c:if test="${projectInfo.libraryHasNewerVersion}">(Project Library has newer version)</c:if>
						</div>
						<br />
					</c:when>
					<c:otherwise>
						<span>No</span><br /><br />
					</c:otherwise>
				</c:choose>
			</div>
			
			<c:if test="${thereIsNewerLibraryVersion}">
				<c:url var="upgradeURL" value="${urls.projectManagerUpgradeWithNewerVersionFromLibrary}">
					<c:param name="<%= UpgradeWithNewerVersionController.PARAMETER_PROJECT_ID %>" value="${projectInfo.project.id}" />
				</c:url>
			<div>
				The library has a newer version of the original project "<c:out value="${originalLibraryVersion.libraryProject.name}" />".
				<a href="${upgradeURL}" id="upgradeToNewerLibraryVersionLink">more ...</a>
			</div>
			<br class="clr"/> <br />
			</c:if>

			<c:if test="${projectInfo.project.project.customizable}">
				<div class="buttons">
					<c:url var="customizationUrl" value="${urls.customizeExistingProject}">
						<c:param name="<%=ExistingProjectCustomizationController.PARAMETER_PROJECT_NAME%>" value="${projectInfo.project.name}"/>
					</c:url>
					<a class="dark" href="${customizationUrl}" id="customizationLink">REVISE PROJECT</a>
				</div>
				<br class="clr"/> <br />
			</c:if>

<script>
	<c:choose>
		<c:when test="${projectInfo.project.offline}">
			<c:set var="onlineSectionDisplayStyle" value="none"/>
			<c:set var="offlineSectionDisplayStyle" value="inline"/>
			var offline = true;
		</c:when>
		<c:otherwise>
			<c:set var="onlineSectionDisplayStyle" value="inline"/>
			<c:set var="offlineSectionDisplayStyle" value="none"/>
			var offline = false;
		</c:otherwise>
	</c:choose>
</script>
			<div id="projectStatus">
				<div id="onlineStatusBusyContainer"></div>
				<div class="label">Current Project Status</div>
				<div id="onlineStatus">
					<span style="display: ${onlineSectionDisplayStyle};" id="onlineSection">
					<img src="/images/silk/accept.png" alt="Active"
												title="Project is Active"
												class="smallIcon" /> Active 
					</span>
					<span style="display: ${offlineSectionDisplayStyle};" id="offlineSection">
						<img src="/images/silk/stop.png" alt="Inactive"
												title="Project is Inactive"
												class="smallIcon" /> Inactive 
					</span>
				</div>
				<div id="onlineButton" class="buttons" style="display: ${offlineSectionDisplayStyle};">
					<a class="dark" onclick="Tawala.ProjectManager.projectStatus.takeOffline(false);return false;" href="#">TAKE PROJECT ONLINE</a>
				</div>
				<div id="offlineButton" class="buttons" style="display: ${onlineSectionDisplayStyle};">
					<a class="dark" onclick="Tawala.ProjectManager.projectStatus.takeOffline(true);return false;" href="#">TAKE PROJECT OFFLINE</a>
				</div>
				<form id="changeOnlineStatusForm" action="${urls.projectManagerChangeOnlineStatus}">
					<input type="hidden" name="<%=ChangeOnlineStatusController.PARAMETER_PROJECT_ID%>" value="${projectInfo.project.id}"/>
					<input type="hidden" name="<%=ChangeOnlineStatusController.PARAMETER_TAKE_OFFLINE%>" value=""/>
				</form>
			</div>
			<br />
			
			<c:url var="linkToHowToIncludeInAnotherWebPage" value="${urls.webpageCode}">
				<c:param name="<%=HowToIncludeInAnotherWebSiteController.PROJECT_NAME_PARAMETER%>" value="${projectInfo.project.name}" />
			</c:url>
			<a href="${linkToHowToIncludeInAnotherWebPage}">
				Include Project in Web Page <img src="/images/red-bullet-arrow-right.gif" />
			</a>
		
			<br /><br />

			<c:url var="linkInvite" value="${urls.projectManagerInvite}">
				<c:param name="<%=DisplayInvitationController.PROJECT_NAME_PARAMETER%>" value="${projectInfo.project.name}" />
			</c:url>
			<div>
				Copy the appropriate Project Link into your emails or <a id="linkToInvitation" href="${linkInvite}">click here</a> for other ways to 
				invite clients and members to use this Project.	
			</div>

		</div>
	</div>
