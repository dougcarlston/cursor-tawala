<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles"%>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags"%>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>SportsDashboards - Youth Sport Leagues Registration, Communication and Management</title>
		
		<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
		<meta http-equiv="content-language" content="EN" />
		<meta name="keywords" content="SportsDashboards, Youth Sport Leagues, Registration, Communication, Management, baseball, softball, basketball, little league, soccer, football" />
        <meta name="description" content="SportsDashboards - Youth Sport Leagues Registration, Communication and Management" />
		
		<!-- Google Webmaster Tools verification -->
		<meta name="verify-v1" content="x0utaorDZhuZpGis8TJAw9dGLGJBW03l+vp54FuKkyA=" />
		<meta name="verify-v1" content="1PVfSLARoIhDtEOZk629tvIsAqxIR66oCumE4c7c5+c=" />
		
		<link rel="icon" type="image/x-icon" href="/images/favicon.ico" />
		<link rel="shortcut icon" type="image/x-icon" href="/images/favicon.ico" />

		<link rel="stylesheet" type="text/css" media="all" href="/scripts/yui/build/reset-fonts-grids/reset-fonts-grids.css" />
		<link rel="stylesheet" type="text/css" href="/scripts/yui/build/container/assets/container.css" />
		<link rel="stylesheet" type="text/css" href="/scripts/yui/build/container/assets/skins/sam/container.css" />
		<link rel="stylesheet" type="text/css" href="/scripts/yui/build/tabview/assets/tabview.css" />
		<link rel="stylesheet" type="text/css" href="/css/sports-tabview-skin.css" />
		<link rel="stylesheet" type="text/css" href="/scripts/photoViewer/build/photoviewer_base.css" />
		<link rel="stylesheet" type="text/css" media="all" href="/css/tawala-sports.css" />
		<link rel="stylesheet" type="text/css" media="all" href="/css/pages/sports-home.css" />
		
		<script src="/scripts/yui/build/utilities/utilities.js" type="text/javascript" charset="utf-8"></script>
		<script src="/scripts/yui/build/container/container-min.js" type="text/javascript" charset="utf-8"></script>
		<script src="/scripts/yui/build/tabview/tabview-min.js" type="text/javascript" charset="utf-8"></script>
		<script src="/scripts/photoViewer/build/photoviewer_base-min.js" type="text/javascript" charset="utf-8"></script>
		<script src="/scripts/tawala.js" type="text/javascript" charset="utf-8"></script>
		<script src="/scripts/pages/sports-home.js" type="text/javascript" charset="utf-8"></script>
		
	</head>
	<body class="yui-skin-sam" <c:if test="${! empty onLoadJavascriptFunction}"> onload="${onLoadJavascriptFunction}"</c:if> >
		<div id="doc2" class="yui-t6">
			<div id="hd">
				<div id="headingLogo">
				</div>
				<div id="topRightCorner">
					&nbsp;&nbsp;
				</div>
				<div id="headingStatus">
					<c:choose>
						<c:when test="${! empty user}">
							Welcome back,
							<span class="userName"><c:out value="${user.id}" /></span>.
							<a class="headingStatusLink" href="/logout">Logout</a>
						</c:when>
						<c:otherwise>
							Hello. <a class="headingStatusLink" href="<tawala:loginUrl />" id="linkToLogin">Login</a> to see your projects. 
<!--							New to Tawala? <a class="headingStatusLink" href="${urls.userInitialRegistration}">Sign up for FREE account here!</a>-->
						</c:otherwise>        
					</c:choose>        
				</div>
				
				<div id="headingMenu">
                	<ul>
						<c:if test="${user.administrator}">
							<li><a href="/administration/users">ADMIN</a></li>
						</c:if>

                        <li><a href="/sportsdashboards/info">ABOUT</a></li>
                        <li><a href="/sportsdashboards/faq">HELP</a></li>
<!--                        
                        <li><a href="http://blog.sportsdashboards.com">SPORTS STORIES</a></li>
