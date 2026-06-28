<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script>
	setPageTitle("Revert Project Change");
</script>

<div id="projectEdit" class="details">
	<form method="POST" action="${urls.libraryRevertEvent}" id="restoreProject">
		<spring:bind path="form.eventId">
		<input type="hidden" name="${status.expression}" value="${status.value}" />
		</spring:bind>
		
		<br />

		<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>		
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="form" />
		</tiles:insert>
		 
        <div class="note">Fields marked with a * are required</div>
		<br />
		
		<div class="section">
			<h3>Project Name: <c:out value="${project.name}" /></h3>
			
		    <table class="edit">
		    	<col style="width: 380px;" />
		    	<col />
	            <tr>
					<td>
	            		Check this box to confirm that you would like to revert the event:
	            		<br /><br />
	            		<spring:message message="${event.reversionDescription}"/>: 
	            	</td>
	                <td class="left"> 
						<spring:bind path="form.confirmRevert">
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						<input type="checkbox" name="${status.expression}" /> *
	    	           	</spring:bind>
	                </td>
			    </tr>
			</table>
		</div>
		<div class="editActions">
			<input type="image" name="save" value="Perform the action" src="/images/submit-button.gif" /> 
			<a href="javascript:history.go(-1);"><img src="/images/cancel-button.gif" /></a> 
		</div>
	</form>
	<br /><br />
</div>