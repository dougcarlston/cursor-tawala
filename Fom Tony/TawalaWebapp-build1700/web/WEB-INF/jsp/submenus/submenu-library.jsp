<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>

<tiles:importAttribute name="subName" />

<c:choose>
	<c:when test="${empty(subName) || subName == 'main'}">
		<c:set var="main" value="selected" />
	</c:when>
	<c:when test="${subName == 'search'}">
		<c:set var="searchTabStyle" value="selected" />
	</c:when>
	<c:when test="${subName == 'categories'}">
		<c:set var="categoryTabStyle" value="selected" />
	</c:when>
	<c:when test="${subName == 'changes'}">
		<c:set var="changesTabStyle" value="selected" />
	</c:when>
</c:choose>
	<div class="sub-menu">
	     <ul>
	    	<li><a class="<c:out value="${searchTabStyle}" default="none" />" href="${urls.librarySearch}" title="Search the library for projects" >Search</a></li>
	    	<c:if test="${! empty user}">
		    <li><a class="<c:out value="${categoryTabStyle}" default="none" />" href="${urls.libraryCategories}" title="View and edit categories">Edit Categories</a></li>
		    </c:if>
		    <li><a class="<c:out value="${changesTabStyle}" default="none" />" href="${urls.libraryRecentChanges}" title="View recent changes">Recent Changes</a></li>
	     </ul>
	</div>
