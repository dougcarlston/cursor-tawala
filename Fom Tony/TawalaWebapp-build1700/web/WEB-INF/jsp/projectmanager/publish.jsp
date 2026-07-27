
<jsp:directive.page import="com.tawala.web.library.PublishProjectToLibraryController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<c:set var="targetId" value="projectEdit" scope="request" /> 
<spring:bind path="form.category.*">
  <c:if test="${! empty status.errorMessages}">
		<c:set var="editProjectSectionStyle" value="none"/>
  </c:if>
</spring:bind>
		

<div class="section iframe">
	<tiles:insert definition="global.errors" ignore="false" >
		<tiles:put name="commandName" value="project" />
	</tiles:insert>
	
	
	<div id="projectEdit" class="details" style="display: <c:out value="${editProjectSectionStyle}" default="block" />">
		<h3>Publish As New Project</h3>
		<form:form method="post" name="publishProjectForm" commandName="form">
			<c:set var="projectIdParameter"><%= PublishProjectToLibraryController.PARAMETER_PROJECT_ID %></c:set>
			<input type="hidden" name="<%=PublishProjectToLibraryController.PARAMETER_PROJECT_ID %>" value="${param[projectIdParameter]}" />
			<jsp:include page="/WEB-INF/jsp/library/editProjectBody.jsp" flush="true"/>
			
			<div class="buttons">
				<button type="submit" name="submit" value="Submit">SUBMIT</button>
				<button type="submit" name="cancel" onclick="parent.currentDialogObject.cancel(); return false;">CANCEL</button>
			</div>
		</form:form>	
	</div>
	
	<jsp:include page="/WEB-INF/jsp/library/newCategory.jsp" flush="true"/>
</div>