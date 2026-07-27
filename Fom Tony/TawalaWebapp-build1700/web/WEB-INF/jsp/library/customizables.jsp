
<jsp:directive.page import="com.tawala.web.library.OneClickTestDriveController"/>
<jsp:directive.page import="com.tawala.web.library.CloneAndCustomizeController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<script>
	setPageTitle("Customizable Applications");
</script>
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

								<c:if test="${project.vetted && project.latestVersion.project.customizable}">
									<c:url var="customizationUrl" value="${urls.libraryCustomizeAndDeploy}">
										<c:param name="<%=CloneAndCustomizeController.PARAMETER_PROJECT_ID%>" value="${project.id}" />
									</c:url>
									<a href="${customizationUrl}" id="startCustomization">USE IT</a>
									<br />
								</c:if>
								
						</div>
					</td>
				</tr>
		</c:forEach>
			</tbody>
		</table>
	</div>
</c:forEach>
</div>
	
