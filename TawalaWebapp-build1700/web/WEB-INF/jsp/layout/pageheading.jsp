<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
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
	<a href="/home">
		<img src="/images/template/tawala-logo-white.gif" width="136" height="24" />
	</a>
</div>
<div id="topRightCorner">
	&nbsp;&nbsp;
</div>

<div id="headingStatus">
		<div id="statusLoggedIn" style="display: none;">
		<c:choose>
			<c:when test="${! empty user.id}">
				<c:set var="shownName" value="${user.id}" />
				<c:if test="${fn:startsWith(user.id, 'user_') and not empty user.firstName}">
					<c:set var="shownName" value="${user.firstName}" />
				</c:if>
				Welcome back, <span class="userName"><c:out value="${shownName}" /></span>.
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
			Hello. <a class="headingStatusLink" href="/app/login.html" id="linkToLogin">Login</a> to see your projects. New to Tawala? <a class="headingStatusLink" href="/app/signup.html" id="linkToSignUp">Sign up for FREE account here</a>
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
<script>window.TAWALA_PUBLIC_SITE=true;</script>
<script src="/app/js/chrome.js?v=20261124-clerk16"></script>
<script>
	function openClerkAccount(action) {
		if (!window.TawalaChrome) return;
		if (action === "signUp" && TawalaChrome.openSignUp) TawalaChrome.openSignUp();
		else if (TawalaChrome.openSignIn) TawalaChrome.openSignIn();
	}
	var loginLink = document.getElementById("linkToLogin");
	if (loginLink) loginLink.onclick = function () { openClerkAccount("signIn"); return false; };
	var signUpLink = document.getElementById("linkToSignUp");
	if (signUpLink) signUpLink.onclick = function () { openClerkAccount("signUp"); return false; };
	var logoutLinks = document.querySelectorAll('a[href="/logout"], a[href="${urls.logout}"]');
	for (var i = 0; i < logoutLinks.length; i++) {
		logoutLinks[i].onclick = function () {
			if (window.TawalaChrome && TawalaChrome.signOutAndLeave) {
				TawalaChrome.signOutAndLeave();
				return false;
			}
		};
	}
</script>

<%-- menu order reversed from display --%>
<div id="headingMenu">
	<%@ include file="/WEB-INF/jsp/admin/administrator-access.jspf" %>
	<ul>
		<c:if test="${user.administrator}">
			<li class="<c:out value="${adminClass}" />" >
				<a href="/administration/users">ADMIN</a>
			</li>
		</c:if>
		<c:if test="${administratorAllowed}">
			<li>
				<a href="/app/administrator.jsp">Administrator</a>
			</li>
		</c:if>

		<li class="<c:out value="${aboutPageClass}" />" >
			<a href="/info">ABOUT</a>
		</li>

		<c:if test="${! empty user}">
			<c:if test="${user.status.allowedToViewDesigner}">
				<li class="<c:out value="${designerPageClass}" />" >
					<a href="${urls.designer}" id="designerLink">DESIGNER</a>
				</li>
			</c:if>
		</c:if>

		<li class="<c:out value="${faqPageClass}" />" >
			<a href="${urls.faq}">FAQ</a>
		</li>

		<li class="<c:out value="${libraryPageClass}" />" >
			<a href="/app/library.html">LIBRARY</a>
		</li>

		<c:if test="${! empty user}">
			<li class="<c:out value="${mytawalaPageClass}" />" >
				<a href="/app/mytawala.html">MY TAWALA</a>
			</li>
		</c:if>

		<li class="<c:out value="${homePageClass}" />" >
			<a href="/">HOME</a>
		</li>
	</ul>
</div>
