<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

<script>
	setPageTitle("${project.name}");
</script>

<h3>Test Drive Project</h3>

<div id="projectDetails">
	<p>
	You are about to test drive version <em>${version.versionNumber}</em> of <b><c:out value="${project.name}" /></b> project. This will
	give you the opportunity to see how the project works before downloading it.
	</p>
	<div class="note">
		When test driving a project the data you enter is not saved after you exit. Some projects have features 
		that require data to be saved so they will not fully work in the test drive environment
	</div>
	<p>
	The test drive consists of two steps:
	</p>
	
	<div class="section">
	<h5>Step 1</h5>
	<p>
	The first step is to run the project. A new browser window will open and you will 
	see the first page of the project. You might want to do a couple of runs to "simulate" several users and 
	collect more data which you will view in step 2. 
	</p>
	<p>
	<c:choose>
		<c:when test="${runnableProject.project.startingPointCount > 1}">
		This project has ${runnableProject.project.startingPointCount} "entry points" listed below. Start running the project by selecting one of them: 
		</c:when>
		<c:otherwise>
		This project has one "entry points". Start running the project by selecting it. 
		</c:otherwise>
	</c:choose>
	</p>
	<br />

	<c:forEach var="form" items="${runnableProject.project.forms}" varStatus="status">
		<c:if test="${form.startingPoint}">
			<a id="testDriveLink${status.count}" href="<tawala:linkToProjectTestDrive 
								project="${runnableProject}" 
								form="${form}" />" target="testdrive"><c:out value="${form.name}" /></a><br />
		</c:if>
	</c:forEach>
	
	</div>
	
	<div class="section">
		<h5>Step 2</h5>
		<div>
		In the second part you will be able to look at the reports being generated as if you were the 
		owner/administrator of this project.
		
		<div class="note">Coming soon...</div>
		</div>
	</div>
</div>
