<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<div class="section">

	<h3>You have successfully created a Tawala Account.</h3>
	<br /> 
	<p>
		You can now access your My Tawala. My Tawala is a personal space where you can save and 
		deploy your own Tawala apps. 
	</p>
	<br />
	<p>
		See the <a href="/projectmanager/view">My Tawala</a> tab above.	
	</p>
</div>

<script type="text/javascript">
	// Report registration sucess to Google Analytics
	urchinTracker('/user/initialsetup/accountcreated');
</script>
