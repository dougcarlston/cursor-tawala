<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.ListProjectWorkflowsInAParticularStateController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.ViewProjectWorkflowDetailsController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<h2>Email Details</h2>

<div class="section">
	<h3 class="sectionHeading">Email Activity By Project</h3>
	<div class="sectionContent">
		<table class="list ruler left small">
			<colgroup>
			</colgroup>
			<thead class="center dark">
				<tr>
					<th>Project</th>
					<th># Emails</th>
					<th># Email 5 days</th>
				</tr>
			</thead>
			<tfoot>
				<tr>
					<td class="left">TOTAL</td>
					<td class="right">xxx</td>
					<td class="right">xxxx</td>
				</tr>
			</tfoot>
			<tbody>
				<tr>
					<td class="left">&lt;&lt; PROJECT NAME &gt;&gt;</td>
					<td class="right">xxx</td>
					<td class="right">xxx</td>
				</tr>
			</tbody>
		</table>		

	</div>
</div>

