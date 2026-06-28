<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<h2>Project Deleted</h2>

<div class="backArrow buggybox"><a href="${urls.librarySearch}"><img src="/images/arrow_left.gif"/> Library</a></div>
 
<div id="projectDetails" class="details">
	<h4>Project "<c:out value="${project.name}" />" has been deleted.</h4>
</div>