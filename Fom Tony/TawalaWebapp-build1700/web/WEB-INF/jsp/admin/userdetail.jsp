<%@ page contentType="text/html" %>
<jsp:directive.page import="com.tawala.web.library.ViewProjectDetailsController"/>
<jsp:directive.page import="com.tawala.web.admin.ViewUserDetailController"/>
<jsp:directive.page import="com.tawala.web.admin.SwitchUserController"/>
<%@page import="com.tawala.web.admin.sportsdashboard.ViewProjectWorkflowDetailsController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<h2>User Details</h2>
<div id="userDetails" class="section">
	<div class="sectionContent">
		<table class="edit">
			<tbody>
				<tr>
					<td>
						<span class="label">ID:</span><span class="info"><c:out value="${currentUser.id}" /></span>
					</td>
					<td>
						<span class="label">Status:</span><span class="info"><c:out value="${currentUser.status}" /></span>
					</td>
				</tr>
				<tr>
					<td>
						<span class="label">Name:</span><span class="info"><c:out value="${currentUser.firstName}" /> <c:out value="${currentUser.lastName}" /></span>
					</td>
					<td>
						<span class="label">Administrator:</span><span class="info"><c:out value="${currentUser.administrator}" /></span>
					</td>
				</tr>
				<tr>
					<td>
						<span class="label">Email:</span><span class="info"><c:out value="${currentUser.email}" /></span>
					</td>
					<td>
						<span class="label">Password will be reset on next log-in:</span><span class="info"><c:out value="${currentUser.requirePasswordReset}" /></span>
					</td>
				</tr>
				<tr>
					<td>
						<span class="label">Registration Date:</span><span class="info"><fmt:formatDate value="${currentUser.registrationDate}" pattern="yyyy/MM/dd hh:mma" /></span>
					</td>
					<td>
					</td>
				</tr>
				<tr>
					<td>
						<span class="label">Last Log-in Date:</span><span class="info">
							<c:choose>
								<c:when test="${empty currentUser.lastLoggedInDate}">never</c:when>
								<c:otherwise><fmt:formatDate value="${currentUser.lastLoggedInDate}" pattern="yyyy/MM/dd hh:mma" /></c:otherwise>
							</c:choose>
						</span>
					</td>
					<td>
					</td>
				</tr>
			</tbody>
		</table>
		<br />
	    <form method="POST" id="actionForm">
	       	<input type="hidden" name="<%= ViewUserDetailController.PARAMETER_ACTION %>" value="" />
	       	<input type="hidden" name="id" value="<c:out value="${currentUser.id}" />" />
	    </form>
				
	<div id="confirmationDialog" style="text-align: left; visibility: hidden;">
		<div class="hd"><div class="tl"></div><span>User Details Action</span><div class="tr"></div></div>
		<div class="bd">
		</div>
		<div class="ft"></div>
	</div>
	<div class="buttons">		
	    <c:if test="${currentUser.status.canBeApproved}">
			<button type="submit" onclick="javascript:confirmApproval()" >Approve</button>
		</c:if>
	    <c:if test="${! currentUser.suspended}">
			<button type="submit" onclick="javascript:confirmSuspend()" >Suspend</button>
	    </c:if>
	    <c:if test="${currentUser.suspended}">
			<button onclick="javascript:confirmUnsuspend()" >Release from Hold</button>
	    </c:if>
		<button type="submit" onclick="javascript:confirmDelete()" >Delete</button>

		<c:url var="switchUserUrl" value="${urls.switchUser}">
			<c:param name="<%= SwitchUserController.USER_ID_PARAMETER %>" value="${currentUser.databaseId}"/>
		</c:url>
		<a href="${switchUserUrl}" id="switchUserLink">Become this user</a> (use with caution!)
	</div>
	</div>
</div>    

<div id="projects" class="section collapsible">
	<h3 class="sectionHeading">Projects (${otherUserProjectCount} total)</h3>
	<c:if test="${otherUserProjectPagingInfo.max < otherUserProjectCount}">
	<h3>Showing first ${otherUserProjectPagingInfo.max} projects sorted by <c:out value="${otherUserProjectPagingInfo.sortOrder.description}"/>.</h3>
	</c:if>
	<div class="sectionContent">
		<table class="list sortable ruler">
			<col  />
			<col style="width: 15%;" />
			<thead>
				<tr>
					<th>Project Name</th>
					<th>Responses</th>
				</tr>
			</thead>
			<tbody>		
			<c:forEach var="stats" items="${otherUserProjectStats}" varStatus="status">
				<c:url var="viewProcessDetailsURL" value="${urls.adminViewProjectWorkflowDetails}">
					<c:param name="<%= ViewProjectWorkflowDetailsController.PROJECT_ID_PARAMETER %>" 
							value="${stats.id}" />
				</c:url>
				<tr>
					<td class="name left"><a href="${viewProcessDetailsURL}"><c:out value="${stats.name}" /></a></td>
					<td>${stats.responseCount}</td>
				</tr>
			</c:forEach>
			</tbody>
		</table>
	</div>
