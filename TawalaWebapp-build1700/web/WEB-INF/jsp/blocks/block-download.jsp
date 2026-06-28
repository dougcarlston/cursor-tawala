<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="block">
	<div class="content">
		<h3>Tawala Designer</h3>
		<div><span class="label">Version:</span><span>Alpha build </span><span id="buildNumber">85</span></div>
		<!-- div><span class="label">Date:</span><span>8/18/2006</span></div -->
		<br />
		<c:choose>
			<c:when test="${! empty user}">
				<div><a href="/Tawala.exe"><img src="images/template/download-tawala-button.gif" width="190" height="25" alt="Download Tawala Designer" /></a></div>
			</c:when>
			<c:otherwise>
				To download Tawala Designer you need to <a href="${urls.userRegistration}">become a registered user</a> or 
					<a href="/login">log in</a>.
			</c:otherwise>
		</c:choose>
	</div>
</div>

<script type="text/javascript">
	Tawala.getDownloadPageBuildVersion();
</script>
