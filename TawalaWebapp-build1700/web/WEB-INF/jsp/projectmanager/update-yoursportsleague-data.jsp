<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags"%>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script>
	setPageTitle("Update YourSportsLeague.com Data");
</script>

<div class="section">
	<h2>${userProject.name}</h2>
</div>

<div class="section">
	<h3 class="sectionHeading">
	</h3>
	<c:choose>
		<c:when test="${! empty  messages}">
			<div class="sectionContent">
			<b>Error(s) occurred during the update!</b> 
			</div>
			Please review the messages below: 
			<ul class="list insideDot">
				<c:forEach items="${messages}" var="message">
					<li><c:out value="${message}"/> </li>
				</c:forEach>
			</ul>
		</c:when>
		<c:otherwise>
			<div class="sectionContent">
			<b>Project data has been updated!</b> 
			</div>
		</c:otherwise>
	</c:choose>
</div>
