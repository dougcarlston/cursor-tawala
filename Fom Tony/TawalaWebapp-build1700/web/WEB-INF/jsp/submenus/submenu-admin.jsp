<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>

<tiles:importAttribute name="subName" />

<c:choose>
	<c:when test="${subName == 'manageusers'}">
		<c:set var="manageUsersMenuStyle" value="selected" />
	</c:when>
	<c:when test="${subName == 'managelibrary'}">
		<c:set var="manageLibraryMenuStyle" value="selected" />
	</c:when>
</c:choose>
	<div class="sub-menu">
	     <ul>
	         <li><a class="<c:out value="${manageUsersMenuStyle}" default="none" />" href="${urls.adminManageUsers}">Manage Users</a></li>
	         <li><a class="<c:out value="${manageLibraryMenuStyle}" default="none" />" href="${urls.adminManageLibrary}">Manage Library</a></li>
	     </ul>
	</div>
