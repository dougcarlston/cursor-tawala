/*
 *  The following variable are defined in the coach-management.jsp file:
 *  
 *	Tawala.ProjectGroup.PROJECT_ID
 *	Tawala.ProjectGroup.EMAIL
 *	Tawala.ProjectGroup.URL_UpdateLateRegistration
 *	Tawala.ProjectGroup.groupId
 */

if(! Tawala) {
	var Tawala = {};
}
Tawala.ProjectGroup = {}


Tawala.ProjectGroup.removeStatus = function() {
	var el = document.getElementById("loadStatus");
	if(el) {
		el.parentNode.removeChild(el);
	}

}

Tawala.ProjectGroup.showStatus = function(text) {
	var ind = document.createElement("span");
	var indImage = document.createElement("img");
	indImage.src = "/images/indicator_medium.gif";
	ind.appendChild(indImage);
	ind.setAttribute("id", "loadStatus");
	document.getElementById("loadStatusContainer").appendChild(ind);
}

Tawala.ProjectGroup.clearCoachesSection = function() {
	var coachesSection = document.getElementById('coachSection');
	if ( coachesSection.hasChildNodes() ) {
	    while ( coachesSection.childNodes.length >= 1 )
	    {
	        coachesSection.removeChild( coachesSection.firstChild );       
	    } 
	}
}
	
