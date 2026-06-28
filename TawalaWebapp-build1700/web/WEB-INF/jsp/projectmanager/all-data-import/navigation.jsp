<jsp:directive.page import="com.tawala.web.projectmanager.alldataimport.AllDataImportController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<div class="editActions buttons">
	<br />
	<c:if test="${pageNumber > 0}">
		<button type="submit" name="<%=AllDataImportController.PARAM_TARGET%>${pageNumber - 1}" value=" &lt;&lt; Previous "> &lt;&lt; PREVIOUS </button>
	</c:if>
	
	<button type="submit" name="cancel" onclick="parent.currentDialogObject.cancel(); return false;">CANCEL</button>
	
	<c:if test="${pageNumber < totalPageCount - 1}">
		<button type="submit" name="<%=AllDataImportController.PARAM_TARGET%>${pageNumber + 1}" value=" Next &gt;&gt; " > NEXT &gt;&gt; </button>
	</c:if>
	
	<c:if test="${pageNumber == totalPageCount - 1}">
		<button type="submit" name="<%=AllDataImportController.PARAM_FINISH%>" value="Finish"> FINISH </button>
	</c:if>
</div>