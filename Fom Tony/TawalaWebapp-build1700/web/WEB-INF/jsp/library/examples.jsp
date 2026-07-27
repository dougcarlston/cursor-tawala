
<jsp:directive.page import="com.tawala.web.library.OneClickTestDriveController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div id="categoryContainer">
<c:forEach var="mapEntry" items="${projectsMap}" varStatus="iterationByCategoryStatus">
	<c:set var="category" value="${mapEntry.key}" />
	<c:set var="projects" value="${mapEntry.value}" />
	<div id="${category.name}" class="category">
		<h2 class="sectionHeading"><c:out value="${category.name}" /></h2>
		<p>
			<c:out value="${category.description}" />
		</p>
		<table class="appsList">
			<tbody>
		<c:forEach var="project" items="${projects}">
				<tr class="odd">
					<td>
						<div class="app">
							<img class="icon" src="${project.iconURL}" alt="<c:out value="${project.name}" />" />
							<div class="title"><c:out value="${project.name}"/></div>
							<p class="description">
								<c:out value="${project.longDescription}"/>
							</p>
							<div class="buttons">
								<c:if test="${! empty project.videoURL}">				
								<a href="#" onclick="window.open('${project.videoURL}','Video','width=820,height=590,toolbar=no,menubar=no,status=no,location=no,resizeable=yes,scrollbars=yes'); return false;">
									VIEW DEMO
								</a>
								</c:if>
	
								<c:set var="oneClickTestDriveUrl" value=""/>
								<c:forEach var="form" items="${project.latestVersion.project.forms}" varStatus="status">
									<c:if test="${form.startingPoint}">
										<c:url var="oneClickTestDriveUrl" value="${urls.libraryOneClickTestDrive}">
											<c:param name="<%=OneClickTestDriveController.PROJECT_ID_PARAMETER_NAME%>" 
												value="${project.id}" />
											<c:param name="<%=OneClickTestDriveController.VERSION_ID_PARAMETER_NAME%>"
												value="${project.latestVersion.id}" />
											<c:param name="<%=OneClickTestDriveController.FORM_NAME_PARAMETER%>"
												value="${form.name}" />
										</c:url>
									</c:if>
								</c:forEach>
								<a id="oneClickTestDriveLink" href="${oneClickTestDriveUrl}" target="testdrive">TRY IT</a>
						</div>
					</td>
				</tr>
		</c:forEach>
			</tbody>
		</table>
	</div>
</c:forEach>
</div>
	
