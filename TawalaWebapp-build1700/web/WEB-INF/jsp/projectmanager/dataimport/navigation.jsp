<%@page import="com.tawala.web.projectmanager.dataimport.ImportDataController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<div class="editActions">
	<c:if test="${pageNumber > 0}">
		<input type="image" src="/images/previous-button.gif" name="<%=ImportDataController.PARAM_TARGET%>${pageNumber - 1}" value=" &lt;&lt; Previous " />
	</c:if>
	
	<input type="image" src="/images/cancel-button.gif" name="<%=ImportDataController.PARAM_CANCEL%>" value="Cancel" />
	
	<c:if test="${pageNumber < totalPageCount - 1}">
		<input type="image" src="/images/next-button.gif" name="<%=ImportDataController.PARAM_TARGET%>${pageNumber + 1}" value=" Next &gt;&gt; " />
	</c:if>
	
	<c:if test="${pageNumber == totalPageCount - 1}">
		<input type="image" src="/images/finished-button.gif" name="<%=ImportDataController.PARAM_FINISH%>" value="Finish" />
	</c:if>
</div>