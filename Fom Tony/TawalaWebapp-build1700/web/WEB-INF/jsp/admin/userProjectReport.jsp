<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<h2 class="sectionHeading">User Project Report</h2>
	<div class="sectionContent">
		<p>Click the link below to generate a User Project Report</p>
	
		<div class="note">This process will take a few minutes to run.</div>
	
		<a href="${urls.reportAllUserProjects}" id="allUserProjectsReportLink">Generate User Project Report</a>
	</div>
</div>