</div>

<div id="search" class="section collapsible">
	<h3 class="sectionHeading">Recent Library Changes</h3>
	<div class="sectionContent">
		<form>
			<input type="hidden" name="<%=ViewUserDetailController.USER_ID_PARAMETER %>" value="${currentUser.databaseId}" />
			Show changes made in the last 
			<input type="text" name="<%= ViewUserDetailController.LIBRARY_CHANGES_SINCE_PARAMETER %>" value="${days}" 
				size="4" /> day(s). 
			<input type="image" value="Refresh" src="/images/silk/arrow_refresh.gif" title="Refresh list" class="smallIcon" />
		</form>
		<c:choose>
			<c:when test="${! empty events}">
				<table class="list sortable ruler">
					<col style="width: 160px;" />
					<col style="" />
					<col style="width: 110px;" />
					<col style="width: 40px;" />		
					<thead>
						<tr>
							<th class="left">Project</th>
							<th class="left">Description</th>
							<th class="left">Date</th>
							<th>&nbsp;</th>
						</tr>
					</thead>
				
					<tfoot>
						<tr>
							<td></td>
							<td></td>
							<td></td>
							<td></td>
						</tr>
					</tfoot>
					
					<tbody>
					<c:forEach items="${events}" var="event">
						<tr>
							<c:choose>
								<c:when test="${event.projectRelated}">
									<c:set var="project" value="${projectMap[event.projectId]}" />
								</c:when>
								<c:otherwise>
									<c:remove var="project" />
								</c:otherwise>
							</c:choose>
							<td class="left">
								<c:choose>
									<c:when test="${empty project}">&nbsp;</c:when>
									<c:when test="${project.deleted }"><c:out value="${project.name}" /></c:when>
									<c:otherwise>
										<a href="${urls.libraryProjectDetailView}?<%= ViewProjectDetailsController.PARAMETER_ID %>=${project.id}"><c:out value="${project.name}" /></a>
									</c:otherwise>
								</c:choose>
							</td>
							<td class="left"><spring:message message="${event.description}" /></td>
							<td class="left"><fmt:formatDate value="${event.date}" pattern="HH:mm MM/dd/yyyy" /></td>
							<td><%@ include file="/WEB-INF/jsp/library/revertChangesLink.jsp" %></td>
						</tr>
					</c:forEach>
					</tbody>
				</table>
			</c:when>
			<c:otherwise>
				<p class="message">No changes found</p>
			</c:otherwise>
		</c:choose>
	</div>
	<br />
</div>
		
		<script type="text/javascript">
			var action = "none";
			var confirmationDialog;
			function init(){
				confirmationDialog = new YAHOO.widget.Dialog("confirmationDialog",  
	                  { width: "300px", 
	                    fixedcenter: true, 
	                    visible: false, 
	                    draggable: false, 
	                    close: true,
	                    modal: true, 
//	                    text: confirmationText, 
	                    icon: YAHOO.widget.SimpleDialog.ICON_WARN, 
	                    constraintoviewport: true, 
	                    buttons: [ { text:"Yes", handler:handleYes}, 
		                    { text:"No",  handler:handleNo, isDefault:true } ] 
		              } ); 
				
				confirmationDialog.render(document.body);
			};
			
			function handleYes() {
				this.hide();
				var form = document.getElementById('actionForm');
				form.<%= ViewUserDetailController.PARAMETER_ACTION %>.value = action;
				form.submit();
			}
				
			function handleNo() {
				this.hide();
				action = "none";
			}
	
			function confirm(actionName, confirmationText, titleText) {
				action = actionName;
				
				confirmationDialog.setBody(confirmationText);
				confirmationDialog.show();
			}
				
			function confirmApproval() {
				confirm('<%= ViewUserDetailController.PARAMETER_ACTION_APPROVE %>', 'Are you sure you want to approve this user?<br />', "Approve");	
			}
				
			function confirmSuspend() {
				confirm('<%= ViewUserDetailController.PARAMETER_ACTION_SUSPEND %>', 'Are you sure you want to suspend this user?<br />', "Suspend");	
			}
			function confirmUnsuspend() {
				confirm('<%= ViewUserDetailController.PARAMETER_ACTION_RELEASE %>', 'Are you sure you want to release this user from hold?', "Unsuspend");	
			}
			function confirmDelete() {
				confirm('<%= ViewUserDetailController.PARAMETER_ACTION_DELETE %>', 'Are you sure you want to delete this user?', "Delete");	
			}

			YAHOO.util.Event.addListener(window, "load", init);
		</script>
			
