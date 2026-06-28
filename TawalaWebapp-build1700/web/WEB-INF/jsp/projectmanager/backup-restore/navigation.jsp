<jsp:directive.page import="org.springframework.web.servlet.mvc.AbstractWizardFormController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<div class="editActions buttons">
	<c:if test="${pageNumber > 0}">
		<button type="submit" name="<%=AbstractWizardFormController.PARAM_TARGET%>${pageNumber - 1}"> &lt;&lt; PREVIOUS </button>
	</c:if>
	
	<button type="submit" name="cancel" onclick="parent.currentDialogObject.cancel(); return false;">CANCEL</button>
	
	<c:if test="${pageNumber < totalPageCount - 1}">
		<button type="submit" name="<%=AbstractWizardFormController.PARAM_TARGET%>${pageNumber + 1}" value="Next"> NEXT &gt;&gt; </button>
	</c:if>
	
	<c:if test="${pageNumber == totalPageCount - 1}">
		<button type="submit" name="<%=AbstractWizardFormController.PARAM_FINISH%>" value="Finish">  RESTORE </button>
	</c:if>
</div>