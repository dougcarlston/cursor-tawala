<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="sectionContent">

<h2>Find User</h2>

<form:form method="POST" id="manageProjectGroupForm">
	<label class="bold">Query:</label> <span><form:errors path="userQuery" cssClass="error"/></span>
	<form:input path="userQuery" cssStyle="width: 50%;" /> <br />

	<jsp:include page="/WEB-INF/jsp/projectmanager/project-group/navigation.jsp" />

</form:form>

</div>
