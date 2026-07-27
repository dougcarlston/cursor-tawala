<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>


<form:form>
	<div class="section">
		<h2 class="sectionHeading">Manage Urgent Message</h2>
		<div class="sectionContent">
			<span>Please make sure the message is a properly formatted HTML snippet!</span><br/>
			<form:textarea path="message.text" cols="80" rows="4"/>
			
			<br />
			<form:checkbox path="removeMessage" /> Remove Message?
			<div class="buttons">
				<button type="submit">SAVE</button>
			</div>
			
		</div>
		
		<br />
		<div>Suggested message text:<br />
		Warning. The SportsDashboards site will be taken offline for maintenance at 2:30 PM Pacific Time. You need to finish any work you are doing by that time. We will be down for approximately 5 minutes. We apologize for any inconvenience.
		</div>
	</div>

</form:form>