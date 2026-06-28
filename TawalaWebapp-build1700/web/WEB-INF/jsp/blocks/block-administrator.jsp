<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

        <div  class="block">
      		<div class="content">
	        	<h3>Administration</h3>
        		<div>
        			<ul class="topicList">
        				<li>
			        		<a class="topic" name="admin_ManageUsers" href="${urls.adminManageUsers}">Manage Users</a>
	        			</li>
	        			<li>
	        				<a class="topicSection" name="adminSD" href="${urls.adminSportsDashboardManagementConsole}">SportsDashboards Management</a>
	        				<ul>
	        					<li><a class="topic" name="adminSD_Overview" href="${urls.adminSportsDashboardManagementConsole}">Overview</a></li>
	        					<li><a class="topic" name="adminSD_ProjectTaskManagement" href="${urls.adminSportsDashboardProjectTaskManagement}">Project Task Management</a></li>
	        					<li><a class="topic" name="adminSD_CreateNewDashboard" href="${urls.adminCreateNewDashboard}">Create New Dashboard</a></li>
	        					<!-- 
	        					<li><a class="topic" name="adminSD_CreateNewClient" href="${urls.adminCreateNewClient}">Create New Client</a></li>
	        					 -->
	        					<li><a class="topic" name="adminSD_AssignRoles" href="${urls.adminAssignUsersToRoles}">Assign Roles</a></li>
	        				</ul>
	        			</li>
	        			<li>
	    		    		<a class="topic" name="admin_ManageLibrary" href="${urls.adminManageLibrary}">Manage Library</a>
	        			</li>
	        			<li>
			        		<a class="topic" name="admin_ManageUserDomains" href="${urls.listUserDomains}">Manage User Domains</a>
	        			</li>
	        			<li>
	    		    		<a class="topic" name="admin_SearchProject" href="${urls.adminSearchProject}" id="searchProjectLink">Search Projects</a>
	        			</li>
	        			<li>
	        				<a class="topic" name="admin_CountOfClonedProjects" href="${urls.reportCountOfClonedProjects}" id="clonedLibraryProjectsReportLink">Cloned Library Projects Stats</a>
	        			</li>
	        			<li>
	        				<a class="topic" name="admin_UserProjectReport" href="${urls.userProjectReport}">User Project Report</a>
	        			</li>
	        			<li>
	        				<a class="topic" name="admin_UpgradeProjects" href="${urls.adminUpgradeProjectsWithNewerVersion}" id="upgradeProjectsWithNewerVersionLink">Upgrade Projects</a>
	        			</li>
	        			<li>
			        		<a class="topic" name="admin_ManageUrgentMessage" href="${urls.adminManageUrgentMessage}">Manage Urgent Message</a>
	        			</li>
	        			<li>
	        				<a class="topic" name="admin_ReportAllEvents" href="${urls.reportAllEvents}" id="allEventsReportLink">Event Report</a>
	        			</li>
	        			<li>
			        		<a class="topic" name="admin_HibernateStatistics" href="${urls.adminViewHibernateStatistics}">Hibernate Statistics</a>
	        			</li>
	        		</ul>
        		</div>
    	    </div>
    	</div>
