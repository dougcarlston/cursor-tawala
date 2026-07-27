<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>

<tiles:importAttribute name="subName" />

<c:choose>
	<c:when test="${empty(subName) || subName == 'myprojects'}">
		<c:set var="main" value="selected" />
	</c:when>
	<c:when test="${subName == 'myaccount'}">
		<c:set var="account" value="selected" />
	</c:when>
	<c:when test="${subName == 'password'}">
		<c:set var="password" value="selected" />
	</c:when>
</c:choose>
	<div class="sub-menu">
	     <ul>
	    	<li><a class="<c:out value="${main}" default="none" />" href="${urls.projectManagerView}" title="Manage your projects" >My Projects</a></li>
	    	<li><a class="<c:out value="${account}" default="none" />" href="${urls.userAccountUpdate}" title="Edit your accounts settings" >My Account</a></li>
	    	<li><a class="<c:out value="${password}" default="none" />" href="${urls.userPasswordChange}" title="Change your password" >Change Password</a></li>
	     </ul>
	</div>
