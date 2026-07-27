<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="sectionContent">

	<h2>Step 3: Select Project</h2>
	
	<form:form method="POST" id="manageProjectGroupForm">
		<label class="bold">Select Project:</label><span><form:errors cssClass="error"/></span>
	
		<form:select path="userProjectId" items="${command.projects}" itemLabel="name" itemValue="id" />
		<br />
		<jsp:include page="/WEB-INF/jsp/projectmanager/project-group/navigation.jsp" />
	</form:form>

</div>
