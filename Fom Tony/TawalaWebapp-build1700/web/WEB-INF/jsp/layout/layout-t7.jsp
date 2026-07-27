<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<tiles:importAttribute name="pageName" />
<tiles:importAttribute name="pageTitle" />
<tiles:importAttribute name="mainBlockList" />
<tiles:importAttribute name="infoBlockList" />

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" >
	<head>
        <title><tiles:getAsString name="pageTitleBar" /></title>
        <meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
        <meta http-equiv="content-language" content="EN" />
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
    </head>
    <tiles:useAttribute name="onLoadJavascriptFunction" ignore="true" />
    <body <c:if test="${! empty onLoadJavascriptFunction}"> onload="${onLoadJavascriptFunction}"</c:if> class="yui-skin-sam">

        <!-- start yui-t7 -->
        <div id="doc" class="yui-t7">
            <!-- start hd -->
            <div id="hd">
                <tiles:insert name="pageheading" >
                    <tiles:put name="pageName" value="${pageName}" />
                </tiles:insert>
            </div>
            <!-- end hd -->

            <!-- start bd -->
            <div id="bd">
				<div class="pageTitle">
					<h1 id="pageTitle"><tiles:getAsString name="pageTitle" /></h1>
					<!-- operation status messages go here -->
					<div id="opStatus"></div>
				</div>

            	<c:if test="${! empty urgentMessage}">
            		<div class="urgent-message">${urgentMessage.text}</div>
            	</c:if>
				
				<div class="clr">
	           		<tiles:insert name="mainContent" />
           		</div>
            </div>
            <!-- end bd -->

            <!-- start ft -->
            <div id="ft" class="footer">
                <tiles:insert name="pagefooter" />
            </div>
            <!-- end ft -->
        </div>
        <!-- end yui-t7 -->

        <!-- start dialogs -->
    	<tiles:useAttribute name="dialogdivs" ignore="true" />
    	<c:if test="${! empty dialogdivs}">
    	    <tiles:insert name="dialogdivs" ignore="true"/>
    	</c:if>
        <!-- end dialogs -->

		<tiles:insert name="googleAnalytics" ignore="true"/>    
    </body>
</html>