-->
						<c:if test="${! empty user}">
	                        <li><a href="${urls.projectManagerView}">MY TAWALA</a></li>
	                    </c:if>

                        <li class="selected"><a href="/sportsdashboards/home">HOME</a></li>
                	</ul>
				</div>
			</div><!-- end hd -->

			<!-- start tagLine -->
			<div id="tagLine"></div>
			<!-- end tagLine -->

			<!-- start callToAction -->
			<div id="callToAction">
				<div id="ctaText">
					<h3>SportsDashboards Saves You Time!</h3>
					<ul class="list">
						<li>Designed for and by League Presidents, Athletic Directors and Commissioners</li>
						<li>Register Players Online</li>
						<li>Great Email Features for Board Members, Parents and Coaches</li>
					</ul>

				</div>

				<div id="ctaSignUpNowButton">
					<a href="${urls.sportsContactUs}" ></a>
				</div>

				<div id="ctaImage">
					<a href="#" onclick="window.open('http://media.tawala.com/HomePage/FreeTrial/FreeTrialExplanation.htm','Video','width=670,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;"></a>
					<p id="ctaImageDescription">More Info about Free Trial</p>
					
				</div>
			</div><!-- end callToAction -->
			
			<!-- start bd -->
			<div id="bd" class="home">
				<div id="yui-main">
					<div class="yui-b first">
						<div class="section">
							<div id="sdinfo" class="yui-navset">
								<ul class="yui-nav">
									<li class="selected">
										<a href="#tab2"><em>Overview</em></a>
									</li>
									<li>
										<a href="#tab1"><em>Players</em></a>
									</li>
									<li>
										<a href="#tab1"><em>Coaches</em></a>
									</li>
									<li>
										<a href="#tab1"><em>Administrators</em></a>
									</li>
									<li>
										<a href="#tab1"><em>Pricing</em></a>
									</li>
									<li>
										<a href="#tab3"><em>Testimonials</em></a>
									</li>
								</ul>
								<div class="yui-content">
									<div>
										<div class="section">
											<h3>From Pre-Season to Post-Season</h3>
											<ul class="list">
												<li>Recruit players</li>
												<li>
													Online registrations and payments &nbsp; 
													<a class="iconLink movie" title="One Minute Movie" href="#" onclick="window.open('http://media.tawala.com/HomePage/OnlineRegistration/OnlineRegistration.htm','Video','width=670,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;">One Minute Movie</a>				
												</li>
												<li>
													Create teams and assign players &nbsp;
													<a class="iconLink movie" title="One Minute Movie" href="#" onclick="window.open('http://media.tawala.com/HomePage/CreateTeamsAssignPlayers/CreateTeamsAssignPlayers.htm','Video','width=670,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;">One Minute Movie</a>
												</li>
												<li>
													Email Players and Coaches easily &nbsp;
													<a class="iconLink movie" title="One Minute Movie" href="#" onclick="window.open('http://media.tawala.com/HomePage/EmailPlayersAndCoaches/EmailPlayersAndCoaches.htm','Video','width=670,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;">One Minute Movie</a>
												</li>
												<li>
													All Players and Coaches have their own, personalized Dashboard &nbsp;
													<a class="iconLink movie" title="One Minute Movie" href="#" onclick="window.open('http://media.tawala.com/HomePage/PlayerAndCoachDashboards/PlayerAndCoachDashboards.htm','Video','width=670,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;">One Minute Movie</a>
												</li>
												<li>
													Manage Volunteers &nbsp;
													<a class="iconLink movie" title="One Minute Movie" href="#" onclick="window.open('http://media.tawala.com/HomePage/Volunteers/Volunteers.htm','Video','width=670,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;">One Minute Movie</a>
												</li>
											</ul>
										</div>
										<div class="section">
											<h5>Frequently Asked Questions</h5>
											<ul class="list">
												<li><a href="/sportsdashboards/faq/introduction">Introduction to SportsDashboards</a></li>
												<li><a href="/sportsdashboards/faq/gettingstarted">Getting Started</a></li>
												<li><a href="/sportsdashboards/faq/setup">Setup</a></li>
												<li><a href="/sportsdashboards/faq/paypal">PayPal</a></li>
												<li><a href="/sportsdashboards/faq/security">Security</a></li>
												<li><a href="/sportsdashboards/faq/recruitment">Recruitment Email Blast</a></li>
												<li><a href="/sportsdashboards/faq/registration">Registration</a></li>
												<li><a href="/sportsdashboards/faq/rosters">Team Rosters</a></li>
											</ul>
										</div>
									</div>
									<div>
										<div class="section">
											<h3>Player's Dashboard</h3>
											<div class="tabScreenshot">
												<a href="#" onclick="YAHOO.photoViewer.controller.getViewer('dashboardScreenshots').open(); return false;">
													<img src="/images/sports/screenshots/player-dashboard-thumb.png" alt="" />
													<span>Click to view screen shots</span>
												</a>
											</div>
												
											<ul class="list">													
												<li>View your Team Roster</li>
												<li>Coach and League Administrator contact information</li>
												<li>Send email to your teammates or coach with one click</li>
												<li>See the latest "Flash Message" from your coach</li>
												<li>Make Online Donations directly from the Player Dashboard</li>
											</ul>
										</div>
									</div>
									<div>
										<div class="section">
											<h3>Coach's Dashboard</h3>
											<div class="tabScreenshot">
												<a href="#" onclick="YAHOO.photoViewer.controller.getViewer('dashboardScreenshots').open(1); return false;">
													<img src="/images/sports/screenshots/coach-dashboard-thumb.png" alt="" />
													<span>Click to view screen shots</span>
												</a>
											</div>
											<ul class="list">
												<li>See your Team Roster with contact info</li>
												<li>Email your team, individual players and other coaches with one click</li>
												<li>Send a "Flash Message" to your Player's Dashboards</li>
											</ul>
												
										</div>
									</div>
									<div>
										<div class="section">
											<h3>Administrator's Dashboard</h3>
											<div class="tabScreenshot">
												<a href="#" onclick="YAHOO.photoViewer.controller.getViewer('dashboardScreenshots').open(2); return false;">
													<img src="/images/sports/screenshots/admin-dashboard-thumb.png" alt="" />
													<span>Click to view screen shots</span>
												</a>
											</div>
											<ul class="list">
												<li>Gives you a snapshot of your league from pre-season to post-season</li>
												<li>Monitor your online registration process</li>
												<li>View online payments</li>
												<li>Assign players to teams</li>
												<li>Email announcements and reminders to the right people at the right time</li>
												<li>Set up Online Donations and Sponsorship</li>
											</ul>
										</div>
									</div>
									<div>
										<div class="section">
											<h3>Pricing</h3>
											<p>
												Online Registration only is $3 per player per season. The SportsDashboards Communication System is also $3 per player per season. You may purchase both for $5 per player per season.
											</p>
											<p>
												Fees are payable after registration closes. There are no setup costs or upfront fees. There are no additional charges for online donations made from Player SportsDashboards. You can send unlimited emails through the system without additional charge.* 
											</p>
											<p>
												The fee does not include amounts payable to PayPal for credit card transactions. PayPal will deduct their fee from the credit card transaction. Our experience is that given a choice, about half of registrants pay by credit card online and the other half prefer to send in checks. Since checks have no fees, this should be factored into a total cost estimate of credit card fees. See our PayPal FAQ for more details about PayPal.
											</p>
											<p>
												<em>*An average sized league of 300 to 500 players sends approximately 18,000 to 30,000 emails through our system during a season.</em>	
											</p>
										</div>
									</div>
									<div>
										<div class="section">
											<h3>
												Testimonials
											</h3>
											<p>
												Here's what Coaches, League Administrators and parents are saying about SportsDashboards:
											</p>

											<div class="tawalaTestimonials showAll"> </div>
										
										</div>
									</div>
								</div>
							</div>
						</div>
					</div><!-- end yui-b -->

				</div><!-- end yui-main -->

				<div class="yui-b last gradient">
