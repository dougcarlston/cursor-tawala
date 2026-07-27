<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>

<div class="bottomCorner"></div>
<div>
	<ul>
		<li><a href="/info">Company Info</a></li>
		<li><a href="/terms">Terms &amp; Conditions</a></li>
		<li><a href="/privacy">Privacy Policy</a></li>
		<li><a href="/jobs">Jobs</a></li>
		<li><a href="mailto:info@tawala.com">Contact Us</a></li>
		<li><%@ include file="/WEB-INF/jsp/blocks/reportBugs.jsp" %></li>
	</ul>
</div>
