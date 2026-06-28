<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<script>
	setPageTitle("Deploy to My Tawala");
</script>

<h2><c:out value="${form.project.name}" /></h2>

<div class="section">
	<br />
	<form:form commandName="form" id="deployForm">
		<label>Project Name: </label>
		<br />
		<form:input path="projectName" maxlength="100" size="50" /> <form:errors path="projectName" cssClass="error"/>
		<br /><br />
		<label>Version Description: </label>
		<br />
		<form:textarea path="versionDescription" cols="50" rows="3" />
		<br /><br />
		<label>Project Theme: </label>
		<br />
		<form:select path="themePath">
			<c:if test="${! empty userDefinedThemes}">
			<optgroup label="My Themes">
				<form:options items="${userDefinedThemes}" itemLabel="name" itemValue="themeId" />
			</optgroup>
			</c:if>
			<optgroup label="Common Themes">
				<form:options items="${commonThemes}" itemLabel="name" itemValue="themeId" />
			</optgroup>
		</form:select><br />
		<br />
		<div class="buttons">
			<button type="submit">DEPLOY</button>
		</div>
	</form:form>
</div>