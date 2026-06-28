YAHOO.widget.Logger.enableBrowserConsole();

var themeTabs = new YAHOO.widget.TabView('themeBuilderTabs');
themeTabs.on('contentReady', function() {

	var viewCSSTab = themeTabs.getTab(1);
	viewCSSTab.addListener('click', viewCSS);
});

Tawala.Theme = {
	getInitialValue: function (styleName, attributeName) {
		var initialValue;
		var previousStyle = Tawala.Theme.styleInfo[styleName];
		if(previousStyle) {
			initialValue = previousStyle[attributeName];
		}
	
		if(! initialValue) {
			var parentStyle = Tawala.Theme.parentThemeStyleInfo[styleName];
			if(parentStyle) {
				initialValue = parentStyle[attributeName];
			}
		}
		
		return initialValue;
	},
	
	allThemesStyleAttributes: {},
	
	themeHeaderImageInfo: {},

	init: function() {
		var menuList, i, menuEl;
		
		Tawala.Theme.deleteTheme();

		menuList = $D.getElementsByClassName("menu", "div", $("themeMenus"));
		
		for(i = 0; i < menuList.length; i++) {
			menuEl = menuList[i].getElementsByTagName("select")[0];			
			YAHOO.util.Event.addListener(menuEl, "click", Tawala.Theme.openSection, this, true);
		}		
	},

	deleteTheme: function() {
		var deleteURL, confirmDeleteDialog, handleYes, handleNo, showDeleteDialog;

		if ($("deleteThemeButton")) {
			deleteURL = $("deleteThemeButton").href;
		}else{
			return;
		}
		handleYes = function() {
			this.hide();
			location.href = deleteURL;
		};

		handleNo = function() {
			this.hide();
		};
		
		showDeleteDialog = function(e) {
			YAHOO.util.Event.stopEvent(e);
			confirmDeleteDialog.show();
		}
		
		confirmDeleteDialog = 
			new YAHOO.widget.SimpleDialog("confirmDeleteTheme", 
					 { width: "300px",
					   fixedcenter: true,
					   visible: false,
					   draggable: false,
					   modal: true,
					   close: true,
					   text: "Are you sure you want to delete this custom theme?",
					   icon: YAHOO.widget.SimpleDialog.ICON_HELP,
					   constraintoviewport: true,
					   buttons: [ { text:"Yes", handler:handleYes, isDefault:true },
								  { text:"No",  handler:handleNo } ]
					 } );

		confirmDeleteDialog.setHeader("<div class='tl'></div><span id='dialogTitle'>Delete Custom Theme</span><div class='tr'></div>");
		confirmDeleteDialog.render(document.body);
		
		YAHOO.util.Event.addListener("deleteThemeButton", "click", showDeleteDialog, confirmDeleteDialog, true);		
	},

	imagePicker: function(containerName, styleName, attributes) {
		YAHOO.util.Event.addListener(containerName, "change", this.changeHeaderImageAlignment);
	},
	
	clearHeaderImage: function() {
		var headerImageInput = document.getElementById('headerImageInput');
		headerImageInput.value = '';
		
		Tawala.Theme.themeHeaderImageInfo = {};
		
		var parentThemeSelect = document.getElementById('themeSelectControl');
		Tawala.Theme.changeParentTheme(parentThemeSelect.options[parentThemeSelect.selectedIndex]);
	},

	applyCustomStyleToPreviewFrame: function() {
		updateCustomCSSInPreview();
		if(Tawala.Theme.themeHeaderImageInfo.url) {
			// See edit.jsp for details on how it's set.
			Tawala.Theme.replaceImageInPreviewProject(Tawala.Theme.themeHeaderImageInfo.url);
		}
	},
	
	changeHeaderImage : function() {
		var imageNameControl = document.getElementById('headerImageFileControl');
		if(imageNameControl.value.length === 0) {
			return;
		}
		var form = document.getElementById('pageHeaderImageUploadForm');

		var responseHandler = function(o) {
			var serverResponse = YAHOO.lang.JSON.parse(Tawala.Theme.cleanUploadResponseIfNeeded(o.responseText));
			var imageURL = serverResponse.imageURL;
	
			Tawala.Theme.replaceImageInPreviewProject(imageURL);		
			Tawala.Theme.setHeadingImageHeight();
			
			Tawala.Theme.themeHeaderImageInfo = {};
			Tawala.Theme.themeHeaderImageInfo.url = serverResponse.imageURL;
			
			var headerImageInput = document.getElementById('headerImageInput');
			headerImageInput.value = serverResponse.imageId + '';
		};

		var failureHandler = function(o) {
			alert("An error occured while uploading the image.");
		};
		
		var url = form.action;
		var transInfo = {
				upload:  responseHandler,
				failure:  failureHandler
		};
		
		YAHOO.util.Connect.setForm(form, true );	// it's an upload
		YAHOO.util.Connect.asyncRequest( 'POST', url, transInfo);
	},

	replaceImageInPreviewProject: function(imageURL) {
		this.sampleProjectDocument = getSampleProjectFrameDocument();
		this.headerEl = YAHOO.util.Dom.getElementsByClassName('pageHeading', 'h1', this.sampleProjectDocument)[0];
		if(! this.headerEl) {
			return;
		}
		var imageEl = this.headerEl.getElementsByTagName("img")[0];
		if(! imageEl || imageEl.tagName.toUpperCase() != 'IMG') {
			imageEl = this.sampleProjectDocument.createElement('img');
			imageEl.setAttribute('src', imageURL);
			if(YAHOO.util.Dom.insertBefore(imageEl, this.headerEl.firstChild) === null) {
				alert('Failed to update header image');
			}

		} else {
			imageEl.setAttribute('src', imageURL);
			imageEl.removeAttribute('width');
			imageEl.removeAttribute('height');
		}
	},
	
	setHeadingImageHeight: function() {
		var imageEl = this.headerEl.getElementsByTagName("img")[0];

		// Seems we have to wait for the DOM to update before we can get accurate dimensions
		// of the image, hence the timeout function
		setTimeout(
			function() {
				var imgRegion = $D.getRegion(imageEl);
				var imgHeight = imgRegion.bottom - imgRegion.top;
				changeStyleAttributeValue("h1.pageHeading", "height", imgHeight + "px");				
			}
			, 500);
	},
		
	changeHeaderImageAlignment: function() {
		var alignSelectContainer, alignSelect;
		
		alignSelectContainer = $D.getElementsByClassName("imagePickerAlignment", "div", this)[0];
		alignSelect = alignSelectContainer.getElementsByTagName("select")[0];
		changeStyleAttributeValue("h1.pageHeading", "textAlign", alignSelect.options[alignSelect.selectedIndex].value );
	},
	
	cleanUploadResponseIfNeeded: function(responseText) {
		var i = responseText.indexOf('<pre');
		if( i >= 0 ) {
			responseText = responseText.substr(i + responseText.indexOf('{'));
		}
		i = responseText.indexOf('</pre>');
		if(i >=0) {
			responseText = responseText.substr(0, i);
		}
		return responseText;
	},
	
	openSection: function(ev) {
		var selectEl, options, optionIndex;
				
		selectEl =$E.getTarget(ev);
		if(selectEl.tagName.toLowerCase() != "select") {
			selectEl =$E.getTarget(ev).parentNode;			
		}
		
		options = selectEl.options;
		optionIndex = selectEl.selectedIndex;
		
		for(var i=0; i< options.length; i++) {
			var sectionId = 'section' + options[i].value;
			var section = document.getElementById(sectionId);
			if(section) {
				if(optionIndex == i) {
					section.style.display = 'inline';
					var onOpeningHandler = handlerMap['onOpening' + options[i].value];
					if(onOpeningHandler) {
						onOpeningHandler.call();
					}
				} else {
					section.style.display = 'none';
				}
			}
		}
	},

	changeParentTheme: function(option) {
		rebuildCSS();
	
		var parentThemeId = option.value;
		var frame = getSampleProjectFrame();
		var url = viewSampleProjectURL + '?' + 'theme_id=' + parentThemeId;
		
		frame.src = url;
		
		Tawala.Theme.parentThemeStyleInfo = Tawala.Theme.allThemesStyleAttributes[parentThemeId];
	},
	
	textDisplay: function(containerName, styleName, attributes) {
		var displayDiv, fah, textDisplayEl, sIndex, i, optionEl, currentTextVisibility;
		
		var textDisplaySelectHandler = function (){
			changeStyleAttributeValue(styleName, "visibility", this.options[this.selectedIndex].value );		
		};

		if(Tawala.Theme.styleInfo[styleName] && Tawala.Theme.styleInfo[styleName]["visibilty"]){
			currentTextVisibility = Tawala.Theme.styleInfo[styleName]["visibility"];
		}
			
		if ($D.getElementsByClassName("textAlignPicker", "div", $(containerName)).length == 0) {
			// Clear the container
			containerEl = $(containerName);
			clearContainer(containerEl);
			
			// Setup the text alignment selection list	
			displayDiv = document.createElement("div");
			$D.setStyle(displayDiv, "clear", "both");
			$D.addClass(displayDiv, "textAlignPicker");
			
			fah = document.createElement("p");
			fah.innerHTML = "Select visibility of header text";
			displayDiv.appendChild(fah);
			
			// Setup the text display selection list
			textDisplayEl = document.createElement("select");
			textDisplayEl.id = "textDisplayList";
			
			sIndex = 0;
			for (i in Tawala.Theme.textDisplayList) {
				sIndex++;
				if (typeof Tawala.Theme.textDisplayList[i] === "string") {
					optionEl = document.createElement("option");
					optionEl.value = Tawala.Theme.textDisplayList[i];
					optionEl.text = Tawala.Theme.textDisplayList[i];
					try {
						textDisplayEl.add(optionEl, null); // standards compliant; doesn't work in IE
					} 
					catch (ex) {
						textDisplayEl.add(optionEl); // IE only
					}

					if(currentTextVisibility && Tawala.Theme.textAlignmentList[i] == currentTextVisibility){
						optionEl.selected = true;
					}	
				}
			}
			
			displayDiv.appendChild(textDisplayEl);
			containerEl.appendChild(displayDiv);
			
			// Text alignment event handler
			YAHOO.util.Event.addListener(textDisplayEl, "change", textDisplaySelectHandler);
		}
	},
	
	styleInfo : {},
		
	fontFamilyList: {
		"Arial, Helvetica, sans-serif": "Arial", 
		"'Arial Black', Gadget, sans-serif": "Arial Black", 
		"'Comic Sans MS', cursive": "Comic Sans MS", 
		"'Courier New', Courier, monospace": "Courier", 
		"Georgia, serif": "Georgia",
		"Impact, Charcoal, sans-serif": "Impact", 
		"Tahoma, Geneva, sans-serif": "Tahoma", 
		"'Times New Roman', Times, serif": "Times", 
		"'Trebuchet MS', Helvetica, sans-serif": "Trebuchet MS", 
		"Verdana, Geneva, sans-serif": "Verdana" 
	},
	
	fontSizeList: [ 9, 10, 11, 12, 13, 14, 18, 20, 24, 28, 36 ],
	
	textAlignmentList: {"left":"Left", "center":"Center", "right":"Right"},
	
	textDisplayList: {"visible": "Visible", "hidden": "Hidden"},
	
	cssAttributes: {
		"background":"background",
		"background-attachment":"backgroundAttachment",
		"background-color":"backgroundColor",
		"background-image":"backgroundImage",
		"background-position":"backgroundPosition",
		"background-repeat":"backgroundRepeat",
		"border":"border",
		"border-collapse":"borderCollapse",
		"border-bottom":"borderBottom",
		"border-bottom-color":"borderBottomColor",
		"border-bottom-style":"borderBottomStyle",
		"border-bottom-width":"borderBottomWidth",
		"border-color":"borderColor",
		"border-left":"borderLeft",
		"border-left-color":"borderLeftColor",
		"border-left-style":"borderLeftStyle",
		"border-left-width":"borderLeftWidth",
		"border-right":"borderRight",
		"border-right-color":"borderRightColor",
		"border-right-style":"borderRightStyle",
		"border-right-width":"borderRightWidth",
		"border-style":"borderStyle",
		"border-top":"borderTop",
		"border-top-color":"borderTopColor",
		"border-top-style":"borderTopStyle",
		"border-top-width":"borderTopWidth",
		"border-width":"borderWidth",
		"clear":"clear",
		"clip":"clip",
		"color":"color",
		"content":"content",
		"cursor":"cursor",
		"direction":"direction",
		"display":"display",
		"empty-cells":"emptyCells",
		"float":"cssFloat",
		"font":"font",
		"font-family":"fontFamily",
		"font-size":"fontSize",
		"font-variant":"fontVariant",
		"font-weight":"fontWeight",
		"height":"height",
		"left":"left",
		"letter-spacing":"letterSpacing",
		"line-height":"lineHeight",
		"list-style":"listStyle",
		"list-style-image":"listStyleImage",
		"list-style-position":"listStylePosition",
		"list-style-type":"listStyleType",
		"margin":"margin",
		"margin-bottom":"marginBottom",
		"margin-left":"marginLeft",
		"margin-right":"marginRight",
		"margin-top":"marginTop",
		"max-height":"maxHeight",
		"max-width":"maxWidth",
		"min-height":"minHeight",
		"min-width":"minWidth",
		"outline":"outline",
		"outline-color":"outlineColor",
		"outline-style":"outlineStyle",
		"outline-width":"outlineWidth",
		"overflow":"overflow",
		"padding":"padding",
		"padding-bottom":"paddingBottom",
		"padding-left":"paddingLeft",
		"padding-right":"paddingRight",
		"padding-top":"paddingTop",
		"position":"position",
		"quotes":"quotes",
		"right":"right",
		"text-align":"textAlign",
		"text-decoration":"textDecoration",
		"text-indent":"textIndent",
		"text-transform":"textTransform",
		"top":"top",
		"unicode-bidi":"unicodeBidi",
		"vertical-align":"verticalAlign",
		"visibility":"visibility",
		"white-space":"whiteSpace",
		"width":"width",
		"word-spacing":"wordSpacing",
		"z-index":"zIndex"
	},

	js2CSSAttributes: {
		"background":"background",
		"backgroundAttachment":"background-attachment",
		"backgroundColor":"background-color",
		"backgroundImage":"background-image",
		"backgroundPosition":"background-position",
		"backgroundRepeat":"background-repeat",
		"border":"border",
		"borderCollapse":"border-collapse",
		"borderBottom":"border-bottom",
		"borderBottomColor":"border-bottom-color",
		"borderBottomStyle":"border-bottom-style",
		"borderBottomWidth":"border-bottom-width",
		"borderColor":"border-color",
		"borderLeft":"border-left",
		"borderLeftColor":"border-left-color",
		"borderLeftStyle":"border-left-style",
		"borderLeftWidth":"border-left-width",
		"borderRight":"border-right",
		"borderRightColor":"border-right-color",
		"borderRightStyle":"border-right-style",
		"borderRightWidth":"border-right-width",
		"borderStyle":"border-style",
		"borderTop":"border-top",
		"borderTopColor":"border-top-color",
		"borderTopStyle":"border-top-style",
		"borderTopWidth":"border-top-width",
		"borderWidth":"border-width",
		"captionSide":"caption-side",
		"clear":"clear",
		"clip":"clip",
		"color":"color",
		"content":"content",
		"cursor":"cursor",
		"direction":"direction",
		"display":"display",
		"emptyCells":"empty-cells",
		"cssFloat":"float",
		"font":"font",
		"fontFamily":"font-family",
		"fontSize":"font-size",
		"fontVariant":"font-variant",
		"fontWeight":"font-weight",
		"height":"height",
		"left":"left",
		"letterSpacing":"letter-spacing",
		"lineHeight":"line-height",
		"listStyle":"list-style",
		"listStyleImage":"list-style-image",
		"listStylePosition":"list-style-position",
		"listStyleType":"list-style-type",
		"margin":"margin",
		"marginBottom":"margin-bottom",
		"marginLeft":"margin-left",
		"marginRight":"margin-right",
		"marginTop":"margin-top",
		"maxHeight":"max-height",
		"maxWidth":"max-width",
		"minHeight":"min-height",
		"minWidth":"min-width",
		"outline":"outline",
		"outlineColor":"outline-color",
		"outlineStyle":"outline-style",
		"outlineWidth":"outline-width",
		"overflow":"overflow",
		"padding":"padding",
		"paddingBottom":"padding-bottom",
		"paddingLeft":"padding-left",
		"paddingRight":"padding-right",
		"paddingTop":"padding-top",
		"position":"position",
		"quotes":"quotes",
		"right":"right",
		"textAlign":"text-align",
		"textDecoration":"text-decoration",
		"textIndent":"text-indent",
		"textTransform":"text-transform",
		"top":"top",
		"unicodeBidi":"unicode-bidi",
		"verticalAlign":"vertical-align",
		"visibility":"visibility",
		"whiteSpace":"white-space",
		"width":"width",
		"wordSpacing":"word-spacing",
		"zIndex":"z-index"
	}

}

