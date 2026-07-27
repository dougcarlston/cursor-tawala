var DOM = YAHOO.util.Dom;
var Event = YAHOO.util.Event;
var $ = DOM.get;

if(!Tawala) { var Tawala = {}; }

Tawala.Customize = {
	setupFrame: document.getElementById("setupFormIFrame"),
	previewFrame: document.getElementById("previewFrame"),
	
	showStatus: function(status, tabLabel) {
		var ind = document.createElement("div");
		var indImage = document.createElement("img");
		indImage.src = "/images/indicator_medium.gif";
		var indText = document.createElement("span");
		indText.innerHTML = status;
		ind.appendChild(indImage);
		ind.appendChild(indText);
		var statEl = tabLabel + "Status";
		document.getElementById(tabLabel + "Content").style.display = "none";
		document.getElementById(tabLabel + "Status").appendChild(ind);
	},
	
	removeStatus: function(tabLabel) {
		var el = document.getElementById(tabLabel + "Status");
		el.removeChild(el.childNodes[0]);
		document.getElementById(tabLabel + "Content").style.display = "block";
	},
	
	/*
	 * Handle theme changes 
	 */
	changeStyleSheet: function(){
		var frameName = "previewFrame";
		var previewFrame = document.getElementById("previewFrame");
		
//		var links = top.frames[frameName].document.getElementsByTagName("link");
		var links = previewFrame.contentWindow.document.getElementsByTagName("link");
	 		
	 	var lastStylesheetElement;
	 	for(var i=0; i < links.length; i++) {
//			var a = top.frames[frameName].document.getElementsByTagName("link")[i];
			var a = previewFrame.contentWindow.document.getElementsByTagName("link")[i];

			if(a.getAttribute("rel").indexOf("style") != -1) {
				lastStylesheetElement = a;
			}
		}
		if(lastStylesheetElement) {
			var newTheme = "/css/project/" + Tawala.Customize.config.selectedTheme + "/project.css";
			a.disabled = true;
			a.href = newTheme;
			a.disabled = false;					
		}
	},
	
	updateTheme: function(theme){
		if(!theme) { return; }
		this.updateConfig(theme, "selectedTheme");			
		this.changeStyleSheet();
	},
	
	updateConfig: function(value, name){	
		Tawala.Customize.config[name] = value;
	},
	
	storeCurrentSetupFormFields: function(){
		Tawala.Customize.config.currentSetupFormValues = this.getFormFields();
	},
	
	getNewSetupFormFields: function(){
		Tawala.Customize.config.newSetupFormValues = this.getFormFields();
	},
	
	getFormFields: function(){
		var fields = {};

//		var form = top.setupFormIFrame.document.getElementById('tawalaProjectForm');
		var form = document.getElementById("setupFormIFrame").contentWindow.document.getElementById('tawalaProjectForm');
		if(form == null) {
			return fields;
		}
		
		for(var i = 0; i < form.elements.length; i++){
			var el = form.elements[i];
			var elId = "el" + i;
			if(el.tagName){
				if(el.tagName.toLowerCase() == "input"){
					if(el.type.toLowerCase() == "text" || 
							el.type.toLowerCase() == "password" || 
							el.type.toLowerCase() == "checkbox" || 
							el.type.toLowerCase() == "radio"){
						fields[elId] = el.value;
					}
				}
	
				if(el.tagName.toLowerCase() == "textarea"){
					fields[elId] = el.value;
				}
	
				if(el.tagName.toLowerCase() == "select"){
					fields[elId] = el.value;
				}
			}
		}
		return fields;
	},
	
	setupFormValuesChanged: function(){
		for(i in Tawala.Customize.config.newSetupFormValues){
			if(Tawala.Customize.config.currentSetupFormValues[i] != Tawala.Customize.config.newSetupFormValues[i]){
				return true;
			}
		}		
		return false;
	},
	
	copyNewConfigValuesToCurrentConfigValues: function(){
		Tawala.Customize.config.newSetupFormValues = {};
		for(i in Tawala.Customize.config.newSetupFormValues){
			Tawala.Customize.config.currentSetupFormValues[i] = Tawala.Customize.config.newSetupFormValues[i];
		}
	},
	
	showConfig: function(){
		var configString = "";

		var showObject = function(o){
			var s = "";
			for(var i in o){
				switch(typeof o[i]){
					case "object":
						s = s + i + ": object\n"
						break;
					case "function":
						s = s + i + ": function\n"
						break;
					default:
						s = s + i + ": " + o[i] + "\n";
						break;
				}
			}
			return s;
		}

		alert("Tawala.Customize.config:\n" + showObject(Tawala.Customize.config));
		alert("Tawala.Customize.config.currentSetupFormValues:\n" + showObject(Tawala.Customize.config.currentSetupFormValues));
		alert("Tawala.Customize.config.newSetupFormValues:\n" + showObject(Tawala.Customize.config.newSetupFormValues));
	},
	
	saveTheme: function() {
		// Check the previous value and return true immediately if nothing changed.
		if(Tawala.Customize.config.selectedTheme == Tawala.Customize.config.currentTheme) {return true;}
		
		this.responseHandler = function(o) {
			Tawala.Customize.removeStatus("appearance");
			Tawala.Customize.config.tabs.set('activeTab', Tawala.Customize.config.tabToNavigateTo.appearance, true);
			Tawala.Customize.config.currentTheme = Tawala.Customize.config.selectedTheme;
		}        
	
		this.failureHandler = function(o) {
			alert("Unable to save the new theme");
			Tawala.Customize.removeStatus("appearance");
		}
	
		Tawala.Customize.showStatus("Saving changes", "appearance");
		
		var form = document.getElementById("themeChangeForm");
		var url = form.action;
		var transInfo = {
				success:  this.responseHandler,
				failure:  this.failureHandler
		};
		
		YAHOO.util.Connect.setForm(form);
		YAHOO.util.Connect.asyncRequest( 'POST', url, transInfo);
		
		return false;
	},

	/*
	 * Update values when changed in setup form
	 */
	 updateContentValue: function(value, name){
	 	
//		var elements = top.frames["previewFrame"].document.getElementsByName(name);
		var elements = document.getElementById("previewFrame").contentWindow.document.getElementsByName(name);

//		alert( document.getElementById("previewFrame").contentWindow.document.getElementById(name));
		
		if(elements && elements.length > 0){
		 	var convertedValue = this.escapeValue(value);
		 	for(var i=0; i<elements.length; i++) {
				elements[i].innerHTML = convertedValue;
			}
		}
	},

	escapeValue: function(value) {
			var convertedValue = "";
			
			for(var i=0; i<value.length; i++) {
				var c = value.charAt(i);
				switch(c) {
					case '\n':
						convertedValue += '<br />\n';
						break;
					case '<':
						convertedValue += '&lt;';
						break;

					case '<':
						convertedValue += '&gt;';
						break;

					case '&':
						convertedValue += '&amp;';
						break;
						
					case '"':
						convertedValue += '&quot;';
						break;
					
					case "'":
						convertedValue += '&#39;';
						break;
					
					default:
						convertedValue += c;
						break;
				}
			}
			
			return convertedValue;
		},

//	wireSetupForm: function(doc) {
	wireSetupForm: function() {
		var setupFormDoc = document.getElementById('setupFormIFrame').contentWindow.document
		enhanceTextFields(setupFormDoc);
		var form = setupFormDoc.getElementById('tawalaProjectForm');
		if(form != null) {
			form.action = setupFormUrl;
		}
	},
	
	isCustomizationComplete: function(doc) {
		var errorMessage = doc.getElementById('errormsg');
		if(errorMessage != null) {
			return false;
		}
				
		var form = doc.getElementById('tawalaProjectForm');
		if(! form ) {
				//--- The case of only a document being displayed.
			return true;
		}
		
		var customizationCompleteMarker = doc.getElementById('_customization_complete');
		return (customizationCompleteMarker != null);
	},
	
	displayCustomizationIncompleteDialog: function() {
			// Define various event handlers for Dialog
			var dialogHandleOK = function() {
				this.hide();
				document.getElementById("previewContainer").style.visibility = "visible";
				return false;
			}
		
			var dialogHandleContinue = function() {
				this.hide();
				Tawala.Customize.config.tabToNavigateTo.edit = Tawala.Customize.config.tab.newValue;
				Tawala.Customize.config.tabs.set('activeTab', Tawala.Customize.config.tabToNavigateTo.edit, true);
			}
		
			var alertDialog =  
				new YAHOO.widget.SimpleDialog("confirmationDialog", 
					 { width: "480px",
					   fixedcenter: true,
					   visible: false,
					   draggable: false,
					   modal: true,
					   close: true,
					   text: "<b>You haven't finished the editing process</b> <br /><br />" +
					   		"Press 'Continue Editing' to continue the editing process or" +
							" press 'Move to the next tab' to move to the tab you clicked on.<br /><br />",
					   icon: YAHOO.widget.SimpleDialog.ICON_WARN,
					   constraintoviewport: true,
					   effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5},
					   buttons: [ { text:"Continue Editing", handler:dialogHandleOK, isDefault:true },
					   				{ text:"Move to the selected tab", handler:dialogHandleContinue } ]
					 } )
					 
			alertDialog.setHeader('<div class="tl"></div><div class="ti">Attention!!</div><div class="tr"></div>');
			alertDialog.render(document.body);								
			alertDialog.show();								
		
	},
		
	saveCustomizationForm: function() {		
		this.getNewSetupFormFields();
				
		if(! this.setupFormValuesChanged()){
//			if( this.isCustomizationComplete(frames['setupFormIFrame'].document)) {
			if( this.isCustomizationComplete(document.getElementById('setupFormIFrame').contentWindow.document)) {
				Tawala.Customize.config.tabToNavigateTo.edit = null;
				return true;
			} else {
				this.displayCustomizationIncompleteDialog();
				return false;
			}
		}
			
		this.copyNewConfigValuesToCurrentConfigValues();

		this.responseHandler = function(o) {
			Tawala.Customize.removeStatus("edit");
			
//			var newDocument = frames['setupFormIFrame'].document;
			var newDocument = document.GetlElementById('setupFormIFrame').contentWindow.document;
			newDocument.open('text/html');
			newDocument.write(o.responseText);
			newDocument.close();
		}        
	
		this.failureHandler = function(o) {
			//-- TODO: handle it.
			alert("Failed");
			Tawala.Customzie.removeStatus("edit");
		}
	
//		var form = frames['setupFormIFrame'].document.getElementById('tawalaProjectForm');
		var form = document.GetelementById('setupFormIFrame').contentWindow.document.getElementById('tawalaProjectForm');
		if(form == null) {
			//--- Means that we are viewing a document, and this is mostly likely the final document
			//--- of the setup process.
			return true;
		}

		Tawala.Customize.showStatus("Saving changes", "edit");
		
		var transInfo = {
				success:  this.responseHandler,
				failure:  this.failureHandler
		};
		
		YAHOO.util.Connect.setForm(form);
		YAHOO.util.Connect.asyncRequest( 'POST', setupFormUrl, transInfo);
		
		return false;
	},
	
	// Partially modelled based on Google's Calendar
	changeIFrameSource: function() {
	  var form = $('embedParameterForm');
	
	  var width = form.elements['width'].value;
	  var height = form.elements['height'].value;
	  var border = form.elements['border'].checked;
	  var  url = form.elements['url'].value;
	
	  var props = [
	    'style="' +  (border ? 'border:solid 1px #777' : 'border-width:0') + '"',
	    'width="' + width + '"',
	    'height="' + height + '"',
	    'frameborder="0"'
	  ];
	
	  // Show & render the html code
	  var code = '<iframe src="' + url + '" ' + props.join(' ') + '></iframe>';
	  $('code-source').value = code;
	},

	wireSetupFormFromOnload: function() {
		
		//Try this instead to get iframe object
		//document.getElementById('setupFormIFrame');
		
		var setupForm = document.getElementById('setupFormIFrame');
		
//		this.wireSetupForm(frames['setupFormIFrame'].document);
		this.wireSetupForm(document.getElementById('setupFormIFrame').contentWindow.document);
		
		if( Tawala.Customize.config.firstTimeLoad ) {
			Tawala.Customize.config.firstTimeLoad = false;
		} else {
//			frames['previewFrame'].location.reload();
			document.getElementById('previewFrame').location.reload();
		}
	
		if(Tawala.Customize.config.tabToNavigateTo.edit != null) {
//			if(Tawala.Customize.isCustomizationComplete(frames['setupFormIFrame'].document)) {
			if(Tawala.Customize.isCustomizationComplete(document.getElementById('setupFormIFrame').contentWindow.document)) {
				Tawala.Customize.config.tabs.set('activeTab', Tawala.Customize.config.tabToNavigateTo.edit, true);
			} else {
				Tawala.Customize.displayCustomizationIncompleteDialog();
			}
			Tawala.Customize.config.tabToNavigateTo.edit = null;
		}
	},
	
	launchTestDrive: function(resetUrl, preserveUrl) {
		window.open( (Tawala.Customize.config.resetTestDrive) ? resetUrl : preserveUrl,
			'TestDrive','width=790,height=590,resizable=yes,scrollbars=yes');

		/*
		 * Subsequent launches of the test drive without changing the tab will not cause data reset.
		 */
		Tawala.Customize.config.resetTestDrive = false;
	}
}

