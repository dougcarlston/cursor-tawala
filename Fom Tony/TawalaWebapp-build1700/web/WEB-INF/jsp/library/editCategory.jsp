<%@ page import="com.tawala.web.library.EditCategoryController" %>
<%@ page import="com.tawala.web.library.ViewProjectDetailsController" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>


<div id="displayCategory" class="section details">

	<h3><c:out value="${category.name}" /></h3>
	<div class="status">
		Library: <c:out value="${library.description}" />
		<br />
		<c:forEach var="parent" items="${category.allParents}"><c:out value="${parent.name}" /> &rarr; </c:forEach> <c:out value="${category.name}" />
	</div>
	<h5>Description</h5>
	<div class="content"><c:out value="${category.description}" /></div>

	<h5>Projects</h5>
		<c:choose>
		<c:when test="${empty projects}">
			<div class="content">
		        <div id="manageProjects">
					<div>Currently there are no projects for this category.</div>
				</div>
			</div>
		</c:when>
		<c:otherwise>
	        <div class="content">
	            <div class="status">
	            	<span>Total Projects: ${fn:length(projects)}</span>
	            </div>
	           
	            <table class="list sortable ruler">
	                <colgroup>
	                    <col style="width: 280px;"/>
	                    <col style="width: 80px;"/>
	                    <col style="width: 80px;"/>
	                </colgroup>
	                <thead>
	                    <tr>
	                    <th class="left">Name</th>
	                    <th>User</th>
	                    <th>Date</th>
	                    </tr>
	                </thead>
	                <tfoot>
	                    <tr>
	                    <td></td>
	                    <td></td>
	                    <td></td>
	                    </tr>
	                </tfoot>
	                <tbody>
	                <c:forEach var="project" items="${projects}">
	                	<c:url var="projectDetailLink" value="${urls.libraryProjectDetailView}">
	                		<c:param name="<%= ViewProjectDetailsController.PARAMETER_ID %>" value="${project.id}"/>
	                	</c:url>
	                    <tr>
	                        <td class="left"><a href="${projectDetailLink}" title="Project Details"><c:out value="${project.name}" /></a></td>
	                        <td class="user"><a href="${projectDetailLink}" title="Project Details"><c:out value="${project.authorId}" /></a></td>
	                        <td class="date">
		                        <fmt:formatDate value="${project.submittedDate}" type="date" dateStyle="short" />
		                    </td>
	                    </tr>
	                </c:forEach>
	                </tbody>
	            </table>
	        </div>
	   	</c:otherwise>
		</c:choose>
		<c:if test="${! empty user}" >
			<div class="buttons">
				<button type="button" onclick="showCategorySection('newCategory')">ADD CATEGORY</button>
				<c:if test="${! category.readOnly}">
					<button type="button" onclick="showCategorySection('editCategory')">EDIT CATEGORY</button>
					<button type="button" onclick="showCategorySection('deleteCategory')">DELETE CATEGORY</buttton>
				</c:if>
			</div>
	</c:if>
	<br />
</div>

