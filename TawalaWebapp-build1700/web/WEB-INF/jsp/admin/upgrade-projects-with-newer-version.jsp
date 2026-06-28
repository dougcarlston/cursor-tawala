<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.admin.ViewUserDetailController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<h2 class="sectionHeading">Update Projects</h2>
	<div class="sectionContent">
		<c:choose>
			<c:when test="${empty projects}">
		There are no projects that need to be upgraded.
			</c:when>
			<c:otherwise>
		<form:form commandName="upgradeForm" id="upgrade-projects-form">
	    <table class="list sortable ruler">
	        <colgroup>
	            <col class="controls"/>
	            <col class="userId"/>
	            <col class="projectName"/>
	            <col class="projectInLibrary"/>
	            <col class="currentVersion"/>
	            <col class="lastestProjectVersion"/>
	        </colgroup>
	        <thead>
	            <tr>
	            <th>&nbsp;</th>
	            <th>User</th>
	            <th>Project Name</th>
	            <th>Library Project</th>
	            <th>Current Version</th>
	            <th>Latest Version</th>
	            <th></th>
	            </tr>
	        </thead>
	        <tbody class="left">
	        <c:forEach var="projectMapEntry" items="${projects}">
	        	<c:set var="project" value="${projectMapEntry.key}"/>
	        	<c:set var="libraryVersion" value="${projectMapEntry.value}"/>
	
	        	<c:url var="viewUserDetailUrl" value="${urls.adminViewUserInfo}">
	        		<c:param name="<%= ViewUserDetailController.USER_ID_PARAMETER %>" value="${project.user.databaseId}" />
	        	</c:url>
	            <tr>
	            	<td><input type="checkbox" name="projectIds" value="${project.id}"/></td>
	                <td class="id"><a href="${viewUserDetailUrl}" id="linkToUserDetails${project.user.databaseId}"><c:out value="${project.user.id}" /></a></td>
	                <td ><c:out value="${project.name}"/></td>
	                <td ><c:out value="${libraryVersion.libraryProject.name}"/></td>
	                <td ><c:out value="${libraryVersion.versionNumber}"/></td>
	                <td ><c:out value="${libraryVersion.libraryProject.latestVersion.versionNumber}"/></td>
	            </tr>
	        </c:forEach>
	           </tbody>
	       </table>
	       <br />
	       <button type="submit">Upgrade Selected Projects</button>
	       </form:form>
			</c:otherwise>
		</c:choose>
	</div>
</div>
