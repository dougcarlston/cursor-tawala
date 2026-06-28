<jsp:directive.page import="com.tawala.web.library.LibrarySearchController"/>
<jsp:directive.page import="com.tawala.web.library.EditCategoryController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div  class="block">
	<div class="content">
		<h3>Categories</h3>

		<c:url var="searchLibraryUrl" value="${searchURL}">
			<c:param name="<%= LibrarySearchController.LIBRARY_ID_PARAMETER %>" value="${currentLibrary.id}" />
		</c:url>
	
		<c:if test="${user.status.allowedToUpdateLibraryProjects}">					
		<div><a class="action" href="recentchanges">Show Recent Changes</a></div>
		</c:if>
		<div id="categoryTree">
			<form name="mainForm" action="javscript:;" style="margin:0px; padding:0px;">
				<div>
					<a class="action" href="${searchLibraryUrl}" title="Show projects in all categories">Show All Categories (${totalProjects})</a>
				</div>
				
				<div class="controls" style="background-color: #dddddd; width: 96%; border: 1px solid #cccccc;" >
					<a href="javascript:tree.expandAll()" title="Expand all"><img src="/images/tree_expand.gif" /></a> 
					<a href="javascript:tree.collapseAll()" title="Collapse all"><img src="/images/tree_collapse.gif" /></a>
				</div>
				<br class="clr" />
				<div id="treeDiv" class="yui-skin-sam" ></div>
			</form>
		</div>
		<br />
		<div class="buttons" style="height: 1.5em;">
			<c:if test="${user.status.allowedToUpdateLibraryProjects}">					
				<c:url var="editCategoriesUrl" value="${urls.libraryCategories}">
					<c:param name="<%= EditCategoryController.PARAMETER_LIBRARY_ID %>" value="${currentLibrary.id}"/>
				</c:url>
				<a class="dark" href="${editCategoriesUrl}"> EDIT CATEGORIES </a>
			</c:if>
		</div>
	</div>
</div>
<script type="text/javascript">
	var tree;
	var categoryDetails = new Array();
      
	function treeInit() {
		tree = new YAHOO.widget.TreeView("treeDiv");
        var nodeStack = new Array();
        nodeStack[0] = tree.getRoot();

		<c:set var="maximumExpandedDepth" value="1" />		
		<c:forEach var="category" items="${categories}">
			var node = new YAHOO.widget.TextNode({
				label:'<spring:escapeBody javaScriptEscape="true">${category.name}</spring:escapeBody> (${category.projectCount})',
				id:'${category.id}',
				href: "javascript:onLabelClick(${category.id})"}, 
				nodeStack[${fn:length(category.allParents)}], 
				${category.depth < maximumExpandedDepth || showAsExpandedNodes[category.id] == true});
                      		
			nodeStack[${fn:length(category.allParents) + 1}] = node;
		</c:forEach>
		
		tree.draw();
	}

	function onLabelClick(categoryId) {
		var input = document.getElementById('searchCategoryId');
		input.value = categoryId;
		
		var form = document.getElementById('searchByCategoryForm');
		form.submit();
	}
</script>

<form method="get" action="${searchURL}" id="searchByCategoryForm">
	<input type="hidden" id="searchCategoryId" name="<%= LibrarySearchController.CATEGORY_ID_PARAMETER %>" />
	<input type="hidden" name="<%= LibrarySearchController.LIBRARY_ID_PARAMETER %>" value="${currentLibrary.id}" />
</form>
    	