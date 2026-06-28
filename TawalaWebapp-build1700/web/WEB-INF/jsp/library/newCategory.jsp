
<jsp:directive.page import="com.tawala.project.library.CategoryEditor"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<script type="text/javascript">
     function showCategorySection(sectionId, targetId) {
		section = document.getElementById(sectionId);
		section.style.display = 'block';
		section = document.getElementById(targetId);
		section.style.display = 'none';
	}     
     function cancelEdit(sectionId, targetId) {
		section = document.getElementById(sectionId);
		section.style.display = 'none';
		section = document.getElementById(targetId);
		section.style.display = 'block';
	}     
</script>

<spring:bind path="form.category.*">
  <c:if test="${! empty status.errorMessages}">
		<c:set var="newCategorySectionStyle" value="block"/>
  </c:if>
</spring:bind>

<div id="newCategory" class="section edit" style="display: <c:out value="${newCategorySectionStyle}" default="none" />">
	<form method="POST">
		<h3>Create New Category</h3>
        <div class="note">Fields marked with a * are required</div>
		<div class="content">		
	    	    <div class="line">
					<spring:bind path="form.category.name">
						<label>Name * 
							<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						</label>
						<input type="text" size="25" maxlength="40" name="${status.expression}" value="<c:out value="${status.value}" />" />
				    </spring:bind>
			    </div>
			    <div class="line">
				    <label>Parent Category *</label>
						<spring:bind path="form.category.parent">
		    	            <select name="${status.expression}">
		        	        	<c:forEach var="library" items="${availableLibraries}">
		    	            		<optgroup label="<c:out value="${library.description}" />">
										<option value="<%= CategoryEditor.TOP_LEVEL_CATEGORY_PREFIX %>${library.id}">Top Level Category</option>
										<c:forEach var="nextCategory" items="${libraryCategories[library]}">
											<option value="${nextCategory.id}">
											<c:forEach items="${nextCategory.allParents}">&nbsp;&nbsp;&nbsp;&nbsp;</c:forEach>
											<c:out value="${nextCategory.name}" /></option>
										</c:forEach>
									</optgroup>
								</c:forEach>
							</select>
						</spring:bind>
				</div>
				<div class="line">
						<spring:bind path="form.category.description">
							<label>Description *
								<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
							</label>
							<input type="text" size="25" maxlength="60"
									name="${status.expression}" value="<c:out value="${status.value}" />" />
					    </spring:bind>
				</div>
			<br />
			<div class="buttons">
				<spring:bind path="form.categoryCreationAction">
				<button name="${status.expression}" value="Create" >SAVE</button>
				</spring:bind>
				<button onClick="javascript:cancelEdit('newCategory', '<c:out value="${targetId}" />'); return false;" 
						value="Cancel">CANCEL</button> 
			</div>
		
		</div>
	</form>
</div>
