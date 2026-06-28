<%@ page import="com.tawala.web.library.ModifyProjectController" %>
<%@ page import="com.tawala.web.library.ViewProjectHistoryController" %>
<%@ page import="com.tawala.web.library.DeleteCommentController" %>
<%@ page import="com.tawala.web.library.DownloadLibraryProjectVersionController" %>
<%@ page import="com.tawala.web.library.DeleteVersionController" %>
<jsp:directive.page import="com.tawala.web.library.TestDriveWithExplanationController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<script>
	setPageTitle("Project Details");
</script>

	<div class="section" id="projectDetails">		
		<c:if test="${! empty project.iconURL }">
			<img class="projectIcon" src="${project.iconURL}" alt="" />
		</c:if>
		<h1>
			${project.name}
		</h1>
		<div id="description" class="section">
			<h3 class="sectionHeading">Description</h3>
			<div class="sectionContent">
				<div>${project.shortDescription}</div>
				<br />
				<div>
					<div class="label">Detailed Description</div>
					<% pageContext.setAttribute("newline", "\n"); %>
					${fn:replace(project.longDescription, newline, '<br />')}					
				</div>
			</div>
		</div>
	</div>
	
	<c:if test="${! empty project.snapshotTile}">			
		<jsp:include flush="true" page="${project.snapshotTile}" />
	</c:if>

	<div id="comments" class="section collapsible">
		<h3 class="sectionHeading">Comments and Ratings</h3>
		<div class="sectionContent">
			<c:choose>
				<c:when test="${! empty project.comments}">
					<table class="list ruler">
						<col style="width: 60px;" />
						<col style="width: 520px;" />
						<col style="width: 80px;" />
						<tbody>
							<c:forEach var="comment" items="${project.comments}" varStatus="status">
								<tr style="vertical-align: top;">
									<td><fmt:formatDate value="${comment.date}" type="date" dateStyle="short" /></td>
									<td class="left">
										<div><c:out value="${comment.userId}" default="anonymous" /></div>
										<div><c:out value="${comment.text}" /></div>
									</td>
									<c:if test="${! empty user}">
									<td>
										<div class="controls">
											<form method="post" class="confirm" action="${urls.libraryDeleteComment}" onclick="return false;">
												<input type="hidden" name="<%=DeleteCommentController.COMMENT_ID_PARAMETER_NAME %>" value="<c:out value="${comment.id}" />" /> 
												<input type="hidden" name="<%=DeleteCommentController.PROJECT_ID_PARAMETER_NAME %>" value="<c:out value="${project.id}" />" /> 
												<input type="hidden" name="action" value="deletecomment" /> 
												<input type="image" name="action" value="deletecomment" src="/images/delete-icon.gif" alt="Delete" title="Delete this comment" class="smallIcon" />
											</form>
										</div>
									</td>
									</c:if>
					    		</tr>
							</c:forEach>
						</tbody>
					</table>
				</c:when>
				<c:otherwise>
					<p>No comments yet</p>
				</c:otherwise>
			</c:choose>
			<br />
			<div>
				<c:if test="${! empty user}">
				<form action="${urls.libraryEditComment}" id="editCommentForm">
					<input type="hidden" name="<%= ModifyProjectController.PARAMETER_PROJECT_ID %>" value="${project.id}" />
					<input type="image" value="Add a comment" title="Add a comment" src="/images/addcomment-button.gif" />
				</form>
				</c:if>
			</div>
		</div>
	</div>

	<c:if test="${user.status.allowedToViewDesigner}">
		<div id="versions" class="section collapsible closed">
			<h3 class="sectionHeading">Project Versions</h3>
			<div class="sectionContent">
				<c:if test="${empty user}">
					<div class="note">
					To download this project you need to <a href="${urls.userRegistration}">become a registered user</a> or 
						<a href="/login">log in</a>.
						<br />
					</div>
				</c:if>
				<table class="list ruler">
					<col style="width: 60px;" />
					<col style="width: 40px;" />				
					<col style="width: 50px;" />
					<col style="width: 240px;" />
					<col style="width: 90px;" />
					<thead>
						<tr>
							<th class="left">Date</th>
							<th>Version</th>
							<th>User</th>
							<th class="left">Notes</th>
							<th></th>
						</tr>
					</thead>
					<tbody>
					<c:forEach var="version" items="${project.versions}">
						<tr>
							<td class="left"><fmt:formatDate value="${version.date}" type="date" dateStyle="short" /></td>
							<td>v.${version.versionNumber}</td>
							<td><c:out value="${version.userId}" /></td>
							<td class="left"><c:out value="${version.text}" /></td>
							<td>
								<div  class="controls">
									<c:if test="${! empty user}">
										<c:url var="versionSampleDataLink" value="${urls.libraryViewSampleData}" >
											<c:param name="<%=DownloadLibraryProjectVersionController.PROJECT_ID_PARAMETER_NAME%>" 
												value="${project.id}" />
											<c:param name="<%=DownloadLibraryProjectVersionController.VERSION_ID_PARAMETER_NAME%>" 
												value="${version.id}" />
										</c:url>
									
										<c:url var="versionDownloadLink" value="${urls.libraryProjectVersionDownload}" >
											<c:param name="<%=DownloadLibraryProjectVersionController.PROJECT_ID_PARAMETER_NAME%>" 
												value="${project.id}" />
											<c:param name="<%=DownloadLibraryProjectVersionController.VERSION_ID_PARAMETER_NAME%>" 
												value="${version.id}" />
										</c:url>
		
										<a href="${versionDownloadLink}" id="downloadVersion${version.id}"><img src="/images/download-icon.gif" title="Download this version of the project" class="smallIcon" /></a>
		
										<form method="post" class="confirm" onclick="return false;" action="${urls.libraryDeleteProjectVersion}">
											<input type="hidden" name="<%=DeleteVersionController.PROJECT_ID_PARAMETER_NAME%>" value="${project.id}" /> 
											<input type="hidden" name="<%=DeleteVersionController.VERSION_ID_PARAMETER_NAME%>" value="<c:out value="${version.id}" />" /> 
											<input type="hidden" name="action" value="deleteprojectversion" /> 
											<input type="image" name="action" value="deleteprojectversion" src="/images/delete-icon.gif" alt="Delete" title="Delete this version" class="smallIcon" />
										</form>	
		
										<a href="${versionSampleDataLink}" id="sampleData${version.id}"><img src="/images/silk/table_row_insert.gif" title="Add sample data" class="smallIcon" /></a>
		
									</c:if>
		
									<c:url var="versionTestDriveLink" value="${urls.libraryTestDrivePreparation}">
												<c:param name="<%=TestDriveWithExplanationController.PROJECT_ID_PARAMETER_NAME%>" 
													value="${project.id}" />
												<c:param name="<%=TestDriveWithExplanationController.VERSION_ID_PARAMETER_NAME%>"
													value="${version.id}" />
									</c:url>
									<a href="${versionTestDriveLink}" id="testDriveVersion${version.id}"><img src="/images/testdrive-icon.gif" title="Test drive this version of the project" class="smallIcon" /></a>
								</div>
							</td>
						</tr>
					</c:forEach>
					</tbody>
				</table>
				<br />
				<div>
					<form action="${urls.libraryProjectHistory}" id="projectHistoryForm">
						<input type="hidden" name="<%= ViewProjectHistoryController.PARAMETER_PROJECT_ID %>" value="${project.id}" />
						<input type="image" value="Project History" src="/images/history-button.gif" />
					</form>
				</div>
			</div>
		</div>	
	</c:if>
		
	
