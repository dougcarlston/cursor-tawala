<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<div class="section">
	<form:form method="POST" commandName="form">
		<p>
			<label class="block bold" for="name">Your Name</label>
			<form:input path="name" size="40" id="name" />
		</p>
		<p>
			<label class="block bold" for="email">Your Email</label>
			<form:input path="email" size="40" id="email" /> <form:errors path="email" cssClass="error" />
		</p>
		<br />
		<p>
			<label class="block bold" for="recipients[0]">Send Email To (one address per box)</label>
			<form:input path="recipients[0]" size="40" id="recipients[0]" /> <form:errors path="recipients[0]"  cssClass="error" />
		</p>
		<p>
			<form:input path="recipients[1]" size="40" id="recipients[1]" /> <form:errors path="recipients[1]"  cssClass="error" />
		</p>
		<p>
			<form:input path="recipients[2]" size="40" id="recipients[2]" /> <form:errors path="recipients[2]"  cssClass="error" />
		</p>
		<p>
			<form:input path="recipients[3]" size="40" id="recipients[3]" /> <form:errors path="recipients[3]"  cssClass="error" />
		</p>
		<p>
			<form:input path="recipients[4]" size="40" id="recipients[4]" /> <form:errors path="recipients[4]"  cssClass="error" />
		</p>
		<p>
			<form:input path="recipients[5]" size="40" id="recipients[5]" /> <form:errors path="recipients[5]"  cssClass="error" />
		</p>
		<br />
		<p>
			<label class="block bold" for="message">Your Message</label>
			<form:textarea path="message" rows="4" cols="64" ></form:textarea>
		</p>

		<p class="emailExtra">
			<label class="block bold" for="email1">Email Extra</label>
			<form:input path="spamTrap" size="40" id="email-extra" />
		</p>
		
		<div class="buttons"><button type="submit" value="SUBMIT">SUBMIT</button></div>
		<br />
	</form:form>
</div>