/*
 * Configuration info gets saved here
 */
Tawala.Customize.config = {};
Tawala.Customize.config.selectedTheme ="";
Tawala.Customize.config.currentTheme = "";
Tawala.Customize.config.currentSetupFormValues = {};
Tawala.Customize.config.newSetupFormValues = {};

Tawala.Customize.config.tab = {};
Tawala.Customize.config.tab.prevValue ="";
Tawala.Customize.config.tab.newValue ="";

Tawala.Customize.config.tabToNavigateTo = {};
Tawala.Customize.config.firstTimeLoad = true;
Tawala.Customize.config.showSaveDialogImmediatelyAfterPageLoad = false;
Tawala.Customize.config.resetTestDrive = true;
/*
 * Init the tabs for the page
 */
Tawala.Customize.tabsInit = function(){
	var myTabs = new YAHOO.widget.TabView("customizePalette");
	Tawala.Customize.config.tabs = myTabs;
	
    myTabs.on('contentReady', function() {

		var tabEdit = myTabs.getTab(0);
		tabEdit.label = "edit";

		var tabAppearance = myTabs.getTab(1);
		tabAppearance.label = "appearance";

/*
		var tabPreview = myTabs.getTab(2);
		tabPreview.label = "preview";

		var tabSave = myTabs.getTab(3);
		tabSave.label = "save";
*/

		var tabPublish = myTabs.getTab(2);
		tabPublish.label = "publish";

		var handleTabChange = function(e) {
			if(e.prevValue == e.newValue) { return; }
			
			Tawala.Customize.config.tab.prevValue =e.prevValue;
			Tawala.Customize.config.tab.newValue =e.newValue;
			
			/* 
			 * This will force the test drive project to be reset. It's necessary because edit tab can change
			 * some data and appearance can change the theme.
			 */
			Tawala.Customize.config.resetTestDrive = true;
			
			switch(e.newValue){
				case tabAppearance:
					document.getElementById("previewContainer").style.visibility = "visible";
					break;
				case tabEdit:
					document.getElementById("previewContainer").style.visibility = "visible";
					break;
				case tabPublish:
					document.getElementById("previewContainer").style.visibility = "hidden";
					break;
			}

			switch(e.prevValue){
				case tabAppearance:
					Tawala.Customize.config.tabToNavigateTo.appearance = e.newValue;
					return Tawala.Customize.saveTheme();
					break;
				case tabEdit:
					Tawala.Customize.config.tabToNavigateTo.edit = e.newValue;
					return Tawala.Customize.saveCustomizationForm();
					break;
				case tabPreview:
					break;
				case tabPublish:
					break;
			}
		};
        
        var handleAppearanceClick = function(e) {
        };
        
        var handleEditClick = function(e) {
        };
               
		myTabs.addListener('beforeActiveTabChange', handleTabChange);
		
		tabAppearance.addListener('click', handleAppearanceClick);
		tabEdit.addListener('click', handleEditClick);

		// Get value of currently selected theme
		Tawala.Customize.config.currentTheme = document.getElementById("themeSelect").value;
		if(Tawala.Customize.config.selectedTheme == ""){
			Tawala.Customize.config.selectedTheme = Tawala.Customize.config.currentTheme;
		}
		
    });
};

