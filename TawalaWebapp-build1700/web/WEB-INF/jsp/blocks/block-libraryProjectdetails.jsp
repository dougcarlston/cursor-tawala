<%@ page contentType="text/html"%>
<%@ page import="com.tawala.web.library.ModifyProjectController"%>
<%@ page import="com.tawala.web.library.RateProjectController"%>
<jsp:directive.page
	import="com.tawala.web.library.TestDriveWithExplanationController" />
<jsp:directive.page
	import="com.tawala.web.library.DownloadLibraryProjectVersionController" />
<jsp:directive.page
	import="com.tawala.web.library.OneClickTestDriveController" />
<jsp:directive.page
	import="com.tawala.web.library.CloneAndCustomizeController" />
<jsp:directive.page
	import="com.tawala.web.library.DeployToMyTawalaController" />
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="tiles"
	uri="http://jakarta.apache.org/struts/tags-tiles"%>

<div class="block">
	<div class="content">
		<div>
			<c:if
				test="${project.vetted && project.latestVersion.project.customizable}">
				<c:url var="customizationUrl"
					value="${urls.libraryCustomizeAndDeploy}">
					<c:param
						name="<%=CloneAndCustomizeController.PARAMETER_PROJECT_ID%>"
						value="${project.id}" />
				</c:url>
				<a href="${customizationUrl}" id="startCustomization"><img
						src="/images/start-button.gif" />
				</a>
				<br />
			</c:if>
			<c:if test="${! empty project.videoURL}">
				<a href="#"
					onclick="window.open('${project.videoURL}','Video','width=820,height=590,toolbar=no,menubar=no,status=no,location=no,resizeable=yes,scrollbars=yes'); return false;">
					<img src="/images/seedemo-button.gif" /> </a>
			</c:if>
		</div>
		<br />
		<c:if test="${! empty user}">
			<div>
				<span class="label">Original Author:</span><span>${project.authorId}</span>
			</div>
			<div>
				<span class="label">Category:</span>
				<span> <c:forEach var="category"
						items="${project.category.allParents}">
						<c:out value="${category.name}" /> &rarr; 
						</c:forEach> <c:out value="${project.category.name}" /> </span>
			</div>
			<div>
				<span class="label">Last updated:</span><span><fmt:formatDate
						value="${project.lastUpdatedDate}" type="date" dateStyle="short" />
				</span>
			</div>
			<c:if test="${user.status.allowedToViewDesigner}">
				<div>
					<span class="label">Downloads:</span><span>${project.downloadCount}</span>
				</div>
			</c:if>
			<c:if test="${project.category.library.showCloneCount}">
				<div>
					<span class="label">Times used:</span>
					<span>${project.cloneCount}</span>
				</div>
			</c:if>
		</c:if>
		<%--
			<div>
				<span class="label">Test Drives:</span><span>${project.testDriveCount}</span>
			</div>