function getSampleProjectFrame() {
	var frame = document.getElementById('sampleproject');
	return frame;
}

function getSampleProjectFrameDocument() {
	var frame = getSampleProjectFrame();
	if(! frame) {
		return null;
	}
	var doc;
    if( frame.contentDocument )
          // For NS6
          doc = frame.contentDocument; 
    else if( frame.contentWindow ) 
          // For IE5.5 and IE6
          doc = frame.contentWindow.document;
    else if( frame.document )
          // For IE5
          doc = frame.document;
    else //other browser
          doc = frame.document;     

    return doc;}

function changeStyleAttributeValue(styleName, attributeName, value) {
	updateStyleAttributeValue(styleName, attributeName, value);
	updateCustomCSSInPreview();	
};

function updateCustomCSSInPreview() {
	var frameName = "sampleproject";
	var headerCSS = getSampleProjectFrameDocument().getElementById("headerCSS");
	
	headerCSS.disabled = true;
	clearContainer(headerCSS);

	var cssTextEl = document.createTextNode(rebuildCSS());	
	headerCSS.appendChild(cssTextEl);	
//	headerCSS.innerText = rebuildCSS();

	headerCSS.disabled = false;					
}

function updateStyleAttributeValue(styleName, attributeName, value) {
	var style = Tawala.Theme.styleInfo[styleName];
	if(! style) {
		style = {};
	}
	style[attributeName] = value;
	Tawala.Theme.styleInfo[styleName] = style;
}

