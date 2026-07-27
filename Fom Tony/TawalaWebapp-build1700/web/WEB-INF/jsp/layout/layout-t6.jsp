<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<tiles:importAttribute name="pageName" />
<tiles:importAttribute name="subMenu" />
<tiles:importAttribute name="subName" />
<tiles:importAttribute name="leftBlockList" />

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
	<head>
        <title><tiles:getAsString name="pageTitle" /></title>
        <meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
        <meta http-equiv="content-language" content="EN" />
    	<link rel="icon" type="image/x-icon" href="/images/favicon.ico" />
    	<link rel="shortcut icon" type="image/x-icon" href="/images/favicon.ico" />
    
    	<tiles:useAttribute id="scriptList" name="scripts" 
    	    classname="java.util.List" ignore="true"/>
    	
    	<c:forEach var="js" items="${scriptList}">
    	    <script type="text/javascript"
    	        src="<%=request.getContextPath()%><c:out value="${js}"/>"></script>
    	</c:forEach>
    	
    
    	<tiles:useAttribute id="styleList" name="styles"
    		classname="java.util.List" ignore="true" />
    	
    	<c:forEach var="css" items="${styleList}">
    		<link rel="stylesheet" type="text/css" media="all"
    			href="<%=request.getContextPath()%><c:out value="${css}"/>" />
    	</c:forEach>

	</head>

    <tiles:useAttribute name="onLoadJavascriptFunction" ignore="true" />
    <body <c:if test="${! empty onLoadJavascriptFunction}"> onload="${onLoadJavascriptFunction}"</c:if> >
	    <a id="top"></a>
        <div id="doc" class="yui-t6">
            <div id="hd">
        		<tiles:insert name="pageheading" >
        			<tiles:put name="pageName" value="${pageName}" />
        			<tiles:put name="subMenu" value="${subMenu}" />
        			<tiles:put name="subName" value="${subName}" />
        		</tiles:insert>
            </div> <!-- end hd -->

            <div id="bd">
            	<c:if test="${! empty urgentMessage}">
            		<div class="urgent-message">${urgentMessage.text}</div>
            	</c:if>

                <div id="yui-main">
                    <div  class="yui-b">
                    		<tiles:insert name="mainContent"/>
                    </div> <!-- end yui-b -->
                </div> <!-- end yui-main -->

                <div class="yui-b">
	           		<tiles:insert name="rightContent"/>
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