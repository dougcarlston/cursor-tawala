<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<h2>Create New Dashboard</h2>

<form:form id="createNewInstanceForm">
	<div>
		<form:errors path="*" cssStyle="color: red"></form:errors>
	</div>
	<p>
		<label class="block bold">Select the project:</label>
		<form:select path="libraryProject" items="${libraryProjects}" itemLabel="name" />
	</p>
	<p>
		<label class="block bold">User Name:</label>
		<form:input path="userName" size="50"/>
	</p>
	<p>
		<label class="block bold">Project Name:</label>
		<form:input path="projectName" size="50" />
	</p>
	<div class="buttons">
		<button type="submit">CREATE NEW INSTANCE</button>
	</div>
</form:form>