function restoreStyle(styleName, attributeName) {
	removeStyleAttribute(styleName, attributeName);
	updateCustomCSSInPreview();	
};

function removeStyleAttribute(styleName, attributeName) {
	var style = Tawala.Theme.styleInfo[styleName];
	if(! style) {
		return;
	}
	delete style[attributeName];
}

function rebuildCSS() {
	var updatedCSS = '';
	for(var styleName in Tawala.Theme.styleInfo) {
		var styleAttributes = Tawala.Theme.styleInfo[styleName];
		if(typeof styleAttributes === 'object') {
			var styleDefinition = styleName + ' {';
			for(var attributeName in styleAttributes) {				
				var attributeValue = styleAttributes[attributeName];

				if(typeof attributeValue === 'string') {
					if (Tawala.Theme.js2CSSAttributes[attributeName]) {
						attributeName = Tawala.Theme.js2CSSAttributes[attributeName]
					}
					styleDefinition = styleDefinition + attributeName + ':' + attributeValue + '; ';
				}
			}
			styleDefinition = styleDefinition + '}\n';
			updatedCSS = updatedCSS + styleDefinition;
		}
	}
	
	return updatedCSS;
}

function clearContainer(containerEl){
	if (containerEl.hasChildNodes()) {
		while (containerEl.childNodes.length >= 1) {
			containerEl.removeChild(containerEl.firstChild);
		}
	}
}

