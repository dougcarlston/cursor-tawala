<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<script>
	setPageTitle("${project.name}");
</script>

<p><a href="/projectmanager/projectdetail?projectName=${project.name}">[return to project details]</a></p>
<br />
<div class="section" >
	<h3>Invite Other People to This Project</h3>
	<p>
	There are three ways of sending invitations to this project to other people.
	</p>
	<br />

	<h5>Option 1: Send an email using your email program.</h5>
	<p>
		Please select a link to the form you would like to invite people to:
	</p>
	<br />
	<c:forEach var="invitationEntry" items="${invitations}" varStatus="status" >
		<c:set var="form" value="${invitationEntry.key}"/>
		<c:set var="invitation" value="${invitationEntry.value}"/>
		<a href="${invitation.mailToURL}" id="mailToLink${status.count}">
			<img src="/images/red-bullet-arrow-right.gif" /> Invitation to <c:out value="${form.name}" />
		</a>
		<br />
	</c:forEach>
	<br />

	<h5>Option 2: Send the URLs of the project starting points using your preferred method.</h5>
	<p>
		You can cut and paste the URLs below:
	</p>
	<table class="list ruler">
		<col style="width:8em;" />
		<col />
		<thead>
			<tr>
				<th style="text-align: left">Form</th>
				<th>URL</th>
			</tr>
		</thead>
		<tbody>
	<c:forEach var="urlEntry" items="${project.entryPointURLs}" >
		<c:set var="form" value="${urlEntry.key}"/>
		<c:set var="url" value="${urlEntry.value}"/>
			<tr>
				<td style="text-align: left"><c:out value="${form.name}" /></td>
				<td style="text-align: left"><c:out value="${url}" /></td>
			</tr>
	</c:forEach>
		</tbody>
	</table>

	<br />
	
	<h5>Option 3: Send an email using Tawala.</h5>
	<p>
		Please select a link to the form you would like to invite people to:
	</p>
	<br />
	<c:forEach var="urlEntry" items="${project.entryPointURLs}" >
		<c:set var="form" value="${urlEntry.key}"/>
		<c:set var="url" value="${urlEntry.value}"/>
		<a href="javascript:alert('coming soon');">
			<img src="/images/red-bullet-arrow-right.gif" /> Invitation to <c:out value="${form.name}" />
		</a>
		<br />
	</c:forEach>
	
	<br />
</div>
