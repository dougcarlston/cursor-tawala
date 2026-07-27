<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section">
	<h3>SportsDashboards Knowledgebase</h3>
	<p>Enter your question or a few keywords in the search box.</p>
	<IFRAME id="salesforceSolutions" 
		title="Content" 
		SRC="http://na6.salesforce.com/sol/public/search.jsp?orgId=00D80000000KK0U" 
		WIDTH="590" 
		HEIGHT="810">
	</IFRAME>
	
</div>