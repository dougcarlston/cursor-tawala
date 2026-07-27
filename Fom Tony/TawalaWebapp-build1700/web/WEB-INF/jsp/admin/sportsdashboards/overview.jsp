<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.ListProjectWorkflowsInAParticularStateController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.ViewProjectWorkflowDetailsController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<h2>Overview</h2>

<div class="section">
	<h3 class="sectionHeading">Pipeline - Potentially Billable Projects</h3>
	<div class="sectionContent">
		<table class="list sortable ruler left small">
			<colgroup>
			</colgroup>
			<thead class="center dark">
				<tr>
					<th>User</th>
					<th>Project</th>
					<th>Launch</th>
					<th># Reg</th>
					<th>Close</th>
					<th>Last Reg</th>
					<th># Reg 5 days</th>
					<th>Billable</th>
					<th>Price</th>
				</tr>
			</thead>
			<tfoot>
				<tr>
					<td class="left">TOTAL</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td class="center"><fmt:formatNumber value="${totalRegistrations}" pattern="###,##0" /></td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td>&nbsp;</td>
					<td class="right"><fmt:formatNumber value="${totalBillable}" pattern="$###,###,##0.00" /></td>
					<td>&nbsp;</td>
				</tr>
			</tfoot>
			<tbody>
			<c:forEach var="item" items="${projectStats}">
				<c:url var="viewProcessDetailsURL" value="${urls.adminViewProjectWorkflowDetails}">
					<c:param name="<%= ViewProjectWorkflowDetailsController.PROJECT_ID_PARAMETER %>" 
							value="${item.userProjectId}" />
				</c:url>
				<tr>
					<td class="left"><c:out value="${item.userId}" /></td>
					<td class="left"><a href="${viewProcessDetailsURL}"><c:out value="${item.projectName}" /></a></td>
					<td class="center"><fmt:formatDate value="${item.registrationOpen}" pattern="MM/dd/yy"/></td>
					<td class="center"><fmt:formatNumber value="${item.registrationCount}" pattern="###,##0" /></td>
					<td class="center"><fmt:formatDate value="${item.registrationClose}" pattern="MM/dd/yy"/></td>
					<td class="center"><fmt:formatDate value="${item.lastRegistrationDate}" pattern="MM/dd/yy"/></td>
					<td class="center"><fmt:formatNumber value="${item.registrationCountLast5Days}" pattern="###,##0" /></td>
					<td class="right"><fmt:formatNumber value="${item.billableAmount}" pattern="$###,###,##0.00" /></td>
					<td class="right"><fmt:formatNumber value="${item.registrationFee}" pattern="$###,###,##0.00" /></td>
				</tr>
			</c:forEach>
			</tbody>
		</table>		
	</div>
</div>

<div class="section">
	<h3 class="sectionHeading">Project Summary</h3>
	<div class="sectionContent">
		<table class="list sortable ruler left">
			<colgroup>
				<col style="width: 75%;" />
				<col style="width: 25%;" />
			</colgroup>
			<thead class="left dark">
				<tr>
					<th>Position in project life cycle</th>
					<th class="right">Number of projects</th>
				</tr>
			</thead>
			<tfoot>
				<tr>
					<td colspan="2" class="left">Total projects: ${projectStatesStats.totalProcessCount}</td>
				</tr>
			</tfoot>
			<tbody>
			<c:forEach var="entry" items="${projectStatesStats.processStateCount}">
				<c:url var="listProcessesInStateURL" value="${urls.adminListProjectWorkflowsInAParticularState}">
					<c:param name="<%= ListProjectWorkflowsInAParticularStateController.STATE_NAME_PARAMETER %>" value="${entry.key}" />
				</c:url>
				<tr>
					<td><a href="${listProcessesInStateURL}" ><c:out value="${entry.key}" /></a></td>
					<td class="right">${entry.value}</td>
				</tr>
			</c:forEach>
			</tbody>
		</table>		
	</div>
</div>

