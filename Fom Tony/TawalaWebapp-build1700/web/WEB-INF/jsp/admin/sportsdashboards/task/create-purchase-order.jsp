<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<br />

<div>
<c:choose>
<c:when test="${! empty createPurchaseOrderURL}">
Follow this link to create a purchase order for this project: <br />
<div align="center"><a href="${createPurchaseOrderURL}" target="_blank">${createPurchaseOrderURL}</a></div>
</c:when>
<c:otherwise>
Unable to find the link to the project that creates purchase orders! Please notify the development team. 
</c:otherwise>
</c:choose>
</div>

<br />

<label class="bold block">Purchase order number:</label>
	<form:input path="purchaseOrderNumber"/><br />
<label class="bold block">Registration start date:</label>
	 <form:input path="registrationStartDate"/>
<label class="bold block">Cost per player:</label>
	 <form:input path="costPerPlayer"/>
<br />
<br />