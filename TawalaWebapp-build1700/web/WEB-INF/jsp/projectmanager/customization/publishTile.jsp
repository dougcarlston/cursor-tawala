<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<div class="yui-u first" style="margin-right: 18em;">
	<div class="section customizeLeft">
		<h3>Go!</h3>
		<div id="publishStatus" class="status"></div>
		<div id="publishContent">
			<div>
				<p>You're all done! You can use the links below to access your application.</p>
				<br />
				<ol>
				<li>
					<div><b>Main Project Link</b></div>
					<p>Copy this link into an email and send it to those you would like to invite to use this application:</p>
					<span style="padding-left: 4em;"><a href="${userProject.mainEntryPointURL}">${userProject.mainEntryPointURL}</a></span><br />
				</li>
				<c:if test="${! empty userProject.adminURL}">
				<li>
					<div><b>Administration Link</b></div>
					<p>Copy this second link by bookmarking it or sending it to yourself by email so that you can see the administration page of your application:</p>
					<span style="padding-left: 4em;"><a href="${userProject.adminURL}">${userProject.adminURL}</a></span><br />
				</li>
				</c:if>
				<li>
					<div><b>Setup Link</b></div>
					<p>Copy this link to get to the Setup form for the application in case you want to make some changes in the future:</p>
					<span style="padding-left: 4em;"><a href="${userProject.setupURL}">${userProject.setupURL}</a></span>
				</li>
				</ol>
				<br />				
			</div>

			<div>
				<h3>Save Project</h3>
				<span id="saveProjectSection">
				<c:choose>
					<c:when test="${empty user}">
						Optionally, you may save your work so you can use it as a starting point in the future. If you don't have a Tawala account yet you can create on now.<br /><br />
						<a class="roundButton" href="javascript:Tawala.Customize.dialogs.signup.show()" title="Signup" id="linkToSignupDialog"><span>Sign Up for a FREE Tawala Account</span></a>
						<a class="roundButton" onclick="this.blur();" href="javascript:Tawala.Customize.dialogs.login.show()" title="Log in" id="linkToLoginDialog"><span>Log In</span></a>
						<br /><br />
					</c:when>
					<c:when test="${projectIsSaved}">
						This project has been saved under your <a href="${urls.projectManagerView}" id="linkToProjectManager">My Tawala</a> account as "<c:out value="${projectName}" />".
					</c:when>
					<c:otherwise>
						Optionally, you may save your work so you can use it as a starting point in the future. <br /><br /> 
						<a class="roundButton" onclick="this.blur();" href="javascript:Tawala.Customize.dialogs.save.show()" title="Save this project"><span>Save this project under My Tawala</span></a>
					</c:otherwise>
				</c:choose>
				</span>
				
				<span id="successfulLoginConfirmationSection" style="display: none;">
					Now that you have logged in you can
					<br /><br /> 
					<a class="roundButton" onclick="this.blur();" href="javascript:Tawala.Customize.dialogs.save.show()"><span>Save your project to My Tawala</span></a>.
				</span>
		
				<span id="successfulSignupConfirmationSection" style="display: none;">
					Now that you have signed up you can<br /><br /> 
					<a class="roundButton" onclick="this.blur();" href="javascript:Tawala.Customize.dialogs.save.show()"><span>Save your project to My Tawala</span></a>.
				</span>
		
				<span id="successfulSaveConfirmationSection" style="display: none;">
					This project has been saved under your <a href="${urls.projectManagerView}" id="linkToProjectManager">My Tawala</a> account!
				</span>
				
				<p id="createAccount" style="display: none">
				</p>
			</div>
		</div>
		<br /><br />
	</div>
</div>