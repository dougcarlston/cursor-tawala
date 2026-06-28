<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section">
	
	<h3>Registration</h3>
	
	<ul class="faq">
		<li><a href="#q1">How do I view registration information?</a></li>
		<li><a href="#q2">How do  I edit registration information for specific players?</a></li>
		<li><a href="#q3">How do I take out duplicate registrations?</a></li>
		<li><a href="#q4">How do I email players who've registered?</a></li>
		<li><a href="#q5">How do I email players who haven't paid?</a></li>
		<li><a href="#q6">How do I mark players who pay by check as paid?</a></li>
		<li><a href="#q7">How do I handle refunds?</a></li>
		<li><a href="#q8">How do I send registration reminder emails to those who haven't registered yet?</a></li>
		<li><a href="#q9">What do I tell customers who get a timeout error message?</a></li>
		<li><a href="#q10">What do I do if customers complain about "lost" emails?</a></li>
	</ul>
	
	<dl>
		<dt><a name="q1">How do I view registration information?</a></dt>
		<dd>
			<p>
			You can see who's registered by looking at View Player Data. There are some basic reports, 
			and you can also create a custom report with the information you are interested in to 
			view or export to Excel.
			</p>
			<p>			
			Go to:
			</p>
			<p>
			<em>Admin Dashboard > View Player Data</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/1A/ViewRegInfo.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			<p>
			Registrations are added as they come in, with the newest registrations at the end of the list. To see newest registrations:
			</p>
			<p>
			Go to:
			</p>
			<p>
			<em>Admin Dashboard > View Player Data</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/1/ViewNewRegistrations.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>			
			</p>
		</dd>

		<dt><a name="q2">How do I edit registration information for specific players?</a></dt>
		<dd>
			<p>
			You can change any of the registration information for a particular player. They can also 
			update this information from their Player Dashboard.
			</p>
			<p>
			Go to:
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Player Registration Tools > View/Edit a specific registration</em>
			</p>

			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/1B/EditRegInfo.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>			
			</p>
		</dd>

		<dt><a name="q3">How do I take out duplicate registrations?</a></dt>
		<dd>
			<p>
			Players sometimes register twice. You will need to clean those out as they cause problems 
			down the road. We recommend you check once every week or two during the registration 
			period. Make sure to never delete registrations marked as paid.
			</p> 
	
			<p>
			Go to:
			</p>
			<p> 
			<em>Admin Dashboard > Preseason Activities > Player Registration Tools > Check for Duplicates.</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/8/DedupingRegistrations.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
	 	</dd>
		
		<dt><a name="q4">How do I email players who've registered?</a></dt>
		<dd>
			<p>
			You can email everyone who has registered who are not yet placed on teams from Player Registration in the Preseason Menu.
			</p>
			<p>
			Go to:
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Player Registration Tools > Send Email to Registered Players.</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/2/EmailRegistrants.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			 
			</p>
			<p>
			In general, you send out emails from different places in SportsDashboards depending on 
			where you are in your season. 
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
			<br />
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
			<br />
			<p>After Opening Day:</p>
			<p>
			Send email to entire Teams and/or Coaches (sends only to Players assigned to Teams)
			</p>
			<p>
			Go to:
			</p> 
			<p>
			<em>Admin Dash > Communicator</em>
			</p>			
			<p>
			Tip: If you want to email a particular group of registrants and you don't see an easy 
			way to do it, you can set up a temporary team, put them in the team, and send email to 
			the team.
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/3/EmailGroupRegistrants.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
	
		<dt><a name="q5">How do I email players who haven't paid?</a></dt>
		<dd>
			<p>
			You can mass email registrants who haven't paid and are not "marked as paid". Registrants 
			are "marked as paid" when they pay by PayPal or you "mark them as paid" from the 
			administrator dashboard.
			</p>
			<p>
			Here is some sample text you can use to send to registrants who haven't paid:
			</p>
			<p> 
			<em>"Our records indicate payment has not been received for your registration. If you believe 
			you're receiving this email in error, please contact us. Otherwise, please go to your 
			Player Dashboard and click "Pay Now" to make payment...".</em>
			</p>
	
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Player Registration Tools > Send Email to Registered Players</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/4/SendPaymentReminders.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
		
		<dt><a name="q6">How do I mark players who pay by check as paid?</a></dt>
		<dd>
			<p>
			When Players pay by check, you can mark them as paid in the system. You can also put in the 
			amount paid. Mark scholarship kids as paid, and enter an amount of 0.
			</p> 
	
			<p>
			Go to:
			</p>
			<p> 
			<em>Admin Dashboard > Preseason Activities > Player Registration Tools > Mark Waivers and Payments</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/5/MarkPlayerasPaid.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>

		<dt><a name="q7">How do I handle refunds?</a></dt>
		<dd>
			<p>
			You can easily give players refunds for transactions. If the refund is within 60 days of the 
			original transaction, you do not pay any processing fees. The refund is a credit on their 
			credit card. If making a partial refund, you also need to edit the player's payment amounts 
			in Player Registration Tools.
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/4/PayPalRefunds.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
			
		<dt><a name="q8">How do I send registration reminder emails to those who haven't registered yet?</a></dt>
		<dd>
			<p>
			It's a good idea to send a few additional emails to your recruitment list reminding them to 
			register, especially as the registration deadline nears.
			</p> 
	
			<p>
			SportsDashboards can send reminder emails to those who haven't registered yet, but if a 
			player puts in a first name that is different in some small way (Alex vs. Alexander) from 
			the name in the email recruitment list, the system will not see them as the same person. 
			They will receive a reminder email even though they've already registered. The easiest 
			thing to do is to take their email off of the recruitment email list since you already 
			have their contact information in their registration.
			</p>
	
			<p>
			In general, when writing a reminder email, It's always a good idea to say, "If you 
			haven't registered already..." as an opening statement.
			</p>
	
			<p>
			Go to:
			</p>
			<p> 
			<em>Admin Dashboard > Preseason Activities > Recruitment List Tools > Send Email to Recruitment List</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/7/EmailRegistrationReminders.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			<p>
			To delete an email from the Recruitment List go to:
			</p>
			<p> 
			<em>Admin Dashboard > Preseason Activities > Recruitment List Tools > Delete a Recruitment Record</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/9/DeleteFromEmailRecruitmentList.htm','Video','width=650,height=500,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
			
		<dt>Common Questions you might receive:</dt>
		<dd>
			
			<p><strong><a name="q9" class="title">Timeout error message</a></strong></p>
	
			<p>
			Some customers will complain that they received a 'timeout' error message after they've submitted 
			their registration forms. This comes about because either they were on the page for more than 45 
			minutes or because they clicked away from the registration page in their browser and then came back 
			to it using the back button on the browser. We recommend you tell those customers:
			</p> 
	
			<p>
			"Please try to register again. First close all your browser windows, and then reopen your registration 
			form. Please complete the form within 45 minutes because after that it will time out. Also, please 
			complete the entire form without going somewhere else on the web and returning to the registration 
			form using the back button on the browser. Certain information is mandatory on the form, so if after 
			you click submit, you see the registration form again, look for some text at the top of the form in 
			red explaining what's missing. It is very important to complete the registration by clicking the submit 
			button at the bottom of the form. If you are still having difficulty, please feel free to contact 
			stevep@tawala.com for customer support."
			</p>
	
			<p><strong><a name="q10" class="title">Lost Emails - 'I didn't receive the email I'm supposed to...'</a></strong></p>
	
			<p>
			90% of the time players find lost emails in their spam folders or discover there is a typo in their 
			email address on SportsDashboards. You can write them to check their spam folders, and if it's there, 
			to change the 'rules' of their spam filter to allow email from you.
			</p>
	
			<p>
			If they check their spam folders, and can't find it still, please forward it to us. We will track 
			down the path of the email and find the cause of non delivery.
			</p>
		</dd>
	</dl>
</div>
