<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<c:set var="tawalaRestoreBack" value="/app/mytawala.html" />
<c:if test="${! empty restoredProject}">
	<c:url var="tawalaRestoreBack" value="/app/mytawala-project.html">
		<c:param name="name" value="${restoredProject.name}" />
	</c:url>
</c:if>
<c:if test="${empty restoredProject && ! empty fileUpload.originalProject}">
	<c:url var="tawalaRestoreBack" value="/app/mytawala-project.html">
		<c:param name="name" value="${fileUpload.originalProject.name}" />
	</c:url>
</c:if>
<a id="tawalaRestoreBack" href="${tawalaRestoreBack}" hidden>Back</a>
<script>
	function tawalaRestoreLeave() {
		var back = document.getElementById("tawalaRestoreBack");
		location.href = back ? back.href : "/app/mytawala.html";
		return false;
	}
</script>
