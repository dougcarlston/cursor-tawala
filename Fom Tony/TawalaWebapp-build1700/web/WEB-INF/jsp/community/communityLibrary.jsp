<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<jsp:directive.page import="com.tawala.web.library.LibrarySearchController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<div class="section">
	<h2>Community Library</h2>
	<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>

	<%@ include file="/WEB-INF/jsp/library/searchLibrary.jsp" %>
</div>
<!-- end main content -->

<script type="text/javascript" charset="utf-8">
	currentPage = "communityLibrary";
	$E.on(window, "load", setMenuHighlight);
</script>
