<jsp:directive.page import="com.tawala.web.library.EditCategoryController"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div  class="block">
	<div class="content">
		<h3>Categories</h3>
		<div id="categoryTree" class="yui-skin-sam" >
		<div class="topBar"></div>
			<form name="mainForm" action="javscript:;" style="margin:0px; padding:0px;">
				<div class="controls">
					<a href="javascript:tree.expandAll()" title="Expand all"><img src="/images/tree_expand.gif" /></a> 
					<a href="javascript:tree.collapseAll()" title="Collapse all"><img src="/images/tree_collapse.gif" /></a>
				</div>
				<br />
				<div id="treeDiv" class="yui-skin-sam"></div>
			</form>
		</div>
	</div>
</div>

<script type="text/javascript">
	var tree;
	var categoryDetails = new Array();
	var libraryId = ${currentLibrary.id};
        
	function treeInit() {
		tree = new YAHOO.widget.TreeView("treeDiv");
		var nodeStack = new Array();
		nodeStack[0] = tree.getRoot();

		<c:forEach var="category" items="${categories}">
			<c:set var="expandThisNode" value="${category.id == currentCategory.id}" />
			var url = "${urls.libraryEditCategory}";
						
			var node = new YAHOO.widget.TextNode({
						label:'<spring:escapeBody javaScriptEscape="true">${category.name}</spring:escapeBody> (${category.projectCount})',
						id:'${category.id}',
						href: "javascript:onLabelClick(${category.id}, '" + url + "')"}, 
						nodeStack[${fn:length(category.allParents)}], 
						false);
                        		
			<c:if test="${expandThisNode == 'true'}" >
				for(var i=1; i < nodeStack.length; ++i) {
					nodeStack[i].expand();
				}
			</c:if>
			nodeStack[${fn:length(category.allParents) + 1}] = node;
		</c:forEach>
				
		tree.draw();
	}


	function onLabelClick(id, targetUrl) {			
		setStatus("Loading...");
		getEditScreenForCategory(id, targetUrl, onFinish);
	}
		
	function onFinish() {
		Tawala.Tables.init()
		hideStatus();
	}

	function setStatus (message){
		if(document.getElementById('ajaxStatus') == null){
			var body = document.getElementsByTagName("body")[0];
			var div = document.createElement("div");
			div.id = 'ajaxStatus';
			body.appendChild(div);
		}
		var node = document.getElementById('ajaxStatus');
		node.innerHTML = message;
		node.style.display = "block";
	}
	
	function hideStatus(){
		var node = document.getElementById('ajaxStatus');
		node.style.display = "none";
	}
		
		
	function getEditScreenForCategory(id, targetUrl, onCompleteCallback) {
		var url = targetUrl + "?<%= EditCategoryController.PARAMETER_CATEGORY_ID %>=" + id + "&<%= EditCategoryController.PARAMETER_LIBRARY_ID %>=" + libraryId;
		var transInfo = {
				success:  handleResponse,
				argument: { id: id, callback: onCompleteCallback},
				failure:  handleFailure
		};

		YAHOO.util.Connect.asyncRequest( 'GET', url, transInfo, null );
	}

	function handleResponse(o) {
		if (o && o.responseText && o.responseText.length > 1) {
			section = document.getElementById("categoryDetails");
			section.innerHTML = o.responseText;
			categoryDetails[o.argument.id] = o.responseText;
		}

        // Notify the tree that we are done processing the data
        o.argument.callback();
     }        

     function handleFailure(o) {
		section = document.getElementById("categoryDetails");
		section.innerHTML = "Error occured while loading category details";
		o.argument.callback();
     }

</script>
    	