function setupColorPicker(containerId, styleName, attributeName) {
	var container = $(containerId);
	// container.style.width = "100%";
	
	var colorPickerContainer = document.createElement('div');
	colorPickerContainer.setAttribute("style", "float: left");
	colorPickerContainer.setAttribute("id", containerId + "ColorPicker");
	
	container.appendChild(colorPickerContainer);
	
    var Event = YAHOO.util.Event;
    var picker = new YAHOO.widget.ColorPicker(containerId + "ColorPicker", {
               showhsvcontrols: false,
               showrgbcontrols: false,
               showhexsummary: false,
               showwebsafe: false,
               showhexcontrols: true,
			images: {
				PICKER_THUMB: "/scripts/yui/build/colorpicker/assets/picker_thumb.png",
				HUE_THUMB: "/scripts/yui/build/colorpicker/assets/hue_thumb.png"
				}
           });
	//a listener for logging RGB color changes;
	//this will only be visible if logger is enabled:
	var onRgbChange = function(o) {
		// o is an object
		//	{ newValue: (array of R, G, B values),
		//	  prevValue: (array of R, G, B values),
		//	  type: "rgbChange"
		//	 }
		
		changeStyleAttributeValue(styleName, attributeName, '#' + picker.get('hex'));
	}
	
	//subscribe to the rgbChange event;
	picker.on("rgbChange", onRgbChange);

	var initialValue = Tawala.Theme.getInitialValue(styleName, attributeName);
	if(initialValue) {
		initialValue = initialValue.substring(1);
		picker.setValue(YAHOO.util.Color.hex2rgb(initialValue), true); //false here means that rgbChange
											     //will fire; true would silence it		
	}

	var resetContainer = document.createElement('div');
	$D.addClass(resetContainer, "buttons");
	$D.addClass(resetContainer, "right");
//	resetContainer.setAttribute("style", "float: right");
	
	var resetButton = document.createElement('button');
	resetButton.innerHTML = 'RESET TO DEFAULT VALUE';
	var onClickHandler = function () { restoreStyle(styleName, attributeName); picker.setValue([255, 255, 255], true);  };
	resetButton.onclick = onClickHandler;
	resetContainer.appendChild(resetButton);

	container.appendChild(resetContainer);
	
	return picker;
}

