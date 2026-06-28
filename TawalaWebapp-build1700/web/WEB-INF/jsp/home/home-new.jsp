<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>


<tiles:importAttribute name="pageName" />
<tiles:importAttribute name="infoBlockList" />

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
	<head>
		<title><tiles:getAsString name="pageTitle" /></title>
		<meta http-equiv="content-type"	content="text/html; charset=iso-8859-1" />
		<meta http-equiv="content-language" content="EN" />
		
		<%-- meta tag added to work with Google Webmaster tools --%>
		<meta name="verify-v1" content="x0utaorDZhuZpGis8TJAw9dGLGJBW03l+vp54FuKkyA=" />
		
		<link rel="icon" type="image/x-icon" href="/images/favicon.ico" />
		<link rel="shortcut icon" type="image/x-icon" href="/images/favicon.ico" />

		<tiles:useAttribute id="scriptList" name="scripts" classname="java.util.List" ignore="true" />

		<c:forEach var="js" items="${scriptList}">
			<script type="text/javascript" src="<%=request.getContextPath()%><c:out value="${js}"/>"></script>
		</c:forEach>


		<tiles:useAttribute id="styleList" name="styles" classname="java.util.List" ignore="true" />

		<c:forEach var="css" items="${styleList}">
			<link rel="stylesheet" type="text/css" media="all" href="<%=request.getContextPath()%><c:out value="${css}"/>" />
		</c:forEach>

	</head>
	<tiles:useAttribute name="onLoadJavascriptFunction" ignore="true" />
	<body <c:if test="${! empty onLoadJavascriptFunction}"> onload="${onLoadJavascriptFunction}"</c:if> >
		<div id="doc" class="yui-t7">

            <!-- start hd -->
            <div id="hd">
                <div id="headingLogo">
                	<a href="/home">
                		<img src="/images/homepage/tawala-logo.gif" />
                		<img src="/images/homepage/beta-white.gif" />
                	</a>
                </div>
                <div id="topRightCorner"></div>
                
				<div id="headingStatus">
					<c:choose>
						<c:when test="${! empty user}">
							Welcome back,
							<span class="userName"><c:out value="${user.id}" /></span>.
							<a class="headingStatusLink" href="/logout">Logout</a>
						</c:when>
						<c:otherwise>
							Hello. <a class="headingStatusLink" href="<tawala:loginUrl />" id="linkToLogin">Login</a> to see your projects. New to Tawala? <a class="headingStatusLink" href="${urls.userInitialRegistration}">Sign up for FREE account here!</a>
						</c:otherwise>        
					</c:choose>        
				</div>

                <div id="headingMenu">
					<c:choose>
						<c:when test="${empty domain.title}">
		                	<ul>
								<c:if test="${user.administrator}">
									<li><a href="/administration/users">ADMIN</a></li>
								</c:if>
		                        <li><a href="/help/manual">HELP</a></li>
				                <li><a href="/designer" id="designerLink">DOWNLOAD</a></li>
		                        <li><a href="/news">NEWS</a></li>
		                        <li><a href="/communityNews">COMMUNITY</a></li>
		                        <li><a href="${urls.faq}">FAQ</a></li>
			                    <li><a href="${urls.projectManagerView}">MY TAWALA</a></li>
		                        <li><a href="/customizables">TAWALA APPS</a></li>
		                        <li><a href="/home">HOME</a></li>
		                	</ul>
						</c:when>
						<c:otherwise>
		                	<ul>
		                        <li><a href="/home"><img src="/images/goToTawalaHome.gif" alt="Got to Tawala home page" /></a></li>
		                	</ul>
                		</c:otherwise>
                	</c:choose>
                </div>
            </div>
            <!-- end hd -->
			<div id="subheading">
				<div id="tagLine">
					<span>Web Tools for Groups and Organizations</span>
				</div>
				<div id="subtagline">
					<ul>
						<li>Designed for small organizations with limited technical resources</li>
						<li>A powerful combination of record keeping, web-design and email communication</li>
						<li>One click hosting makes it easy to get online and going within minutes</li>
						<li>Make membership communication a two-way street</li>
					</ul>	
				</div>
				<div id="callout">
				</div>		
			</div>
            
			<div id="featuredApps">
				<div id="apps">
					<div id="step1" class="webapp">
						<div class="content" style="padding-left: 22px; z-index: 2;">
							<embed
								src="/demos/tawala-intro/flvplayer.swf"
								width="228"
								height="185"
								allowfullscreen="true"
								allowscriptaccess="always"
								flashvars="height=185&width=228&file=/demos/tawala-intro/tawala.flv&image=/demos/tawala-intro/poster.jpg&backcolor=0x000000&frontcolor=0xcccccc&lightcolor=0x557722&id=player1&usefullscreen=true"
							/>
						</div>
					</div>
					
					<div id="boxArrow1">
						<img src="/images/home-new/boxArrow.jpg" width="35" height="41" />
					</div>
					
					<a href="/examples" >
						<div id="step2" class="webapp">
							<div id="slideshow" class="yui-sldshw-displayer content">
								<div class="yui-sldshw-frame yui-sldshw-active">
									<div class="app">
										<img alt="List Builder" src="/images/webapps/default-icon.gif"/>
										<div class="title"></div>
										<p class="description">These are some example projects that were built using Tawala</p>
									</div>
								</div>
								<c:forEach var="mapEntry" items="${exampleProjectsMap}" varStatus="iterationByCategoryStatus">
									<c:set var="category" value="${mapEntry.key}" />
									<c:set var="projects" value="${mapEntry.value}" />
										<c:forEach var="project" items="${projects}">
												<div class="yui-sldshw-frame">
													<div class="app">
														<img src="${project.iconURL}" alt="<c:out value="${project.name}" />" />
														<div class="title"><c:out value="${project.name}"/></div>
														<p class="description">
															<c:out value="${project.shortDescription}"/>
														</p>
													</div>
												</div>
										</c:forEach>
								</c:forEach>
							</div>
						</div>
					</a>
					<div id="boxArrow2">
						<img src="/images/home-new/boxArrow.jpg" width="35" height="41" />
					</div>
					
					<a href="/designer" >
						<div id="step3" class="webapp">
								<div  class="content"></div>
						</div>
					</a>

				</div>				
			</div><!-- end featured apps -->

			<!-- start bd -->
			<div id="bd">
				<div id="yui-main">
					<div class="yui-b">
						<div id="readyToRun" >
							<a href="/customizables" style="display: block;">
								<img src="/images/home-new/readytorun-up.jpg" />
							</a>
						</div>

					</div>
				</div>
