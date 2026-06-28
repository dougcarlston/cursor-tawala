function setMenuHighlight() {
	var menu;
	var menuLi = [];
	var menuLinks = [];
	
	menu = document.getElementById("communityMenu");

	var findLiMethod = function(e){ if(e.tagName.toLowerCase() == "li"){return true;}else{return false;}}	
	menuLi = $D.getChildrenBy(menu, findLiMethod);

	for (var i = 0; i < menuLi.length; i++) {
		menuLinks = menuLi[i].getElementsByTagName("a");
			
		for (var j = 0; j < menuLinks.length; j++) {
			if ((menuLinks[j].name == currentPage) && (!$D.hasClass(menuLinks[j], "selected"))) {
				$D.addClass(menuLinks[j], "selected");
			}
			else {
				if ($D.hasClass(menuLinks[j], "selected")) {
					$D.removeClass(menuLinks[j], "selected");
				}
			}
		}
	}
}

var getNews = function(){
	if (currentPage == "communityNews") {
		var sUrl = "/blog/communityNews/feed/entries/atom";
		var feedXML = {};
		if (YAHOO.env.ua.ie > 0) {
			feedXML = Tawala.XML.load(sUrl);
			var feed = new Tawala.Feed.Atom(feedXML);
			var container = $("communityNews");
			feed.renderEntries(container);
		}else {
			 function successHandler (o) {
			 feedXML = (o.responseXML.documentElement);
			 
			 var feed = new Tawala.Feed.Atom(feedXML);
			 var container = $("communityNews");
			 feed.renderEntries(container);
			 }
			 
			 function failureHandler (o) {
			 alert("Failure\n\n" + o.statusText);
			 }
			 
			 // Initiate the HTTP GET request.
			 var request = YAHOO.util.Connect.asyncRequest('GET', sUrl, {success:successHandler, failure:failureHandler});
		}
	}
}

YAHOO.util.Event.onDOMReady(getNews)