function fontPicker(containerName, styleName, attributes) {
	var containerEl, familyDiv, sizeDiv, fontListEl, optionEl, ffh, i, j, currentFontFamily, currentFontSize;
	
	var familySelectHandler = function (){
		changeStyleAttributeValue(styleName, "font-family", this.options[this.selectedIndex].value );		
	};
	
	var sizeSelectHandler = function (){
		changeStyleAttributeValue(styleName, "font-size", this.options[this.selectedIndex].value + "pt" );		
	};

	if(Tawala.Theme.styleInfo[styleName] && Tawala.Theme.styleInfo[styleName]["font-size"]){
		currentFontSize = Tawala.Theme.styleInfo[styleName]["font-size"].slice(0,-2);
	}
	
	if(Tawala.Theme.styleInfo[styleName] && Tawala.Theme.styleInfo[styleName]["font-family"]){
		currentFontFamily = Tawala.Theme.styleInfo[styleName]["font-family"];
	}

	
	if ($D.getElementsByClassName("fontPicker", "div", $(containerName)).length == 0) {
		// Clear the container
		var containerEl = $(containerName);
		clearContainer(containerEl)
		
		// Setup the font family selection list
		familyDiv = document.createElement("div");
		$D.setStyle(familyDiv, "float", "left");
		familyDiv.style.marginRight = "10px";
		$D.addClass(familyDiv, "fontPicker");
		
		ffh = document.createElement("p");
		ffh.innerHTML = "Select Font Family";
		familyDiv.appendChild(ffh);
		
		sizeDiv = document.createElement("div");
		$D.setStyle(sizeDiv, "float", "left");
		
		fsh = document.createElement("p");
		fsh.innerHTML = "Select Font Size";
		sizeDiv.appendChild(fsh);
		
		// Setup the font family selection list
		fontListEl = document.createElement("select");
		fontListEl.id = "fontFamilyList";

		var previousFontFamily = Tawala.Theme.getInitialValue(styleName, 'font-family');
		
		for (i in Tawala.Theme.fontFamilyList) {
			if (typeof Tawala.Theme.fontFamilyList[i] === "string") {
				optionEl = document.createElement("option");
				optionEl.value = i;
				optionEl.text = Tawala.Theme.fontFamilyList[i];
				if(optionEl.value == previousFontFamily) {
					optionEl.selected = true;
				}
				try {
					fontListEl.add(optionEl, null); // standards compliant; doesn't work in IE
				} 
				catch (ex) {
					fontListEl.add(optionEl); // IE only
				}

				if(currentFontFamily && Tawala.Theme.fontFamilyList[i] == currentFontFamily){
					optionEl.selected = true;
				}
			}
		}
		
		familyDiv.appendChild(fontListEl);
		
		// Setup the font size selection list
		fontSizeEl = document.createElement("select");
		fontSizeEl.id = "fontSizeList";
		
		var previousSize = Tawala.Theme.getInitialValue(styleName, 'font-size');
		if(previousSize) {
			previousSize = previousSize.substr(0, previousSize.length - 2);
		}
		for (j = 0; j < Tawala.Theme.fontSizeList.length; j++) {
			optionEl = document.createElement("option");
			optionEl.value = Tawala.Theme.fontSizeList[j];
			optionEl.text = Tawala.Theme.fontSizeList[j] + "pt";
			if(optionEl.value == previousSize) {
				optionEl.selected = true;
			}
			try {
				fontSizeEl.add(optionEl, null); // standards compliant; doesn't work in IE
			} 
			catch (ex) {
				fontSizeEl.add(optionEl); // IE only
			}

			if(currentFontSize && Tawala.Theme.fontSizeList[j] == currentFontSize){
				optionEl.selected = true;
			}
		}
		
		containerEl.appendChild(familyDiv);
		containerEl.appendChild(sizeDiv);
		sizeDiv.appendChild(fontSizeEl);		

		// Setup the font family event handler
		YAHOO.util.Event.addListener(fontListEl, "change", familySelectHandler);
		
		// Setup the font size event handler
		YAHOO.util.Event.addListener(fontSizeEl, "change", sizeSelectHandler);
	}
}

