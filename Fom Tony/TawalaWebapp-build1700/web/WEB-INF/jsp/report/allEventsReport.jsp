<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<div class="section">
	<h2 class="sectionHeading">All Events Report</h2>
	<div class="sectionContent">
		<form:form commandName="form" id="allEventsReportForm">
			<table class="edit">
				<col style="width: 180px;" />
				<col />
				<tbody>
					<tr>
						<td class="label">
							Select report start date:
						</td>
						<td> 
							<div id="eventReportStart">
								<form:input path="startDate" />
								<img class="showCal" src="/images/calendar-icon-small.jpg" id="showEventReportStart" title="Click to view calendar" alt="View Calendar"/><br />
								<div id="eventReportStartCalContainer" class="cal"></div>
							</div>
						</td>
					</tr>
					<tr>
						<td class="label">
							Select report end date:
						</td>
						<td> 
							<div id="eventReportEnd">
								<form:input path="endDate" />							
								<img class="showCal" src="/images/calendar-icon-small.jpg" id="showEventReportEnd" title="Click to view calendar" alt="View Calendar"/><br />
								<div id="eventReportEndCalContainer" class="cal"></div>
							</div>
						</td>
					</tr>
					<tr>
						<td></td>
						<td></td>
					</tr>
					<tr>
						<td></td>
						<td><input type="submit" value="Generate Report" /></td>
					</tr>
				</tbody>
			</table>
		</form:form>
	</div>
</div>