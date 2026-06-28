<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section">
	
	<h3>PayPal</h3>
	<ul class="faq">	
		<li><a href="#q1">What is PayPal?</a></li>
		<li><a href="#q2">How do I make PayPal work with SportsDashboards?</a></li>
		<li><a href="#q3">How do I create a nonprofit Business account with PayPal?</a></li>
		<li><a href="#q4">Why is my PayPal email address important?</a></li>
		<li><a href="#q5">What does PayPal charge?</a></li>
		<li><a href="#q6">How do I handle refunds?</a></li>
		<li><a href="#q7">Do our Registrants have to create an account on PayPal to pay?</a></li>
		<li><a href="#q8">How does our name appear on our registrants' credit card statement?</a></li>
		<li><a href="#q9">How do I receive notification of payment?</a></li>
		<li><a href="#q10">What is the procedure for resolving disputes?</a></li>
		<li><a href="#q11">How does money get from our PayPal account to our bank account?</a></li>
		<li><a href="#q12">How can I maintain payment records?</a></li>
		<li><a href="#q13">Can I use PayPal outside of SportsDashboards to charge for other items?</a></li>
		
	</ul>	
	
	<dl class="faq">
		<dt><a name="q1">What is PayPal?</a></dt>
		<dd>
			<p>
			<a href="http://en.wikipedia.org/wiki/Paypal">PayPal</a> is a service for collecting and transferring money on the Internet and is a division 
			of Ebay. PayPal handles about 70 mil. accounts worldwide.
			</p>
			<p>
			PayPal will let organizations collect money on the web from registrations via major credit 
			card. Money is collected into your own PayPal account which you can transfer anytime into 
			your bank account.
			</p>
			<p>
			When a player uses SportsDashboards online registration, the player is automatically sent to 
			PayPal with the necessary payment information. When the player pays on PayPal, the information 
			about the successful payment is sent back to SportsDashboards by PayPal.
			</p>
			<p>
			SportsDashboards never handles or has access to your money.
			</p>
		</dd>
	
		<dt><a name="q2">How do I make PayPal work with SportsDashboards?</a></dt>
		<dd>
			<p>
			If you want to accept payment by credit card you will need to set up your own PayPal account. The 
			reason for this is that we at SportsDashboards never receive or handle your money.
	 		</p>
			<p>
			When the money is collected from registrations, it is deposited in your organization's PayPal account, 
			which you can then transfer to your bank account easily (<a href="#q10">see this section</a>).
			</p> 
			<p>
			You can either set up a "Business" account, or a "Business Account for Non-profits".
	 		</p>
	 		<p>
			The difference is that the transaction fees for nonprofits is 2.2%+$.30 per transaction vs. 
			2.9%+$.30 per transaction for regular businesses.
	 		</p>
	 		<p>
			However, setting up a non profit account with PayPal involves sending in:
	 		</p>
	 		<ol>
				<li>Information about the nature of your organization and the type of payments you intend to process 
				with PayPal, and</li>
				<li>Evidence of tax exempt status or registration with any regulatory bodies; i.e., the 501c3 
				letter.</li>
	 		</ol>
			<p> 
			PayPal will let you set up the nonprofit account right away. You will need to follow up by sending 
			in the requested documents. If PayPal doesn't receive those documents, they will turn off the ability 
			to receive monies into your PayPal account after about 45 days. That can be quickly restored with a 
			phone call to PayPal, but you will still need to send in the requested documents.
	 		</p>
	 		<p>
			It is very difficult to change the PayPal account type once it has been set up and linked to your bank 
			account.
	 		</p>
	 		<p>
			It's up to you which way you go. The difference in cost between a regular Business Account and a 
			Non-profit account is quite small.
	 		</p>
	 		<p>
			There are no set up or monthly costs for either kind of account.
	 		</p>
	 		<p>
			Choose "Website Payments Standard" for the Payment Solution.
	 		</p>
	 		<p>
			It takes 3-5 days to set up the account because PayPal needs to verify your bank account transfer 
			capabilities with an actual test.
	 		</p>
	 		<p>
			When you set up your PayPal account, please send us your account log in email address. We do not 
			need your password.		
			</p>
			<p>
			How to get Started with PayPal -
			</p>
			<p> 
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/PayPal/SetupPayPal/SetupPayPal.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			<p>
			Link to PayPal: <a href="http://www.paypal.com/">www.paypal.com</a>
			</p>
			<p>
			Payment Experience with PayPal for Player's Family 
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/2/PayPalUserExperience.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
		
		<dt><a name="q3">How do I create a nonprofit Business account with PayPal?</a></dt>
		<dd>
			<p>
			Go to the PayPal website and set up a nonprofit business account on PayPal. 
			</p>

			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/PayPal/SetupPayPal/SetupPayPal.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			<p>
			Nonprofits should always set up nonprofit PayPal accounts right from the start. It is difficult to change from a business account to a nonprofit business account, and vice versa. 
			</p>
			<p>
			Nonprofits should not accept any funds until PayPal has approved nonprofit status. The process takes about a business week. 
			</p>
			<p>
			If down the road you need to change Treasurers, the best way to change the owner of the PayPal account is to delete the PayPal account and create a new PayPal account. This step wipes out any information associated with the old treasurer, including personal information, and any links to bank accounts. The new treasurer can then set up a new nonprofit PayPal account. Make sure to delete the old account first before setting up the new account. You will have to get approval from PayPal for nonprofit status again. 
			</p>
			<p>
			Here is the text of an email from PayPal on how to get nonprofit status approved: 
			</p>
			<p>
			From PayPal:
			</p>
			<em>
			<p>
			Hello, 
			</p>
			<p>
			PayPal appreciates that you have chosen us to accept payments for your organization. 
			As part of PayPal's compliance program, we request that entities wishing to accept donations on behalf of a charity or other non-profit organization provide evidence of their legitimacy. 
			Please provide the following information: 
			</p>
			<ol>
				<li>Evidence of tax exempt status and/or registration with any applicable regulatory bodies governing your jurisdiction.</li>
				<li>Link to confirm the organization's registration status online (if applicable).</li> 
				<li>A brief organizational summary or Mission Statement.</li> 
				<li>Subordination letter from the parent organization (if applicable).</li> 
			</ol>

			<p>
			Please fax the requested information to 303-395-2862 within seven days. 
			</p>
			<p>			
			Once we have received and reviewed the aforementioned material, we reserve the right to ask additional questions on your policies and procedures. 
			</p>
			<p>
			Failure to provide the information may result in the closing of your account. 
			</p>
			<p>
			Send any questions to compliance@paypal.com. Please remember to include your email address as registered on your PayPal account on any correspondence or faxed items. 
			</p>
			<p>
			Your assistance and expediency in this matter is appreciated. 
			</p>
			<p>
			If you have any further questions, please feel free to contact us again. 
			</p>
			
			<p>
			Merchant Solutions<br /> 
			PayPal, an eBay Company 
			</p>
			<p>
			PayPal Customer Service:<br /> 
			1-402-935-2050 <br />
			(a U.S. telephone number)<br /> 
			4:00 AM PST to 10:00 PM PST Monday through Friday<br /> 
			6:00 AM PST to 8:00 PM PST Saturday and Sunday 
			</p>
			<p>
			PayPal Help Page
			</p>
			<p>
			<a href="https://www.paypal.com/in/cgi-bin/helpweb?cmd=_help" target="_blank">https://www.paypal.com/in/cgi-bin/helpweb?cmd=_help</a>
			</p>
			</em>
		</dd>
		
		<dt><a name="q4">Why is my PayPal email address important?</a></dt>
		<dd>
			<p>
			PayPal uses an email address for the account name. Since this email address is what player parents 
			see when registering, it is advisable to use an official email address from your organization 
			instead of a personal email address. If no official email address exists, set up a <a href="http://gmail.com/">Gmail account</a>, 
			using some organizational identifier as the username. For instance, you could be 
			SpringfieldLittleLeague@gmail.com. The reason SportsDashboards recommends Gmail is that it's easy to 
			forward mail to your own mailbox so you don't have to check the Gmail inbox all the time.
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/3/Gmail.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
		
		<dt><a name="q5">What does PayPal charge?</a></dt>
		<dd>
			<p>
			PayPal charges you 2.9%+$.30 per transaction. On a $100 fee, that is $3.20. There is a volume discount after you reach $3,000 per month, and eventually at $100,000 per month you will reach 1.9% + $.30. This money is deducted from the proceeds. So if you set a price of $100 for your program, you will receive about $97. Your parents will see they paid $100 and will not see the credit card processing fee.
			</p>
			<p>
			Our experience with Leagues is that, when given a choice, about half of Registrants pay online and 
			the other half pay with checks. If they pay by check, there is no transaction cost. So your total 
			PayPal cost will reflect this behavior and be less than if everybody paid online using PayPal.
			</p>
		</dd>
	
		<dt><a name="q6">How do I handle refunds?</a></dt>
		<dd>
			<p>
			You can easily give players refunds for transactions. If the refund is within 60 days of the 
			original transaction, you do not pay any processing fees. The refund is a credit on their 
			credit card.
			</p>
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/4/PayPalRefunds.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
	
		<dt><a name="q7">Do our Registrants have to create an account on PayPal to pay?</a></dt> 
		<dd>
			<p>
			No. They can just use their credit card.
			</p>
			
			<p>
			However, they have to log into PayPal if they already have a PayPal account and it is linked to the particular credit card they want to use.
			</p>
		</dd>
					
		<dt><a name="q8">How does our name appear on our registrants' credit card statement?</a></dt>
		<dd>
			<p>
			Usually the name displayed on credit card statements is the name of the PayPal account.
			</p>
			
			<p>
			You can check how your name appears on credit card statements and modify it if necessary in your PayPal account.
			</p> 
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/5/HowChargesAppearonCC.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>
	
		<dt><a name="q9">How do I receive notification of payment?</a></dt>
		<dd>
			<p>
			When a payment is received by PayPal, you and the registrant will be notified by an email 
			from PayPal. SportsDashboards will also be informed about the payment. Your accounting 
			department will want to reconcile payments on the PayPal system with those on our system.
			</p>	
		</dd>
		
		<dt><a name="q10">What is the procedure for resolving disputes?</a></dt>
		<dd>
			<p>
			If a customer says they have paid, but it doesn't show in the system, ask them to send you 
			the confirmation number that was emailed to them. With that information, you can look up 
			the transaction within PayPal and make a determination about what occurred. This happens 
			infrequently.
			</p>
		</dd>
		
		<dt><a name="q11">How does money get from our PayPal account to our bank account?</a></dt>
		<dd>
			<p>
			Money received is held in your PayPal account when it clears. In the case of credit cards, it clears instantly. There is also a bank transfer option for payers which takes 5 business days to clear. 
			</p>		
			<p>
			You can transfer the money into your bank account at any time from PayPal by going online to your PayPal account and transferring the funds to your bank account.
			</p>		
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/6/TransferFundsToBank.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
			
			<p>
			Sometimes there is a concern that PayPal limits the amount transferable to your bank account to $500 per month. This will happen during a verification period, but is less likely to happen if you have a Personal Premier or Business Account. Our Leagues have found that after verification, if the restriction is still on, they can easily lift the restriction and make transfers unlimited with a short phone call to PayPal. This is part of PayPal's extensive fraud detection program.
			</p>
		</dd>
		
		<dt><a name="q12">How can I maintain payment records?</a></dt>
		<dd>
			<p>
			You can download all of the transactions into an Excel spreadsheet from PayPal for analysis purposes. 
			</p>		
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/7/ExportPayPalInfo.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			</p>
		</dd>

		<dt><a name="q13">Can I use PayPal outside of SportsDashboards to charge for other items?</a></dt>
		<dd>
			<p>
			You can use your PayPal account to create a "Buy Now" link or button for any goods or services
			you wish to sell independently of SportsDashboards.			
			</p>		
			<p>
			<a class="iconLink movie" target="oneMinuteMovie" onclick="window.open('http://media.tawala.com/FAQ/P/8/CreateBuyNowLinkinPayPal.htm','Video','width=650,height=550,toolbar=0,menubar=0,status=0,location=0,resizeable=no,scrollbars=no,screenX=200,screenY=130,left=200,top=130'); return false;" href="#">One Minute Movie</a>
			
			</p>
		</dd>
	</dl>
</div>