function textAlignPicker(containerName, styleName, attributes) {
	var alignDiv,fah, textAlignEl, optionEl, containerEl, i, currentTextAlign;
	
	var textAlignmentSelectHandler = function (){
		changeStyleAttributeValue(styleName, "textAlign", this.options[this.selectedIndex].value );		
	};
	
	if(Tawala.Theme.styleInfo[styleName] && Tawala.Theme.styleInfo[styleName]["text-align"]){
		currentTextAlign = Tawala.Theme.styleInfo[styleName]["text-align"];
	}
	
	if($D.getElementsByClassName( "textAlignPicker","div", $(containerName)).length == 0){
		// Clear the container
		containerEl=$(containerName);
		clearContainer(containerEl);

		// Setup the text alignment selection list	
		alignDiv = document.createElement("div");
		$D.setStyle(alignDiv, "clear", "both");
		$D.addClass(alignDiv, "textAlignPicker");
	
		fah = document.createElement("p");
		fah.innerHTML = "Select Text Alignment";
		alignDiv.appendChild(fah);
		
		// Setup the text alignment selection list
		textAlignEl = document.createElement("select");
		textAlignEl.id = "textAlignList";
		
		var sIndex = 0;
		for ( i in Tawala.Theme.textAlignmentList ){
			sIndex++;
			if(typeof Tawala.Theme.textAlignmentList[i] === "string"){
				optionEl = document.createElement("option");
				optionEl.value = Tawala.Theme.textAlignmentList[i];				
				optionEl.text = Tawala.Theme.textAlignmentList[i];
				try {
				    textAlignEl.add(optionEl, null); // standards compliant; doesn't work in IE
	  			} catch(ex) {
	    			textAlignEl.add(optionEl); // IE only
	  			}
				
				if(currentTextAlign && Tawala.Theme.textAlignmentList[i] == currentTextAlign){
					optionEl.selected = true;
				}

			}
		}
		
		containerEl.appendChild(alignDiv);	

		// Text alignment event handler
		YAHOO.util.Event.addListener(textAlignEl, "change", textAlignmentSelectHandler);
		alignDiv.appendChild(textAlignEl);		
	}
}

