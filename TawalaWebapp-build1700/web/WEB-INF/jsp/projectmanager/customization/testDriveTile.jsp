<jsp:directive.page import="com.tawala.web.projectmanager.UserProjectTestDriveController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<div class="yui-gd">
	<div class="yui-u first">
		<div class="section customizeLeft">
			<h3>Test</h3>
			<div id="previewStatus" class="status"></div>
			<div id="previewContent">
				<p>
				In order to get a better idea of how your webapp will work when complete you use the buttons 
				below to take it for a "test drive". Each button will run a different form in the project and allow you to see the flow. 
				</p>
				<br />
				<p>
				Any data entered will not be saved.
				</p>
				<br />
				
				<c:forEach var="form" items="${userProject.project.formsSuitableForTestDrive}">
					<c:url var="oneClickTestDriveUrlWithReset" value="${urls.userProjectTestDrive}">
						<c:param name="<%=UserProjectTestDriveController.PARAMETER_PROJECT_RANDOM_ID%>" 
								value="${userProject.uniqueRandomId}" />
						<c:param name="<%=UserProjectTestDriveController.PARAMETER_FORM_NAME%>"
								value="${form.name}" />
						<c:param name="<%=UserProjectTestDriveController.PARAMETER_PROJECT_FORCE_REINITIALIZATION %>"
								value="yes" />
					</c:url>
					<c:url var="oneClickTestDriveUrlWithoutReset" value="${urls.userProjectTestDrive}">
						<c:param name="<%=UserProjectTestDriveController.PARAMETER_PROJECT_RANDOM_ID%>" 
								value="${userProject.uniqueRandomId}" />
						<c:param name="<%=UserProjectTestDriveController.PARAMETER_FORM_NAME%>"
								value="${form.name}" />
					</c:url>
					<a class="roundButton" href="#" id="testDriveLink_${form.name}"
						onclick="Tawala.Customize.launchTestDrive('${oneClickTestDriveUrlWithReset}', '${oneClickTestDriveUrlWithoutReset}'); this.blur(); return false;">
						<span>Test Drive ${form.name}</span>
					</a>
					<br /><br />
				</c:forEach>

				<br /><br /><br /><br />
			</div>
		</div>
	</div>
	<div class="yui-u">
	</div>
</div>
