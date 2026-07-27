<jsp:directive.page import="com.tawala.project.commands.ExecutionContext"/>
<jsp:directive.page import="com.tawala.web.oldhtml.TextInput"/>
<jsp:directive.page import="com.tawala.project.FieldReference"/>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<div class="yui-gd">
<script type="text/javascript">
<!--
<c:url var="setupFormUrl" value="${userProject.entryPointURLs[userProject.project.customizerForm]}">
	<c:param name="<%= ExecutionContext.OVERRIDE_THEME_PARAMETER %>" value="setup"/>
	<c:param name="<%= ExecutionContext.SURPRESS_ADS_PARAMETER %>" value="y"/>
</c:url>
var setupFormUrl = '${setupFormUrl}';

function enhanceTextFields(doc) {
	var field;
	<c:forEach var="textFieldToEnhance" items="${customizationMetaData.textFieldsToEnhance}">
		field = doc.getElementById('<%= TextInput.ID_PREFIX %>${textFieldToEnhance}');
		if(field != null) {
			field.onkeyup = function() {Tawala.Customize.updateContentValue(this.value, '<%= FieldReference.REFERENCE_PREFIX %>Customize_' + this.name);};
			field.onblur = function() {Tawala.Customize.updateContentValue(this.value, '<%= FieldReference.REFERENCE_PREFIX %>Customize_' + this.name);};
		}
	</c:forEach>

	Tawala.Customize.storeCurrentSetupFormFields();
}
-->
</script>
	<div class="yui-u first">
		<div class="section customizeLeft">
			<h3>Edit</h3>
			<div id="editStatus" class="status"></div>
			<div id="editContent">
				<iframe frameborder="0" height="900px" id="setupFormIFrame" name="setupFormIFrame" src="${setupFormUrl}" width="100%" 
						onload="Tawala.Customize.wireSetupFormFromOnload();"></iframe>
			</div>																				
		</div>
	</div>
	<div class="yui-u">
	</div>
</div>					