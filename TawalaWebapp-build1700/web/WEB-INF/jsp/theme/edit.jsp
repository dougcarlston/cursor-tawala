
<jsp:directive.page import="com.tawala.web.project.theme.AddUserUploadedImageController"/>
<jsp:directive.page import="com.tawala.web.project.theme.ViewSampleProjectController"/>
<jsp:directive.page import="com.tawala.web.project.theme.DeleteThemeController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<script type='text/javascript'>
	setPageTitle("Theme Builder");
	
	var viewSampleProjectURL = '${urls.viewSampleProject}';
	Tawala.Theme.styleInfo = ${theme.styleDefailsAsJSONObject};
	Tawala.Theme.allThemesStyleAttributes = ${allThemeAttributes};
	Tawala.Theme.parentThemeStyleInfo = Tawala.Theme.allThemesStyleAttributes['${theme.parentThemeId}'];
<c:if test="${! empty theme.headerImage }">
	Tawala.Theme.themeHeaderImageInfo.url = '${theme.headerImage.fileURL}';
</c:if>
</script>

<div id="themeEditor" class="yui-skin-sam">
	<div id="themeControls">
		<form:form id="editThemeForm" commandName="theme">
			<div id="themeSelect">
				Base theme: <form:select id="themeSelectControl" path="parentThemeId" 
								onchange="Tawala.Theme.changeParentTheme(this.options[this.selectedIndex]);" >
								<form:option label="None" value="plain"></form:option>
								<form:options items="${availableThemes}"  itemValue="path" itemLabel="name" />
							</form:select>
			</div>
			
			<div id="saveTheme" class="buttons left">
				<a class="button" href="/projectmanager/view">CANCEL</a>
	
				<c:if test="${theme.id > 0}">
					<c:url var="deleteThemeURL" value="${urls.deleteUserTheme}">
						<c:param name="<%= DeleteThemeController.THEME_ID %>" value="${theme.id}" />
					</c:url>
					<a id="deleteThemeButton" class="button" href="${deleteThemeURL}">DELETE THIS THEME</a>
				</c:if>

				<button type="submit" onclick="document.getElementById('editThemeForm').styleDefinitions.value=rebuildCSS(); return true;">SAVE THEME AS: </button>
				 <form:input path="name" />
			</div> 
			<input type="hidden" name="styleDefinitions" />
			<form:hidden path="headerImage" id="headerImageInput" />
		</form:form>
		
		<div class="clr buttons" style="margin-top: 14px;">
		</div>
			
	</div>

	<div id="themeBuilderTabs" class="yui-navset">
	    <ul class="yui-nav">
	        <li class="selected"><a href="previewTheme"><em>Preview / Edit Theme</em></a></li>
	        <li><a href="viewCSS"><em>View CSS</em></a></li>
	    </ul>            
	    <div class="yui-content">
	        <div id="previewTheme">
				<div id="themeMenus">
					<div id="mainMenu" class="menu">
<%--						<select id="topLevelList" size="7" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
						<select id="topLevelList" size="7" >
							<option value="Body">Body</option>
							<option value="PageHeader">Page Header</option>
							<option value="MainHeading">Main Heading</option>
							<option value="SubHeading">Sub Heading</option>
							<option value="InstructionalText">Instructional Text</option>
							<option value="ErrorText">Error Text</option>
							<option value="Footer">Footer</option>
						</select>
					</div>
				
					<div class="levelOne">
						<div id="sectionPageHeader" class="levelTwo">
							<div class="menu" style="float: left;">
<%--								<select id="pageHeaderSelect" size="7" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
								<select id="pageHeaderSelect" size="7">
									<option value="PageHeaderBackgroundColor">Background color</option>
									<option value="PageHeaderImage">Image</option>
									<option value="PageHeaderFontColor">Font color</option>
									<option value="PageHeaderFont">Font</option>
									<option value="PageHeaderTextAlign">Text Alignment</option>
									<option value="PageHeaderHeight">Height</option>
									<option value="PageHeaderTextDisplay">Text Display</option>
								</select>
							</div>
							
							<div id="sectionPageHeaderBackgroundColor" class="editor">
								<div id="pageHeaderBackgroundColorPicker" class="colorPickerPane"></div>
							</div>
							
							<div id="sectionPageHeaderImage" class="editor">
								<div id="pageHeaderImagePicker" class="imagePickerPane">
									<div class="imagePickerUpload" style="float: left;">
										<form id="pageHeaderImageUploadForm" action="${urls.addImage}" enctype="multipart/form-data" method="post">
											<div style="margin-bottom: 10px;">
												<p>Please specify an image file:<p>											
												<input type="file" name="<%=AddUserUploadedImageController.FILE_PARAMETER %>"" size="30" id="headerImageFileControl"></input>
											</div>
											<div class="buttons">
												<button id="pageHeaderImageUploadButton" value="Upload" onclick="Tawala.Theme.changeHeaderImage(); return false;">UPLOAD IMAGE</button>
												<button type="button" id="pageHeaderImageClearButton" value="Clear" onclick="Tawala.Theme.clearHeaderImage(); return false;">CLEAR IMAGE</button>
											</div>
										</form>
									</div>
									<div class="imagePickerAlignment" style="float: left; margin-left: 10px;">
										<p>Image position</p>
										<select id="pageHeaderImageAlignment">
											<option value="left">Left</option>
											<option value="center">Center</option>
											<option value="right">Right</option>
										</select>
									</div>
								</div>
							</div>
							<div id="sectionPageHeaderFontColor" class="editor">
								<div id="pageHeaderFontColorPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionPageHeaderFont" class="editor">
								<div id="pageHeaderFontPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionPageHeaderTextAlign" class="editor">
								<div id="pageHeaderTextAlignPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionPageHeaderHeight" class="editor">
								<div id="pageHeaderHeightPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionPageHeaderTextDisplay" class="editor">
								<div id="pageHeaderTextDisplay" class="colorPickerPane"></div>
							</div>
						</div>
				
						<div id="sectionBody" class="levelTwo">
							<div class="menu">
