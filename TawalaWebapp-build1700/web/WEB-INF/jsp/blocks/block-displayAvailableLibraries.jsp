<jsp:directive.page import="com.tawala.web.library.LibrarySearchController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div  class="block">
	<div class="content">
		<h3>Select a Library</h3>

		<ul class="menuList" id="communityMenu">
			<c:forEach var="library" items="${availableLibraries}">
				<c:choose>
					<c:when test="${library == currentLibrary}">
						<c:set var="libraryLinkStyle" > class="selected"</c:set>
					</c:when>
					<c:otherwise>
						<c:set var="libraryLinkStyle" value="" />
					</c:otherwise>
				</c:choose>

				<c:url var="viewDifferentLibraryUrl" value="${urls.librarySearch}">
					<c:param name="<%= LibrarySearchController.LIBRARY_ID_PARAMETER %>" value="${library.id}" />
				</c:url>
				<li>
					<a href="${viewDifferentLibraryUrl}" ${libraryLinkStyle} id="selectLibraryLink${library.id}" title="<c:out value="${library.description}" />"><c:out value="${library.description}" /></a>
				</li>
			</c:forEach>
		</ul>

	</div>
</div>