Tawala.ProjectGroup.loadCoaches = function(groupId, projectId) {
		var responseHandler = function(o) {
			Tawala.ProjectGroup.removeStatus();
			Tawala.ProjectGroup.clearCoachesSection();

			response = eval(o.responseText);
			if(! response.success) {
				alert("An error occured when trying to load the coachs for this league. Please try again.");
				return;
			}

			Tawala.ProjectGroup.coaches = response.coaches;
			Tawala.ProjectGroup.statusMap = response.statusMap;
			Tawala.ProjectGroup.adminDashboardURL = response.adminDashboardURL;
			
			Tawala.ProjectGroup.displayCurrentCoaches();
		}        
	
		var failureHandler = function(o) {
			Tawala.ProjectGroup.removeStatus();
			Tawala.ProjectGroup.clearCoachesSection();
			
			alert("Unable to load coaches for this league. Please try again.");
			removeStatus();
		}

		Tawala.ProjectGroup.clearCoachesSection();
		Tawala.ProjectGroup.showStatus("Loading coaches for project");
		
		var url = Tawala.ProjectGroup.URL_LoadCoaches;
		url += '?group_id=' + groupId + '&project_id=' + projectId;
		var transInfo = {
				success:  responseHandler,
				failure:  failureHandler
		};
		
		YAHOO.util.Connect.asyncRequest( 'GET', url, transInfo);
		
		return false;
	}

	Tawala.ProjectGroup.displayCurrentCoaches = function() {
		var coachesSection = document.getElementById('coachSection');
		
		if(Tawala.ProjectGroup.adminDashboardURL) {
			var linkToViewDetailedCoachesData = document.createElement("a");
			linkToViewDetailedCoachesData.setAttribute("href", Tawala.ProjectGroup.adminDashboardURL);
			linkToViewDetailedCoachesData.setAttribute("target", "_blank");
			linkToViewDetailedCoachesData.appendChild(document.createTextNode("Admin Dashboard"));

			coachesSection.appendChild(linkToViewDetailedCoachesData);
			coachesSection.appendChild(document.createElement("br"));
			coachesSection.appendChild(document.createElement("br"));
		}
		

		var coachesTable = document.createElement("table");
		coachesTable.id = "coachesTable";
		coachesTable.setAttribute("class", "list left sortable ruler");
		
		var headerSection = document.createElement('thead');
		var headerRow = document.createElement("tr");
		Tawala.ProjectGroup.addTableHeader(headerRow, 'First');
		Tawala.ProjectGroup.addTableHeader(headerRow, 'MI');
		Tawala.ProjectGroup.addTableHeader(headerRow, 'Last');
		Tawala.ProjectGroup.addTableHeader(headerRow, 'Birthday');
		Tawala.ProjectGroup.addTableHeader(headerRow, 'Email');
		Tawala.ProjectGroup.addTableHeader(headerRow, 'Status/Memo');
		
		headerSection.appendChild(headerRow);
		coachesTable.appendChild(headerSection);
				
		var bodySection = document.createElement('tbody');
		for(var i = 0; i < Tawala.ProjectGroup.coaches.length; i++) {
			var coach = response.coaches[i];
			
			var coachRow = document.createElement("tr");
			Tawala.ProjectGroup.addTableCell(coachRow, coach.firstName);
			Tawala.ProjectGroup.addTableCell(coachRow, coach.middleName);
			Tawala.ProjectGroup.addTableCell(coachRow, coach.lastName);
			Tawala.ProjectGroup.addTableCell(coachRow, coach.birthDay);
			Tawala.ProjectGroup.addTableCell(coachRow, coach.email);
			Tawala.ProjectGroup.addTableCell(coachRow, 
								[ Tawala.ProjectGroup.createSelectStatusElement(coach.id, coach.statusId),
									Tawala.ProjectGroup.createStatusMemoElement(coach.id, coach.statusMemo)], 
								{"cellType":'td', "id":"statusMemo" + coach.id, "className":"statusMemo"});
			
			bodySection.appendChild(coachRow); 
		}
		coachesTable.appendChild(bodySection);
		coachesSection.appendChild(coachesTable);
	}
	
	Tawala.ProjectGroup.createSelectStatusElement = function(coachId, coachStatusId) {
		var result = document.createElement("select");
		result.setAttribute("id", 'coachStatusSelect' + coachId);
		for (var key  in Tawala.ProjectGroup.statusMap) {
			if(typeof Tawala.ProjectGroup.statusMap[key] != 'object') {
				continue;
			}
			var option = document.createElement("option");
			option.value = key;
			option.innerHTML = Tawala.ProjectGroup.statusMap[key].name;
			if(coachStatusId == key) {
				option.selected = true;
			}
			result.appendChild(option);
		}
		
		result.onchange = function() {
			Tawala.ProjectGroup.updateCoachStatus(Tawala.ProjectGroup.groupId, Tawala.ProjectGroup.currentProjectId, coachId, this.value); 
		}
		return result;
	} 
	
	Tawala.ProjectGroup.createStatusMemoElement = function(coachId, statusMemo) {
		var link = document.createElement("a");
		link.id = "memoLink" + coachId;
		link.onclick = function() {
			Tawala.ProjectGroup.showMemo(Tawala.ProjectGroup.groupId, Tawala.ProjectGroup.currentProjectId, coachId); 
		}
		if(statusMemo.length == 0) {
			link.appendChild(document.createTextNode("Add"));
		} else {
			link.appendChild(document.createTextNode("View/Edit"));
		}

		var valueToShow = statusMemo;
		if(valueToShow.length > 160) {
			valueToShow = valueToShow.substring(0,160) + "...";
		}
		var title = document.createElement("div");
		title.className = "memoTitle";
		title.innerHTML = "Memo:";
		var memoText = document.createElement("div");
		memoText.id = "memoText" + coachId;
		memoText.className = "memoText";
		memoText.innerHTML = valueToShow;
		var result = document.createElement('div');
		result.className = "memo";
		result.appendChild(title);
		result.appendChild(memoText);
		result.appendChild(link);				
		return result;
	} 
	
	Tawala.ProjectGroup.showMemo = function(groupId, currentProjectId, coachId) {
		if(! Tawala.ProjectGroup.memoDialog) {
			var dialogHandleOK = function() {
				this.callback.scope.updatedMemo = this.getData()[Tawala.ProjectGroup.MEMO];
				this.submit();
			}
		
			var dialogHandleCancel = function() {
				this.cancel();
			}

			Tawala.ProjectGroup.memoDialog =  
			new YAHOO.widget.Dialog("memoDialog", 
				 { fixedcenter: true,
				   visible: false,
				   draggable: true,
				   modal: true,
				   close: true,
				   effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:0.5},
				   buttons: [ { text:"Save", handler:dialogHandleOK, isDefault:true },
				   				{ text:"Cancel", handler:dialogHandleCancel } ]
				 } )
				 
			Tawala.ProjectGroup.memoDialog.render();
		}
		var form = document.getElementById('memoForm');
		var coach = Tawala.ProjectGroup.getCoachById(coachId);
		
		form[Tawala.ProjectGroup.PROJECT_ID].value = currentProjectId;
		form[Tawala.ProjectGroup.COACH_ID].value = coachId;
		form[Tawala.ProjectGroup.MEMO].value = coach.statusMemo;
		
		var responseProcessor = {
			coachId: coachId,
			
			successHandler: function(o) {
				response = eval(o.responseText);
				if(response.success) {
					var coach = Tawala.ProjectGroup.getCoachById(this.coachId);
					coach.statusMemo = this.updatedMemo;

					var memoText = document.getElementById("memoText" + this.coachId);
					memoText.innerHTML = this.updatedMemo;
					var memoLink = document.getElementById("memoLink" + this.coachId);
					if(this.updatedMemo.length > 0) {
						memoLink.innerHTML = "View/Edit";
					}else{
						memoLink.innerHTML = "Add";
					}
					
				} else {
					this.processFailure();
					return;
				}
			},
			
			failureHandler: function(o) {
				Tawala.ProjectGroup.removeStatus();
				this.processFailure();				
			},

			processFailure: function() {
				alert("Failed to update status memo. Please try again.");
			} 
		}
		
		Tawala.ProjectGroup.memoDialog.callback.success = responseProcessor.successHandler;
		Tawala.ProjectGroup.memoDialog.callback.failure = responseProcessor.failureHandler;
		Tawala.ProjectGroup.memoDialog.callback.scope = responseProcessor;
		
		Tawala.ProjectGroup.memoDialog.show();		
	}
	
	Tawala.ProjectGroup.getCoachById = function(coachId) {
		for(var i = 0; i < Tawala.ProjectGroup.coaches.length; i++) {
			var coach = Tawala.ProjectGroup.coaches[i];
			if(coach.id == coachId) {
				return coach;
			}
		}
		return null;
	}

	Tawala.ProjectGroup.updateCoachStatus = function(groupId,projectId,coachId,statusId) {
		var updateStatusObject = {
			coachId: coachId,
			newStatusId: statusId,
			 
			responseHandler: function(o) {
				Tawala.ProjectGroup.removeStatus();
			
				response = eval(o.responseText);
				if(response.success) {
					var coach = Tawala.ProjectGroup.getCoachById(this.coachId);
					coach.statusId = this.newStatusId;
				} else {
					this.processFailure();
					return;
				}
			},
			
			processFailure: function() {
				alert("Failed to update coach's status. Please try again.");
				var coach = Tawala.ProjectGroup.getCoachById(this.coachId);
				var selectElement = document.getElementById('coachStatusSelect' + this.coachId);
				
				for ( var i = 0; i < selectElement.options.length; i++) {
					var currentOption = selectElement.options[i];
					if(currentOption.value == coach.statusId) {
						selectElement.selectedIndex = i;
						return;
					}
				}
				alert("Failed to restore previous coach status. Please reload the page and try again." );
			}, 
			
			failureHandler: function(o) {
				Tawala.ProjectGroup.removeStatus();
				this.processFailure();				
			}
		};

		Tawala.ProjectGroup.showStatus("Saving");

		var url = Tawala.ProjectGroup.URL_UpdateCoachStatus + 
			'?' + 'group_id=' + groupId + "&project_id=" + projectId + "&coach_id=" + coachId + "&status_id=" + statusId;
		
		var transInfo = {
				success:  updateStatusObject.responseHandler,
				failure:  updateStatusObject.failureHandler,
				scope: updateStatusObject
		};
		
		YAHOO.util.Connect.asyncRequest( 'POST', url, transInfo);
		
		return false;
	}
	
	Tawala.ProjectGroup.addTableHeader = function(row, value) {
		Tawala.ProjectGroup.addTableCell(row, value, {"cellType":'th'});
	}
	
	Tawala.ProjectGroup.addTableCell = function(row, value, props) {
		var i, cell, cellData;
		 
		 if(!props) {
		 	props = {};
		 }
		 
		if("cellType" in props) {
			cell = document.createElement(props.cellType);
		}else{
			cell = document.createElement("td");
		}

		if("id" in props && typeof props.id == 'string') {
			cell.id = props.id;
		}
		
		if("className" in props && typeof props.className == 'string') {
			cell.className = props.className;
		}
		
		if(value instanceof Array) {
			for(i = 0; i < value.length; i++){
				if(typeof value[i] == 'string') {
				  cellData = document.createTextNode(value[i]);
				} else {
					cellData = value[i];
				}
				cell.appendChild(cellData);
			}
		}else{
			if(typeof value == 'string') {
			  cellData = document.createTextNode(value);
			} else {
				cellData = value;
			}
			cell.appendChild(cellData);
		}

		row.appendChild(cell);
	}


	Tawala.ProjectGroup.initNotificationLinks = function() {
		var nLinks = YAHOO.util.Dom.getElementsByClassName("notificationLink","a");
		for (var i = 0; i<nLinks.length; i++){
			YAHOO.util.Event.addListener(nLinks[i].id, "click", Tawala.ProjectGroup.showNotificationPanel);
		}
	}

	Tawala.ProjectGroup.showNotificationPanel = function (e) {
		YAHOO.util.Event.stopEvent(e);
		var targ = YAHOO.util.Event.getTarget(e);
		
		// Instantiate the notification panel 
		Tawala.ProjectGroup.container = {};
		Tawala.ProjectGroup.container.panel2 = new YAHOO.widget.Panel("panel2", { width:"950px", height:"590px", visible:false, close:true, x:40, y:20 ,constraintoviewport:true, underlay:"shadow", modal:true } );
		Tawala.ProjectGroup.container.panel2.setHeader("<div class='tl'></div><span id='dialogTitle'>Notification of Late Team Assignments</span><div class='tr'></div>");
		Tawala.ProjectGroup.container.panel2.setBody("<iframe width='100%' height='560' src='"+ targ.href + "'></iframe>");
		Tawala.ProjectGroup.container.panel2.setFooter("<div class='buttons right'style='padding-top:5px;'><button class='button' type='button' onclick='Tawala.ProjectGroup.container.panel2.hide();'>CLOSE WINDOW</button></div>");
		Tawala.ProjectGroup.container.panel2.hideEvent.subscribe(Tawala.ProjectGroup.reloadPage);
		Tawala.ProjectGroup.container.panel2.render(document.body);
		Tawala.ProjectGroup.container.panel2.show();		
	}
	
	Tawala.ProjectGroup.reloadPage = function (e) {
		window.location.reload();
	}
	
	YAHOO.util.Event.onDOMReady(Tawala.ProjectGroup.initNotificationLinks);

