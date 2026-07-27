<jsp:directive.page import="com.tawala.web.library.ProjectDescriptionController"/>
<jsp:directive.page import="com.tawala.domain.Status"/>
<jsp:directive.page import="com.tawala.web.library.ViewProjectDetailsController"/>
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
		<div id="doc" class="yui-t6">

            <!-- start hd -->
            <div id="hd">
                <div id="headingLogo">
                	<a href="/home">
                		<img src="/images/homepage/tawala-logo.gif" />
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
		                        <li><a href="/info">ABOUT</a></li>
								<c:if test="${! empty user}">
									<c:if test="${user.status.allowedToViewDesigner}">
				                        <li><a href="/designer" id="designerLink">DESIGNER</a></li>
				                    </c:if>
		                        </c:if>
		                        <li><a href="${urls.faq}">FAQ</a></li>
		                        <li><a href="${urls.librarySearch}">LIBRARY</a></li>
								<c:if test="${! empty user}">
			                        <li><a href="${urls.projectManagerView}">MY TAWALA</a></li>
			                    </c:if>
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
               	<c:choose>
               		<c:when test="${empty domain.title}">
		                <div id="tagLine">
		                	<span>Surveys, Sign Ups, Event Planning - all in seconds and for FREE</span>
<%--           	         		<img src="/images/homepage/headline-tagline.png" />--%>
           	         	</div>	
						<div id="subtagline">
							A wide set of easy-to-use email and Web tools that you can put to work immediately with absolutely no technical knowledge.
						</div>
                    </c:when>
                    <c:otherwise>
		                <div id="tagLine">         	
                    			<span>${domain.title}</span>
                    		</div>
						<div id="subtagline">
							${domain.subtitle}
						</div>
                    </c:otherwise>
            	</c:choose>
            </div>
            <div id="featuredApps">
                <div id="featuredFree">
					<c:choose>
               			<c:when test="${empty domain.title}">
							<p><b>Tawala Featured Solutions</b>&nbsp;&nbsp; All absolutely free. No registration required!</p>
