<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section">
	<h3>Security</h3>
	
	<ul class="faq">	
		<li><a href="#q1">What is the SportsDashboards security model?</a></li>
		<li><a href="#q2">What happens if the administrator URL is erroneously distributed?</a></li>
		<li><a href="#q3">How is your data center where the data resides secured?</a></li>
	</ul>
	
	<dl>
		<dt><a name="q1">What is the SportsDashboards security model?</a></dt>
		<dd>
			<p> 
			SportsDashboards has a tiered security model using randomized URL's and username/password protection. This allows a high level of protection for certain features and a high level of convenience and ease of use for others.
			</p>
			<p>		
			A randomized URL is a URL designed to make it impossible for someone to guess a particular link. An example would be:<br /><br />
			http://www.tawala.com/p/wlwns1vlyhtsj7m/nmo8jzd.AdminDash
			</p>
			<p>
			The trade off between security and convenience plays out as follows:
			</p>
			<ul>
				<li>Player and Coach Dashboards - randomized URL, unique to each individual</li>
				<li>Administrator Dashboard - randomized URL, optional password protection</li>
				<li>Project Manager in My Tawala - Username and password log-in account protection</li>
			</ul>
			<p>
			Hacking into a system such as SportsDashboards is very expensive, takes time, and requires 
			advanced technical skills. Most hackers look for information leading to immediate financial 
			gain, such as credit card information.
			</p>
			<p>
			As a security measure, SportsDashboards does not receive your online payment monies or any 
			information relating to online payments such as credit card numbers. These are all handled 
			by PayPal in your own PayPal account.
			</p>
			<p>
			PayPal has developed a security infrastructure involving hundreds of developers and a very 
			significant budget to combat this problem. Not only do they have to protect against 
			intrusions, they have to do the harder task of detecting when intrusions are successfully 
			accomplished and information stolen (something to consider when looking at other online 
			registration and web services vendors).
			</p>
			<p>
			The cases where youth sports organizations have lost money have generally involved online 
			registration providers who go out business suddenly, still owing registration fees 
			collected on their behalf.
			</p>
			<p>
			Though there is no financial motivation for hackers to attack SportsDashboards, youth 
			sports organizations are understandably concerned about player and family personal 
			information being kept private. In particular, they do not want the information used by 
			third parties for marketing purposes or for identity theft. SportsDashboards does not 
			provide this information to any third parties and considers the information to be the 
			property of each individual youth sports organization. Identity theft usually involves 
			credit card applications, and in youth sports most registrants are too young to qualify 
			for credit cards.
			</p>
			<p>
			With this structure, SportsDashboards is not a target for financial gain by the kind of 
			people who have the requisite technical skills.
			</p> 
		</dd>
	
		<dt><a name="q2">What happens if the administrator URL is erroneously distributed?</a></dt>
		<dd>
			<p>
			The Administrator can set up or change an optional password for the Administrator Dashboard.
			</p>
		</dd>
	
		<dt><a name="q3">How is your data center where the data resides secured?</a></dt>
		<dd>
			<p>
			We host our servers at a modern co-location facility.
			</p>
			<p>		
			<a href="http://www.sonic.net">www.sonic.net</a>
			</p> 
			<p>
			Here is a description of their security arrangements from their website:
			</p>
			<p>
			<i>Sonic.net recognizes the security needs of its customers. Key card entry combined with 
			biometric identification (hand recognition scanning) is required for entry. Each cabinet 
			is individually keyed with Medico lock cores and cabinets are re-keyed for each new 
			customer. This system allows our customers 24x7 access to the facility without fear of 
			unauthorized access.</i>
			</p>
			<p>
			<i>In addition to biometric controls, Sonic.net has employed a mantrap that regulates access to 
			the data center. This portal accepts a single person and uses biometric security combined 
			with key card verification prior to allowing access to the data center. This guarantees 
			that only authorized personnel are granted access and prohibits "tailgating" of non-authorized 
			persons.</i>
			</p>
			<p>
			<i>24x7 video surveillance keeps a visual record of all activity. Each cabinet door is outfitted 
			with magnetic sensors which are triggered when the door to the cabinet is opened. Through our 
			online tools, a security contact can be configured to receive an email when a cabinet is 
			accessed. IP cameras allow authorized customers to view the activity at their cabinet if 
			they receive an alert that their space was accessed.</i>
			</p>
		</dd>
	</dl>
</div>
 