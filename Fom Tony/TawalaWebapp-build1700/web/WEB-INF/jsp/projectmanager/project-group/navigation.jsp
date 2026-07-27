<%@page import="com.tawala.web.projectmanager.projectgroup.AddProjectToGroupController"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<br />
<div class="buttons">
	<c:if test="${pageNumber > 0}">
		<button  type="submit" name="<%=AddProjectToGroupController.PARAM_TARGET%>${pageNumber - 1}"> &lt;&lt; PREVIOUS </button>
	</c:if>
	
	<button type="submit" name="<%=AddProjectToGroupController.PARAM_CANCEL%>">CANCEL</button>
	
	<c:if test="${pageNumber < totalPageCount - 1}">
		<button type="submit" name="<%=AddProjectToGroupController.PARAM_TARGET%>${pageNumber + 1}"> NEXT &gt;&gt; </button>
	</c:if>
	
	<c:if test="${pageNumber == totalPageCount - 1}">
		<button type="submit" name="<%=AddProjectToGroupController.PARAM_FINISH%>">FINISH</button>
	</c:if>
</div>