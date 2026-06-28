<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<div class="block">

<c:choose>
<c:when test="${! empty detailedMessage}">
	<h3>An error occurred!</h3>
	<p>
		<c:out value="${detailedMessage}"></c:out> 
    </p>
</c:when>

<c:otherwise>
	<h3>We are very sorry!</h3>
	<p>
		The page you tried to access is not currently available, but we are working on it. 
        <br /><br />
        Thanks,<br />
        The Tawala Team
    </p>
</c:otherwise>
</c:choose>
    
</div>