<%--                     		<img src="/images/homepage/tawalafeatured-text.gif" />--%>
                     	</c:when>
                     	<c:otherwise>
							<p><b>${domain.featuredSolutionsCaption}</b>&nbsp;&nbsp; All absolutely free. No registration required!</p>
                     	</c:otherwise>
                     </c:choose>
                </div>

                <div id="apps">
                	<c:set var="constructionCount" value="0" />
					<c:forEach var="project" items="${featuredProjects}" end="3">
						<c:url var="projectDescriptionUrl" value="${urls.libraryProjectDetailView}">
							<c:param name="<%=ViewProjectDetailsController.PARAMETER_ID%>" value="${project.id}" />
						</c:url>
						<div class="webapp">
							<c:choose>
								<c:when test="${! project.underConstruction}">
									<a class="title" href="${projectDescriptionUrl}">
										<img class="icon" src="${project.iconURL}" alt="<c:out value="${project.name}" />" />
									</a>
									<div class="title">
										<a class="title" href="${projectDescriptionUrl}" id="projectDescription${project.id}"><c:out value="${project.name}" /></a>
									</div>
									<p><c:out value="${project.shortDescription}" /></p>
			                        <div class="links">
										<c:if test="${! empty project.videoURL}">				
											<a href="#" onclick="window.open('${project.videoURL}','Video','width=860,height=600,toolbar=no,menubar=no,status=no,location=no,resizeable=yes,scrollbars=yes'); return false;">
												See the demo
											</a>
										</c:if>
			                	    </div>
	                	    	</c:when>
	                	    	<c:otherwise>
									<span class="title">
										<img class="icon" src="${project.iconURL}" alt="<c:out value="${project.name}" />" />
										<img class="overlay" src="/images/construction2.gif" alt="Under Construction" />
									</span>
									<div class="title">
										<span class="title" id="projectDescription${project.id}"><c:out value="${project.name}" /></span>
									</div>
									<p><c:out value="${project.shortDescription}" /></p>
			                        <div class="links">
			                	    </div>	                	    	
				                	<c:set var="constructionCount" value="${constructionCount + 1}" />
	                	    	</c:otherwise>
	                	    </c:choose>
	                	    
						</div>
					</c:forEach>
                </div>

				<%
				pageContext.setAttribute("initialRegistrationStatus", Status.REGISTERED_INITIAL);
				%>
				
	            <div class="featuredLinks">
	            	<div class="link1">
						<a href="#" onclick="window.open('/demos/Tawala Overview Demo/800 Tawala Overview Demo.htm','Video','width=860,height=600,toolbar=no,menubar=no,status=no,location=no,resizeable=yes,scrollbars=yes'); return false;">
   		            		<span>View the Tawala Demo</span>
   		            	</a>
	            	</div>
	            
					<c:choose>
						<c:when test="${empty user}">
	       		            <div class="link2">
	       		            	<a href="${urls.userInitialRegistration}" id="linkToInitialSetup">
	       		            		<img src="/images/homepage/nextlevel-button.gif" alt="Save your customized apps" />
	       		            		<br /><span>Save your customized apps</span>
	       		            	</a>
	       		            </div>
	                	</c:when>
	                	<c:when test="${user.status == initialRegistrationStatus}">
	       		            <div class="link2">
	       		            	<a href="${urls.userDisplayNextLevel}" id="linkToUpgradeToFullyRegistered">
	       		            		<img src="/images/homepage/nextlevel-button.gif" alt="Create your own Tawala apps" />
	       		            		<br /><span>Create your own Tawala apps</span>
	       		            	</a>
	       		            </div>
	                	</c:when>
	                </c:choose>
                </div>
            </div>

            <!-- start bd -->
            <div id="bd">
                <div id="yui-main">
                    <div class="yui-b">
						<div class="yui-g">
							<div class="yui-u first">
                                 <div class="content">
                                    <c:if test="${!empty domain.title && !empty domain.descriptionCaption}">
										<div class="block">
	                                        <h4>${domain.descriptionCaption}</h4>
											<div>
												${domain.description}
											</div>
											<br />
										</div>
									</c:if>                            
                                    <div class="block">
                                        <h4>Featured Solutions</h4>
                                        <div id="featuredList">
											<c:forEach var="project" items="${featuredProjects}" begin="4">
												<c:url var="projectDescriptionUrl" value="${urls.libraryProjectDetailView}">
													<c:param name="<%=ViewProjectDetailsController.PARAMETER_ID%>" value="${project.id}" />
												</c:url>
												<div class="webapp">
													<c:choose>
														<c:when test="${! project.underConstruction}">
															<div class="title">
																<a href="${projectDescriptionUrl}"><c:out value="${project.name}" /></a>
															</div>
															<p>
																<% pageContext.setAttribute("newline", "\n"); %>
																${fn:replace(project.longDescription, newline, '<br />')}															
															</p>
			                                                <div class="links">
			                                                    <a href="${projectDescriptionUrl}">Tell me more... <img src="/images/homepage/red-bullet-arrow-right.gif" /></a>
			                                                </div>
	                                                	</c:when>
	                                                	<c:otherwise>
															<div class="webapp construction">
																<div class="title">
																	<span href=""><c:out value="${project.name}" /></span>
																</div>
																	<p>
																		<% pageContext.setAttribute("newline", "\n"); %>
																		${fn:replace(project.longDescription, newline, '<br />')}															
																	</p>
				                                                <div class="links">
				                                                </div>
															</div>
	                                                	</c:otherwise>
	                                                </c:choose>		                                              
												</div>
											</c:forEach>
                                        </div>
									</div>
									<c:if test="${!empty domain.title}">
						                <div class="block">
											<a href="/"><img src="/images/homepage/red-bullet-arrow-right.gif" /> Go to the Tawala Home Page</a>
						                </div>
									</c:if>
                                </div>
							</div>
							<div class="yui-u">
                               	<div class="content">
                                   	<div class="block">
                                   	<!--
										<div id="tawalaNews" class="newsFeed"></div>																				
	                                    <div class="links">
	                                        <a id="tawalaNewsLink" href="/news">More Tawala news... <img src="/images/homepage/red-bullet-arrow-right.gif" /></a>
	                                    </div>
	                                -->
									</div>
								</div>
							</div>
                        </div>
                     </div>
				</div>
                <div class="yui-b left">
                	<div class="content">
						<c:forEach var="block" items="${infoBlockList}" varStatus="status">
							<tiles:insert name="${block}" />
						</c:forEach>                           																		
					</div>
                </div>
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

		<script type="text/javascript">

			var getNews = function(){
				var sUrl = "/blog/tawalanews/feed/entries/atom";	
				var feedXML = {};
			
				if (YAHOO.env.ua.ie > 0) {
					feedXML = Tawala.XML.load(sUrl);
					var feed = new Tawala.Feed.Atom(feedXML);
					var container = $("tawalaNews");
					feed.renderEntries(container);
				} else {
					 function successHandler (o) {
						 feedXML = (o.responseXML);
						 
						 var feed = new Tawala.Feed.Atom(feedXML);
						 var container = $("tawalaNews");
						 feed.renderEntries(container, 1);
					 }
					 
					 function failureHandler (o) {
					 alert("Failure\n\n" + o.statusText);
					 }
					 
					 // Initiate the HTTP GET request.
					 var request = YAHOO.util.Connect.asyncRequest('GET', sUrl, {success:successHandler, failure:failureHandler});
				}
			}
			
//			YAHOO.util.Event.onDOMReady(getNews)
		
		</script>
	</body>
</html>
