<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>

<tiles:importAttribute name="subName" />

<c:choose>
	<c:when test="${subName == ''}">
		<c:set var="intro" value="selected" />
	</c:when>
	<c:when test="${subName == 'whatsnew'}">
		<c:set var="whatsNewPageClass" value="selected" />
	</c:when>
	<c:when test="${subName == 'news'}">
		<c:set var="newsPageClass" value="selected" />
	</c:when>
	<c:when test="${subName == 'manual'}">
		<c:set var="manualPageClass" value="selected" />
	</c:when>
	<c:when test="${subName == 'tutorial'}">
		<c:set var="tutorialPageClass" value="selected" />
	</c:when>
</c:choose>
	<div class="sub-menu">
	     <ul>
	    	<li><a class="<c:out value="${intro}" default="none" />" href="/" title="Tawala home page" >Intro</a></li>
	    	<li><a class="<c:out value="${whatsNewPageClass}" default="none" />" href="/whatsnew" title="What's new in the current version of Tawala" >What's New</a></li>
		    <li><a class="<c:out value="${newsPageClass}" default="none" />" href="/news" title="The latest news about Tawala">News</a></li>
	        <li><a class="<c:out value="${manualPageClass}" default="none" />" href="/manual" title="Detailed information on how to use the features of Tawala">Manual</a></li>
	        <li><a class="<c:out value="${tutorialPageClass}" default="none" />" href="/tutorial1" title="Step-by-step directions for creating a Tawala project">Tutorial</a></li>
	     </ul>
	</div>
