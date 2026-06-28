/*
 * Sports FAQ scripts
 */


/*
 *	Topic List
 *	
 *	HTML markup example:
 *	<ul id="topicList" class="topicList">
 *		<li><a class="topic" name="topicName" href="url-of-content-to-display">Topic 1</a></li>
 *		<li>
 *			<a class="topicSection" name="topicName" href="#">Section 1</a>
 *			<ul>
 *				<li><a class="topic" name="topicName" href="url-of-content-to-display">Topic 2</a></li>
 *				<li><a class="topic" name="topicName" href="url-of-content-to-display">Topic 3</a></li>
 *			</ul>
 *		</li>
 *	</ul>
 * 
 *	<div id="topicContentContainer"></div>
 */

Tawala.topicList = function() {	
}

Tawala.topicList.prototype = {
	topicList: "",
	sectionList: {},
	topicListContainer: "topicList",
	contentContainer: "topicContentContainer",
	selectedTopic: "",
	selectedTopicSection: "",
	
	init: function() {
		var i, itemLink;

		this.selectedTopic = Tawala.config.pageName;
		this.topicList = this.topicListContainer.getElementsByTagName("li");
		
		for(i = 0; i < this.topicList.length; i++) {
			itemLink = this.topicList[i].getElementsByTagName("a")[0];
			if(itemLink.rel) {
				this.sectionList[itemLink.rel] = itemLink;
			}
			if(YAHOO.util.Dom.hasClass(itemLink, "topicSection") ) {
				YAHOO.util.Event.addListener( itemLink, "click", this.toggleSection, this, true);
				list = itemLink.parentNode.getElementsByTagName("ul")[0];
				this.openSection(itemLink, list);
			}else if (YAHOO.util.Dom.hasClass(itemLink, "topic")) {
				if(itemLink.name === this.selectedTopic){
					YAHOO.util.Dom.addClass(itemLink, "selected")					
				}else{
					YAHOO.util.Dom.removeClass(itemLink, "selected")					
				}
			}
			
		}
	},
	
	toggleSection: function(e) {
		var targetName, targetList;
		
		YAHOO.util.Event.stopEvent(e);
		targetName = YAHOO.util.Event.getTarget(e);
		targetList = targetName.parentNode.getElementsByTagName("ul")[0];

		if(!targetList) {return};

		if(YAHOO.util.Dom.hasClass(targetName, "open")) {
			this.closeSection(targetName, targetList);
		}else{
			this.openSection(targetName, targetList);
		}
	},
	
	openSection: function(target, list) {
		list.style.display = "block";
		YAHOO.util.Dom.addClass(target, "open");
	},
	
	closeSection: function(target, list) {
		list.style.display = "none";
		YAHOO.util.Dom.removeClass(target, "open");
	}
}


Tawala.topicList.initTopicLists = function() {
	var tLists, i, topicList;
	
	tLists =YAHOO.util.Dom.getElementsByClassName("topicList");
	for (i = 0; i < tLists.length; i++) {
		topicList = new Tawala.topicList();
		topicList.topicListContainer = tLists[i];
		topicList.init();
	}
}

YAHOO.util.Event.on(window, "load", Tawala.topicList.initTopicLists);