/*
 * Setup Customize dialogs
 */
Tawala.Customize.dialogs = {};

Tawala.Customize.dialogs.init = function() {

	/*
	 *  Login dialog setup
	 */
	var handleLoginCancel = function() { 
	    this.cancel(); 
	} 
	var handleLoginSubmit = function() { 
    	this.submit(); 
	} 
	
	var handleLoginSuccess = function(o) {
		// This will create serverResponse variable
		var serverResponse = eval(o.responseText);
				
		if( serverResponse.success) {
			userName = serverResponse.userName;
			
			var section = document.getElementById('saveProjectSection');
			section.style.display = "none";

			section = document.getElementById('successfulLoginConfirmationSection');
			section.style.display = "block";

			if(emptyUser){
				emptyUser = false;
				changeLoginState();
			}
			
			Tawala.Customize.dialogs.save.show();
		} else {
			var section = document.getElementById('loginErrorMessage');
			var i;
			var errorMessages = "";
			for(i=0; i< serverResponse.messages.length; i++) {
				errorMessages += serverResponse.messages[i];
				errorMessages += "<br />";
			}
			section.innerHTML = errorMessages;
			Tawala.Customize.dialogs.login.show();
		}
	}
	
	var handleLoginFailure = function(o) {
		var section = document.getElementById('loginErrorMessage');
		section.innerHTML = 'An error occured. Please try again.';
		Tawala.Customize.dialogs.login.show();
	}
	
	var loginButtons = [{text:"Submit", handler:handleLoginSubmit, isDefault:true }, 
		{ text:"Cancel", handler:handleLoginCancel }];
			    	              
	Tawala.Customize.dialogs.login = new YAHOO.widget.Dialog("loginDialog", 
		{ 	width:"400px", 
			visible:false, 
			draggable:false, 
			fixedcenter:true,
			constraintoviewport:true, 
			modal:true
//			effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5}
		 });
	Tawala.Customize.dialogs.login.cfg.queueProperty("buttons", loginButtons); 
	Tawala.Customize.dialogs.login.callback = { success: handleLoginSuccess, failure: handleLoginFailure };
	
	Tawala.Customize.dialogs.login.render(); 

	/*
	 *  Save dialog setup
	 */
	var handleSaveCancel = function() { 
	    this.cancel(); 
	} 
	var handleSaveSubmit = function() { 
    	this.submit(); 
	} 
	
	var handleSaveSuccess = function(o) {
		// This will create saveResponse variable
		var saveResponse = eval(o.responseText);
	
		if( saveResponse.success) {
			var section = document.getElementById('successfulLoginConfirmationSection');
			section.style.display = "none";

			var section = document.getElementById('successfulSignupConfirmationSection');
			section.style.display = "none";

			var section = document.getElementById('saveProjectSection');
			section.style.display = "none";

			section = document.getElementById('successfulSaveConfirmationSection');
			section.style.display = "block";
		} else {
			var section = document.getElementById('saveErrorMessage');
			var i;
			var errorMessages = "";
			for(i=0; i< saveResponse.messages.length; i++) {
				errorMessages += saveResponse.messages[i];
				errorMessages += "<br />";
			}
			section.innerHTML = errorMessages;
			Tawala.Customize.dialogs.save.show();
		}
	}
	
	var handleSaveFailure = function(o) {
		var section = document.getElementById('saveErrorMessage');
		section.innerHTML = 'An error occured. Please try again.';
		Tawala.Customize.dialogs.save.show();
	}
	
	var saveButtons = [{text:"Save", handler:handleSaveSubmit, isDefault:true }, 
		{ text:"Cancel", handler:handleSaveCancel }];
			    	              
	Tawala.Customize.dialogs.save = new YAHOO.widget.Dialog("saveDialog", 
		{ 	width:"480px", 
			visible:false, 
			draggable:false, 
			fixedcenter:true,
			constraintoviewport:true, 
			modal:true
			// effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5}
		 });
	Tawala.Customize.dialogs.save.cfg.queueProperty("buttons", saveButtons); 
	Tawala.Customize.dialogs.save.callback = { success: handleSaveSuccess, failure: handleSaveFailure };
	
	Tawala.Customize.dialogs.save.render(); 



	/*
	 *  startCustomization dialog setup
	 */
	var handleSaveCancel = function() { 
	    this.cancel(); 
	} 
	var handleSaveSubmit = function() { 
    	this.submit(); 
	} 
	
	var handleSaveOnStartupSuccess = function(o) {
		// This will create saveResponse variable
		var saveResponse = eval(o.responseText);
	
		if( saveResponse.success) {
			var section = document.getElementById('successfulLoginConfirmationSection');
			section.style.display = "none";

			var section = document.getElementById('successfulSignupConfirmationSection');
			section.style.display = "none";

			var section = document.getElementById('saveProjectSection');
			section.style.display = "none";

			section = document.getElementById('successfulSaveConfirmationSection');
			section.style.display = "block";
		} else {
			var section = document.getElementById('saveOnStartupErrorMessage');
			var i;
			var errorMessages = "";
			for(i=0; i< saveResponse.messages.length; i++) {
				errorMessages += saveResponse.messages[i];
				errorMessages += "<br />";
			}
			section.innerHTML = errorMessages;
			Tawala.Customize.dialogs.startCustomization.show();
		}
	}
	
	var handleSaveOnStartupFailure = function(o) {
		var section = document.getElementById('saveOnStartupErrorMessage');
		section.innerHTML = 'An error occured. Please try again.';
		Tawala.Customize.dialogs.startCustomization.show();
	}
	
	var saveButtons = [{text:"Save now", handler:handleSaveSubmit, isDefault:true }, 
		{ text:"Continue without saving", handler:handleSaveCancel }];
			    	              
	Tawala.Customize.dialogs.startCustomization = new YAHOO.widget.Dialog("startCustomizationDialog", 
		{ 	width:"480px", 
			visible:false, 
			draggable:false, 
			fixedcenter:true,
			constraintoviewport:true, 
			modal:true
			// effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5}
		 });
	Tawala.Customize.dialogs.startCustomization.cfg.queueProperty("buttons", saveButtons); 
	Tawala.Customize.dialogs.startCustomization.callback = { success: handleSaveOnStartupSuccess, failure: handleSaveOnStartupFailure };
	
	Tawala.Customize.dialogs.startCustomization.render(); 


	/*
	 *  Signup dialog setup
	 */
	var handleSignupCancel = function() { 
	    this.cancel(); 
	} 
	var handleSignupSubmit = function() { 
    	this.submit(); 
	} 
	
	var handleSignupSuccess = function(o) {
		// This will create signupResponse variable
		var signupResponse = eval(o.responseText);
	
		if( signupResponse.success) {
			// Report registration sucess to Google Analytics
			var _uacct = "UA-617124-1";
			urchinTracker('/user/registration_during_customization/success');
			
			var section = document.getElementById('saveProjectSection');
			section.style.display = "none";

			section = document.getElementById('successfulSignupConfirmationSection');
			section.style.display = "block";
			
			if(emptyUser){
				emptyUser = false;
				changeLoginState();
			}
			
			Tawala.Customize.dialogs.save.show();
		} else {
			var section = document.getElementById('signupErrorMessage');
			var i;
			var errorMessages = "";
			for(i=0; i< signupResponse.messages.length; i++) {
				errorMessages += signupResponse.messages[i];
				errorMessages += "<br />";
			}
			section.innerHTML = errorMessages;
			Tawala.Customize.dialogs.signup.show();
		}
	}
	
	var handleSignupFailure = function(o) {
		// Report registration failure to Google Analytics
		var _uacct = "UA-617124-1";
		urchinTracker('/user/registration_during_customization/failure');
		
		var section = document.getElementById('signupErrorMessage');
		section.innerHTML = 'An error occured. Please try again';
		Tawala.Customize.dialogs.signup.show();
	}
	
	var signupButtons = [{text:"Signup", handler:handleSignupSubmit, isDefault:true }, 
		{ text:"Cancel", handler:handleSignupCancel }];
			    	              
	// Report registration dispaly to Google Analytics
	var _uacct = "UA-617124-1";
	urchinTracker('/user/registration_during_customization/display');
	
	Tawala.Customize.dialogs.signup = new YAHOO.widget.Dialog("signupDialog", 
		{ 	width:"500px", 
			visible:false, 
			draggable:false, 
			fixedcenter:true,
			constraintoviewport:true, 
			modal:true
			//effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5}
		 });
	Tawala.Customize.dialogs.signup.cfg.queueProperty("buttons", signupButtons); 
	Tawala.Customize.dialogs.signup.callback = { success: handleSignupSuccess, failure: handleSignupFailure };
	
	Tawala.Customize.dialogs.signup.render(); 


	/*
	 *  ShowLink dialog setup
	 */
	var handleShowLinkClose = function() { 
	    this.cancel(); 
	} 
		
	var showLinkButtons = [	{ text:"Close", handler:handleShowLinkClose }];
			    	              	
	Tawala.Customize.dialogs.showLink = new YAHOO.widget.Dialog("showLinkDialog", 
		{ 	width:"600px", 
			visible:false, 
			draggable:false, 
			fixedcenter:true,
			constraintoviewport:true, 
			modal:true,
			effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5}
		 });
	Tawala.Customize.dialogs.showLink.cfg.queueProperty("buttons", showLinkButtons); 
	
	Tawala.Customize.dialogs.showLink.render(); 

	/*
	 *  Email links dialog setup
	 */
	var handleEmailCancel = function() { 
	    this.cancel(); 
	} 

	var handleEmailSubmit = function() { 
    	this.submit(); 
	} 
	
	var handleEmailSuccess = function(o) {
		// This will create sendResponse variable
		eval(o.responseText);
	
		if( sendResponse.success) {
			//--- TODO: display a success message.		
		} else {
			var section = document.getElementById('emailErrorMessage');
			var i;
			var errorMessages = "";
			for(i=0; i< sendResponse.messages.length; i++) {
				errorMessages += sendResponse.messages[i];
				errorMessages += "<br />";
			}
			section.innerHTML = errorMessages;
			Tawala.Customize.dialogs.email.show();	
		}
	}
	
	var handleEmailFailure = function(o) {
		var section = document.getElementById('emailErrorMessage');
		section.innerHTML = 'An error occured. Please try again';
		Tawala.Customize.dialogs.email.show();
	}
	
	var emailButtons = [{text:"OK", handler:handleEmailSubmit, isDefault:true }, 
		{ text:"Cancel", handler:handleEmailCancel }];
			    	              
	Tawala.Customize.dialogs.email = new YAHOO.widget.Dialog("emailDialog", 
		{ 	width:"500px", 
			visible:false, 
			draggable:false, 
			fixedcenter:true,
			constraintoviewport:true, 
			modal:true
//			, effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5}
		 });
	
	Tawala.Customize.dialogs.email.display = function(){
		var section = document.getElementById('emailErrorMessage');
		section.innerHTML = "";
		Tawala.Customize.dialogs.email.show();
	}
		 
	Tawala.Customize.dialogs.email.cfg.queueProperty("buttons",emailButtons); 
	Tawala.Customize.dialogs.email.callback = { success: handleEmailSuccess, failure: handleEmailFailure };
	
	Tawala.Customize.dialogs.email.render();
	
	if(Tawala.Customize.config.showSaveDialogImmediatelyAfterPageLoad) {
		Tawala.Customize.dialogs.startCustomization.show();
	}
}


/*
 *	Events
 */
YAHOO.util.Event.on(window, "load", Tawala.Customize.tabsInit);
YAHOO.util.Event.on(window, "load", Tawala.Customize.dialogs.init);
