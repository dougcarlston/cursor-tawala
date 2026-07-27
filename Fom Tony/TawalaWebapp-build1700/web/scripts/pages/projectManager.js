var DOM = YAHOO.util.Dom;
var Event = YAHOO.util.Event;

if(!Tawala) { var Tawala = {}; }

Tawala.ProjectManager = function(){

	function getSelectedItems(){
		var selected = "";
		var items = DOM.getElementsByClassName("versionSelected", "input");
		var moreThanOne = false;
		for(var i = 0; i < items.length; i++){
			if(items[i].checked){
				if(moreThanOne) {
					selected += '&';
				}
				selected += 'version_id=';
				selected += items[i].value;
				
				moreThanOne = true;
			}
		}		
		return(selected);
	}
		
	return {
		
		selectedItems: "",
		
		deleteSelectedItems: function(e){
			Event.stopEvent(e);
			this.selectedItems = getSelectedItems();
			if(this.selectedItems.length == 0){
			}else{
				Tawala.ProjectManager.confirmationDialog.show();
			}
			
			return(false);
		},

		showSelectedFormsOnly:function(onlySelected) {
			showSelectedFormsOnlyVar = onlySelected;
		  	YAHOO.util.Connect.asyncRequest('POST', storeShowFormsStateURL + onlySelected);
		  	if(showSelectedFormsOnlyVar) {
		  		document.getElementById('showAllFormsButton').style.display = 'inline';
		  		document.getElementById('hideSelectedFormsButton').style.display = 'none';
		  	} else {
		  		document.getElementById('showAllFormsButton').style.display = 'none';
		  		document.getElementById('hideSelectedFormsButton').style.display = 'inline';
		  	}
		  	this.updateFormList();
		},
		saveFormSelection: function(saveFormSelectionURL) {
		  	YAHOO.util.Connect.asyncRequest('POST', saveFormSelectionURL);
		  	this.updateFormList();
		},	

		updateFormList: function() {
			if(showSelectedFormsOnlyVar) {
				this.redisplaySelectedFormsOnly();
			} else {
				this.redisplayAllForms();
			}
		},
		
		redisplayAllForms: function() {
			var rows = document.getElementById('projectForms').rows;
			for(var i = 0; i < rows.length; i++){
				rows[i].style.display = '';
			}		
		},	

		redisplaySelectedFormsOnly: function() {
			var rows = document.getElementById('projectForms').rows;
			for(var i = 0; i < rows.length; i++){
				var row = rows[i];
				var checkBox = document.getElementById('hideFormCheckbox' + (i + 1));
				if(checkBox.checked) {
					row.style.display = ''; //--- Should be table-row, but IE is not happy....
				} else {
					row.style.display = 'none';
				}
			}		
		},	

		init: function(){			
			YAHOO.util.Event.addListener("PMDeleteSelected", "click", 
				Tawala.ProjectManager.deleteSelectedItems, Tawala.ProjectManager, true);

			// Define various event handlers for Dialog
			var dialogHandleYes = function() {
	
				// Start the transaction.
				Tawala.ProjectManager.connectionObject.connectionURL = "/projectmanager/delete-multiple-versions";	
				Tawala.ProjectManager.connectionObject.startRequest();	

				this.hide();
			}
		
			var dialogHandleNo = function() {
				this.hide();
			}

			Tawala.ProjectManager.confirmationDialog =  
				new YAHOO.widget.SimpleDialog("confirmationDialog", 
					 { width: "350px",
					   fixedcenter: true,
					   visible: false,
					   draggable: false,
					   modal: true,
					   close: true,
					   text: "You are about to delete versions of this project." +  
					   			"<br /><br />Are you sure you want to do this?",
					   icon: YAHOO.widget.SimpleDialog.ICON_WARN,
					   constraintoviewport: true,
					   effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5},
					   buttons: [ { text:"Yes", handler:dialogHandleYes, isDefault:true },
								  { text:"No",  handler:dialogHandleNo } ]
					 } )
					 
			Tawala.ProjectManager.confirmationDialog.setHeader('<div class="tl"></div><div class="ti">Delete Project Versions</div><div class="tr"></div>');
			Tawala.ProjectManager.confirmationDialog.render(document.body);								
		}
	};

}();


