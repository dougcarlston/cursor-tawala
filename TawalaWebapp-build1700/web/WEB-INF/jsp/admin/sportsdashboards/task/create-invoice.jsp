<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<br />

<div>
<c:set var="poNumber" value="${command.processTask.userProject.purchaseOrderNumber}" />
<c:choose>
	<c:when test="${empty poNumber}">There is no PO number associated with the project.</c:when>
	<c:otherwise>Use PO number <b><c:out value="${poNumber}"/></b> to generate the invoice.</c:otherwise>
</c:choose>

</div>

<div>
<c:choose>
<c:when test="${! empty createInvoiceURL}">
<a href="${createInvoiceURL}" target="_blank">Follow this link to create an invoice for this project.</a> <br />
</c:when>
<c:otherwise>
Unable to find the link to the project that creates invoices! Please notify the development team. 
</c:otherwise>
</c:choose>
</div>

<br />


Invoice Number: <form:input path="invoiceNumber"/>

<br />
<br />