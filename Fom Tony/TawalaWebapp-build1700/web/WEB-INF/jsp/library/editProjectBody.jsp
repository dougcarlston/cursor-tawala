<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

		<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>		
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="form" />
		</tiles:insert>

		<div id="editProject" class="section edit">
        	<div class="note">Fields marked with a * are required</div>
			<div class="line">
				<spring:bind path="form.project.name">
					<label>Project Name *
					<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %></label>
	    	    </spring:bind>
	    	    <form:input path="project.name" maxlength="${meta.libraryProjectNameMaxLength}" cssClass="text" size="50" />
			</div>
			<c:if test="${user.administrator}">
	            <div class="line">
					<label>Project Author</label>
	                <form:input path="project.authorId" cssClass="text" maxlength="20" size="50"/>
			    </div>
	            <div class="line">
					<label>Project featured on the home page?</label>
	                Featured?:<form:checkbox path="project.featured" />&nbsp;&nbsp;&nbsp;&nbsp;Priority: <form:input path="project.featuredOrder" cssClass="text" maxlength="4" size="4" />
				</div>					
	            <div class="line">
					<label>Project Icon URL</label>
	                <form:input path="project.iconURL" cssClass="text" />
			    </div>
	            <div class="line">
					<label>Project Video URL</label>
	                <form:input path="project.videoURL" cssClass="text" />
			    </div>
	            <div class="line">
					<label>Snapshot Tile Path:</label>
	                <form:input path="project.snapshotTile" cssClass="text" size="50" maxlength="100"/>
			    </div>
			</c:if>
			<div class="line">
					<spring:bind path="form.project.category">
				        <label>Category *
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %></label>
						
						<c:choose>
							<c:when test="${form.savedCategoryId != 0}">
								<c:set var="selectedCategoryId" value="${form.savedCategoryId}"/>
							</c:when>
							<c:when test="${form.project.category.id != 0}">
								<c:set var="selectedCategoryId" value="${form.project.category.id}"/>
							</c:when>
						</c:choose>
	    	            <select name="${status.expression}">
	        	        	<option value="">Please select ...</option>

                	<c:forEach var="library" items="${availableLibraries}">
                		<optgroup label="<c:out value="${library.description}" />">
	                 		<c:forEach var="category" items="${libraryCategories[library]}">
	                    		<option value="${category.id}" <c:if test="${category.id == selectedCategoryId}">selected</c:if> >
	                    		<% //--- This is how indentation is done. Not pretty, but works. %>
	                    		<c:forEach items="${category.allParents}">&nbsp;&nbsp;&nbsp;&nbsp;</c:forEach>
		                    	<c:out value="${category.name}" /></option>
	                  		</c:forEach>
	                  	</optgroup>
                	</c:forEach>
	                	</select> 
	                	<a id="addCategoryLink" class="" 
	                			href="javascript:showCategorySection('newCategory', 
	                			'<c:out value="${targetId}" />')" title="Add new category" >
	                			<img  src="/images/silk/add.gif" 
	                			alt="Add" title="Add new category" class="smallIcon" /></a>
					</spring:bind>
			    </div>
			    <div class="line">
					<spring:bind path="form.project.shortDescription">
				        <label>Short Description *
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %></label>
						
						<input class="text" type="text" style="width: 85%;" maxlength="${meta.libraryProjectShortDescriptionMaxLength}" 
								name="${status.expression}" value="<c:out value="${status.value}" />"/> *
	        		</spring:bind>
	        	</div>
			    <div class="line">
			        <label>Detailed Description</label>
					<spring:bind path="form.project.longDescription">
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						<textarea class="text" style="width: 85%;" name="${status.expression}" rows="4" cols="65"><c:out value="${status.value}" /></textarea>
			        </spring:bind>
			    </div>
		</div>
		