Tawala.ProjectManager.connectionObject = function(){
	var postData;	
	var requestObj;
	
	return{	
		connectionURL: "",
		
		handleSuccess:function(o){
			this.processResult(o);
		},
	
		handleFailure:function(o){
			// Failure handler
			console.log("connection failure\n" + o.status + "\n" + o.statusText + "\n" + o.argument);
		},
	
		processResult:function(o){
			window.location.reload();	
		},
	
		startRequest:function() {
			postData = Tawala.ProjectManager.selectedItems;
			var callback = {
				success: this.handleSuccess,
				failure: this.handleFailure,
				scope: Tawala.ProjectManager.connectionObject
			};
						
		  	YAHOO.util.Connect.asyncRequest('POST', this.connectionURL, callback, postData);
		}
	};
}();

Tawala.ProjectManager.Export = function() {
	return {
		showExportDataSourceDialog: function(dataSourceName, dataSourceNumber) {
			this.showDialog("Export data from: <b>" + dataSourceName + "</b>", 'Export Data Source Data', dataSourceNumber);
		},
		showExportFormDialog: function(formName, formNumber) {
			this.showDialog("Export data from: <b>" + formName + "</b>", 'Export Form Data', formNumber);
		},
		showDialog: function(exportDescription, headerDescription, linkNumber) {
			function exportInCSV() {
				this.hide();
				document.location.href = Tawala.ProjectManager.Export.currentURLToCSVExport;
			}
			function exportInExcel() {
				this.hide();
				this.hide();
				document.location.href = Tawala.ProjectManager.Export.currentURLToExcelExport;
			}
			function cancelExport() {
				this.cancel();
			}
						
			Tawala.ProjectManager.exportDialog =  
				new YAHOO.widget.SimpleDialog("exportDialog", 
					 { width: "40em",
					   fixedcenter: true,
					   visible: false,
					   draggable: true,
					   modal: true,
					   close: true,
					   text: exportDescription +
					   			"<br /><br />Please choose the format of the exported file:",
					   icon: YAHOO.widget.SimpleDialog.ICON_INFO,
					   effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5},
					   constraintoviewport: true,
					   buttons: [ { text:"Comma Separated Values (CSV)", handler:exportInCSV, isDefault:true },
								  { text:"Excel Spreadsheet",  handler:exportInExcel },
								  { text: "Cancel", handler:cancelExport} ]
					 } )
					 
			Tawala.ProjectManager.exportDialog.setHeader('<div class="tl"></div><div class="ti">' + headerDescription + '</div><div class="tr"></div>');
			Tawala.ProjectManager.exportDialog.render(document.body);								

			Tawala.ProjectManager.Export.currentURLToCSVExport = document.getElementById('exportCSV' + linkNumber).href;
			Tawala.ProjectManager.Export.currentURLToExcelExport = document.getElementById('exportExcel' + linkNumber).href;
			
			Tawala.ProjectManager.exportDialog.show();
		}
	}
} ();

Tawala.ProjectManager.ChangeAppearance = function() {
	function removeStatus() {
		var el = document.getElementById("changeAppearanceStatus");
		el.parentNode.removeChild(el);
	
	}
	function showStatus() {
		var ind = document.createElement("span");
		var indImage = document.createElement("img");
		indImage.src = "/images/indicator_medium.gif";
		ind.appendChild(indImage);
		ind.setAttribute("id", "changeAppearanceStatus");
		document.getElementById("projectAppearanceStatusContainer").appendChild(ind);
	}

	return {
		saveTheme: function() {
			this.responseHandler = function(o) {
				removeStatus();
			}        
		
			this.failureHandler = function(o) {
				alert("Unable to save the new theme");
				removeStatus();
			}
		
			showStatus("Saving changes");
			
			var form = document.getElementById("themeChangeForm");
			var url = form.action;
			var transInfo = {
					success:  this.responseHandler,
					failure:  this.failureHandler
			};
			
			YAHOO.util.Connect.setForm(form);
			YAHOO.util.Connect.asyncRequest( 'POST', url, transInfo);
			
			return false;
		}
	}
} ();