function heightPicker(containerName, styleName, attributes) {
	var containerEl, currentHeight, heightText, heightDiv, heightInput, 
		heightSliderBg, heightSliderThumb, slider, newHeight, headingImage, hiRegion;
	
	var heightSelectHandler = function (){
		newHeight = parseInt(heightInput.value);
		if(heightInput.value == "" || isNaN(newHeight) ) {
			newHeight = currentHeight;	
		}
		changeStyleAttributeValue(styleName, "height", newHeight + "px", attributes );	
		slider.setValue(Number(newHeight) / 1.5);	
	};
	
	if(Tawala.Theme.styleInfo[styleName] && Tawala.Theme.styleInfo[styleName].height){
		currentHeight = Tawala.Theme.styleInfo[styleName].height.slice(0,-2);
	}else{
		currentHeight = 100;
	}
	
	if ($D.getElementsByClassName("heightPicker", "div", $(containerName)).length == 0) {	
		// Clear the container
		containerEl = $(containerName);
		clearContainer(containerEl)
		
		heightDiv = document.createElement("div");
		$D.setStyle(heightDiv, "clear", "both");
		$D.addClass(heightDiv, "heightPicker");
		
		heightText = document.createElement("span");
		heightText.innerHTML = "Page Header Height: ";
		heightDiv.appendChild(heightText);
		heightInput = document.createElement("input");
		heightInput.type = "text";
		heightInput.size = "3";
		heightInput.setAttribute('maxlength','3');
		
		if (currentHeight) {
			heightInput.value = currentHeight + "px";
		}
		
		heightInputDesc = document.createElement("span");
		heightInputDesc.style.paddingLeft = "6px";
		heightInputDesc.style.paddingRight = "6px";
		heightInputDesc.innerHTML = "px";
		heightInputUpdate = document.createElement("input");
		heightInputUpdate.type = "button";
		heightInputUpdate.value = "Update";
		
		heightInput.id = "heightInput";
		heightDiv.appendChild(heightInput);
		heightDiv.appendChild(heightInputDesc);
		heightDiv.appendChild(heightInputUpdate);
		
		heightSliderBg = document.createElement("div");
		heightSliderBg.id = "heightSliderBg";
		heightSliderThumb = document.createElement("div");
		heightSliderThumb.id = "heightSliderThumb";
		heightSliderThumbImg = document.createElement("img");
		heightSliderThumbImg.src = "/images/slider/assets/thumb-n.gif";
		heightSliderThumb.appendChild(heightSliderThumbImg);
		heightSliderBg.appendChild(heightSliderThumb);
		heightDiv.appendChild(heightSliderBg);
		
		containerEl.appendChild(heightDiv);
		
		slider = YAHOO.widget.Slider.getHorizSlider(heightSliderBg, heightSliderThumb, 0, 200);
		slider.animate = true;
		
		slider.setValue(Number(currentHeight) / 1.5);
		
		heightInput.slider = slider;
		// Height picker event handlers
		YAHOO.util.Event.addListener(heightInput, "blur", heightSelectHandler);
		
		slider.subscribe("change", function(offsetFromStart) {
			var actualValue = Math.round(offsetFromStart * 1.5);
			if(actualValue < 0 ) { actualValue = 0;}
			$("heightInput").value = actualValue;
			changeStyleAttributeValue(styleName, "height", actualValue + "px");
		});
	}
}

function viewCSS (e) {
	var cssView = $("themeCSS");
	cssView.value = rebuildCSS();	
}

var objectMap = {
};

