<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<%@ page import="com.tawala.web.projectmanager.ViewProjectManagerDetailsController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<div id="commentEdit" class="section edit">
	<form method="POST" id="addVersionForm">
		<spring:bind path="form.projectName">
		        <input type="hidden" name="${status.expression}" value="<c:out value="${status.value}" />" />
        </spring:bind>

		<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>		
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="form" />
		</tiles:insert>
		
		<h3>Publish Project As New Version</h3> 
        <div class="note">Fields marked with a * are required</div>
		<br />
		
        <div class="line">
			<spring:bind path="form.versionDescription">
				<label>Description *
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
				</label>
				<input class="text" type="text" style="width: 95%;" maxlength="120" 
					name="${status.expression}" value="<c:out value="${status.value}" />" />
 	        </spring:bind>
	    </div>
	    <br />
		<div class="line">
	        <c:choose>
	        	<c:when test="${showCategories}">
						<b>Selected Project: </b>
						<spring:bind path="form.libraryProjectId">
							<input type="hidden" name="${status.expression}" id="libraryProjectId" value="${status.value}" />
							<span id="selectedProjectErrorMessage">
								<b><%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %></b>
							</span>
							<span id="selectedProjectName">
									<c:out value="${form.libraryProject.name}" />
							</span>
			            </spring:bind>
	        	</c:when>
		        <c:otherwise>
						<spring:bind path="form.projectName">
							<b>Project Name: </b>
			            	<c:out value="${status.value}" />
		    			</spring:bind>
		        </c:otherwise>
			</c:choose>
		</div>
		<br />
		<div class="line">
			Select a project from the list below 
			<span>
				<a href="javascript:tree.expandAll()" title="Expand all"><img src="/images/tree_expand.gif" /></a> 
				<a href="javascript:tree.collapseAll()" title="Collapse all"><img src="/images/tree_collapse.gif" /></a>
			</span>					
		</div>

		<div class="line">
			<c:if test="${showCategories}">
				<div id="categoryTree">
					<div id="treeDiv" class="treemenu" 
						style="height: 150px; width: 95%; overflow: scroll; border: 1px solid #cccccc;">
					</div>
				</div>
			</c:if>
		</div>

		<div class="editActions buttons">
			<c:url var="backToProjectDetailsLink" value="${urls.projectManagerProjectDetailView}">
				<c:param name="<%= ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME %>" value="${form.projectName}" />
			</c:url>
			<button type="submit" name="save" value="Save" title="Save new version">SAVE</button> 
			<button type="submit" name="cancel" onclick="parent.currentDialogObject.cancel(); return false;">CANCEL</button>
		</div>
	</form>
	<br />
</div>

	<script type="text/javascript">
        var tree;
        var categoryDetails = new Array();
        
        function treeInit() {
                tree = new YAHOO.widget.TreeView("treeDiv");
		        var nodeStack = new Array();

				<c:forEach var="library" items="${availableLibraries}">
				    var libraryNode = new YAHOO.widget.TextNode({
                        	label:'<spring:escapeBody javaScriptEscape="true">${library.description}</spring:escapeBody>' }, 
                        		tree.getRoot(), 
                        		true);
			        nodeStack[0] = libraryNode;
					<c:forEach var="category" items="${libraryCategories[library]}">
                        var node = new YAHOO.widget.TextNode({
                        	label:'<spring:escapeBody javaScriptEscape="true">${category.name}</spring:escapeBody> (${category.projectCount})' }, 
                        		nodeStack[${fn:length(category.allParents)}], 
                        		true);
                        		
                        nodeStack[${fn:length(category.allParents) + 1}] = node;
                        <c:forEach var="project" items="${category.projects}">
	                        var projectNode = new YAHOO.widget.TextNode({
    	                    	label:'<spring:escapeBody javaScriptEscape="true">${project.name}</spring:escapeBody>',
            	            	href: "javascript:onLabelClick('${project.id}', '<spring:escapeBody javaScriptEscape="true">${project.name}</spring:escapeBody>')"}, 
               	        		node, 
                        		false);
                        		projectNode.labelStyle = "projectName";
                        </c:forEach>
					</c:forEach>
				</c:forEach>
				
                tree.draw();
        }

		function onLabelClick(projectId, projectName) {
			var description = document.getElementById('selectedProjectName');
			description.innerHTML = projectName;

			var input = document.getElementById('libraryProjectId');
			input.value = projectId;
			
			var errorMessage = document.getElementById('selectedProjectErrorMessage');
			errorMessage.style.display = 'none';
			
			document.location.href = '#top';
		}
	</script>
