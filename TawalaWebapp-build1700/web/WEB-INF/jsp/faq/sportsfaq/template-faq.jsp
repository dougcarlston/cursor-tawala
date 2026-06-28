<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	Tawala.config.pageName = '${pageName}';
</script>

<div class="section"><%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName"></tiles:importAttribute>

<script type="text/javascript" charset="utf-8">
	var Tawala.config.pageName = '${pageName}';
</script>



	<h3>Title</h3>
	
	<ul class="faq">	
		<li><a href="#q1"></a></li>
		<li><a href="#q2"></a>
			<ul>	    
				<li><a href="#q2.1"></a></li>
			    <li><a href="#q2.2"></a></li>
			</ul>
		</li>
		<li><a href="#q3"></a></li>
	</ul>
	
	<dl>
		<dt><a name="q1">Question</a></dt>
		<dd>
			<p> 
			Answer
			</p> 
		</dd>
	
		<dt><a name="q2">Question</a></dt>
		<dd>
			<p>
			Answer
			</p>
		</dd>
	
		<dt><a name="q3">Question</a></dt>
		<dd>
			<p>
			Answer
			</p>
		</dd>
	</dl>
</div>