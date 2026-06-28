<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

    <div class="section">
		<div class="status">
			<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>		
			<tiles:insert definition="global.errors" ignore="false" >
				<tiles:put name="commandName" value="login" />
			</tiles:insert>
			
			<c:if test="${passwordHasBeenReset}">
				Your password has been reset and an email with a temporary password has been sent to your email account.
			</c:if>
		</div>
	    <div class="note">Passwords are case sensitive</div>
   		<c:if test="${empty login}">
			<c:set var="login" value="<%= new com.tawala.web.user.LoginForm() %>" scope="request" />
    	</c:if>
    	
    	<form:form id="loginForm" cssClass="login" action="${urls.login}" method="POST" commandName="login">
    		<form:hidden path="redirectTo" />
   	    	<div class="section edit">
   		        		<div class="line">
   		        			<label>User Name</label>
					       	<form:input path="userName" cssClass="text" size="32" maxlength="${meta.userNameMaxLength}" />
	        			</div>
	        			<div class="line">
	        				<label>Password</label>
        				    <form:password path="password" cssClass="text" size="32" maxlength="100" />
						</div>
	        			<div class="line">
	        				<form:checkbox path="keepSignedIn" /> Keep me signed in
						</div>
						<br />
	       	    <div class="buttons">
	       	    	<button type="submit" name="submitbutton">SUBMIT</button>
					<a class="button" href="javascript:history.go(-1);" title="Return to previous page" >CANCEL</a>
	       	    </div>
   		    </div>
	    </form:form>
		    
		    <br />
		    <div id="passwordResetLink"><a href="javascript:showPasswordResetDialog();">If you've forgotten your password you can click here to reset it</a></div>

			<div id="passwordResetDialog" style="display: none; border-top: 1px solid #cccccc;">
				
				<br />
				<h3>Reset Password</h3>
				<p>
				Please enter your user name. 
				A new temporary password will be sent to your email address. 
				You will be required to reset it on the first successful login.
				</p>
				<br />
		        <form class="login" id="passwordResetForm" method="post" action="<c:url value="${urls.userPasswordReset}" />">
		        	<div class="section edit">
			   	    	<div class="line">
			   		        <label>User Name</label>
				        	<spring:bind path="login.userName">
				        	    <input class="text" type="text" size="20" maxlength="${meta.userNameMaxLength}" name="${status.expression}" value="<c:out value="${status.value}" />" /><br />
				        	</spring:bind>
				        	<br />
			   		    </div>
			       	    <div class="buttons">
			       	    	<button name="submitbutton" type="submit">RESET PASSWORD</button>
							<a class="button" href="javascript:hidePasswordResetDialog();" title="Return to previous page" >CANCEL</a>
			       	    </div>
		   		    </div>
			    </form>
			    <br />
			</div>
	</div>

<script type="text/javascript">
    function showPasswordResetDialog() {
	    document.getElementById('passwordResetLink').style.display = "none";
        var container = document.getElementById('passwordResetDialog');
        container.style.display = "block";
    }

    function hidePasswordResetDialog() {
	    document.getElementById('passwordResetLink').style.display = "block";
        var container = document.getElementById('passwordResetDialog');
        container.style.display = "none";
    }
</script>    