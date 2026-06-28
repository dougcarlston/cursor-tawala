<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

	<div class="yui-u first" style="margin-right: 18em;">
		<div class="section customizeLeft">
			<h3>Save</h3>
			<div id="saveStatus" class="status"></div>
			<div id="saveContent">
				<p>
					If you haven't signed up for a free account, you can still use these links to access your 
					customized web app. Simply save these links now by copying and pasting them to a safe place:					
				</p>
				<br />
					
			<c:forEach var="urlEntry" items="${userProject.entryPointURLsSuitableForSavingDuringCustomization}" >
				<c:set var="form" value="${urlEntry.key}"/>
				<c:set var="url" value="${urlEntry.value}"/>
				<p>${form.description}</p>
				<input  style="width: 65%" readonly type="text" name="link" class="text" 
					value="<c:out value="${url}" />" />
				<br /><br />
			</c:forEach>
					
				<p>
					If you'd like we can email the links for the web application to you:
				</p>
				<br />
				 <a class="roundButton" onclick="this.blur();" href="javascript:Tawala.Customize.dialogs.email.display()" title="Email Links" id="linkToEmailDialog"><span>Email the links to me</span></a>
				 <br /><br />

				<h5>Save your project to My Tawala</h5>
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
<script type="text/javascript">
	Tawala.Customize.config.showSaveDialogImmediatelyAfterPageLoad = true;
</script>
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
				<br /><br /><br /><br />
			</div>

		</div>
	</div>
