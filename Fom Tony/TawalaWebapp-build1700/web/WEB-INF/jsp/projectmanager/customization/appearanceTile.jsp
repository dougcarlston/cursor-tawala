<jsp:directive.page import="com.tawala.web.projectmanager.ChangeProjectThemeController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<div class="yui-gd">
	<div class="yui-u first">
		<div class="section customizeLeft">
			<h3>Appearance</h3>
			<div id="appearanceStatus" class="status"></div>
			<div id="appearanceContent">
				<form:form id="themeChangeForm" action="${urls.changeProjectTheme}" commandName="userProject">
				<input type="hidden" name="<%= ChangeProjectThemeController.PARAMETER_PROJECT_ID %>" 
					value="${userProject.uniqueRandomId}"/>
				<p>Choose a theme from the list below to change the appearance of your project.</p>
				<br />
				
				<form:select id="themeSelect" path="project.theme.themeId" 
					items="${availableThemes}" itemValue="path" itemLabel="name" 
					cssStyle="width: 100%;" onchange="Tawala.Customize.updateTheme(this.value);" />
				
				</form:form>
			</div>
			<br /><br /><br /><br />
		</div>
	</div>
	<div class="yui-u">
	</div>
</div>