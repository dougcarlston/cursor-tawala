<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
    
    <tiles:insert page="/WEB-INF/jsp/registration/accountEdit.jsp" ignore="false">
    	<tiles:put name="introductionText">
    		Please register for your FREE account. 
    		<br /><br />
    		Registration will create your own private 
    		space, available in "MyTawala", to which you can deploy new projects.	
    	</tiles:put>
    </tiles:insert>
