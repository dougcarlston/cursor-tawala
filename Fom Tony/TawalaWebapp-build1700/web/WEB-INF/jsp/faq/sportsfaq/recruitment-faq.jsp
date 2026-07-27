<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section">
	
	<h3>Recruitment Email Blast</h3>
	<ul class="faq">
		<li><a href="#q1">What is the Recruitment Email Blast?</a></li>
		<li><a href="#q2">How do I prepare the Recruitment Email List?</a></li>
		<li><a href="#q3">How do I put the Recruitment Email List into SportsDashboards?</a></li>
		<li><a href="#q4">How do I send my Recruitment Email?</a></li>
		<li><a href="#q5">How do I send registration reminder emails to those who haven't registered yet?</a></li>
		<li><a href="#q6">What should I do with bounced emails?</a></li>
	</ul>
	
	<dl>
		<dt><a name="q1">What is the Recruitment Email Blast?</a></dt>
		<dd>
			<p>
			You may want to email everybody from last year's program to see if they would like to register 
			again for a new season. 
			</p>
			<p>
			You can send a registration invitation email from SportsDashboards to everyone on a recruitment 
			email list. The recruitment email has your own words, comes from you, and contains a link to 
			the online registration form. 
			</p>
		</dd>
		
		<dt><a name="q2">How do I prepare the Recruitment Email List?</a></dt>
		<dd>
			<p>
			Please create an Excel file containing the Player First Name, Player Last Name, Email address1, 
			Email address2, and Parent First Name of those you wish to invite.  
			</p>
			<p>
			Don't worry if you don't have all this information. We can manage with as little as just the 
			email addresses. 
			</p>
			<p>
			Typically, for parents with two or more kids, we put in an entry for each kid. The parent 
			will get two emails, but if the email salutation is set to 'parents of (kid name)', which 
			SportsDashboards can do for you, it will look OK to the parents because they see an email 
			for each kid. This allows SportsDashboards to compare the registration list to the recruitment 
			list accurately based on the player name. Then, when you send subsequent emails out to your 
			recruitment list (for instance, 'reminder - registration closes Friday'), you don't send 
			email to players who have already registered. 
			</p>
		</dd>
		
		<dt><a name="q3">How do I put the Recruitment Email List into SportsDashboards?</a></dt>
		<dd>
			<p>
			We will import the information for you into your SportsDashboards. 
			</p>
		</dd>
		
		<dt><a name="q4">How do I send my Recruitment Email?</a></dt>
		<dd>
			<p>
			You can send your Recruitment Email by going to:
			</p>
			<p>
			<em>Administrator Dashboard > Preseason Activities > Recruitment List Tools > Send Email to Recruitment List</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Rec/1/SendRecruitmentEmail.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			<p>
			As I'm sure you already know, the recruitment email itself should cover the essentials of your 
			program. These include: Key Dates, Eligibility, Uniform information, volunteer policies, 
			financial assistance, Clinics, etc.
			</p>
		</dd>
		
		<dt><a name="q5">How do I send registration reminder emails to those who haven't registered yet?</a></dt>
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
			In general, when writing a reminder email, It's always a good idea to say, "If you haven't 
			registered already..." as an opening statement.
			</p>
			<p>
			To send a reminder email go to: 
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
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Reg/9/DeleteFromEmailRecruitmentList.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
		
		<dt><a name="q6">What should I do with bounced emails?</a></dt>
		<dd>		
			<p>
			In any given year about 5% of parents change their email addresses, resulting in a fair 
			number of bounced email notices coming to your email account.
			</p>
			<p>
			You can email the registration link to them once you get their correct email address.
			</p>
			<p>
			You can also enter their correct email addresses into SportsDashboards so that when you 
			send your next registration reminder email, they will receive it with everyone else and 
			you will not have bounces. 
			</p>
			<p>
			Go to: 
			</p>
			<p>
			<em>Admin Dashboard > Preseason Activities > Recruitment List Tools > Edit a Recruitment Record</em>
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Rec/2/EditRecruitmentList.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
	</dl>
</div>
 