Tawala.ProjectManager.projectStatus = function() {
	function removeOnlineStatusChangeProgress() {
		var el = document.getElementById("onlineStatusBusy");
		el.parentNode.removeChild(el);
	}

	function showOnlineStatusChangeProgress() {
		var ind = document.createElement("span");
		var indImage = document.createElement("img");
		indImage.src = "/images/indicator_medium.gif";
		ind.appendChild(indImage);
		ind.setAttribute("id", "onlineStatusBusy");
		document.getElementById("onlineStatusBusyContainer").appendChild(ind);
	}

	return {
		takeOffline: function (doTakeOffline) {
		this.responseHandler = function(o) {
			removeOnlineStatusChangeProgress();
			offline = eval(o.responseText);
			if(offline) {
				document.getElementById("onlineSection").style.display = 'none';
				document.getElementById("offlineButton").style.display = "none";
				document.getElementById("offlineSection").style.display = 'inline';
				document.getElementById("onlineButton").style.display = "block";
			} else {
				document.getElementById("onlineSection").style.display = 'inline';
				document.getElementById("offlineButton").style.display = "block";
				document.getElementById("offlineSection").style.display = 'none';
				document.getElementById("onlineButton").style.display = "none";
			}
		}        
	
		this.failureHandler = function(o) {
			alert("Unable to change project's status");
			removeOnlineStatusChangeProgress();
		}
	
		showOnlineStatusChangeProgress();
		
		var form = document.getElementById("changeOnlineStatusForm");
		form.offline.value = "" + doTakeOffline;
		var url = form.action;
		var transInfo = {
				success:  this.responseHandler,
				failure:  this.failureHandler
		};
		
		YAHOO.util.Connect.setForm(form);
		YAHOO.util.Connect.asyncRequest( 'POST', url, transInfo);
		
		return false;
	}
	
	}
}();

