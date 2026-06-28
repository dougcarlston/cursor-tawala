<%@ page contentType="text/html" %>
<%@ page import="com.tawala.web.user.ManageUsersController" %>
<jsp:directive.page import="com.tawala.web.admin.ViewUserDetailController"/>
<jsp:directive.page import="com.tawala.project.library.Constants"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="section">
	<h2 class="sectionHeading">Manage Users</h2>
	<div class="sectionContent">
		<div class="buttons">
		    <a href="http://lucene.apache.org/java/2_4_1/queryparsersyntax.html" target="_blank">Search tips</a>
		
		    <c:url var="reindexUrl" value="">
		    	<c:param name="<%= ManageUsersController.PARAMETER_REINDEX_USERS %>" value="true" />
		    </c:url>
		    <a href="${reindexUrl}">Reindex users</a>
		
			<c:url var="viewUserDetailUrl" value="${urls.adminViewUserInfo}">
				<c:param name="<%= ViewUserDetailController.USER_ID_PARAMETER %>" value="<%= String.valueOf(Constants.ANONYMOUS_USER_ID)%>" />
			</c:url>
			<a href="${viewUserDetailUrl}">Anonymous user's details</a>
		</div>
	</div>
</div>

<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>
<div class="section">
	<div class="sectionContent">
		<form id="searchByQueryForm">
			<input type="text" name="<%= ManageUsersController.PARAMETER_QUERY %>" value="<c:out value="${query}" />" size="60" />
		    <input type="submit" value="Search"/> 
		</form>
	</div>
</div>

<!-- 
Disable for now. The exising status search doesn't make much sense at the moment.
<form id="searchByStatusForm">
	<input type="hidden" name="<%= ManageUsersController.PARAMETER_ACTION_SEARCH_BY_STATUS %>" value="true" />
	Show users with status:  
	<select name="<%= ManageUsersController.PARAMETER_STATUS %>">
	<c:forEach var="nextStatus" items="${searchableStatuses}">
		<option value="${nextStatus}"<c:if test="${status == nextStatus}"> selected="true"</c:if> >${nextStatus}</option>
	</c:forEach>
	</select>
    <input type="submit" value="View"/>
</form>
 -->

<div class="section">
	<div class="sectionContent">
	<form id="searchByRegistrationDateForm">
		<input type="hidden" name="<%= ManageUsersController.PARAMETER_ACTION_SEARCH_BY_REGISTRATION_DATE %>" value="true" />
		Show users registered in the last <input name="<%= ManageUsersController.PARAMETER_REGISTRATION_DAYS_BACK %>" value="<c:out value="${registrationDaysBack}" default="5"/>"
				 type="text" size="3" /> days.
	    <input type="submit" value="View"/>
	</form>
	</div>
</div>
<div class="section">
	<div class="sectionContent">
		<c:choose>
			<c:when test="${ ! empty messages }">
				<% //--- Do nothing. There were errors on the page %>
			</c:when>	
			<c:when test="${empty query && empty status && empty registrationDaysBack}">
				<% //-- Do nothing; this is the first page hit or the query was empty %>
			</c:when>
			<c:when test="${! empty query && empty users}">
				<div>No users were found that matched the search criteria.</div>
			</c:when>
			<c:when test="${! empty status && empty users}">
				<div>No users found with status ${status}</div>
			</c:when>
			<c:when test="${! empty registrationDaysBack && empty users}">
				<div>No users were registered in the last ${registrationDaysBack} day(s).</div>
			</c:when>
			<c:otherwise>
		        <div id="administration">
		            <div id="userManager">
		            <div class="status"><span class="stats">Found ${fn:length(users)} user<c:if test="${fn:length(users) > 1}">s</c:if></span><span class="plan"></span></div>
		            <table class="list sortable ruler">
		                <colgroup>
		                    <col class="id"/>
		                    <col class="name"/>
		                    <col class="email"/>
		                    <col class="userStatus"/>
		                    <col class="date"/>
		                    <col class="controls"/>
		                </colgroup>
		                <thead>
		                    <tr>
		                    <th>Id</th>
		                    <th>Name</th>
		                    <th>Email</th>
		                    <th>Status</th>
		                    <th>Registered</th>
		                    <th>Last Login</th>
		                    <th># of Projects</th>
		                    </tr>
		                </thead>
		                <tbody class="left">
		                <c:forEach var="currentUser" items="${users}">
		                	<c:url var="viewUserDetailUrl" value="${urls.adminViewUserInfo}">
		                		<c:param name="<%= ViewUserDetailController.USER_ID_PARAMETER %>" value="${currentUser.databaseId}" />
		                	</c:url>
		                    <tr>
		                        <td class="id"><a href="${viewUserDetailUrl}" id="linkToUserDetails${currentUser.databaseId}"><c:out value="${currentUser.id}" /></a></td>
		                        <td class="name">
		                        	<a href="${viewUserDetailUrl}"><c:out value="${currentUser.firstName}" /></a>
		                        	<a href="${viewUserDetailUrl}"><c:out value="${currentUser.lastName}" /></a>
		                        </td>
		                        <td class="email"><a href="${viewUserDetailUrl}"><c:out value="${currentUser.email}" /></a></td>
		                        <td class="user_status"><a href="${viewUserDetailUrl}"><c:out value="${currentUser.status}" /></a></td>
		                        <td class="date"><a href="${viewUserDetailUrl}"><fmt:formatDate value="${currentUser.registrationDate}" pattern="yyyy/MM/dd" /></a></td>
		                        <td class="date"><a href="${viewUserDetailUrl}"><fmt:formatDate value="${currentUser.lastLoggedInDate}" pattern="yyyy/MM/dd" /></a></td>
		                        <td class="projectcount"><a href="${viewUserDetailUrl}">${projectCounts[currentUser]}</a></td>
		                    </tr>
		                </c:forEach>
		                </tbody>
		            </table>
		        </div>
		     </div>
		   </c:otherwise>
		</c:choose>
	</div>
</div>