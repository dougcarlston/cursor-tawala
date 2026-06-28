<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="sectionContent">

<h2>Step 2: Select User</h2>

	<form:form method="POST" id="manageProjectGroupForm">
		<label class="bold">Select user from list</label><span><form:errors cssClass="error"/></span><br />
		
		<form:select path="userId" items="${command.users}" itemLabel="id" itemValue="databaseId" />
		<br />
		<jsp:include page="/WEB-INF/jsp/projectmanager/project-group/navigation.jsp" />
	</form:form>

</div>