<%--								<select id="bodySelect" size="4" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
								<select id="bodySelect" size="4">
									<option value="BodyBackground">Background color</option>
									<option value="BodyBorder">Border color</option>
									<option value="BodyFontColor">Font color</option>
									<option value="BodyFont">Font</option>
								</select>
							</div>
							
							<div id="sectionBodyBackground" class="editor">
								<div id="bodyBackgroundColorPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionBodyBorder" class="editor">
								<div id="bodyBorderColorPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionBodyFont"  class="editor">
								<div id="bodyFontPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionBodyFontColor" class="editor">
								<div id="bodyFontColorPicker" class="colorPickerPane"></div>
							</div>
						</div>
						
						<div id="sectionMainHeading" class="levelTwo">
							<div class="menu" >
<%--								<select id="mainHeadingSelect" size="3" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
								<select id="mainHeadingSelect" size="3">
									<option value="MainHeadingBackground">Background color</option>
									<option value="MainHeadingFontColor">Font color</option>
									<option value="MainHeadingFont">Font</option>
								</select>
							</div>
							
							<div id="sectionMainHeadingBackground" class="editor">
								<div id="mainHeadingBackgroundColorPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionMainHeadingFont" class="editor">
								<div id="mainHeadingFontPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionMainHeadingFontColor" class="editor">
								<div id="mainHeadingFontColorPicker" class="colorPickerPane"></div>
							</div>
						</div>
						
						<div id="sectionSubHeading" class="levelTwo">
							<div class="menu">
<%--								<select id="subHeadingSelect" size="3" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
								<select id="subHeadingSelect" size="3">
									<option value="SubHeadingBackground">Background color</option>
									<option value="SubHeadingFontColor">Font color</option>
									<option value="SubHeadingFont">Font</option>
								</select>
							</div>
							
							<div id="sectionSubHeadingBackground" class="editor">
								<div id="subHeadingBackgroundColorPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionSubHeadingFont" class="editor">
								<div id="subHeadingFontPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionSubHeadingFontColor" class="editor">
								<div id="subHeadingFontColorPicker" class="colorPickerPane"></div>
							</div>
						</div>
				
						<div id="sectionInstructionalText" class="levelTwo">
							<div class="menu">
<%--								<select id="instructionalTextSelect" size="3" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
								<select id="instructionalTextSelect" size="3">
									<option value="InstructionalTextBackground">Background color</option>
									<option value="InstructionalTextFontColor">Font color</option>
									<option value="InstructionalTextFont">Font</option>
								</select>
							</div>
							
							<div id="sectionInstructionalTextBackground" class="editor">
								<div id="instructionalTextBackgroundColorPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionInstructionalTextFont" class="editor">
								<div id="instructionalTextFontPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionInstructionalTextFontColor" class="editor">
								<div id="instructionalTextFontColorPicker" class="colorPickerPane"></div>
							</div>
						</div>
				
						<div id="sectionErrorText" class="levelTwo">
							<div  class="menu">
<%--								<select id="errorTextSelect" size="3" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
								<select id="errorTextSelect" size="3">
									<option value="ErrorTextBackground">Background color</option>
									<option value="ErrorTextFontColor">Font color</option>
									<option value="ErrorTextFont">Font</option>
								</select>
							</div>
							
							<div id="sectionErrorTextBackground" class="editor">
								<div id="errorTextBackgroundColorPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionErrorTextFont" class="editor">
								<div id="errorTextFontPicker" class="colorPickerPane"></div>
							</div>
							<div id="sectionErrorTextFontColor" class="editor">
								<div id="errorTextFontColorPicker" class="colorPickerPane"></div>
							</div>
						</div>

						<div id="sectionFooter" class="levelTwo">
							<div  class="menu">
<%--								<select id="footerSelect" size="2" onclick="Tawala.Theme.openSection(this.selectedIndex, this.options);">--%>
								<select id="footerSelect" size="2">
									<option value="FooterBackground">Background color</option>
								</select>
							</div>
							
							<div id="sectionFooterBackground" class="editor">
								<div id="footerBackgroundColorPicker" class="colorPickerPane"></div>
							</div>
						</div>
					</div>
				</div>
			
				<div id="previewPane">
					<c:url var="sampleProjectURL" value="${urls.viewSampleProject}">
						<c:if test="${theme.id > 0}">
							<c:param name="<%= ViewSampleProjectController.THEME_ID %>" value="${theme.id}" />
						</c:if>
					</c:url>
					<iframe frameborder="0" height="600px" id="sampleproject" name="sampleproject" src="${sampleProjectURL}" width="100%" onload="Tawala.Theme.applyCustomStyleToPreviewFrame();">></iframe>
					<div id="iframeOverlay" ></div>
				</div>
	        </div>
	        <div id="viewCSS">
	        	<div class="buttons">
<%--	        		<button type="submit">APPLY CHANGES</button>--%>
	        	</div>
	        	<div style="clear: both; padding-top: 10px;">
	        		<textarea name="themeCSS" id="themeCSS"></textarea>
	        	</div>
	        </div>
	    </div>
	</div>

</div>
