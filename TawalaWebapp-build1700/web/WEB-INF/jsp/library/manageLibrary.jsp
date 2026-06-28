<%@ page import="com.tawala.web.library.ManageLibraryController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<div class="section">
	<h2 class="sectionHeading">Manage Library</h2>
	<div class="sectionContent">
		<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>
	
		<form method="post" id="reindexLibrary">
			<input type="submit" name="<%= ManageLibraryController.PARAMETER_REINDEX_LIBRARY %>" value="Reindex Library" />	
		</form>
		<br />
	
		<form method="post" id="resetCategoryProjectCounts">
			<input type="submit" name="<%= ManageLibraryController.PARAMETER_RESET_CATEGORY_PROJECT_COUNT %>" value="Reset Category Project Counts" />	
		</form>
	</div>
</div>
	