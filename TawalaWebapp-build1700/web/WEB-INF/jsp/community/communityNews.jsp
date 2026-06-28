<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

	<div class="section">
		<p>
			The Community Pages is you're area for communicating with other Tawala designers.
			You can post tips and tricks here in the Community News section, share projects in 
			the Community Library or check out the FAQ section for answers to common questions about Tawala.
	</div>
		
		<div class="yui-gc">
			<div class="yui-u first">
				<div id="communityNews" class="section newsFeed" >
				</div>
			</div>
			<div class="yui-u">News navigation will be found here...</div>
		</div>
	
<script type="text/javascript" charset="utf-8">
	currentPage = "communityNews";
	$E.on(window, "load", setMenuHighlight);
</script>
	