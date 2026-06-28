<%@ page contentType="text/html" %>
<%@ page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>


<c:if test="${! empty user && empty domain.title}">
	<div class="block" style="overflow: hidden; overflow-x: hidden; overflow-y: hidden;">
		<div class="content">
			<h3>My Project List</h3>
			<div>
		 		<c:choose>
		 			<c:when test="${projectCount == 0}">
			    		You have no projects deployed
			    	</c:when>
				    <c:when test="${projectCount == 1 }">
				    	You have 1 project deployed
			    	</c:when>
		 			<c:otherwise>
		 				<c:if test="${projectCount > 10 }">
							<a href="/projectmanager/view" title="Click here to go to My Tawala">
								<b>Go to My Tawala</b>
							</a>
							<br /><br />
						</c:if>
		   				You have ${projectCount} projects deployed
			   		</c:otherwise>
		   		</c:choose>
				<table class="blockProjectList ruler">
					<tbody>		
					<c:forEach var="projectInfo" items="${projectsInfo}" varStatus="status" end="${user.preferences.numberOfProjectsInBlock - 1}">
						<c:url var="linkToProjectDetails" value="${urls.projectManagerProjectDetailView}">
							<c:param name="<%= ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME %>" value="${projectInfo.project.name}" />
						</c:url>
						<tr class="link" onclick="window.location='${linkToProjectDetails}'; return false;">
							<td class="name left">
								<div>
								<a href="${linkToProjectDetails}" title="View details for project: <c:out value='${projectInfo.project.name}'/>" >
									<c:out value="${projectInfo.project.name}" />
								</a>
								</div>
							</td>
							<td class="count">
								<a href="${linkToProjectDetails}" title="# of responses">${projectInfo.responses}</a>
							</td>
						</tr>
					</c:forEach>
					</tbody>
				</table>
					<c:if test="${user.preferences.numberOfProjectsInBlock < projectCount}">
						<a href="${urls.projectManagerView}" title="See more projects in My Tawala">more ...</a>
					</c:if>
		  	</div>
			<br />
			<a href="${urls.projectManagerView}" title="Click here to go to My Tawala">
				<b>Go to My Tawala</b>
			</a>
		</div>
	</div>
</c:if>