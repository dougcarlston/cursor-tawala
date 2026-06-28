<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script>
	setPageTitle("${project.name}");
</script>

<h3>Add Comment</h3>

<div id="projectEdit" class="section details">
	<form method="POST" id="modifyCommentForm">
		<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>		
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="comment" />
		</tiles:insert>
		 
        <div class="note">Fields marked with a * are required</div>
		<br />
		
		    <table>
		    	<col style="width: 120px;" />
		    	<col />
	            <tr>
					<td class="label">Comment:</td>
	                <td class="left"> 
						<spring:bind path="comment.text">
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						<input class="text" type="text" size="80" maxlength="80" 
							name="${status.expression}" value="<c:out value="${status.value}" />" /> *
	    	           	</spring:bind>
	                </td>
			    </tr>
	</table>
	<div class="buttons">
		<button type="submit" name="save" value="Save">SAVE</button> 
		<button type="button" onclick="window.location ='${urls.libraryProjectDetailView}?<%= ViewProjectDetailsController.PARAMETER_ID %>=${project.id}'">CANCEL</button>
	</div>
	</form>
</div>

