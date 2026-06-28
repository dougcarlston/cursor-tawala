<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

<div class="block">
	<div class="content">
		<div class="buttons">
			<button type="submit"  id="pmViewRestoreProject" class="dark" title="Restore project data">
				RESTORE PROJECT FROM BACKUP
			</button>
		</div>
	</div>
	<br />
</div>

<!-- New IFRAME based dialog -->
<div id="dialogIFrame" style=" text-align: left;">
	<div class="hd"><div class="tl"></div><span id="dialogTitle">Publish Project to the Community Library</span><div class="tr"></div></div>
	<div class="bd">
		<div id="dialogContentIFrame" class="content">
			<div id="restoreContentIF" style="display:none;">
				<div id="restoreContentIFrame"></div>
			</div>

		</div>
	</div>
	<div class="ft"></div>
</div>

<c:url var="restoreURL" value="${urls.projectManagerRestoreProjectFromBackup}">
</c:url>

<script type="text/javascript">
<!--
var linkToRestore = '${restoreURL}';
var isAdmin = ${user.administrator};
-->
</script>
