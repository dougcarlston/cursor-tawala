<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>

<tiles:importAttribute name="pageName" />

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
	<head>
		<title><tiles:getAsString name="pageTitle" />
		</title>
		<meta http-equiv="content-type"
			content="text/html; charset=iso-8859-1" />
		<meta http-equiv="content-language" content="EN" />
		<link rel="icon" type="image/x-icon" href="/images/favicon.ico" />
		<link rel="shortcut icon" type="image/x-icon"
			href="/images/favicon.ico" />

		<tiles:useAttribute id="scriptList" name="scripts"
			classname="java.util.List" ignore="true" />

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
		<div id="doc" class="yui-t7 homePage">
			<div id="hd">
				<!-- start header content-->
				<div id="headerTop">
					<div id="headerLogo">
						<a href=""><img src="/images/home/tawalahome-logo.gif"	width="158" height="39" /></a>
					</div>
					<div id="headerLogin">
						<c:if test="${empty login}">
							<c:set var="login"
								value="<%=new com.tawala.web.user.LoginForm()%>"
								scope="request" />
						</c:if>
						<form method="post" action="${urls.login}">
							<spring:bind path="login.userName">
								<label>
									Username
								</label>
								<input class="text" type="text" size="16"
									maxlength="${meta.userNameMaxLength}"
									name="${status.expression}"
									value="<c:out value="${status.value}" />" />
							</spring:bind>

							<spring:bind path="login.password">
								<label>
									Password
								</label>
								<input class="text" type="password" size="16" maxlength="100"
									name="${status.expression}" value="" />
							</spring:bind>

							<input type="image" src="/images/home/login-button.gif"
								name="submitbutton" value="Log In" />
						</form>
						<div>
							<a href="/user/registration">Register for a new account</a>
							<a href="/login">Forgot your password?</a>
						</div>
					</div>
				</div>
				<div id="headerBottom">
					<div id="headerMenu">
						<ul>
							<li><a href="/user/registration">MY TAWALA</a></li>
							<li><a href="${urls.librarySearch}">LIBRARY</a></li>
							<li><a href="/faq">FAQ</a></li>
							<li><a href="http://www.tawala.com/forum/">FORUM</a></li>
							<li><a href="/news">NEWS</a></li>
							<li><a href="/designer">DESIGNER</a></li>
							<li><a href="/info">ABOUT US</a></li>
						</ul>
					</div>
				</div>
				<!-- end header content-->
			</div>
			<!-- end header -->

			<div id="bd" class="homePage">
				<div id="yui-main">
					<div class="yui-b">
						<div class="yui-ge first homeTop">
							<div class="yui-u first">
								<div class="content" style="width: 70%;">
									<div style="padding-top: 5.5em; padding-left: .6em; font-size: .9em;">
										Quickly and easily coordinate your group with our scheduling, reservation, 
										fund raising, polling and other projects. Customize from existing projects 
										or quickly build your own from scratch with our simple designer system!
									</div>
									<p class="alphaWarning">
										Tawala is "pre-beta" and still in the early stages of development. Things are changing quickly. 
										Please backup your work often.
									</p>
									<div style="padding-top: .8em;">
									<!-- 	  
                                        <a href="" alt="Learn More"><img src="/images/home/learnmore-button.gif" width="91" height="19" /></a>  
                                    -->
									</div>
								</div>
							</div> 
							<div class="yui-u last">
								<div class="content">
								</div>
							</div>
						</div>
						<!-- start: stack grids here -->
						<div class="yui-u homeBottom">
							<!-- start: your content here -->
							<div class="content" style="float: left; width: 190px;">
								<img class="headline" src="/images/getstarted-headline.gif" alt="Getting Started" />
								<ul class="outsideDot">
									<li>Read our <a href="/faq">FAQ</a></li>
									<li>Visit the <a href="${urls.librarySearch}">project library</a> and test drive sample projects</li>
									<li>Download <a href="/user/registration">Tawala Designer</a> and see how easy it is to create your own projects</li>
									<li>Build your first project with our <a href="/tutorial1">step-by-step tutorial</a></li>
								</ul>
							</div>
							<!-- end: your content here -->
							<!-- start: your content here -->
							<div class="content" style="float: left; width: 190px;">
								<img class="headline" src="/images/keyfeatures-headline.gif" alt="Key Features" />
								<ul class="outsideDot">
									<li>Easy to deploy</li>
									<li>Customizable templates</li>
									<li>Design your own project with our simple point and click interface</li>
									<li>Works in your website and personal email</li>
								</ul>
							</div>
							<!-- end: your content here -->
							<!-- start: your content here -->
							<div class="content" style="float: left; width: 190px;">
								<img class="headline" src="/images/nowebpage-headline.gif" alt="No webpage? No problem!" />
								<p>
                                	Try out one of our email oriented projects:
								</p>
                                	<ul style="margin: .5em 0 .8em; 0">
                                		<li style="margin-bottom: 0; padding-bottom: .2em;">
                                			<a href="http://www.tawala.com/projects/T2JtYSDWGy1bwdhXT2U4PZH4jZ6yvce2d8nUSotZ/Setup" target="_blank">
                                			Alumni List Builder
                                			</a>
                                		</li>
                                		<li style="margin-bottom: 0; padding-bottom: .2em;">
                                			<a href="http://www.tawala.com/projects/0ZBeADEdJXahLIWcO8Xx0SYeF7q9Y9fOZ0Ou6iZe/LivingWill" target="_blank">
                                			Living Will
                                			</a>
                                		</li>
                                		<li style="margin-bottom: 0; padding-bottom: .2em;">
                                			<a href="http://www.tawala.com/projects/GAYcMFcuWzQSUg6qBW3ryE6wKIWBpddeoWCfrDx2/Petition" target="_blank">
                                			Political Campaign Tool
                                			</a>
                                		</li>
                                	</ul>
                               	<h5>
                               		<a href="/faq#price">
										<img src="/images/home/arrow-bullet.gif" width="12" height="12" />                            	
		                               	Tawala Pricing Info
	                               	</a>
	                            </h5>
							</div>
							<!-- end: your content here -->
							<!-- start: your content here -->
							<div class="content featured"
								style="float: left; width: 280px; padding-top: 2px;">
								<img class="headline" src="/images/featuredprojects-headline.gif" alt="Featured Projects" />
								<ul>
									<li>
										<a href="http://www.tawala.com/projects/2BahKceYTVJqcskzadYDLPlhJfO2MbdUN3upuNZ4/Contents" target="_blank">
											Tour of Tawala Features
										</a>
										<div>Take a quick tour to see what Tawala can do.</div>
									</li>
									<li>
										<a href="http://www.tawala.com/projects/so4umsT4elY9HgybfSWYqCwme9qhgrxUrPEKn9Tu/Menu" target="_blank">
											Shared Contact Manager
										</a>
										<div>Share a Rolodex among members of a team or group</div>
									</li>
									<li>
										<a href="http://www.tawala.com/projects/dGanKrw0nvHh4WvdcemaI0hONYXuWKskX3aknFM3/Main+Form" target="_blank">
											Due Diligence Demo
										</a>
										<div>A simple task manager for a venture firm</div>
									</li>
								</ul>
								<div>
									<h5><a href="${urls.librarySearch}">
									<img src="/images/home/arrow-bullet.gif" width="12" height="12" /> 
									More in the Project Library</a>
									</h5>
								</div>
							</div>
							<!-- end: your content here -->
						</div>
						<!-- end: stack grids here -->
					</div>
				</div>
			</div>
			<!-- end bd -->

			<!-- start footer -->
			<div id="ft">
				<div>
					<ul>
						<li>
							<a href="/info">Company Info</a>
						</li>
						<li>
							<a href="/terms">Terms &amp; Conditions</a>
						</li>
						<li>
							<a href="/privacy">Privacy Policy</a>
						</li>
						<li>
							<a href="/jobs">Jobs</a>
						</li>
						<li>
							<a href="mailto:info@tawala.com">Contact Us</a>
						</li>
					</ul>
				</div>
			</div>
			<!-- end footer -->
		</div>
		<!-- end yui-t2 -->

		<tiles:insert name="googleAnalytics" ignore="true" />
	</body>
</html>
