<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section">
	
	<h3>Setup</h3>
	
	<ul class="faq">	
		<li><a href="#q1">Who should be the SportsDashboards Administrator?</a></li>
		<li><a href="#q2">How do I customize my Player Registration Form?</a></li>
		<li><a href="#q3">What is the Coach Registration Form?</a></li>
		<li><a href="#q4">Can Board Members and others use SportsDashboards?</a></li>
		<li><a href="#q5">How do you handle Registration late fees?</a></li>
		<li><a href="#q6">What about sibling discounts?</a></li>
	</ul>
	
	<dl>
		<dt><a name="q1">Who should be the SportsDashboards Administrator?</a></dt>
		<dd>
			<p>
			The Athletic Director and/or Registrar are good choices for the SportsDashboards 
			Administrator as they will be communicating with Players and their families. You 
			can put two people on as administrators, e.g. "Jill Smith (AD)/ Bob Thornton (Registrar)", 
			but there is only one email address associated with the SportsDashboards administrator. 
			</p>
			<p>
			Your Treasurer will want access to the Administrator Dashboard to track payments and 
			send out payment reminder notices. 
			</p>
			<p>
			You can share the Administrator Dashboard with others, and they can send mass emails 
			from the Administrator Dashboard and have the emails show as coming from them on an 
			email by email basis.
			</p>
			<p>
			If security is a concern, you can enable password protection for the Administrator 
			Dashboard. 
			</p>
		</dd>
		
		<dt><a name="q2">How do I customize my Player Registration Form?</a></dt>
		<dd>		
			<p>
			You can customize your registration form from your Administrator Dashboard. 
			</p>
			<p>
			You can decide whether to include certain questions, add up to 3 custom questions, 
			set up divisions, set up your pricing plan, create special instructions, and add an 
			online waiver and online code of conduct (or change the language of those). 
			</p>
			<p>
			If you want to re-order certain lists such as divisions or grade, you can do so, but it is 
			awkward. Please contact us to do it for you.
			</p>
			<p>
			Go to: 
			</p>
			<p><em>Admin Dashboard > View/Edit Administrator Setup</em></p>
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/S/1/CustomizeRegForm.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
		</dd>

		<dt><a name="q3">What is the Coach Registration Form?</a></dt>
		<dd>		
			<p>
			There is an online Coach Registration form you can set up. In addition to asking for 
			basic contact information, you can also ask custom questions and include a Coach 
			Agreement which all prospective coaches must click to accept. 
			</p>
			<p>
			Here's an overview:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2/CoachOverview.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			<p>
			You can invite potential coaches to register by emailing them a link to the Coach 
			Registration Form:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2A/CoachRecruitment.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			<p>
			You can also manually enter Coach information directly if you don't want them to fill out a 
			registration form:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2B/CoachEntry.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			<p>
			To set up and customize the Coach Registration Form:
			</p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/Ros/2C/CustomizeCoachForm.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
		</dd>
				
		<dt><a name="q4">Can Board Members and others use SportsDashboards?</a></dt>
		<dd>
			<p> 
			Sometimes leagues have Board Members and others who are not coaches or players, but who would like to 
			send or be copied on group emails. If they want to send emails, you can give them access to the 
			Administrator Dashboard and they can send emails and enter their name and return email address.
			</p>
			<p>
			If they also want to receive emails, such as emails the Administrator sends to the entire league, 
			you can set them up as "Coaches" and assign them to their own team of one. When emails are sent to 
			all coaches, for instance, they will be included.
			</p> 
		</dd>
	
		<dt><a name="q5">How do you handle registration late fees?</a></dt>
		<dd>
			<p> 
			You can change the pricing on your Registration Form at any time from Administrator Setup. People 
			like having control over this as they will often extend the registration period at normal prices 
			as an incentive to get people to hurry up and register. You can indicate a late registration fee 
			in one of the custom text boxes at the top of the Registration Form. 
			</p> 
		</dd>
	
		<dt><a name="q6">What about sibling discounts?</a></dt>
		<dd>
			<p>
			We don't have a direct way of handling sibling discounts on the registration form.
			</p>
	
			<p>
			It gets a little complicated for us. Different leagues have different policies on sibling 
			discounts, including progressive discounts. It is difficult to support registration by family 
			as opposed to registration by player without things becoming too complicated for everybody. 
			We have learned, though, that some leagues are OK with "trust" based systems, where people 
			basically elect to take the sibling discount and mark it that way on the registration form.
			</p>
	
			<p>
			Here are some ways our customers are addressing sibling discounts with SportsDashboards.
			</p>
	
			<ul>
				<li>
				<strong>Set up a Custom Pricing Schedule</strong> - If your league has relatively simple 
				pricing, you can set up a custom pricing schedule where registrants select from a set of 
				choices. It could be, "First child price, Second child price, etc." with different 
				amounts. This would be an honor system on the part of registrants. 
				</li>
				<li>
				<strong>Have families pay by check</strong> - You can tell people there is a sibling discount, 
				and if they want it, they can pay the correct amount by check. One of our leagues has put this in 
				one of their custom questions (e.g. There is a sibling discount. If you would like to take it, 
				please pay by check and mark this question "sibling discount").
				</li> 
	
				<li>
				<strong>Pay Refunds</strong> - Other leagues we know have been offering refund payments for sibling 
				discounts. They basically say something to the effect of, "If you would like a sibling discount, 
				please contact us and we will issue a partial refund on PayPal or a check". In practice, not 
				many parents follow up on this.
				</li>
	
				<li>
				<strong>Eliminate sibling discount</strong> - Our experience is that many leagues are starting to 
				eliminate sibling discounts because it is an administrative burden relative to the amount of money 
				involved.
				</li>
			</ul>
		</dd>
	</dl>
</div>
