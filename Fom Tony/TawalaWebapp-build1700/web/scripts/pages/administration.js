var DOM = YAHOO.util.Dom;
var Event = YAHOO.util.Event;

if(!Tawala) { var Tawala = {}; }
Tawala.EventReport = {};

Tawala.EventReport.calendar = function() {
	
	var calStart;
	var selectedStartDate;
	var calEnd;
	var selectedEndDate;
	
	return {
		init: function() {	
			if (document.getElementById("eventReportStartCalContainer")) {
				var today = new Date();
				calStart = new YAHOO.widget.Calendar("eventReportStart", "eventReportStartCalContainer", {
					title: "Select Start Date",
					close: true,
					pagedate: today,
					maxDate: today
				});
				calStart.render();
				
				calStart.selectEvent.subscribe(Tawala.EventReport.calendar.handleStartSelect, calStart, true);
				Event.addListener("showEventReportStart", "click", calStart.show, calStart, true);
				
				calEnd = new YAHOO.widget.Calendar("eventReportEnd", "eventReportEndCalContainer", {
					title: "Select End Date",
					pagedate: today,
					maxDate: today,
					close: true
				});
				calEnd.render();
				
				calEnd.selectEvent.subscribe(Tawala.EventReport.calendar.handleEndSelect, calEnd, true);
				Event.addListener("showEventReportEnd", "click", calEnd.show, calEnd, true);
			}
		},

		handleStartSelect: function(type,args,obj) {
			selectedStartDate = obj.getSelectedDates()[0];
			Tawala.EventReport.calendar.rerenderEndCal();
			Tawala.EventReport.calendar.handleSelect(args, obj, "startDate");
		},
		
		handleEndSelect: function(type,args,obj) {
			selectedEndDate = obj.getSelectedDates()[0];
			Tawala.EventReport.calendar.rerenderStartCal();
			Tawala.EventReport.calendar.handleSelect(args, obj, "endDate");
		},
		
		handleSelect: function(args,obj, inputId) {
			var txtDate = document.getElementById(inputId);
			var arrDates = obj.getSelectedDates();
			txtDate.value = '';
			for (var i = 0; i < arrDates.length; ++i) {
				var date = arrDates[i];			
				var month = date.getMonth() + 1;
				var year = date.getFullYear();
				var day = date.getDate();

				if(txtDate.value){
					txtDate.value = txtDate.value + "\n" + month + "/" + day + "/" + year;
				}else{
					txtDate.value = month + "/" + day + "/" + year;				
				}
			}
		},
		
		rerenderStartCal: function(){
			if(selectedEndDate){
				calStart.cfg.setProperty("maxDate", selectedEndDate);
				calStart.render();
			}
		},
		
		rerenderEndCal: function(){
			if(selectedStartDate){
				calEnd.cfg.setProperty("minDate", selectedStartDate);
				calEnd.render();
			}
		}
		
	};
}();

Event.on(window, "load", Tawala.EventReport.calendar.init);
	

/*
 *	Topic List
 *	
 *	HTML markup example:
 *	<ul id="topicListId" class="topicList">
 *		<li><a class="topic" href="url-of-content-to-display">Topic 1</a></li>
 *		<li>
 *			<a class="topicSection" href="#">Section 1</a>
 *			<ul>
 *				<li><a class="topic" href="url-of-content-to-display">Topic 2</a></li>
 *				<li><a class="topic" href="url-of-content-to-display">Topic 3</a></li>
 *			</ul>
 *		</li>
 *	</ul>
 * 
 *	<div id="topicContentContainer"></div>
 */
Tawala.topicList = function() { }

Tawala.topicList.prototype = {
	topicList: "",
	id: "",
	topicListContainer: "topicList",
	contentContainer: "topicContentContainer",
	selectedTopic: "",
	selectedTopicSection: "",
	
	init: function() {
		var i, list, itemLink;
		
		this.topicList = this.topicListContainer.getElementsByTagName("li");
		
		for(i = 0; i < this.topicList.length; i++) {
			itemLink = this.topicList[i].getElementsByTagName("a")[0];
			if(YAHOO.util.Dom.hasClass(itemLink, "topicSection") ) {
				YAHOO.util.Event.addListener( itemLink, "click", this.toggleSection, this, true);
				list = itemLink.parentNode.getElementsByTagName("ul")[0];
				this.openSection(itemLink, list);
			}else if (YAHOO.util.Dom.hasClass(itemLink, "topic")) {
//				YAHOO.util.Event.addListener( itemLink, "click", this.loadContent, this, true)
			}
			
		}

		this.updateTopics();
	},
	
	toggleSection: function(e) {
		var targetName, targetList;
		
		YAHOO.util.Event.stopEvent(e);
		targetName = YAHOO.util.Event.getTarget(e);
		targetList = targetName.parentNode.getElementsByTagName("ul")[0];

		if(!targetList) {return};

		if(YAHOO.util.Dom.hasClass(targetName, "open")) {
			targetList.style.display = "none";
			YAHOO.util.Dom.removeClass(targetName, "open");
		}else{
			targetList.style.display = "block";
			YAHOO.util.Dom.addClass(targetName, "open");
		}
	},
	
	openSection: function(target, list) {
		if ( ! YAHOO.util.Dom.hasClass(target, "open")) {
			list.style.display = "block";
			YAHOO.util.Dom.addClass(target, "open");
		}
	},
	
	closeSection: function(target, list) {
		if (YAHOO.util.Dom.hasClass(target, "open")) {
			list.style.display = "none";
			YAHOO.util.Dom.removeClass(target, "open");
		}
	},
	
	loadContent: function(e) {			
		var targetURL, contentContainer, link, i;

		if( this.contentContainer === "" ) return;
	},
	
	updateTopics: function() {
		var topicLink, i;
		
		for(i = 0; i < this.topicList.length; i++) {
			topicLink = this.topicList[i].getElementsByTagName("a")[0];
			
			if (YAHOO.util.Dom.hasClass(topicLink, "selected")) {
				YAHOO.util.Dom.removeClass(topicLink, "selected")
			}

			if ( (topicLink === this.selectedTopic) || (topicLink.name === Tawala.config.pageName) ){		
				YAHOO.util.Dom.addClass(topicLink, "selected")
			}
		}
	}
			
}


Tawala.topicList.initTopicLists = function() {
	var tLists, i, topicList, topicLists = [];
	
	tLists =YAHOO.util.Dom.getElementsByClassName("topicList");
	for (i = 0; i < tLists.length; i++) {
		topicList = new Tawala.topicList();
		topicList.topicListContainer = tLists[i];
		topicList.contentContainer = "topicContentContainer";
		topicList.init();
		topicLists.push(topicList);
		if(tLists[i].id != "") {
			 topicList.id = tLists[i].id;
		}	
	}	
}

YAHOO.util.Event.on(window, "load", Tawala.topicList.initTopicLists);

	