var handlerMap = {
	onOpeningBodyBackground : function() {
		if(! objectMap.bodyBackgroundColorPicker) {
			objectMap.bodyBackgroundColorPicker = setupColorPicker("bodyBackgroundColorPicker", "div#tawalaProjectContainer", "background-color");
		}
    },
    
	onOpeningBodyBorder : function() {
		if(! objectMap.bodyBorderColorPicker) {
			objectMap.bodyBorderColorPicker = setupColorPicker("bodyBorderColorPicker", "body", "background-color");
		}
    },
    
    onOpeningBodyFont : function() {
		if(! objectMap.bodyFontPicker) {
			objectMap.bodyFontPicker = fontPicker("bodyFontPicker", "body", ["font-family", "font-size"]);
		}
    },
    
    onOpeningBodyFontColor : function() {
		if(! objectMap.bodyFontColorPicker) {
			objectMap.bodyFontColorPicker = setupColorPicker("bodyFontColorPicker", "body", "color");
		}
    },
    
    onOpeningPageHeaderBackgroundColor : function() {
		if(! objectMap.pageHeaderBackgroundColorPicker) {
			objectMap.pageHeaderBackgroundColorPicker = setupColorPicker("pageHeaderBackgroundColorPicker", "h1.pageHeading", "background-color");
		}
    },
    
    onOpeningPageHeaderFontColor : function() {
		if(! objectMap.pageHeaderFontColorPicker) {
			objectMap.pageHeaderFontColorPicker = setupColorPicker("pageHeaderFontColorPicker", "h1.pageHeading", "color");
		}
    },

    onOpeningPageHeaderFont : function() {
		if(! objectMap.pageHeaderFontPicker) {
			objectMap.pageHeaderFontPicker = fontPicker("pageHeaderFontPicker", "h1.pageHeading", 
															{ "fontfamily": "font-family", 
																"fontSize": "font-size" }
														);
		}
    },
    
    onOpeningPageHeaderTextAlign : function() {
		if(! objectMap.pageHeaderTextAlignPicker) {
			objectMap.pageHeaderTextAlignPicker = textAlignPicker("pageHeaderTextAlignPicker", "h1.pageHeading div", 
																			{ "textAlign": "text-align" } 
																		);
		}
    },
    
    onOpeningPageHeaderHeight : function() {
		if(! objectMap.pageHeaderHeightPicker) {
			objectMap.pageHeaderHeightPicker = heightPicker("pageHeaderHeightPicker", "h1.pageHeading", 
																{ "height": "height" } 
															);
		}
    },
    
    onOpeningPageHeaderImage : function() {
		if(! objectMap.pageHeaderImagePicker) {
			objectMap.pageHeaderImagePicker = Tawala.Theme.imagePicker("pageHeaderImagePicker", "h1", 
																{ "textAlign": "text-align" } 
															);
		}
    },
    
    onOpeningPageHeaderTextDisplay : function() {
		if(! objectMap.pageHeaderTextDisplay) {
			objectMap.pageHeaderTextDisplay = Tawala.Theme.textDisplay("pageHeaderTextDisplay", "h1.pageHeading div", 
																{ "visibility": "visibility" } 
															);
		}
    },
    
	onOpeningMainHeadingBackground : function() {
		if(! objectMap.mainHeadingBackgroundColorPicker) {
			objectMap.mainHeadingBackgroundColorPicker = setupColorPicker("mainHeadingBackgroundColorPicker", "h1", "background-color");
		}
    },
    
    onOpeningMainHeadingFontColor : function() {
		if(! objectMap.mainHeadingFontColorPicker) {
			objectMap.mainHeadingFontColorPicker = setupColorPicker("mainHeadingFontColorPicker", "h1", "color");
		}
    },        

    onOpeningMainHeadingFont : function() {
		if(! objectMap.mainHeadingFontPicker) {
			objectMap.mainHeadingFontPicker = fontPicker("mainHeadingFontPicker", "h1", { 
				"fontfamily": "font-family", 
				"fontSize": "font-size" }
		);
		}
    },
    
	onOpeningSubHeadingBackground : function() {
		if(! objectMap.subHeadingBackgroundColorPicker) {
			objectMap.subHeadingBackgroundColorPicker = setupColorPicker("subHeadingBackgroundColorPicker", "h2", "background-color");
		}
    },
    
    onOpeningSubHeadingFontColor : function() {
		if(! objectMap.subHeadingFontColorPicker) {
			objectMap.subHeadingFontColorPicker = setupColorPicker("subHeadingFontColorPicker", "h2", "color");
		}
    },
            
    onOpeningSubHeadingFont : function() {
		if(! objectMap.subHeadingFontPicker) {
			objectMap.subHeadingFontPicker = fontPicker("subHeadingFontPicker", "h2", { 
				"fontfamily": "font-family", 
				"fontSize": "font-size" }
			);
		}
    },
    
	onOpeningInstructionalTextBackground : function() {
		if(! objectMap.instructionalTextBackgroundColorPicker) {
			objectMap.instructionalTextBackgroundColorPicker = setupColorPicker("instructionalTextBackgroundColorPicker", "div.text.instructional", "background-color");
		}
    },
    
    onOpeningInstructionalTextFontColor : function() {
		if(! objectMap.instructionalTextFontColorPicker) {
			objectMap.instructionalTextFontColorPicker = setupColorPicker("instructionalTextFontColorPicker", "div.text.instructional", "color");
		}
    },        

    onOpeningInstructionalTextFont : function() {
		if(! objectMap.instructionalTextFontPicker) {
			objectMap.instructionalTextFontPicker = fontPicker("instructionalTextFontPicker", "div.text.instructional", { 
				"fontfamily": "font-family", 
				"fontSize": "font-size" }
		);
		}
    },
    
	onOpeningErrorTextBackground : function() {
		if(! objectMap.errorTextBackgroundColorPicker) {
			objectMap.errorTextBackgroundColorPicker = setupColorPicker("errorTextBackgroundColorPicker", "div.text.error", "background-color");
		}
    },
    
    onOpeningErrorTextFontColor : function() {
		if(! objectMap.errorTextFontColorPicker) {
			objectMap.errorTextFontColorPicker = setupColorPicker("errorTextFontColorPicker", "div.text.error", "color");
		}
    },
	        
    onOpeningErrorTextFont : function() {
		if(! objectMap.errorTextFontPicker) {
			objectMap.errorTextFontPicker = fontPicker("errorTextFontPicker", "div.text.error", { 
				"fontfamily": "font-family", 
				"fontSize": "font-size" }
		);
		}
    },

	onOpeningFooterBackground : function() {
		if(! objectMap.footerBackgroundColorPicker) {
			objectMap.footerBackgroundColorPicker = setupColorPicker("footerBackgroundColorPicker", "div#innerTawalaFooter", "background-color");
		}
    }
    
};

YAHOO.util.Event.addListener(window, "load", Tawala.Theme.init);

