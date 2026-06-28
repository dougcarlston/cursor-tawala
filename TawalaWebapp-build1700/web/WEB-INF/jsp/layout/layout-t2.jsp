<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<tiles:importAttribute name="pageName" />
<tiles:importAttribute name="pageTitle" />
<tiles:importAttribute name="mainBlockList" />
<tiles:importAttribute name="infoBlockList" />
<tiles:importAttribute name="secondaryContent" />

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
	<head>
        <title><tiles:getAsString name="pageTitleBar" /></title>
        <meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
        <meta http-equiv="content-language" content="EN" />
        
		<meta name="keywords" content="SportsDashboards, Youth Sport Leagues, Registration, Communication, Management, baseball, softball, basketball, little league, soccer, football">
        <meta name="description" content="SportsDashboards - Youth Sport Leagues Registration, Communication and Management">
        
    	<link rel="icon" type="image/x-icon" href="/images/favicon.ico" />
    	<link rel="shortcut icon" type="image/x-icon" href="/images/favicon.ico" />
    
    	<tiles:useAttribute id="scriptList" name="scripts" 
    	    classname="java.util.List" ignore="true"/>

    	<tiles:useAttribute id="additionalScriptList" name="additionalScripts" 
    	    classname="java.util.List" ignore="true"/>
    	
    	<c:forEach var="js" items="${scriptList}">
    	    <script type="text/javascript"
    	        src="<%=request.getContextPath()%><c:out value="${js}"/>"></script>
    	</c:forEach>
    	<c:forEach var="js" items="${additionalScriptList}">
    	    <script type="text/javascript"
    	        src="<%=request.getContextPath()%><c:out value="${js}"/>"></script>
    	</c:forEach>
    	   
    	<tiles:useAttribute id="styleList" name="styles"
    		classname="java.util.List" ignore="true" />
    	
    	<tiles:useAttribute id="additionalStyleList" name="additionalStyles"
    		classname="java.util.List" ignore="true" />
    	
    	<c:forEach var="css" items="${styleList}">
    		<link rel="stylesheet" type="text/css" media="all"
    			href="<%=request.getContextPath()%><c:out value="${css}"/>" />
    	</c:forEach>

    	<c:forEach var="css" items="${additionalStyleList}">
    		<link rel="stylesheet" type="text/css" media="all"
    			href="<%=request.getContextPath()%><c:out value="${css}"/>" />
    	</c:forEach>

   	    <script type="text/javascript">
   	    	// Pass config values from Java to Javascript here
   	    	Tawala.config.pageName = "${pageName}";   	    	
   	    </script>
    	
	</head>

    <tiles:useAttribute name="onLoadJavascriptFunction" ignore="true" />

    <body <c:if test="${! empty onLoadJavascriptFunction}"> onload="${onLoadJavascriptFunction}"</c:if> >
	    <a id="top"></a>
        <div id="doc" class="yui-t3">
            <div id="hd">
        		<tiles:insert name="pageheading" >
        			<tiles:put name="pageName" value="${pageName}" />
        		</tiles:insert>
            </div> <!-- end hd -->

            <div id="bd">
					<div class="pageTitle">
						<h1 id="pageTitle"><tiles:getAsString name="pageTitle" /></h1>
						<!-- operation status messages go here -->
						<div id="opStatus">
							<div id="ajaxStatus"></div>
						</div>
                    </div>
            	<c:if test="${! empty urgentMessage}">
            		<div class="urgent-message">${urgentMessage.text}</div>
            	</c:if>
                <div id="yui-main">
                    <div  class="yui-b first">
						<tiles:insert name="mainContent">
							<tiles:put name="block-tawalaissues" value="/WEB-INF/jsp/blocks/block-tawalaissues.jsp" type="page" />
							<tiles:put name="block-faq" value="/WEB-INF/jsp/blocks/block-faq.jsp" type="page" />
							<tiles:put name="secondaryContent" value="${secondaryContent}"/>		                    		                    
		                    <tiles:putList name="rightBlockList" >
								<c:forEach var="block" items="${rightBlockList}" varStatus="status">
									<tiles:add content="${block}" />
								</c:forEach>
							</tiles:putList>
		                </tiles:insert>
                    </div><!-- end yui-b -->
                </div> <!-- end yui-main -->

                <div class="yui-b last">
                    <div class="content">
						<c:forEach var="block" items="${infoBlockList}" varStatus="status">
							<tiles:insert name="${block}" />
						</c:forEach>
                    </div>
                </div> <!-- end yui-b -->

            </div> <!-- end bd -->

            <div id="ft">
                <tiles:insert name="pagefooter" />
            </div> <!-- end ft -->
        </div>

    	<tiles:useAttribute name="dialogdivs" ignore="true" />
    	<c:if test="${! empty dialogdivs}">
    	<tiles:insert name="dialogdivs" ignore="true"/>
    	</c:if>
    	
		<tiles:insert name="googleAnalytics" ignore="true"/>
    </body>
</html>