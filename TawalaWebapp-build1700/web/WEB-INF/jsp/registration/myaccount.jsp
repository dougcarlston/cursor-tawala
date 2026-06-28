<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

    <div class="section">
    	<c:if test="${updateSuccessful}"><div>Your account has been updated!</div></c:if>
	    <form:form method="POST" id="editUser" commandName="userForm">
		<p>
			Edit my Tawala account information.
		</p>
		<br />
        
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="userForm" />
		</tiles:insert>

	    <table class="edit">
	    	<col style="width: 170px;" />
	    	<col style="width: 150px;" />
	    	<col style="width: 90px;" />
	    	<col />
	    	<tbody>
            <tr>
				<td class="label">First Name:</td>
                <td> 
					<spring:bind path="userForm.user.firstName">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input name="${status.expression}" type="text" value="<c:out value="${status.value}" />"/>
	               	</spring:bind>
                </td>

		        <td class="label">Last Name:</td>
                <td>
					<spring:bind path="userForm.user.lastName">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input name="${status.expression}" type="text" value="<c:out value="${status.value}" />"/>
    	           	</spring:bind>
	            </td>
    	    </tr>
		    <tr>
		        <td class="label">Email:</td>
                <td>
					<spring:bind path="userForm.emailAddress">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input name="${status.expression}" type="text" value="<c:out value="${status.value}" />"/> 
	                </spring:bind>
                </td>
                <td></td>
                <td></td>
		    </tr>
		    <tr>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		    <tr>
		        <td class="label">See advanced features?</td>
                <td><form:checkbox path="seeAdvancedFeatures" /></td>
                <td></td>
                <td></td>
		    </tr>
		    <c:if test="${userForm.user.administrator}">
		    <tr>
		        <td class="label">Will ads appear on my projects?</td>
                <td><form:checkbox path="user.enableAds" /></td>
                <td></td>
                <td></td>
		    </tr>
		    </c:if>
		    <c:if test="${userForm.user.administrator || originalUser.administrator}">
		    <tr>
		        <td class="label">PayPal Account: </td>
                <td><form:input path="user.payPalAccountId" maxlength="50" /></td>
                <td></td>
                <td></td>
		    </tr>
		    </c:if>
		</tbody>
	</table>

	<div class="editActions">
		<input type="image" src="/images/submit-button.gif" name="submit" value="Submit"  title="Submit" /> 
		&nbsp;&nbsp;<input type="image" src="/images/cancel-button.gif" onclick="javascript:history.go(-1); return false;"title="Cancel" />
	</div>

    </form:form>
    </div>
    