<c:if test="${! empty user}" >
	<div id="deleteCategory" class="section edit" style="display: none;">
		<h3>Delete Category</h3>
		Are you sure you would like to delete the category:
		<div class="name">
			<c:forEach var="parent" items="${category.allParents}"><c:out value="${parent.name}" /> &rarr;</c:forEach> <c:out value="${category.name}" />
		</div>
		<br />
		<div class="buttons">
			<form method="POST" action="${urls.libraryEditCategory}" id="deleteCategoryForm">
				<input type="hidden" name="<%= EditCategoryController.PARAMETER_CATEGORY_ID %>" value="${category.id}" />
				<button type="submit" name="<%= EditCategoryController.PARAMETER_DELETE 
						%>" value="Delete" >DELETE</button> 
			</form>
			<form style="display: inline;">
				<button type="button" onClick="javascript:cancelEdit('deleteCategory');" value="Cancel" title="Cancel">CANCEL</button> 
			</form>
		</div>
	</div>
	
	<div id="newCategory" class="section" style="display: none;">
		<form method="POST" id="newCategoryForm" action="${urls.libraryEditCategory}">
			<input type="hidden" name="<%= EditCategoryController.PARAMETER_LIBRARY_ID 
					%>" value="${library.id}" />
		<h3>Add a new category</h3>
	    <div class="note">Fields marked with a * are required</div>
		    <table class="edit">
		    	<col style="width: 120px;" />
		    	<col />
	            <tr>
					<td class="label">
	            		Category Name:
	            	</td>
	                <td> 
						<input type="text" size="60" maxlength="40" 
							name="<%= EditCategoryController.PARAMETER_NAME %>" value="" /> *
	                </td>
			    </tr>
			    <tr>
			        <td class="label">
			        	Parent Category:
			        </td>
	                <td>
			                <select name="<%= EditCategoryController.PARAMETER_PARENT_ID %>">
			                  <option value="">Top Level Category</option>
			                  <c:forEach var="nextCategory" items="${categories}">
			                    <option value="${nextCategory.id}" <c:if test="${nextCategory.id == category.id}">selected</c:if> >
			                    	<c:forEach items="${nextCategory.allParents}">&nbsp;&nbsp;&nbsp;&nbsp;</c:forEach>
				                    <c:out value="${nextCategory.name}" /></option>
			                  </c:forEach>
			                </select> *
	                </td>
			    </tr>
			    <tr>
			        <td class="label">
			        	Description:
			        </td>
		            <td>
						<input type="text" size="40" maxlength="60"
								name="<%= EditCategoryController.PARAMETER_DESCRIPTION %>" value="" /> *
	                </td>
			    </tr>
	      </table>
	      <br />
		<div class="buttons">
			<button type="submit" name="<%= EditCategoryController.PARAMETER_NEW %>" value="Create">SAVE</button> 
			<button type="button" onClick="javascript:cancelEdit('newCategory');">CANCEL</button>
		</div>
		</form>
	</div>
	
	<div id="editCategory" class="section" style="display: none;">
		<form method="POST" action="${urls.libraryEditCategory}" id="editCategoryForm">
			<input type="hidden" name="<%= EditCategoryController.PARAMETER_CATEGORY_ID 
					%>" value="${category.id}" />
			<input type="hidden" name="<%= EditCategoryController.PARAMETER_LIBRARY_ID 
					%>" value="${library.id}" />
		<h3>Edit Category: ${category.name}</h3>				
	    <div class="note">Fields marked with a * are required</div>
		    <table class="edit">
		    	<col style="width: 120px;">
		    	<col >
	            <tr>
					<td class="label">
	            		Name:
	            	</td>
	                <td> 
							<input type="text" size="25" maxlength="40" name="<%= EditCategoryController.PARAMETER_NAME 
								%>" value="<c:out value="${category.name}" />" /> *
	                </td>
			    </tr>
			    <tr>
			        <td class="label">
			        	Parent Category:
			        </td>
	                <td>
							<span class="info">
				                <select name="<%= EditCategoryController.PARAMETER_PARENT_ID %>">
				                  <option value="">Top Level Category</option>
				                  <c:forEach var="nextCategory" items="${categoriesWithoutCurrentSubtree}">
				                    <option value="${nextCategory.id}" <c:if test="${nextCategory.id == category.parent.id}">selected</c:if>>
				                    	<c:forEach items="${nextCategory.allParents}">&nbsp;&nbsp;&nbsp;&nbsp;</c:forEach>
					                    <c:out value="${nextCategory.name}" /></option>
				                  </c:forEach>
				                </select> *
							</span> 
	                </td>
			    </tr>
			    <tr>
			        <td class="label">
			        	Description:
			        </td>
		            <td>
							<input type="text" size="32" maxlength="60" name="<%= 
								EditCategoryController.PARAMETER_DESCRIPTION %>" value="<c:out value="${category.description}" />" /> *
	                </td>
			    </tr>
	      </table>
	      <br />
			<div class="buttons">
				<button type="submit" name="<%= EditCategoryController.PARAMETER_UPDATE %>" value="Update">SAVE</button> 
				<button type="button" onClick="javascript:cancelEdit('editCategory');">CANCEL</button> 
			</div>
		</form>
	</div>
</c:if>
