<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<form:form id="nextLevelForm" action="${urls.userUpgradeToNextLevel}">
<div id="registration">
    <div class="section">
    	<img style="float: right; padding-left: 1em; padding-bottom: 1em;" src="/images/designer-screenshot1.gif" />
	    <p>
		All Tawala apps were created using the Tawala Designer. It is designed for moderately technical, 
		do it yourself, web enthusiasts. You do not have to be a programmer or engineer. 
		</p>
		<br />
		<p>
			We have created a community where you can download, modify and share Tawala apps with others 
			<br /><br />
			<a href="#" onclick="window.open('/demos/Create your Own Web Apps/Create your Own Web Apps.htm','Video','width=1040,height=700,toolbar=no,menubar=no,status=no,location=no,resizeable=yes,scrollbars=yes'); return false;">
	       		<span><img src="/images/template/red-bullet-arrow-right.gif" /> View the Tawala Designer Demo</span>
	       	</a>
		</p>
		<br />
		<p>
		To access Tawala Designer, our community, and the expanded project library, all you need to do is to 
		change your account level from "intermediate" to "advanced" and these features will become visible. 
		You can always change it back if you want to in your account settings. 
		</p>
		<br />
		<p>
		You can create and deploy hosted Tawala apps at no charge during our beta period.
		</p>
		<br />
		<div class="editActions">
			<button type="submit" name="submit" value="Submit"  title="Upgrade">Upgrade</button> 
			<a href="javascript:history.go(-1);" title="Return to previous page" ><button type="button"  title="Cancel" >Cancel</button></a>
		</div>
    </div>
</div>
</form:form>
