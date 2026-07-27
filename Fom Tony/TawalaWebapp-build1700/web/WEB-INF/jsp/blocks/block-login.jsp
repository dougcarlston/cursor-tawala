<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<c:if test="${empty user}">
    <div class="block">
		<%@ include file="/WEB-INF/jsp/layout/showMessages.jsp" %>		
		<tiles:insert definition="global.errors" ignore="false" >
			<tiles:put name="commandName" value="login" />
		</tiles:insert>
		 
    	<div class="content">
	    	<h3>Log In</h3>
    
    		<c:if test="${empty login}">
    			<c:set var="login" value="<%= new com.tawala.web.user.LoginForm() %>" scope="request" />
    		</c:if>

			<form:form action="${urls.login}" cssClass="login" method="POST" commandName="login">
       	    	<div>
       		        <div class="label">Name:</div>
					<div>
	       	    		<spring:bind path="login.userName">
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						<form:input path="userName" size="20" maxlength="32" /> *
		    	        </spring:bind>
	    	        </div>
       	    		<br />
       		        
	                <div class="label">Password:</div>
                	<div>
	       	    		<spring:bind path="password">
						<%@ include file="/WEB-INF/jsp/layout/showFieldErrors.jsp" %>
						<form:password path="password" size="20" maxlength="32" /> *
		    	        </spring:bind>
	    	        </div>
                	<br />

	                <div class="label">Keep me signed in:</div>
                	<div>
						<form:checkbox path="keepSignedIn" />
	    	        </div>
                	
   	        	    <input name="submitbutton" type="submit" />
       		    </div>
			</form:form>
	    </div>
	</div>
</c:if>	