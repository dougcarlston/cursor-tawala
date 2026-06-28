<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>
<div class="section">
	<form:form method="POST" commandName="form">
		<p>
			<label class="block bold" for="name">Name</label>
			<form:input path="name" size="40" id="name" />
		</p>

		<p>
			<label class="block bold" for="email">Email (kept strictly confidential)</label>
			<form:input path="email" size="40" id="email" />
		</p>
		<p>
			<label class="block bold" for="sport">Sport or Sports (e.g. Baseball, CYO, Soccer)</label>
			<form:input path="sport" size="40" id="sport" />
		</p>
		<p>
			<label class="block bold" for="URL">Website URL (to get your logo only)</label>
			<form:input path="URL" size="40" id="URL" />
		</p>

		<p class="emailExtra">
			<label class="block bold" for="spamTrap">Email Extra</label>
			<form:input path="spamTrap" size="40" id="email-extra" />
		</p>

		<br />		
		<div class="buttons large"><button class="icon check" type="submit" value="SUBMIT">YES, SET UP MY SPORTSDASHBOARDS<br /> WITHIN ONE DAY - CLICK HERE</button></div>
		<br />

	</form:form>
</div>