<!--				
					<div class="block">
						<div id="sports-stories">
							<a href="http://blog.sportsdashboards.com"></a>
						</div>
					</div>
-->
					<div class="block">
						<div id="satisfaction">
						</div>
					</div>
					<div class="block">
						<div class="content" id="emailBoard">
							<a href="/sportsdashboards/emailboard"></a>
						</div>
					</div>

					<!-- Testimonials block -->
					<div class="block">
						<div id="testimonialBlock">
							<div class="content tawalaTestimonials rotateOne"></div>
						</div>
					</div>
					
					<div class="block">
						<div id="poweredByButton">
							<a href="/home"></a>
						</div>
					</div>
				</div><!-- end yui-b -->


			</div><!-- end bd -->

			<div id="ft">
				<div class="bottomCorner"></div>
				<div>
					<ul>
						<li>
							<a href="/sportsdashboards/info">Company Info</a>
						</li>
						<li>
							<a href="/sportsdashboards/terms">Terms &amp; Conditions</a>
						</li>
						<li>
							<a href="/sportsdashboards/privacy">Privacy Policy</a>
						</li>
						<li>
							<a href="/sportsdashboards/jobs">Jobs</a>
						</li>
						<li>
							<a href="mailto:info@tawala.com">Contact Us</a>
						</li>
						<li>
							<a href="mailto:bugs%20at%20tawala.com" id="bugReportLink" name="bugReportLink">Report a bug</a> <script type="text/javascript">
								var el = document.getElementById('bugReportLink');
							if(el) {
								el.href = 'mailto:' + 'bugs' + '@' + 'tawala.com';
							}
							</script>
						</li>
					</ul>
				</div>
			</div><!-- end ft -->
		</div>

		<div id="dashboardScreenshots">
			<a href="/images/sports/screenshots/player-dashboard.png" title="Player's Dashboard" class="photoViewer"><img src="/images/sports/screenshots/player-dashboard.png" alt="" /></a>
			<a href="/images/sports/screenshots/coach-dashboard.png" title="Coach's Dashboard" class="photoViewer"><img src="/images/sports/screenshots/coach-dashboard.png" alt="" /></a>
			<a href="/images/sports/screenshots/admin-dashboard.png" title="Administrator's Dashboard" class="photoViewer"><img src="/images/sports/screenshots/admin-dashboard.png" alt="" /></a>
			<a href="/images/sports/screenshots/communicator.png" title="Communicator" class="photoViewer"><img src="/images/sports/screenshots/communicator.png" alt="" /></a>
			<a href="/images/sports/screenshots/preseason-activities.png" title="Preseason Activities" class="photoViewer"><img src="/images/sports/screenshots/preseason-activities.png" alt="" /></a>
			<a href="/images/sports/screenshots/recruitment.png" title="Recruitment" class="photoViewer"><img src="/images/sports/screenshots/recruitment.png" alt="" /></a>
			<a href="/images/sports/screenshots/registration-management.png" title="Registration Management" class="photoViewer"><img src="/images/sports/screenshots/registration-management.png" alt="" /></a>
			<a href="/images/sports/screenshots/team-assignment.png" title="Team Assignment" class="photoViewer"><img src="/images/sports/screenshots/team-assignment.png" alt="" /></a>
			<a href="/images/sports/screenshots/teams-coaches.png" title="Teams and Coaches" class="photoViewer"><img src="/images/sports/screenshots/teams-coaches.png" alt="" /></a>
			<a href="/images/sports/screenshots/uniforms.png" title="Uniforms" class="photoViewer"><img src="/images/sports/screenshots/uniforms.png" alt="" /></a>
		</div>

		<tiles:insert name="googleAnalytics" ignore="true" />
	</body>
</html>