<div class="section">
	<h3 class="sectionHeading">Registrations</h3>
	<div class="sectionContent">
		<div class="chart" id="regChart">Unable to load Flash content. The YUI Charts Control requires Flash Player 9.0.45 or higher. You can download the latest version of Flash Player from the <a href="http://www.adobe.com/go/getflashplayer">Adobe Flash Player Download Center</a>.</div>
	</div>
</div>

<div class="section">
	<h3 class="sectionHeading">Emails</h3>
	<div class="sectionContent">	
		<div class="chart" id="emailChart">Unable to load Flash content. The YUI Charts Control requires Flash Player 9.0.45 or higher. You can download the latest version of Flash Player from the <a href="http://www.adobe.com/go/getflashplayer">Adobe Flash Player Download Center</a>.</div>
	</div>		
</div>


<script type="text/javascript">

	YAHOO.widget.Chart.SWFURL = "/scripts/yui/build/charts/assets/charts.swf";
	
//--- setup the data

	// Registartion data
	Tawala.config.weeklyRegistrations =
	[
		<c:forEach items="${registrationTrend}" var="trendEntry" varStatus="status">
			<c:set var="currentDate" value="${trendEntry.key}" />
			<c:set var="entryCount" value="${trendEntry.value}" />
				{ 	day: "<fmt:formatDate value="${currentDate}" pattern="MM/dd"/>", 
					registrations: ${entryCount}}<c:if test="${! status.last}">,</c:if>
		</c:forEach>
	];

	var myRegSource = new YAHOO.util.DataSource( Tawala.config.weeklyRegistrations );
	myRegSource.responseType = YAHOO.util.DataSource.TYPE_JSARRAY;
	myRegSource.responseSchema =
	{
		fields: [ "day", "registrations" ]
	};

	// Email data
	Tawala.config.weeklyEmails =
	[
		<c:forEach items="${emailTrend}" var="trendEntry" varStatus="status">
			<c:set var="currentDate" value="${trendEntry.key}" />
			<c:set var="entryCount" value="${trendEntry.value}" />
				{ 	day: "<fmt:formatDate value="${currentDate}" pattern="MM/dd"/>", 
					emails: ${entryCount}}<c:if test="${! status.last}">,</c:if>
		</c:forEach>
	];

	var myEmailSource = new YAHOO.util.DataSource( Tawala.config.weeklyEmails );
	myEmailSource.responseType = YAHOO.util.DataSource.TYPE_JSARRAY;
	myEmailSource.responseSchema =
	{
		fields: [ "day", "emails" ]
	};


//--- Define and draw charts
	
	// Axis and data tip config
	Tawala.config.getDataTipText = function( item, index, series )
	{
		var toolTipText = item[series.yField] + " " +series.displayName;
		toolTipText += "\n" + item.day;
		return toolTipText;
	}

	var currencyAxis = new YAHOO.widget.NumericAxis();
	currencyAxis.minimum = 0;
	currencyAxis.labelFunction = Tawala.config.formatCurrencyAxisLabel;

	Tawala.config.formatCurrencyAxisLabel = function( value )
	{
		return YAHOO.util.Number.format( value,
		{
			prefix: "",
			thousandsSeparator: ",",
			decimalPlaces: 0
		});
	}


	// Registration chart
	var seriesDef = 
	[
		{ displayName: "Registrations", yField: "registrations" }
	];

	var myRegistrationschart = new YAHOO.widget.LineChart( "regChart", myRegSource, 
	{
		series: seriesDef,
		xField: "day",
		dataTipFunction: Tawala.config.getDataTipText
	});

	
	// Email chart
	var seriesDef = 
		[
			{ displayName: "Emails", yField: "emails",
				style: 
				{
					color: 0xcc4444
				}
			 }
		];

	var myEmailchart = new YAHOO.widget.LineChart( "emailChart", myEmailSource,
	{
		series: seriesDef,
		xField: "day",
		dataTipFunction: Tawala.config.getDataTipText
	});
	
</script>
