<%@ page contentType="text/html" %>
<%@page import="com.tawala.web.admin.sportsdashboard.AssignTaskController"%>
<%@page import="com.tawala.web.admin.SwitchUserController"%>
<%@page import="com.tawala.web.admin.sportsdashboard.task.DefaultViewTaskController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>


<h2><c:out value="${userProject.name}" /></h2>
<div class="section">
	<div class="sectionContent">
		<p>
			<label class="bold">User:</label> <c:out value="${userProject.user.id}" />
		</p>
		<p>
			<label class="bold">Number of Registrations:</label> <c:out value="${registrationCount}" />
		</p>
		<p>
			<label class="bold">Date of last update:</label> <fmt:formatDate value="${lastDataRecordedDate}" type="both" pattern="yyyy-MM-dd" />
		</p>
		<p>
			<label class="bold">Link to Admin Menu:</label> <a href="${linkToAdminMenu}" target="_blank">${linkToAdminMenu}</a>
		</p>
	</div>
</div>

<c:if test="${! empty projectProcessInstance}">		
	<div class="section">
		<div class="sectionContent">
			<p>
				<label class="bold">Created:</label> <fmt:formatDate value="${projectProcessInstance.processInstance.start}" pattern="yyyy-MM-dd hh:mm a"/>
					by <c:out value="${creator}" />
			</p>
			<p>		
				<label class="bold">State(s):</label> 
				<c:forEach var="token" items="${tokens}">
					"<c:out value="${token.node.description}" />" (since <fmt:formatDate value="${token.nodeEnter}" pattern="MMMM dd hh:mm a"/>)
				</c:forEach>
			</p>
		</div>
	</div>
</c:if>

<c:if test="${empty projectProcessInstance}">		
	<div class="section">
		<div class="sectionContent">
			<b>This project doesn't have a workflow associated with it.</b>
		</div>
	</div>
</c:if>

<div class="section collapsible">
	<h3 class="sectionHeading">Additional Project Details</h3>
	<div class="sectionContent">
		<c:url value="${urls.switchUser}" var="projectDetailsURL">
			<c:param name="<%= SwitchUserController.USER_ID_PARAMETER %>" value="${userProject.user.databaseId}"/>
			<c:param name="<%= SwitchUserController.PROJECT_NAME_PARAMETER %>" value="${userProject.name}" />
		</c:url>
	
		<div style="float:right;"><a href="${projectDetailsURL}" />Go to Project Manager</a></div>
		
		<c:if test="${! empty creator}">
		</c:if>
		<p>
			<label class="bold">Start Date: </label><span><c:out value="${userProject.registrationStartDate}" /></span>
		</p>
		<p>
			<label class="bold">End Date: </label><span><c:out value="${userProject.registrationCloseDate}" /></span>
		</p>
		<p>
			<label class="bold">Purchase Order #: </label><span><c:out value="${userProject.purchaseOrderNumber}" /></span>
		</p>
		<p>
			<label class="bold">Invoice #: </label><span><c:out value="${userProject.invoiceNumber}" /></span>
		</p>
		<p>
			<label class="bold">Invoice Date: </label><span><c:out value="${userProject.registrationInvoiceDate}" /></span>
		</p>
		<p>
			<label class="bold">Cost Per Player: </label><span><c:out value="${userProject.registrationFee}" /></span>
		</p>
	</div>
</div>
	
 
<c:if test="${! empty projectProcessInstance}">		
	<div class="section collapsible">
		<h3 class="sectionHeading">Tasks</h3>
		<div class="sectionContent">
			<table class="list left">
				<thead>
					<tr>
						<th>Name</th>
						<th>Assigned To</th>
						<th>Assigned To Group</th>
						<th>&nbsp;</th>
					</tr>
				</thead>
				<tbody>
			<c:forEach var="projectTaskInstance" items="${tasks}">
				<c:url var="viewTaskURL" value="${projectTaskInstance.viewTaskURL}">
					<c:param name="<%= DefaultViewTaskController.TASK_ID_PARAMETER %>" value="${projectTaskInstance.taskInstance.id}"/>
				</c:url>
			
					<tr>
						<td><c:out value="${projectTaskInstance.taskInstance.description}"/></td>
						<td><c:out value="${projectTaskInstance.taskInstance.actorId}"/></td>
						<td><c:forEach var="group" items="${projectTaskInstance.taskInstance.pooledActors}" varStatus="iterationStatus">
							<c:if test="${iterationStatus.index > 0}">, </c:if>
							<c:out value="${group.actorId}"/>
						</c:forEach>
						</td>
						<td><a href="${viewTaskURL}">Perform Action</a></td>
					</tr>
			</c:forEach>
				<tbody>
			</table>
		</div>
	</div>
</c:if>

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
		var toolTipText = series.displayName + " for " + item.day;
		toolTipText += "\n" + YAHOO.example.formatCurrencyAxisLabel( item[series.yField] );
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

<div class="section collapsible closed">
	<h3 class="sectionHeading">History</h3>
	<div class="sectionContent">
		<table class="list left">
			<c:forEach var="logEntry" items="${logs}">
				<tr>
					<td><c:out value="${logEntry}"/></td>
					<td><fmt:formatDate value="${logEntry.date}" pattern="yyyy-MM-dd hh:mm a"/></td>
				</tr>
			</c:forEach>
		</table>
	</div>
</div>
