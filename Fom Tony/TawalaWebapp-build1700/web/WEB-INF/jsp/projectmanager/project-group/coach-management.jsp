
<%@page import="com.tawala.web.projectmanager.projectgroup.GroupRosterReportController"%>
<%@page import="com.tawala.web.projectmanager.projectgroup.GroupCoachReportController"%>
<%@page import="com.tawala.web.projectmanager.projectgroup.UpdateCoachStatusMemoController"%><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<h2><c:out value="${group.name}"/></h2>
	
<br />
			<c:if test="${empty group.userProjects}">
					<tr>
						<td colspan="3">No leagues are added yet.</td>
					</tr>
			</c:if>
			<select onchange="Tawala.ProjectGroup.onProjectSelect(this);">
				<option value="">Select the league to view coaches</option>
			<c:forEach var="userProject" items="${group.userProjects}">
				<option value="${userProject.id}"><c:out value="${userProject.user.id}" /> - <c:out value="${userProject.name}" /></option>
			</c:forEach>
			</select>
			
			<br />
			
			<div id="loadStatusContainer" style="height: 15px;"></div>
			
			<br />
			<div id="coachSection">
			</div>
</div>

<div id="memoDialog" class="panel" style="visibility: hidden;">
    <div class="hd"><div class="tl"></div><span id="dialogTitle">Status Memo</span><div class="tr"></div></div>
    <div class="bd">
        <form id="memoForm" method="POST" action="${urls.projectGroupUpdateCoachStatusMemo}">
        	<input type="hidden" name="<%= UpdateCoachStatusMemoController.GROUP_ID %>" value="${group.id}">
        	<input type="hidden" name="<%= UpdateCoachStatusMemoController.COACH_ID %>" value="">
        	<input type="hidden" name="<%= UpdateCoachStatusMemoController.PROJECT_ID %>" value="">
        	
            <p>Memo:</p>
            <textarea rows="5" cols="60" name="<%= UpdateCoachStatusMemoController.MEMO %>">
            </textarea>
        </form>
    </div>
</div>

<script>
	Tawala.ProjectGroup.PROJECT_ID = "<%= UpdateCoachStatusMemoController.PROJECT_ID %>";
	Tawala.ProjectGroup.COACH_ID = "<%= UpdateCoachStatusMemoController.COACH_ID %>";
	Tawala.ProjectGroup.MEMO = "<%= UpdateCoachStatusMemoController.MEMO %>";
	Tawala.ProjectGroup.URL_LoadCoaches = '<c:url value="${urls.projectGroupLoadCoachesForProject}" />';
	Tawala.ProjectGroup.URL_UpdateCoachStatus = '<c:url value="${urls.projectGroupUpdateCoachStatus}" />';

	Tawala.ProjectGroup.groupId = ${group.id};
</script>