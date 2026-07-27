<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<form:form id="registrationForm" commandName="userForm">
		<p>
			Please create a user name and password.
    	</p>
        
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="userForm" />
		</tiles:insert>

   		<label>Email</label><br />
		<spring:bind path="userForm.emailAddress">
		<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
              </spring:bind>
		<form:input path="emailAddress" cssClass="text"/> *
		(all email addresses kept private)
		<br />
        <label>User Name</label><br />
		<spring:bind path="userForm.user.id">
			<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
        </spring:bind>
		<form:input cssClass="text" path="user.id" maxlength="${meta.userNameMaxLength}" /> *
		<br />

        <label>Password:</label><br />
		<spring:bind path="userForm.password">
			<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
        </spring:bind>
		<form:password cssClass="text" path="password" /> *
		<br />

        <label>Re-enter Password</label><br />
		<spring:bind path="userForm.repeatedPassword">
			<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
        </spring:bind>
        <form:password cssClass="text" path="repeatedPassword" /> *
		
		<br /><br />

		<div class="buttons">
			<button type="submit" name="submit" value="Submit"  
				onClick="javascript:urchinTracker('/user/initialsetup/accountcreated'); return true;" 
				title="Save my information" />SUBMIT</button>
				 
			<a href="javascript:history.go(-1);" title="Return to previous page"
				 alt="Cancel" title="Cancel">CANCEL
			</a>
		</div>
	</form:form>
	<br /><br /><br /><br /><br />
</div>
