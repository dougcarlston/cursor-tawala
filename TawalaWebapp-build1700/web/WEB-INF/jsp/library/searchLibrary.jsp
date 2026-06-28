<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<jsp:directive.page import="com.tawala.web.library.LibrarySearchController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<div id="library" >
    <!-- start search -->
    <div id="search">
    	<span class="category">
	    	<h3>
				<c:choose>
					<c:when test="${! empty selectedCategory}" >
						All projects in "<c:out value="${selectedCategory.name}" />" category
					</c:when>
					<c:when test="${! empty query}" >
			    		Search results for "<c:out value="${query}" />"
		    		</c:when>
					<c:otherwise>		    		
			    		All projects
			    	</c:otherwise>
			    </c:choose>
	    	</h3>
    	</span>
        <form method="get" id="searchForm">
			<input type="hidden" name="<%= LibrarySearchController.LIBRARY_ID_PARAMETER %>" value="${currentLibrary.id}" />
           <span class="searchInput">
   	            <input class="text" type=text name="query" id="query" value="<c:out value="${query}" />" size="32" />
       	        <input class="button" type="submit" value="Search">
           </span>
        </form>
    </div>
    <!-- end search -->

       <table class="projectListing sortable stripe ruler clr">
           <colgroup>
               <col class="info"/>
               <col class="rating"/>
               <c:if test="${currentLibrary.showCloneCount}">
               <col class="cloneCount" />
               </c:if>
               <col class="comments"/>
               <col class="updated"/>
           </colgroup>
           <thead>
               <tr>
                   <th class="left">Name</th>
                   <th class="left">Rating</th>
                <c:if test="${currentLibrary.showCloneCount}">
   	            <th>Times used</th>
       	        </c:if>
                   <th>Comments</th>
                   <th class="left">Updated</th>
               </tr>
           </thead>
           <tfoot>
               <tr>
               <td></td>
               <td></td>
               <c:if test="${currentLibrary.showCloneCount}">
               <td></td>
               </c:if>
               <td></td>
               <td></td>
               </tr>
           </tfoot>
           <tbody>
			<c:forEach var="project" items="${projects}" varStatus="status">
                <tr class="link" onclick="window.location='${urls.libraryProjectDetailView}?<%= ViewProjectDetailsController.PARAMETER_ID %>=${project.id}'; return false;">
					<c:choose>
						<c:when test="${project.underConstruction}">
							<c:set var="underConstruction" value="construction" />
						</c:when>
						<c:otherwise>
							<c:set var="underConstruction" value="" />
						</c:otherwise>
					</c:choose>
					<c:choose>
						<c:when test="${project.vetted}">
							<c:set var="vetted" value="vetted" />
						</c:when>
						<c:otherwise>
							<c:set var="vetted" value="" />
						</c:otherwise>
					</c:choose>
                    <td class="info left ${underConstruction} ${vetted}">
	                        <span class="title">
	                        	<a href="${urls.libraryProjectDetailView}?<%= ViewProjectDetailsController.PARAMETER_ID %>=${project.id}" id="projectDetailsLink${project.id}">
	                        		<c:out value="${project.name}" />
	                        	</a>
	                        </span><br />
	                        <span class="description"><c:out value="${project.shortDescription}" /></span>
                    </td>
                    <td class="rating left"><img src="/images/${project.rating}star.gif" alt="${project.rating}" "border="none"></td>
                    <c:if test="${currentLibrary.showCloneCount}">
                    <td class="clonedCount">${project.cloneCount}</td>
                    </c:if>
                    <td class="comments">${project.commentCount}</td>
                    <td class="updated left"><fmt:formatDate value="${project.lastUpdatedDate}" type="date" dateStyle="short" /></td>
                </tr>
			</c:forEach>
           </tbody>
       </table>
</div>
