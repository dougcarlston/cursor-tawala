<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

    <div class="section">
    	<c:if test="${passwordSuccessfullyChanged}"><div>Your password has been changed!</div></c:if>
	
	    <form method="POST" id="changePassword">
        <div>
	        <div class="note">Fields marked with a * are required</div>
        </div>
        
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="userForm" />
		</tiles:insert>

	    <table class="edit">
	    	<col style="width: 170px;">
	    	<col style="width: 160px;">
	    	<col style="width: 90px;">
	    	<col style="width: 160px;">
	    	<tbody>
		    <tr>
		        <td class="label">Old Password:</td>
                <td>
					<spring:bind path="passwordChangeForm.oldPassword">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input name="${status.expression}" type="password" value=""/> *
	               	</spring:bind>
                </td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		    <tr>
		        <td class="label">New Password:</td>
                <td>
					<spring:bind path="passwordChangeForm.password">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input name="${status.expression}" type="password" value=""/> *
	               	</spring:bind>
                </td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		    <tr>
		        <td class="label">Re-enter New Password:</td>
                <td>
	               	<spring:bind path="passwordChangeForm.repeatedPassword">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input name="${status.expression}" type="password" value=""/> *
	               	</spring:bind>
                </td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		</tbody>
	</table>
	<div class="editActions">
		<input type="image" src="/images/submit-button.gif" name="submit" value="Submit"  title="Submit registration information" /> 
		<a href="javascript:history.go(-1);" title="Return to project details" ><img src="/images/cancel-button.gif" alt="Cancel" title="Cancel registration" /></a>
	</div>
    </form>
    </div>
