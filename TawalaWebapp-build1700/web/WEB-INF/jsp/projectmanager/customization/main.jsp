<jsp:directive.page import="com.tawala.project.commands.ExecutionContext"/>
<jsp:directive.page import="com.tawala.web.projectmanager.SaveDuringCustomizationController"/>
<jsp:directive.page import="com.tawala.web.projectmanager.SendLinksByEmailController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<script>
	setPageTitle("Customize ${projectName}");
</script>

	<div id="customizeContentDiv" class="section" >
		<br class="clr"/>
		<div id="customizePalette" class="yui-navset">
		    <ul class="yui-nav">
		        <li id="edit" class="selected"><a href="#tab1"><em>Step 1: Add Content</em></a></li>
		        <li id="appearance"><a href="#tab2"><em>Step 2: Appearance</em></a></li>
		        <li id="publish"><a href="#tab3"><em>Step 3: Create Web Page</em></a></li>
		    </ul> 

			<div id="previewContainer" class="section customizeRight">
				<h3>Preview</h3>
				<c:url var="previewChangesUrl" value="${userProject.customizationPreviewEntryPointURL}">
					<c:param name="<%= ExecutionContext.INCLUDE_CUSTOMIZATION_MARKERS %>" value="yes" />
					<c:param name="<%= ExecutionContext.SURPRESS_ADS_PARAMETER %>" value="y"/>
				</c:url>
				
				<iframe frameborder="0" height="900px" id="previewFrame" name="previewFrame" src="${previewChangesUrl}" width="100%"></iframe>
				<div class="previewOverlay"></div>
			</div>
		               
		    <div class="yui-content">
				<%@ include file="/WEB-INF/jsp/projectmanager/customization/editTile.jsp" %>
				<%@ include file="/WEB-INF/jsp/projectmanager/customization/appearanceTile.jsp" %>
				<!-- %@ include file="/WEB-INF/jsp/projectmanager/customization/testDriveTile.jsp" % -->
				<!-- %@ include file="/WEB-INF/jsp/projectmanager/customization/saveTile.jsp" % -->
				<%@ include file="/WEB-INF/jsp/projectmanager/customization/publishTile.jsp" %>
		    </div>
		</div>					
	</div> <!-- end yui-navset -->

<div id="loginDialog" class="dialog" style="visibility:hidden;">
	<div class="hd"><div class="tl"></div><div class="ti">Log In</div><div class="tr"></div></div>
	<div class="bd">
		<p>Please enter your user name and password in order to login and save this project to your My Tawala area.</p>
		<div id="loginErrorMessage" style="text-align: center; color: red"></div>
		<form action="${urls.loginDuringCustomization}" method="post" id="loginDuringCustomizationForm">
			<table class="edit">
				<tr>
					<td class="label">User name:</td>
					<td><input class="text" type="text" name="userName" /></td>
				</tr>
				<tr>
					<td class="label">Password:</td>
					<td><input class="text" type="password" name="password" /></td>
				</tr>
				<tr>
	        		<td class="label">Keep me signed in:</td>
	        		<td><input type="checkbox" name="keepSignedIn" /></td>
				</tr>
			</table>
		</form>
		<br />
	</div>
	<div class="ft"></div>
</div>

<c:url var="saveUrl" value="${urls.projectManagerSaveDuringCustomization}">
	<c:param name="<%=SaveDuringCustomizationController.PROJECT_ID_PARAMETER %>" value="${userProject.id}" />
</c:url>

<div id="startCustomizationDialog" class="dialog" style="visibility:hidden;">
	<div class="hd"><div class="tl"></div><div class="ti">Starting Customization</div><div class="tr"></div></div>
	<div class="bd">
		<p>
			The next few steps let you customize, review and test this Tawala project safely.
			When you name your project, all changes will automatically be saved.
			You can access your named project any time in your My Tawala area.
		</p>
		<p>
			You may name your project now, but you can skip this step and name it later as well.
		</p>
		<p>
			Enter the name you would like to save this project as in your My Tawala area
		</p>
		<div id="saveOnStartupErrorMessage" style="text-align: center; color: red"></div>
		<form action="${saveUrl}" method="post" id="saveForm">
			<table class="edit">
		    	<col style="width: 120px;" />
				<tr>
					<td class="label">Project Name:</td>
					<td>
						<input class="text" style="width: 90%;" type="text" name="<%= SaveDuringCustomizationController.PROJECT_NAME_PARAMETER %>" value="${projectName}"/>
					</td>
				</tr>					
			</table>
		</form>
		<br />
	</div>
	<div class="ft"></div>
