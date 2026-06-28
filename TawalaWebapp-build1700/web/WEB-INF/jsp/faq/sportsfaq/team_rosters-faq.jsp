<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section">
	<h3>Team Rosters</h3>
	<ul class="faq">	
		<li><a href="#q1">How do I close registration?</a></li>
		<li><a href="#q2">How do I take out duplicate registrations?</a></li>
		<li><a href="#q3">How do I email players for tryouts?</a></li>
		<li><a href="#q4">What is the Coach Registration Form?</a></li>
		<li><a href="#q5">How do I create teams with coach and player assignments?</a>
			<ul>	    
				<li><a href="#q5.1">Step 1 - Set up Teams</a></li>
				<li><a href="#q5.2">Step 2 - Assign Coaches to Teams</a></li>
			    <li><a href="#q5.3">Step 3 - Assign Players to Teams</a></li>
			    <li><a href="#q5.4">Step 4 - Send Coaches their dashboards</a></li>
			    <li><a href="#q5.5">Step 5 - Release the final rosters to Players</a></li>
			</ul>
		</li>
		<li><a href="#q6">How do I email certain Coaches only?</a></li>
		<li><a href="#q7">What about Coach training?</a></li>
		<li><a href="#q8">Can I print Player medical information for Coaches?</a></li>
	</ul>
	
	<dl>
		<dt><a name="q1">How do I close registration?</a></dt>
		<dd>	
			<p>
			You can close or reopen the registration form. After registration is closed, anyone who 
			clicks the registration link will not see the registration form, but instead will see a 
			web page saying registration is closed and to contact the administrator.
			</p>
			<p>
			If you want to register someone after registration is closed, you can manually register 
			someone in Preseason Activities > Player Registration Tools. Or, you can email the 
			registration link to the new registrant and temporarily open registration, allowing 
			them to register online.
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dashboard > Close/Reopen Registration Period</em></p>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/1/CloseRegistration.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
	
		<dt><a name="q2">How do I take out duplicate registrations?</a></dt>
		<dd>		
			<p>
			Players sometimes register twice. You will need to clean those out as they cause problems down the road. We recommend you check once every week or two during the registration period. Make sure to never delete registrations marked as paid.
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Player Registration Tools > Check for Duplicates</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/8/DedupingRegistrations.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			 </p>
		</dd>
		<dt><a name="q3">How do I email players for tryouts?</a></dt>
		<dd>
			<p>
			You can email everyone who has registered who are not yet placed on teams from Player 
			Registration Tools in the Preseason Menu.
			</p>
			<p>
			You can email specific groups, such as by grade or division. 
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Registration Tools > Send Email to Registered Players</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/2/EmailRegistrants.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>



			<p>
			In general, you send out emails from different places in SportsDashboards depending on where you are in your season. 
			</p>
			<p>
			Here's a guide: 
			</p>
			<p>Recruitment:</p>
			<p>
			Send email to people who haven't registered. They are in your recruitment list.
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dash > Preseason Activities > Recruitment List Tools > Send Email</em>
			</p>
			<p>Registration Phase:</p>
			<p>
			Send email to people who have registered (whether on teams or not)
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dash > Preseason Activities > Player Registration Tools > Send Email to Registered Players</em> 
			</p>
			
			<p>After Opening Day:</p>
			<p>
			Send email to entire Teams, Coaches, or Volunteers (sends only to Players assigned to Teams) 
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dash > Communicator</em>
			</p>

			<p>
			Tip: If you want to email a particular group of registrants and you don't see an easy way to do it, you can set up a temporary team, put them in the team, and send email to the team.
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/3/EmailGroupRegistrants.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>

		<dt><a name="q4">What is the Coach Registration Form?</a></dt>
		<dd>
			<p>
			There is an online Coach Registration form you can set up. In addition to asking for basic contact information, you can also ask custom questions and include a Coach Agreement that all prospective coaches must click to accept. 
			</p>
			<p>
			Here's an overview:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2/CoachOverview.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			<p>			
			You can invite potential coaches to register by emailing them a link to the Coach Registration Form:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2A/CoachRecruitment.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			<p>
			You can also manually enter Coach information directly if you don't want them to fill out a registration form:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2B/CoachEntry.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			<p>
			To set up and customize the Coach Registration Form:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2C/CustomizeCoachForm.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
		</dd>
			
		<dt><a name="q5">How do I create teams with coach and player assignments?</a></dt>
		<dd>
			<p>
			Once you have finished tryouts, you can start setting up teams and coaches and assigning 
			players to teams. 
			</p> 
			
			<p><strong><a name="q5.1" class="title">Step 1 - Set up Teams</a></strong></p>
			<p>
			Go to:
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Set up Teams and Coaches</em>
			</p> 
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/3/SetupTeamsCoaches.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			<p>
			When you set up a team, you also assign it to a division. It might be easier during player 
			selection for teams if the Team names contain the division name in parentheses. E.g. Tigers 
			(Majors). You can always rename the team later to take this information off.
			</p> 
			
			<p><strong><a name="q5.2" class="title">Step 2 - Assign Coaches to Teams</a></strong></p>
			<p>
			Go to:
			</p>
			<p> 
			<em>Admin Dashboard > Preseason Activities > Set up Teams and Coaches</em>
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/3A/AssignCoaches.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			
			<p>A Coach can be assigned to more than one team. Here's how:</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/4/AssignCoachToTwoTeams.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			

			<p><strong><a name="q5.3" class="title">Step 3 - Assign Players to Teams</a></strong></p>	
			<p>
			Assign Players to Teams, or make adjustments to Team assignments. 
			</p>
			<p>
			Go to:
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Assign Players to Teams</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/5/AssignPlayersToTeams.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			<p>
			If division names are too long, you can shorten the division description in Administrator Setup. 
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dashboard > View/Edit Administrator Setup > Change Division Setup > Edit Division Name</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/6/EditDivisionName.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			
			<p><strong><a name="q5.4" class="title">Step 4 - Send Coaches their Dashboards</a></strong></p>
			
			<p>
			Send Coaches their Coach Dashboards. You can create a custom email message to Coaches in 
			the email sent out to Coaches with their private dashboard link. Team Rosters, if created, 
			are displayed on their Coach Dashboards. You can preview what a Coach Dashboard might look 
			like by going to Admin Dash: Send/View Coach/Player Dashboard and looking at any of the 
			Coach Dashboards.
			</p>
			<p>
			Go to:
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Send Dashboards to Coaches</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/7/SendCoachDashboards.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			
			<p><strong><a name="q5.5" class="title">Step 5 - Release the final rosters to players</a></strong></p>
			
			<p>
			Release Team Roster information to Players on their Player Dashboards. 
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/8/ReleaseTeamInfo.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
	
		<dt><a name="q6">How do I email certain Coaches only?</a></dt>
		<dd>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dashboard > Communicator > All Divisions > One or more individual coaches</em>
			</p>
		</dd>

		<dt><a name="q7">What about Coach Training?</a></dt>
		<dd>
			<p>
			Here is a movie that describes the Coach Dashboard for Coaches - Three Minute Movie 
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/InSD/Coach/1/CoachDashOverview.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">Three Minute Movie</a>
			</p>
			<p>
			You can email it to your Coaches. To do so, paste the following link into your email:
			</p>		
			<p>
			http://media.tawala.com/InSD/Coach/1/CoachDashOverview.htm
			</p>
			<p>
			This movie is available at the top of every Coach Dashboard.
			</p>
		</dd>
		
		<dt><a name="q8">Can I print Player medical information for Coaches?</a></dt>
		<dd>
			<p>
			To print medical information for your Coaches by team:
			</p>
			<p>
			Go to:
			</p>
			<p>
			<em>Admin Dash > View Team Rosters > Display all Rosters</em>
			</p>
			<p>
			You can display Team lists.
			</p>
			<p>
			If you click "Print this List" next to the list of Players, it will bring up your printer dialog box.
			</p>
			<p>
			Because there's quite a bit of information and it's wide, try setting the page setup for printing in your browser to landscape. If you have a "fit on one page" command, you can use that too.
			</p>
			<p>
			You can also export the player list to Excel to format and print, or to email to the coach to print themselves.
			</p>		
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/10/PrintRosters.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
	</dl>
</div>
