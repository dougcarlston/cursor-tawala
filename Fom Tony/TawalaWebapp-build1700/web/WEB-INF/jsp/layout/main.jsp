<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<tiles:importAttribute name="pageName" />
<tiles:importAttribute name="subMenu" />
<tiles:importAttribute name="subName" />
<tiles:importAttribute name="leftBlockList" />

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" >

<head>
    <title><tiles:getAsString name="pageTitle" /></title>
    <meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
    <meta http-equiv="content-language" content="EN" />
	<link rel="icon" type="image/x-icon" href="/images/favicon.ico" />
	<link rel="shortcut icon" type="image/x-icon" href="/images/favicon.ico" />

	<tiles:useAttribute id="styleList" name="styles"
		classname="java.util.List" ignore="true" />
	
	<c:forEach var="css" items="${styleList}">
		<link rel="stylesheet" type="text/css" media="all"
			href="<%=request.getContextPath()%><c:out value="${css}"/>" />
	</c:forEach>

	<tiles:useAttribute id="scriptList" name="scripts" 
	    classname="java.util.List" ignore="true"/>
	
	<c:forEach var="js" items="${scriptList}">
	    <script type="text/javascript"
	        src="<%=request.getContextPath()%><c:out value="${js}"/>"></script>
	</c:forEach>
	
</head>

<tiles:useAttribute name="onLoadJavascriptFunction" ignore="true" />
<body<c:if test="${! empty onLoadJavascriptFunction}"> onload="${onLoadJavascriptFunction}"</c:if> >
    <!-- start container -->
    <div class="container fixed">
    <a id="top"></a>

    <!-- start header -->
    <div id="header">
		<tiles:insert name="pageheading" >
			<tiles:put name="pageName" value="${pageName}" />
			<tiles:put name="subMenu" value="${subMenu}" />
			<tiles:put name="subName" value="${subName}" />
		</tiles:insert>
    </div>
    <!-- end header -->

    <!-- start left -->
    <div id="leftbox">

		<tiles:useAttribute name="useBlockLoginIfNoUser" ignore="false" />
		<c:choose>
			<c:when test="${! empty user || useBlockLoginIfNoUser eq 'false'}" >
				<c:forEach var="block" items="${leftBlockList}" varStatus="status">
					<tiles:insert name="${block}" >
						<tiles:put name="pageName" value="${pageName}" />
						<tiles:put name="subMenu" value="${subMenu}" />
						<tiles:put name="subName" value="${subName}" />
					</tiles:insert>
				</c:forEach>
			</c:when>
<%--			<c:otherwise>--%>
<%--				<tiles:insert name="block-login" />--%>
<%--			</c:otherwise>--%>
		</c:choose>
    </div>
    <!-- end left box -->
    
    <!-- start mainbox -->
    <div id="mainbox">
		<tiles:insert name="content"/>
	    <div class="toplink">
          <a href="#top"><img src="/images/arrow_top.gif" alt="top" width="9" height="7" />Top</a>
        </div>
    </div>
    <!-- end mainbox -->

    <!-- start footer -->
    <div id="footer">
		<tiles:insert name="pagefooter" />
    </div>
    <!-- end footer -->

    </div>
    <!-- end container -->
	<tiles:useAttribute name="dialogdivs" ignore="true" />
	<c:if test="${! empty dialogdivs}">
	<tiles:insert name="dialogdivs" ignore="true"/>
	</c:if>
    
	<tiles:insert name="googleAnalytics" ignore="true"/>
</body>
</html>