--%>

		<div>

			<c:if test="${! empty user}">
				<c:choose>
					<c:when
						test="${project.versions[0].project.startingPointCount == 1}">
						<c:forEach var="form" items="${project.versions[0].project.forms}"
							varStatus="status">
							<c:if test="${form.startingPoint}">
								<c:url var="oneClickTestDriveUrl"
									value="${urls.libraryOneClickTestDrive}">
									<c:param
										name="<%=OneClickTestDriveController.PROJECT_ID_PARAMETER_NAME%>"
										value="${project.id}" />
									<c:param
										name="<%=OneClickTestDriveController.VERSION_ID_PARAMETER_NAME%>"
										value="${project.versions[0].id}" />
									<c:param
										name="<%=OneClickTestDriveController.FORM_NAME_PARAMETER%>"
										value="${form.name}" />
								</c:url>
							</c:if>
						</c:forEach>
						<div class="bigButton">
							<a id="oneClickTestDriveLink" href="${oneClickTestDriveUrl}"
								target="testdrive"><img
									src="/images/testdrivelatest-button-180.gif" />
							</a>
						</div>
					</c:when>
					<c:otherwise>
						<div class="bigButton">
							<a id="oneClickTestDriveLinkPopup"
								href="javascript:showTestDriveUrls();"><img
									src="/images/testdrivelatest-button-180.gif" />
							</a>
						</div>
					</c:otherwise>
				</c:choose>

				<c:url var="versionTestDriveLink"
					value="${urls.libraryTestDrivePreparation}">
					<c:param
						name="<%=TestDriveWithExplanationController.PROJECT_ID_PARAMETER_NAME%>"
						value="${project.id}" />
					<c:param
						name="<%=TestDriveWithExplanationController.VERSION_ID_PARAMETER_NAME%>"
						value="${project.versions[0].id}" />
				</c:url>
				<a href="${versionTestDriveLink}">Test Drive Details...</a>
			</c:if>

		</div>

		<c:if test="${currentUserAllowedToDownload}">
			<br />
			<div>
				<c:url var="versionDownloadLink"
					value="${urls.libraryProjectVersionDownload}">
					<c:param
						name="<%= DownloadLibraryProjectVersionController.PROJECT_ID_PARAMETER_NAME %>"
						value="${project.id}" />
					<c:param
						name="<%= DownloadLibraryProjectVersionController.VERSION_ID_PARAMETER_NAME %>"
						value="${project.versions[0].id}" />
				</c:url>
				<div class="bigButton">
					<a href="${versionDownloadLink}" id="downloadLink"><img
							src="/images/downloadlatest-button-180.gif" />
					</a>
				</div>
			</div>
		</c:if>

		<c:if test="${user.administrator || originalUser.administrator}">
			<br />
			<div class="buttons">
				<c:url var="deployToMyTawalaURL" value="${urls.libraryProjectDeployToMyTawala}">
					<c:param name="<%= DeployToMyTawalaController.PARAMETER_PROJECT_ID %>" value="${project.id}" />
				</c:url>
				<a class="dark" href="${deployToMyTawalaURL}" id="deployToMyTawalaLink">DEPLOY TO MY TAWALA</a>
			</div>
		</c:if>

		<br class="clr" /><br />
		<div>
			<div class="label">
				Community Rating
			</div>
			<div>
				<c:choose>
					<c:when test="${project.rating == 0}">
							Not Yet Rated
						</c:when>
					<c:otherwise>
						<c:set var="usersRated"
							value="${fn:length(project.ratingsByUsers)}" />
						<img src="/images/${project.rating}star.gif" border="none"> 
							(by ${usersRated}
							<c:choose>
							<c:when test="${usersRated == 1}">user</c:when>
							<c:otherwise>users</c:otherwise>
						</c:choose> )
						</c:otherwise>
				</c:choose>
			</div>
		</div>
		<c:if test="${! empty user}">
			<br />
			<div>
				<div class="label">
					Your Rating
				</div>
				<div class="buttons" id="ratingButtons" >
					<c:set var="previousRating" value="${project.ratingsByUsers[user.id]}" />
					<c:choose>
						<c:when test="${! empty previousRating }">
							<img src="/images/${previousRating.value}star.gif" style="padding-bottom: 6px;">
							<a class="dark" href="javascript:showRatingSection();">EDIT YOUR RATING</a>
						</c:when>
						<c:otherwise>
							<a class="dark" href="javascript:showRatingSection();">RATE PROJECT</a>
						</c:otherwise>
					</c:choose>
				</div>
			</div>
		</c:if>

		<div class="clr" style="display: none;" id="ratingSection">
			<form id="editRating" method="post"
				action="${urls.libraryRateProject}">
				<div>
					<input type="hidden"
						name="<%=RateProjectController.PARAMETER_PROJECT_ID%>"
						value="${project.id}" />
					<c:forEach var="rating" items="1,2,3,4,5">
						<div>
							<input type="radio"
								name="<%=RateProjectController.PARAMETER_RATING%>"
								value="${rating}"
								<c:if test="${rating == previousRating.value}">checked="checked"</c:if> />
							&nbsp;
							<img src="/images/${rating}star.gif">
						</div>
					</c:forEach>
				</div>
				<div>
					<!-- 	Explanation of rating: <input class="text" type="text" name="<%=RateProjectController.PARAMETER_TEXT%>" value="<c:out value="${previousRating.text}" />" 
								size="70" maxlength="120" / -->
					<input type="image" src="/images/save-button.gif" value="Save" />
					<input type="image" src="/images/cancel-button.gif"
						onclick="javascript:hideRatingSection(); return false;"
						value="Cancel" />
				</div>
			</form>
		</div>

		<br />

		<%--
			<div>
				<div class="label">Short Description</div>
				<div>${project.shortDescription}</div>
			</div>
			<br />
			<div>
				<div class="label">Detailed Description</div>
				<div>${project.longDescription}</div>
			</div>
			<br />
