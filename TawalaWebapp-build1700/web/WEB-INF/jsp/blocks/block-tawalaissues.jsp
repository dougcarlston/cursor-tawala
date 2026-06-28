<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>

	<div class="block">
		<script type="text/javascript">
			function getMantisIssuesRSS(){
				setStatus("Loading...");
				var maxIssues = "5";
				var callback = {
					success: function(o) {
						var target = document.getElementById("mantisListRSS");
						var channel = o.responseXML.getElementsByTagName("channel")[0];
						var description = decodeHtml(channel.getElementsByTagName("description")[0].firstChild.nodeValue);
						var link = channel.getElementsByTagName("link")[0].firstChild.nodeValue;
						var items = channel.getElementsByTagName("item");
						var rssDiv = createHtmlElement("div", {"class":"bugList", "style":"padding-top:4px; padding-bottom:4px; overflow: hidden;"});
		//				  var channelTitle = createHtmlElement("div");
		//				  channelTitle.appendChild(createHtmlElement("a", {"href":link, "target":"_blank"}, description));
		//				  rssDiv.appendChild(channelTitle);
		
						for(var i = 0; (i < items.length) && (i < maxIssues); i++){
							var itemDiv = createHtmlElement("div", {"style":"overflow:hidden; overflow-x: hidden;"});
							var itemLink = items[i].getElementsByTagName("link")[0].firstChild.nodeValue;
							if(itemLink.length){
								var itemTitle = decodeHtml(items[i].getElementsByTagName("title")[0].firstChild.nodeValue);
								var bugNum = itemTitle.slice(0,8);
								var bugTitle = itemTitle.slice(9);
								itemDiv.appendChild(createHtmlElement("a", {"href":itemLink, "target":"_blank"}, bugNum));
								itemDiv.appendChild(createHtmlElement("span",{"style":"padding-left:.3em;"}, bugTitle));
/*
								if(i % 2 == 0){
									YAHOO.util.Dom.addClass(itemDiv, "evenColor");
								}else{
									YAHOO.util.Dom.addClass(itemDiv, "oddColor");
								}
*/								
								rssDiv.appendChild(itemDiv);									
							}
						}
						target.appendChild(rssDiv);
						hideStatus();
					},
		
					failure: function(o) {
						var errorMsg = 'Problem reading the Mantis RSS feed: ' + o.status + ' -- ' + o.statusText;
						var target = document.getElementById("mantisListRSS");
						var rssDiv = createHtmlElement("div",{"class":"error"}, errorMsg);
//						target.appendChild(rssDiv);
						hideStatus();
					}
				};

				function setStatus (message){
					if(document.getElementById('ajaxStatus') == null){
//						var body = document.getElementsByTagName("body")[0];
						var rssList = document.getElementById("mantisListRSS");
						var div = document.createElement("div");
						div.id = 'ajaxStatus';
						rssList.appendChild(div);
					}
					
					var node = document.getElementById('ajaxStatus');
					node.innerHTML = message;
					node.style.display = "block";
				}
				
				function hideStatus(){
					var node = document.getElementById('ajaxStatus');
					node.style.display = "none";
				}

				function handleException(e) {
					var errorMsg = "Sorry, there appears to be a problem reading the Mantis RSS feed." + e;
					var target = document.getElementById("mantisListRSS");
					var rssDiv = createHtmlElement("div",{"class":"error"}, errorMsg);
					target.appendChild(rssDiv);
//					hideStatus();
				}
				
				try{				
				  	var transaction = YAHOO.util.Connect.asyncRequest('GET', '/mantis/issues_rss.php', callback, null);
			  	}catch(e){
			  		handleException(e);
			  	}
			}
			
			YAHOO.util.Event.addListener(window, "load", getMantisIssuesRSS);

		</script>

		<h3>Known Bugs</h3>
		<div id="mantisListRSS" class="content"></div>
		<a href="http://mantis.tawala.com" title="Click here to go to the Tawala bug tracking system" target="_blank">
			<img src="/images/template/arrowRight.gif" width="8" height="11"/> View the bugbase
		</a>
	</div>
