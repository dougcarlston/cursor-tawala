<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" >
	<head>
        <meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
        <meta http-equiv="content-language" content="EN" />
    
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
    	<style>
    		html {
    			background-color: transparent;
			}
			    		
    		body {
    			background-color: transparent;
    			padding: 0;
    			margin: 0;
    			text-align: left;
    		}
    		
    	</style>
    </head>
    <tiles:useAttribute name="onLoadJavascriptFunction" ignore="true" />
    <body <c:if test="${! empty onLoadJavascriptFunction}"> onload="${onLoadJavascriptFunction}"</c:if> >
		<tiles:insert name="mainContent" />
    </body>
</html>
