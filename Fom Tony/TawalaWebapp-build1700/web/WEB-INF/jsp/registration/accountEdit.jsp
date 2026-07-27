<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

    <div id="registration">
    <div class="section">
        <tiles:getAsString name="introductionText"/>
	    <form method="POST" id="editUser">
		<br />
		<h3>Account Information</h3>
        <div>
	        <div class="note">Fields marked with a * are required</div>
        </div>
        
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="userForm" />
		</tiles:insert>

	    <table>
	    	<col style="width: 140px;">
	    	<col style="width: 160px;">
	    	<col style="width: 90px;">
	    	<col style="width: 160px;">
	    	<tbody>
            <tr>
				<td class="label">
            		First Name:
            	</td>
                <td> 
					<spring:bind path="userForm.user.firstName">
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						<input class="text" name="${status.expression}" type="text" value="<c:out value="${status.value}" />"/>
	               	</spring:bind>
                </td>

		        <td class="label">
		        	Last Name:
		        </td>
                <td>
					<spring:bind path="userForm.user.lastName">
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						<input class="text" name="${status.expression}" type="text" value="<c:out value="${status.value}" />"/>
	               	</spring:bind>
                </td>
                <td></td>
		    </tr>
		    <tr>
		        <td class="label">
		        	Email (all email addresses kept private):
		        </td>
                <td>
					<spring:bind path="userForm.emailAddress">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input class="text" name="${status.expression}" type="text" value="<c:out value="${status.value}" />"/>
	                </spring:bind>
                </td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		    <tr>
		        <td class="label">
		        	User Name:
		        </td>
                <td>
				<spring:bind path="userForm.user.id">
		    <c:choose>
			    <c:when test="${editUser}"><c:out value="${status.value}" /></c:when>
			    <c:otherwise>
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input class="text" name="${status.expression}" type="text" value="<c:out value="${status.value}" />"/> *
				</c:otherwise>
            </c:choose>
                </spring:bind>
                </td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		    <tr>
		        <td class="label">Password:</td>
                <td>
					<spring:bind path="userForm.password">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input class="text" name="${status.expression}" type="password" value="<c:out value="${status.value}" />"/> *
	               	</spring:bind>
                </td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		    <tr>
		        <td class="label">Re-enter Password:</td>
                <td>
	               	<spring:bind path="userForm.repeatedPassword">
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
					<input class="text" name="${status.expression}" type="password" value="<c:out value="${status.value}" />"/> *
	               	</spring:bind>
                </td>
                <td></td>
                <td></td>
                <td></td>
		    </tr>
		</tbody>
	</table>
		<div class="editActions">
			<button type="submit" name="submit" value="Submit"  title="Submit registration information">Submit</button> 
			<a href="javascript:history.go(-1);" title="Return to previous page" ><button type="button"  title="Cancel registration" >Cancel</button></a>
		</div>
    </form>
    </div>
</div>
