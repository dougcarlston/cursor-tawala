<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<br />
<c:choose>
<c:when test="${! empty prepareForReleaseURL}">
Follow this link to prepare the project for release: <br />
<div align="center"><a href="${prepareForReleaseURL}" target="_blank">${prepareForReleaseURL}</a></div>
</c:when>
<c:otherwise>
Unable to find the link to the prepare for release form. Make sure to clean the data before the admin allows access to the users of this project! 
</c:otherwise>
</c:choose>
<br />