//--- New IFrame based dialogs.
Tawala.ProjectManager.ActionsIFrame = function() {

	var removeAllChildren = function(element) {
	  while(element.hasChildNodes()) { 
	  	element.removeChild( element.lastChild ); 
	  }
	}

	var createIframe = function() {
			var iframe = document.createElement('IFRAME');
			iframe.frameBorder = "0";
			iframe.marginwidth = "0";
			iframe.marginheight = "0";
			iframe.scrolling = "no"; 
			return(iframe);
	}
	
	var setDialogTitle = function(title){
		$("dialogTitle").innerHTML = title;	
	}
	
	return {
		init: function() {
			var docXY = $D.getXY("doc");
			var dr = $D.getRegion("doc");
			var docWidth = dr["right"] - dr["left"];
			var dialogWidth = 650;
			
			Tawala.ProjectManager.dialogIFrame =  
				new YAHOO.widget.Dialog("dialogIFrame", 
					 { width: dialogWidth + "px",
					   fixedcenter: false,
					   xy: [ docXY[0] + ((docWidth - dialogWidth) / 2), 50],
					   visible: false,
					   draggable: true,
					   modal: true,
					   close: true,
					   icon: YAHOO.widget.SimpleDialog.ICON_INFO,
					   effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5},
					   constraintoviewport: true
					 } );
								 
			Tawala.ProjectManager.dialogIFrame.render(document.body);
			YAHOO.util.Event.addListener("pmActionsPublish", "click", Tawala.ProjectManager.ActionsIFrame.showPublishDialog, Tawala.ProjectManager.ActionsIFrame, true);
			YAHOO.util.Event.addListener("pmActionsExportAll", "click", Tawala.ProjectManager.ActionsIFrame.showExportAllDialog, Tawala.ProjectManager.ActionsIFrame, true);
			YAHOO.util.Event.addListener("pmActionsBackup", "click", Tawala.ProjectManager.ActionsIFrame.showBackupDialog, Tawala.ProjectManager.ActionsIFrame, true);
			YAHOO.util.Event.addListener("pmActionsImportAll", "click", Tawala.ProjectManager.ActionsIFrame.showImportAllDialog, Tawala.ProjectManager.ActionsIFrame, true);
			YAHOO.util.Event.addListener("pmActionsRestore", "click", Tawala.ProjectManager.ActionsIFrame.showRestoreDialog, Tawala.ProjectManager.ActionsIFrame, true);
			YAHOO.util.Event.addListener("pmViewRestoreProject", "click", Tawala.ProjectManager.ActionsIFrame.showRestoreDialog, Tawala.ProjectManager.ActionsIFrame, true);
			
			YAHOO.util.Event.addListener("publishNewIFrame", "click", Tawala.ProjectManager.ActionsIFrame.publishNew, Tawala.ProjectManager.ActionsIFrame, true);
			YAHOO.util.Event.addListener("publishAsVersionIFrame", "click", Tawala.ProjectManager.ActionsIFrame.publishAsVersion, Tawala.ProjectManager.ActionsIFrame, true);
			YAHOO.util.Event.addListener(document.getElementsByName("pmActionsRestoreFromOnlineBackup"), "click", Tawala.ProjectManager.ActionsIFrame.showRestoreFromOnlineBackupDialog);
		},
		 
		showPublishDialog: function() {
			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("publishSelectContentIF");

			$("dialogIFrame").style.height = "auto";
			$("dialogContentIFrame").style.height = "auto";

			var handleCancel = function() {
				this.cancel();
			}
			
			var handleSubmit = function() {
				this.submit();
			}
			var myButtons = [ {  text:"Cancel", handler:handleCancel } ];
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("y", 50);
			Tawala.ProjectManager.dialogIFrame.render(document.body);

			setDialogTitle("Publish Project to the Community Library");
			Tawala.ProjectManager.dialogIFrame.show();			
		},
		
		publishNew: function() {
			var attributes2 = {};

			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("publishNewContentIF");
						
			removeAllChildren($("publishNewContentIFrame"));

			var iframe = createIframe();
		    iframe.src = linkToPublishAsNew;
		    iframe.width = "600";
			
			
			if (isAdmin) {
				iframe.height = "745";
				attributes2 = {
					height: { to: 750 }
				}
			}else{
				iframe.height = "485";
				attributes2 = {
					height: { to: 490 }
				}
			}
			
		    $("publishNewContentIFrame").appendChild(iframe);
		    
		    currentDialogObject = Tawala.ProjectManager.dialogIFrame;
			var myButtons = [ ];
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.render(document.body);

    		var anim2 = new YAHOO.util.Anim('dialogContentIFrame', attributes2, .5);
			anim2.animate();
		},
		
		publishAsVersion: function() {
			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("publishAsVersionContentIF");

			removeAllChildren($("publishAsVersionContentIFrame"));

			var iframe = createIframe();
		    iframe.src = linkToPublishAsVersion;
		    iframe.height = "430";
		    iframe.width = "600";
		    $("publishAsVersionContentIFrame").appendChild(iframe);
			
		    currentDialogObject = Tawala.ProjectManager.dialogIFrame;
			var myButtons = [];
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.render(document.body);

			var attributes2 = {
        		height: { to: 450 }
    		};
    		var anim2 = new YAHOO.util.Anim('dialogContentIFrame', attributes2, 1);
			anim2.animate();
		},
		
		showExportAllDialog: function(ev){
			var attributes = {};
						
			YAHOO.util.Event.stopEvent(ev);
			
			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("exportAllContentIF");

			removeAllChildren($("exportAllContentIFrame"));

			var handleCancel = function() {
				this.cancel();
			}
			
			var handleExport = function() {
				//iframe id="file_download" width="0" height="0" scrolling="no" frameborder="0"
				// src="${exportURL}"></iframe>
				var dlIframe = createIframe();
			    dlIframe.id = "file_download";
			    dlIframe.src = exportURL;
			    dlIframe.width = "0";
			    dlIframe.height = "0";
			    dlIframe.scrolling = "no";
			    dlIframe.frameborder = "no";
			    $("exportAllContentIFrame").appendChild(dlIframe);

				this.cancel();
			}

			var iframe = createIframe();
		    iframe.src = linkToExportAll;
		    iframe.width = "600";			
			iframe.height = "150";
			
			attributes = {
				height: { to: 160 }
			}
			
		    $("exportAllContentIFrame").appendChild(iframe);
		    
		    currentDialogObject = Tawala.ProjectManager.dialogIFrame;
			var myButtons = [ 
				{  text:"EXPORT", handler:handleExport }, 
				{  text:"CANCEL", handler:handleCancel } 
			];
			setDialogTitle("Export All Project Data to Excel");
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.render(document.body);
			Tawala.ProjectManager.dialogIFrame.show();			

    		var anim2 = new YAHOO.util.Anim('dialogContentIFrame', attributes, .5);
			anim2.animate();
		},
		
		showBackupDialog: function(ev) {
			var attributes = {};

			YAHOO.util.Event.stopEvent(ev);
			
			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("backupContentIF");

			removeAllChildren($("backupContentIFrame"));

			var handleCancel = function() {
				this.cancel();
			}
			
			var handleBackup = function() {
				//iframe id="file_download" width="0" height="0" scrolling="no" frameborder="0"
				// src="${exportURL}"></iframe>
				var dlIframe = createIframe();
			    dlIframe.id = "file_download";
			    dlIframe.src = backupURL;
			    dlIframe.width = "0";
			    dlIframe.height = "0";
			    dlIframe.scrolling = "no";
			    dlIframe.frameborder = "no";
			    $("backupContentIFrame").appendChild(dlIframe);

				this.cancel();
			}

			var iframe = createIframe();
		    iframe.src = linkToBackup;
		    iframe.width = "600";			
			iframe.height = "150";
			
			attributes = {
				height: { to: 160 }
			}
			
		    $("backupContentIFrame").appendChild(iframe);
		    
		    currentDialogObject = Tawala.ProjectManager.dialogIFrame;
			var myButtons = [ 
				{  text:"BACKUP", handler:handleBackup }, 
				{  text:"CANCEL", handler:handleCancel } 
			];

			setDialogTitle("Backup Project");
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.render(document.body);
			Tawala.ProjectManager.dialogIFrame.show();			

    		var anim2 = new YAHOO.util.Anim('dialogContentIFrame', attributes, .5);
			anim2.animate();
		},

		showImportAllDialog: function() {
			var attributes = {};

			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("importAllContentIF");

			removeAllChildren($("importAllContentIFrame"));

			var iframe = createIframe();
		    iframe.src = linkToImportAll;
		    iframe.width = "600";
			
			
			iframe.height = "360";
			attributes = {
				height: { to: 365 }
			}
			
		    $("importAllContentIFrame").appendChild(iframe);
		    
		    currentDialogObject = Tawala.ProjectManager.dialogIFrame;
			currentDialogObject.importHandleSubmit = function(e){
				$E.stopEvent(e);
				this.cancel();	
				window.location.reload();	
			}
			
			currentDialogObject.beforeSubmitEvent.subscribe(currentDialogObject.importHandleSubmit, currentDialogObject);
			


			setDialogTitle("Import Project Data");
			var myButtons = [ ];
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.render(document.body);
			Tawala.ProjectManager.dialogIFrame.show();			

    		var anim = new YAHOO.util.Anim('dialogContentIFrame', attributes, .5);
			anim.animate();
		},

		showRestoreDialog: function() {
			var attributes = {};

			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("restoreContentIF");

			removeAllChildren($("restoreContentIFrame"));

			var iframe = createIframe();
		    iframe.src = linkToRestore;
		    iframe.width = "600";
			
			
			iframe.height = "290";
			attributes = {
				height: { to: 300 }
			}
			
		    $("restoreContentIFrame").appendChild(iframe);
		    
		    currentDialogObject = Tawala.ProjectManager.dialogIFrame;

			currentDialogObject.restoreHandleSubmit = function(e){
				$E.stopEvent(e);
				this.cancel();	
				window.location.reload();	
			}
			
			currentDialogObject.beforeSubmitEvent.subscribe(currentDialogObject.restoreHandleSubmit, currentDialogObject);
			
			setDialogTitle("Restore Project");
			var myButtons = [ ];
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.render(document.body);
			Tawala.ProjectManager.dialogIFrame.show();			

    		var anim = new YAHOO.util.Anim('dialogContentIFrame', attributes, .5);
			anim.animate();
		},

		showRestoreFromOnlineBackupDialog: function() {
			var attributes = {};

			Tawala.ProjectManager.ActionsIFrame.showSectionNamed("restoreOnlineBackupIF");

			removeAllChildren($("restoreOnlineBackupIFrame"));

			var iframe = createIframe();
		    iframe.src = linkToRestoreOnlineBackupURL + this.value;
		    iframe.width = "600";
			
			
			iframe.height = "290";
			attributes = {
				height: { to: 300 }
			}
			
		    $("restoreOnlineBackupIFrame").appendChild(iframe);
		    
		    currentDialogObject = Tawala.ProjectManager.dialogIFrame;

			currentDialogObject.restoreHandleSubmit = function(e){
				$E.stopEvent(e);
				this.cancel();	
				window.location.reload();	
			}
			
			currentDialogObject.beforeSubmitEvent.subscribe(currentDialogObject.restoreHandleSubmit, currentDialogObject);
			
			setDialogTitle("Restore Project from Online Backup");
			var myButtons = [ ];
			Tawala.ProjectManager.dialogIFrame.cfg.queueProperty("buttons", myButtons);
			Tawala.ProjectManager.dialogIFrame.render(document.body);
			Tawala.ProjectManager.dialogIFrame.show();			

    		var anim = new YAHOO.util.Anim('dialogContentIFrame', attributes, .5);
			anim.animate();
		},
		
		showSectionNamed : function(sectionName) {
			var sectionList = ["publishSelectContentIF", "publishNewContentIF", "publishAsVersionContentIF", "exportAllContentIF", 
				"backupContentIF", "importAllContentIF", "restoreContentIF", "restoreOnlineBackupIF"];

			for(var i = 0; i < sectionList.length; i++){
				var currentSection = sectionList[i];
				var displayMode = "none";
				if(currentSection == sectionName) {
					displayMode = "block";
				}	
				if ($(currentSection)) {
					$(currentSection).style.display = displayMode;
				}
			}
		}
	}
} ();

