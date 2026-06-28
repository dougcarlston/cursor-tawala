var getNews = function(){
	var sUrl = "/blog/tawalanews/feed/entries/atom";	
	var feedXML = {};

	if (YAHOO.env.ua.ie > 0) {
		feedXML = Tawala.XML.load(sUrl);
		var feed = new Tawala.Feed.Atom(feedXML);
		var container = $("tawalaNews");
		feed.renderEntries(container);
	} else {
		 function successHandler (o) {
		 feedXML = (o.responseXML);
		 
		 var feed = new Tawala.Feed.Atom(feedXML);
		 var container = $("tawalaNews");
		 feed.renderEntries(container);
		 }
		 
		 function failureHandler (o) {
		 alert("Failure\n\n" + o.statusText);
		 }
		 
		 // Initiate the HTTP GET request.
		 var request = YAHOO.util.Connect.asyncRequest('GET', sUrl, {success:successHandler, failure:failureHandler});
	}
}

// YAHOO.util.Event.onDOMReady(getNews)