<!--
				<div class="yui-b left">
					<div class="content">
						<div class="block" style="font-size: 105%;">
							<h3>Testimonials</h3>
							<div class="content">
								<blockquote>
									<p class="openQ">
										Wow! Tawala is great! How did I ever live without it?
										<span class="closeQ"></span>
									</p>
									<p> -
										<cite>Tony Fardella</cite>
									</p>
								</blockquote>
							</div>
						</div>
						
						<div class="block">
							<h3>
								Tell us what we can do for you!
							</h3>
							<div class="content">
								<p>
									Tawala web apps are easy to change and enhance, and you can even do it yourself if you like. But tell us what you want and we will respond to you. Email us at:
								</p><br />
								<a href="mailto:TellUs@tawala.com">TellUs@tawala.com</a><br />
								<br />
								<p>
									If you do run into any problems using Tawala please drop us a note at:
								</p><br />
								<a href="mailto:bugs@tawala.com">bugs@tawala.com</a><br />
								<br />
							</div>
						</div>
					</div>
				</div>
-->
				
			</div>
			<!-- end bd -->

            <!-- start ft -->
            <div id="ft" class="footer">
                <div class="bottomCorner"></div>
                <div>
                    <ul>
                        <li><a href="/info">Company Info</a></li>
                        <li><a href="/terms">Terms &amp; Conditions</a></li>
                        <li><a href="/privacy">Privacy Policy</a></li>
                        <li><a href="/jobs">Jobs</a></li>
                        <li><a href="mailto:info@tawala.com">Contact Us</a></li>
						<li><%@ include file="/WEB-INF/jsp/blocks/reportBugs.jsp" %></li>
                    </ul>
                </div>
            </div>
            <!-- end ft -->
            <c:if test="${!empty domain.title && !empty domain.descriptionCaption && constructionCount > 1}">
				${constructionCount}
				<div id="calloutImage">
					<a href="/" alt="Home">
					<img src="/images/homepage/callout-image.gif" />
					</a>
				</div>
			</c:if>
		</div>
		<!-- end yui-t2 -->
		<tiles:insert name="googleAnalytics" ignore="true" />		

	</body>
</html>