--%>

		<div>
			<c:if test="${currentUserAllowedToEdit}">
				<br /><br />
				<div class="buttons">
					<a class="dark" id="linkToEditProject"
						href="${urls.libraryEditProject}?<%=ModifyProjectController.PARAMETER_PROJECT_ID%>=${project.id}">
						EDIT PROJECT INFO
					</a>
				</div>
			</c:if>
			<br />
		</div>
	</div>
</div>


<div id="oneClickTestDrivePanel"
	style="visibility:hidden; text-align: left;">
	<div class="hd">
		<div class="tl"></div>
		<span>Select Starting Point</span>
		<div class="tr"></div>
	</div>
	<div class="bd">
		<p>
			This project contains several starting points. Please select one:
		</p>
		<ul>
			<c:forEach var="form" items="${project.versions[0].project.forms}"
				varStatus="status">
				<c:if test="${form.startingPoint}">
					<c:url var="oneClickTestDriveUrl"
						value="${urls.libraryOneClickTestDrive}">
						<c:param
							name="<%=OneClickTestDriveController.PROJECT_ID_PARAMETER_NAME%>"
							value="${project.id}" />
						<c:param
							name="<%=OneClickTestDriveController.VERSION_ID_PARAMETER_NAME%>"
							value="${project.versions[0].id}" />
						<c:param
							name="<%=OneClickTestDriveController.FORM_NAME_PARAMETER%>"
							value="${form.name}" />
					</c:url>
					<li style="text-align: left; padding-left: 2em;">
						<a href="${oneClickTestDriveUrl}" target="testdrive"> <img
								src="/images/testdrive-icon.gif"
								title="Test drive this version of the project" class="smallIcon"
								width="18" height="18" /> <c:out value="${form.name}" /> </a>
					</li>
				</c:if>
			</c:forEach>
		</ul>
	</div>
	<div class="ft"></div>
</div>

<script type="text/javascript" defer="true">
	var ratingButtons = document.getElementById('ratingButtons');
	var ratingSection = document.getElementById('ratingSection');
	var ratingList = document.getElementById('ratingList');
	
	function showRatingSection() {
		ratingSection.style.display = "block";
		ratingButtons.style.display = "none";
	}
	function hideRatingSection() {
		ratingSection.style.display = "none";
		ratingButtons.style.display = "block";
	}

	function showRatingList() {
		ratingList.style.display = "block";
		
		section = document.getElementById('ratingListLink');
		section.href = 'javascript:hideRatingList();';
		section.innerHTML = 'Hide all ratings';
	}
	function hideRatingList() {
		ratingList.style.display = "none";

		section = document.getElementById('ratingListLink');
		section.href = 'javascript:showRatingList();';
		section.innerHTML = 'Show all ratings';
	}
	
	var oneClickTestDrivePanel;
	
	function init() {
		oneClickTestDrivePanel = new YAHOO.widget.Panel("oneClickTestDrivePanel", { 
				width:"30em", 
				fixedcenter: true, 
				constraintoviewport: true, 
				underlay:"none", 
				close:true, 
				modal:true,
				visible:false, 
				effect:{effect:YAHOO.widget.ContainerEffect.FADE, duration:.25},
				draggable:true } );
								
		oneClickTestDrivePanel.render(document.body);
	}
	
	function showTestDriveUrls() {

		oneClickTestDrivePanel.show();
	}
	
	YAHOO.util.Event.addListener(window, "load", init);
</script>