</div>

<div id="saveDialog" class="dialog" style="visibility:hidden;">
	<div class="hd"><div class="tl"></div><div class="ti">Save Project</div><div class="tr"></div></div>
	<div class="bd">
		<p>Enter the name you would like to save this project as in your My Tawala area</p>
		<div id="saveErrorMessage" style="text-align: center; color: red"></div>
		<form action="${saveUrl}" method="post" id="saveForm">
			<table class="edit">
		    	<col style="width: 120px;" />
				<tr>
					<td class="label">Project Name:</td>
					<td>
						<input class="text" style="width: 90%;" type="text" name="<%= SaveDuringCustomizationController.PROJECT_NAME_PARAMETER %>" value="${projectName}"/>
					</td>
				</tr>					
			</table>
		</form>
		<br />
	</div>
	<div class="ft"></div>
</div>

<div id="signupDialog" class="dialog" style="visibility:hidden;">
	<div class="hd"><div class="tl"></div><div class="ti">Sign up</div><div class="tr"></div></div>
	<div class="bd">
		<p>
			You can also create a free Tawala account and save your customized settings. Here are
			some of the benefits you gain by creating an account:
		</p>

		<ul class="outsideDot">
			<li>A personal area will be setup where all your customized applications and data are stored</li>
			<li>You can come back at a later date and update this application</li>
			<li>Collected data can be viewed and exported</li>
			<li>It's Free!</li>
		</ul>

		<div id="signupErrorMessage" style="text-align: center; color: red"></div>

		<form action="${urls.userRegistrationDuringCustomization}" method="post" id="signupDuringCustomizationForm">
		<table class="edit">
	    	<col style="width: 140px;" />
	    	<col  />
	    	<tbody>
		    <tr>
		        <td class="label">
		        	Email Address:
		        </td>
                <td>
					<input class="text" name="emailAddress" type="text" /> *
					<br />(All email addresses are kept private)
                </td>
		    </tr>
		    <tr>
		        <td class="label">
		        	User Name:
		        </td>
                <td>
					<input class="text" name="user.id" type="text" /> *
                </td>
		    </tr>
		    <tr>
		        <td class="label">Password:</td>
                <td>
					<input class="text" name="password" type="password" /> *
                </td>
		    </tr>
		    <tr>
		        <td class="label">Re-enter Password:</td>
                <td>
					<input class="text" name="repeatedPassword" type="password" /> *
                </td>
		    </tr>
			</tbody>
		</table>
		</form>
		<br />
	</div>
	<div class="ft"></div>
</div>

<div id="showLinkDialog" class="dialog" style="visibility:hidden;">
	<div class="hd"><div class="tl"></div><div class="ti">Customized Project Link</div><div class="tr"></div></div>
	<div class="bd">
		<p>Below is the link to your newly customized web application. You may want to make a copy of it so 
		that you can easily return to the application:</p>
		<p>
			<c:forEach var="urlEntry" items="${userProject.entryPointURLsWithoutCustomizerForm}" >
				<c:set var="form" value="${urlEntry.key}"/>
				<c:set var="url" value="${urlEntry.value}"/>
				<b><c:out value="${url}" /></b>
				<br /><br />
			</c:forEach>
		</p>
		<br />
	</div>
	<div class="ft"></div>
</div>

<div id="emailDialog" class="dialog" style="visibility:hidden;">
	<div class="hd"><div class="tl"></div><div class="ti">Email Application Links</div><div class="tr"></div></div>
	<div class="bd">
		<p>Enter the email address you would like the links sent to:</p>
		<div id="emailErrorMessage" style="text-align: center; color: red"></div>
		<form action="${urls.emailProjectLinks}" method="post" id="sendEmailWithProjectLinksForm">
			<table class="edit">
		    	<col style="width: 120px;" />
				<tr>
					<td class="label">Email Address:</td>
					<td>
						<input class="text" style="width: 90%;" type="text" name="<%= SendLinksByEmailController.EMAIL_PARAMETER %>" value=""/>
						<input type="hidden" name="<%= SendLinksByEmailController.PROJECT_ID_PARAMETER %>" value="${userProject.uniqueRandomId}"/>
						<input type="hidden" name="<%= SendLinksByEmailController.ORIGINAL_PROJECT_NAME_PARAMETER %>" value="${projectName}"/>
					</td>
				</tr>					
			</table>
		</form>
	</div>
	<div class="ft"></div>
</div>