YAHOO.util.Event.addListener(window, "load", 
		Tawala.ProjectManager.init, Tawala.ProjectManager, true);
YAHOO.util.Event.addListener(window, "load", 
		Tawala.ProjectManager.Export.init, Tawala.ProjectManager, true);
YAHOO.util.Event.addListener(window, "load", 
		Tawala.ProjectManager.confirmationDialogInit, Tawala.ProjectManager, true);

YAHOO.util.Event.onDOMReady(Tawala.ProjectManager.ActionsIFrame.init, Tawala.ProjectManager.ActionsIFrame, true);

var currentDialogObject;

//--- Project email related code
var initializeEmailDataTable = function () {
    var DataSource = YAHOO.util.DataSource,
        DataTable  = YAHOO.widget.DataTable,
        Paginator  = YAHOO.widget.Paginator;

    var emailDataSource = new DataSource('/email/get-project-emails?');
    emailDataSource.responseType   = DataSource.TYPE_JSON;
    emailDataSource.responseSchema = {
        resultsList : 'records',
        fields      : ["status", "to", "cc", "from", "subject", "date_created", "id"],
        metaFields : {
            totalRecords: 'totalRecords' // The totalRecords meta field is
                                         // a "magic" meta, and will be passed
                                         // to the Paginator.
        }
    };

    var buildQueryString = function (state,dt) {
        return "startIndex=" + state.pagination.recordOffset +
               "&results=" + state.pagination.rowsPerPage + "&user_project_id=" + userProjectId;
    };

    var emailPaginator = new Paginator({
        containers         : ['email-paging-top', 'email-paging-bottom'],
        pageLinks          : 5,
        rowsPerPage        : 15,
        rowsPerPageOptions : [15,30,60],
        template           : "<strong>{CurrentPageReport}</strong> {PreviousPageLink} {PageLinks} {NextPageLink} {RowsPerPageDropdown}"
    });

    var emaiTableConfig = {
        initialRequest         : 'startIndex=0&results=25&user_project_id=' + userProjectId,
        generateRequest        : buildQueryString,
        paginationEventHandler : DataTable.handleDataSourcePagination,
        paginator              : emailPaginator
    };

	var showLinkToViewWholeEmail = function(el, oRecord, oColumn, oData) {
        el.innerHTML = '<a href="' + viewEmailURL + oData + '">View</a>';
    }
	
    var emaiColumnDefs = [
        {key:"status", label: "Status"},
        {key:"to", label: "To"},
        {key:"cc", label: "Cc"},
        {key:"from", label: "From"},
        {key:"subject", label: "Subject"},
        {key:"date_created", label: "Date Created", width: 110},
        {key:"id", label: "", formatter: showLinkToViewWholeEmail}
    ];

    var myTable = new DataTable('email-data-table', emaiColumnDefs, emailDataSource, emaiTableConfig);
}