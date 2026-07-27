<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="block">
	<div class="content">
		<h3>Categories</h3>
		<ul class="menuList" id="menu1">
			<li><a name="all" class="selected" href="">All</a></li>
			<c:forEach var="mapEntry" items="${projectsMap}" varStatus="iterationByCategoryStatus">
				<c:set var="category" value="${mapEntry.key}" />
						<li><a name="${category.name}" href=""><c:out value="${category.name}" /></a></li>
			</c:forEach>
		</ul>
	</div>
</div>
