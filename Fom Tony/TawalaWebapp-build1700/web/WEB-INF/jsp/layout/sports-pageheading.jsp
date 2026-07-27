<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

<tiles:importAttribute name="pageName" />

<c:choose>
	<c:when test="${empty(pageName) || pageName == 'home'}">
		<c:set var="homePageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'tawalaDesigner'}">
		<c:set var="tawalaDesignerPageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'news'}">
		<c:set var="newsPageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'designer'}">
		<c:set var="designerPageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'faq'}">
		<c:set var="faqPageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'mytawala'}">
		<c:set var="mytawalaPageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'library'}">
		<c:set var="libraryPageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'about'}">
		<c:set var="aboutPageClass" value="selected" />
	</c:when>
	<c:when test="${pageName == 'admin'}">
		<c:set var="adminClass" value="selected" />
	</c:when>
</c:choose>



<div id="headingLogo">
	<a href="/sportsdashboards/home">
		<img src="/images/sports/sportsdashboards-logo.jpg" width="264" height="26" 
			alt="SportsDashboards - The Easiest Way to Register, Manage and Communicate with your Teams">
	</a>
</div>
<div id="topRightCorner">
	&nbsp;&nbsp;
</div>

<div id="headingStatus">
		<div id="statusLoggedIn" style="display: none;">
		<c:choose>
			<c:when test="${! empty user.id}">
				Welcome back, <span class="userName"><c:out value="${user.id}" /></span>.
			</c:when>
			<c:otherwise>
				Welcome back!
			</c:otherwise>
		</c:choose>	
			<a href="${urls.logout}">Logout</a>
			<c:if test="${! empty originalUser }">
			or <a href="${urls.restoreOriginalUser}" id="restoreOriginalUserLink">Become <span class="userName"><c:out value="${originalUser.id}"/></span> again</a>
			</c:if>
		</div>
		
		<div id="statusLoggedOut" style="display: none;">
			Hello. <a class="headingStatusLink" href="<tawala:loginUrl />" id="linkToLogin">Login</a> to see your projects. 
<!--			New to Tawala? <a class="headingStatusLink" href="${urls.userInitialRegistration}">Sign up for FREE account here</a>-->
		</div>
</div>
		
<script>
	// Code to deal with changing the login message in the page heading
	var emptyUser = ${empty user};
	var userName = "${user.id}";
	var loggedInObj = document.getElementById("statusLoggedIn");
	var loggedOutObj = document.getElementById("statusLoggedOut");
	changeLoginState();
	
	function changeLoginState(){	
		if(emptyUser) {
			loggedOutObj.style.display = "block";
			loggedInObj.style.display = "none";
		}else{
			loggedOutObj.style.display = "none";
			loggedInObj.style.display = "block";
		} 
	}
</script>

<%-- menu order reversed from display --%>
<div id="headingMenu">
	<ul>
		<c:if test="${user.administrator}">
			<li class="<c:out value="${adminClass}" />" >
				<a href="/administration/users">ADMIN</a>
			</li>
		</c:if>

		<li class="<c:out value="${aboutPageClass}" />" >
			<a href="/sportsdashboards/info">ABOUT</a>
		</li>

<%--
		<c:if test="${! empty user}">
			<c:if test="${user.status.allowedToViewDesigner}">
				<li class="<c:out value="${designerPageClass}" />" >
					<a href="${urls.designer}" id="designerLink">DESIGNER</a>
				</li>
			</c:if>
			<li class="<c:out value="${newsPageClass}" />" >
				<a href="/news">NEWS</a>
			</li>
		</c:if>
--%>

		<li class="<c:out value="${faqPageClass}" />" >
			<a href="/sportsdashboards/faq">HELP</a>
		</li>

<%--
		<li class="<c:out value="${libraryPageClass}" />" >
			<a href="${urls.librarySearch}">LIBRARY</a>
		</li>
--%>

		<c:if test="${! empty user}">
			<li class="<c:out value="${mytawalaPageClass}" />" >
				<a href="${urls.projectManagerView}">MY TAWALA</a>
			</li>
		</c:if>

		<li class="<c:out value="${homePageClass}" />" >
			<a href="/sportsdashboards/home">HOME</a>
		</li>
	</ul>
</div>
