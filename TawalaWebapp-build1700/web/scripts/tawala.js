var $D = YAHOO.util.Dom;
var $E = YAHOO.util.Event;
var $ = $D.get;

var Tawala = new function(){
	
    this.version = {
    	major: 0, minor: 1, patch: 0,
    	revision: Number("$Rev: 1321 $".match(/[0-9]+/)[0]),
    	toString: function() {
    		var v = this.version;
    		return v.major + "." + v.minor + "." + v.patch + " (" + v.revision + ")";
    	}
    }

    this.getDownloadPageBuildVersion = function(){
        var callback = {
                success: function(o) {
					var elem = document.getElementById("buildNumber");
					elem.innerHTML = o.responseXML.getElementsByTagName("buildNumber")[0].firstChild.nodeValue;
                },
        	    failure: function(o) { alert('Error getting build number: ' + o.status + ' -- ' + o.statusText); return null}
        	};

        var transaction = YAHOO.util.Connect.asyncRequest('GET', '/clientinfo', callback, null);
    }    
};

Tawala.config = {
	pageName: ""	
}

Tawala.constant = {
	months: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
	days: ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"],
	cssAttributes: {
		"background":"background",
		"background-attachment":"backgroundAttachment",
		"background-color":"backgroundColor",
		"background-image":"backgroundImage",
		"background-position":"backgroundPosition",
		"background-repeat":"backgroundRepeat",
		"border":"border",
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
		"cursor":"cursor",
		"display":"display",
		"filter":"filter",
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
		"overflow":"overflow",
		"padding":"padding",
		"padding-bottom":"paddingBottom",
		"padding-left":"paddingLeft",
		"padding-right":"paddingRight",
		"padding-top":"paddingTop",
		"page-break-after":"pageBreakAfter",
		"page-break-before":"pageBreakBefore",
		"position":"position",
		"float":"styleFloat",
		"text-align":"textAlign",
		"text-decoration":"textDecoration",
		"text-indent":"textIndent",
		"text-transform":"textTransform",
		"top":"top",
		"vertical-align":"verticalAlign",
		"visibility":"visibility",
		"width":"width",
		"z-index":"zIndex"
	}
		
}

Tawala.Tables = new function(){

    this.init = function() {
        // Find all tables with class sortable, stripe and rule
        if (!document.getElementsByTagName) return;
        tables = document.getElementsByTagName("table");
        for (ti=0;ti<tables.length;ti++) {
            thisTable = tables[ti];
            // find sortable
            if (YAHOO.util.Dom.hasClass(thisTable, "sortable") &&
            	(thisTable.getElementsByTagName("tbody")[0].getElementsByTagName("tr").length > 0)) {
                Tawala.Tables.Sort.makeSortable(thisTable);
            }
            // find stripe
            if (YAHOO.util.Dom.hasClass(thisTable, "stripe")) {
                Tawala.Tables.Stripe.addStripes(thisTable);
            }
            // find rule
            if (YAHOO.util.Dom.hasClass(thisTable, "ruler")) {
                Tawala.Tables.rule(thisTable);
            }
        }
    }

    this.rule = function(table) {
        var trs = table.getElementsByTagName('tr');
		for(var j = 0; j < trs.length; j++) {
		    if(trs[j].parentNode.nodeName.toLowerCase()=='tbody') {

			    var controls = YAHOO.util.Dom.getElementsByClassName("controls", "", trs[j]);
	
		    	if(controls.length > 0 && YAHOO.util.Dom.hasClass(controls[0], "hide")){
	 			    trs[j].onmouseover = function(){
							controls =  YAHOO.util.Dom.getElementsByClassName("controls", "", this);
	 			    		controls[0].style.display = "block";
	 			    		YAHOO.util.Dom.addClass(this, "ruled");
	 			    		return false;
	 			    	};
					trs[j].onmouseout = function(){
							controls =  YAHOO.util.Dom.getElementsByClassName("controls", "", this);
							controls[0].style.display = "none";
	 			    		YAHOO.util.Dom.removeClass(this, "ruled");
	 			    		return false;
						};
				}else{
	 			    trs[j].onmouseover=function(){YAHOO.util.Dom.addClass(this, "ruled");return false};
	 				trs[j].onmouseout=function(){YAHOO.util.Dom.removeClass(this, "ruled");return false};
				}
		    }
		    
		}	
    }
    
};

Tawala.Tables.Sort = new function(){
    var SORT_COLUMN_INDEX;

    this.init = function() {
        // Find all tables with class sortable and make them sortable
        if (!document.getElementsByTagName) return;
        tbls = document.getElementsByTagName("table");
        for (ti=0;ti<tbls.length;ti++) {
            thisTbl = tbls[ti];
            if (YAHOO.util.Dom.hasClass(thisTbl, "sortable")) {
                _makeSortable(thisTbl);
            }
        }
    }

    this.makeSortable = function(table) {
        headingRow =  table.getElementsByTagName("thead")[0].getElementsByTagName("tr");
        if(headingRow.length > 0) {
            var firstRow = headingRow[0];
        }
        if (!firstRow) return;

        for (var i=0;i<firstRow.cells.length;i++) {
            var cell = firstRow.cells[i];
            var txt = getInnerText(cell);

            cell.innerHTML = '<a href="" class="sortheader" onclick="Tawala.Tables.Sort.resortTable(this);return false;" title="Click to sort by this column">'+txt+'<span class="sortarrow"></span></a>';
        }
    }

    this.resortTable = function(lnk) {
        // get the span
        var span;
        for (var ci=0;ci<lnk.childNodes.length;ci++) {
            if (lnk.childNodes[ci].tagName && lnk.childNodes[ci].tagName.toLowerCase() == 'span') span = lnk.childNodes[ci];
        }

        var td = lnk.parentNode;
        var column = td.cellIndex;
        var table = getParent(td,'table');

        // Work out a type for the column
        if (table.rows.length <= 1) return;
        tbodies = table.getElementsByTagName("tbody");
        if(tbodies && tbodies.length > 0) {
            for(var tb=0; tb < tbodies.length; tb++) {
                tbody = tbodies[tb];
                var itm = trim(getInnerText(tbody.rows[0].cells[column]));
                sortfn = _sort_caseinsensitive;
                if (itm.match(/^\d{1,2}[\/-]\d{1,2}[\/-]\d{2,4}$/)) sortfn = _sort_date;
                if (itm.match(/^\d\d[\/-]\d\d[\/-]\d\d$/)) sortfn = _sort_date;
                if (itm.match(/^[?$]/)) sortfn = _sort_currency;
                if (itm.match(/^[\d\.]+$/)) sortfn = _sort_numeric;
                if(tbody.rows[0].cells[column].getElementsByTagName("IMG").length > 0){
                	var images = tbody.rows[0].cells[column].getElementsByTagName("IMG");
                	if(images[0].alt.toLowerCase() == "published"){
                		sortfn = _sort_published;
                	}else{
                		sortfn = _sort_rating;
                	}
                }
				
                SORT_COLUMN_INDEX = column;
    			
//    			alert("td =" + td + "\ncolumn =" + column + "\nitm =" + itm + "\n\nsortfn = " + sortfn);
    			
                var firstRow = new Array();
                var newRows = new Array();
                for (i=0;i<tbody.rows[0].length;i++) { firstRow[i] = tbody.rows[0][i]; }
                for (j=0;j<tbody.rows.length;j++) { newRows[j] = tbody.rows[j]; }
    
                newRows.sort(sortfn);
    
                if (span.getAttribute("sortdir") == 'down') {
                    ARROW = '&nbsp;&nbsp;&darr;';
                    newRows.reverse();
                    span.setAttribute('sortdir','up');
                } else {
                    ARROW = '&nbsp;&nbsp;&uarr;';
                    span.setAttribute('sortdir','down');
                }
    
                // We appendChild rows that already exist to the tbody, so it moves them rather than creating new ones
                // don't do sortbottom rows
                for (i=0;i<newRows.length;i++) { if (!newRows[i].className || (newRows[i].className && (newRows[i].className.indexOf('sortbottom') == -1))) tbody.appendChild(newRows[i]);}
                // do sortbottom rows only
                for (i=0;i<newRows.length;i++) { if (newRows[i].className && (newRows[i].className.indexOf('sortbottom') != -1)) tbody.appendChild(newRows[i]);}

               	if(YAHOO.util.Dom.hasClass(table, "stripe")) {
               		Tawala.Tables.Stripe.addStripes(table);
               	}
    
                // Delete any other arrows there may be showing
                var allspans = document.getElementsByTagName("span");
                for (var ci=0; ci<allspans.length; ci++) {
                    if (allspans[ci].className == 'sortarrow') {
                        if (getParent(allspans[ci],"table") == getParent(lnk,"table")) { // in the same table as us?
                            allspans[ci].innerHTML = '&nbsp;&nbsp;&nbsp;';
                        }
                    }
                }
                span.innerHTML = ARROW;    
            }
        }
    }

    //
    //Column Sorting functions
    //
    function _sort_date(a,b) {

        // y2k notes: two digit years less than 50 are treated as 20XX, greater than 50 are treated as 19XX
        dt1 = _parseDate(getInnerText(a.cells[SORT_COLUMN_INDEX]));
        dt2 = _parseDate(getInnerText(b.cells[SORT_COLUMN_INDEX]));

        if (dt1==dt2) return 0;
        if (dt2<dt1) return -1;
        return 1;
    }

    function _parseDate(date) {
        if(date.indexOf("/") > 0){
            da = date.split("/");
            if(da[0].length == 1) { da[0] = "0"+da[0];}
            if(da[1].length == 1) { da[1] = "0"+da[1];}
            if(da[2].length < 4){
                if (parseInt(da[2]) < 50) { da[2] = '20'+da[2]; } else { da[2] = '19'+da[2]; }
            }
            dc = da[2]+da[0]+da[1];
        }else{
            if(date.indexOf("-") > 0){
                da = date.split("-");
                if(da[0].length == 1) { da[0] = "0"+da[0];}
                if(da[1].length == 1) { da[1] = "0"+da[1];}
                if(da[2].length < 4){
                    if (parseInt(da[2]) < 50) { da[2] = '20'+da[2]; } else { da[2] = '19'+da[2]; }
                }
                dc = da[2]+da[0]+da[1];
            }
        }
        return dc;
    }

    function _sort_currency(a,b) {
        aa = getInnerText(a.cells[SORT_COLUMN_INDEX]).replace(/[^0-9.]/g,'');
        bb = getInnerText(b.cells[SORT_COLUMN_INDEX]).replace(/[^0-9.]/g,'');
        return parseFloat(bb) - parseFloat(aa);
    }

    function _sort_numeric(a,b) {
        aa = parseFloat(getInnerText(a.cells[SORT_COLUMN_INDEX]));
        if (isNaN(aa)) aa = 0;
        bb = parseFloat(getInnerText(b.cells[SORT_COLUMN_INDEX]));
        if (isNaN(bb)) bb = 0;
        return bb-aa;
    }

	// value is in the alt atribute of the rating image
    function _sort_rating(a,b) {
    	if(! a.cells[SORT_COLUMN_INDEX].getElementsByTagName("IMG")[0] ||
	    	! b.cells[SORT_COLUMN_INDEX].getElementsByTagName("IMG")[0]){return 0;}
        aa = parseFloat(a.cells[SORT_COLUMN_INDEX].getElementsByTagName("IMG")[0].alt);
        bb = parseFloat(b.cells[SORT_COLUMN_INDEX].getElementsByTagName("IMG")[0].alt);
        if (isNaN(aa)) aa = 0;
        if (isNaN(bb)) bb = 0;
        return bb-aa;
    }

	function _sort_published(a,b){
		var aa = 0;
		var bb = 0;
    	if(a.cells[SORT_COLUMN_INDEX].getElementsByTagName("IMG")[0]) aa = 1;
	    if(b.cells[SORT_COLUMN_INDEX].getElementsByTagName("IMG")[0]) bb = 1;
		return aa-bb;
	}
	
    function _sort_caseinsensitive(a,b) {
        aa = getInnerText(a.cells[SORT_COLUMN_INDEX]).toLowerCase();
        bb = getInnerText(b.cells[SORT_COLUMN_INDEX]).toLowerCase();
        if (aa==bb) return 0;
        if (aa<bb) return -1;
        return 1;
    }

    function _sort_default(a,b) {
        aa = getInnerText(a.cells[SORT_COLUMN_INDEX]);
        bb = getInnerText(b.cells[SORT_COLUMN_INDEX]);
        if (aa==bb) return 0;
        if (aa<bb) return -1;
        return 1;
    }
} // end Tawala.Tables.Sort


//
// Modified from A List Apart article
//
Tawala.Tables.Stripe = new function() {

    this.getTables = function() {
        // Find all tables with class stripe and add stripes
        if (!document.getElementsByTagName) return;
        tbls = document.getElementsByTagName("table");
        for (ti=0;ti<tbls.length;ti++) {
            thisTbl = tbls[ti];
            if (((' '+thisTbl.className+' ').indexOf("stripe") != -1) && (thisTbl.id)) {
                Tawala.Tables.Stripe.addStripes(thisTbl);
            }
        }
    }

    this.addStripes = function(table) {
        if (!table) { return; }

        var tbodies = table.getElementsByTagName("tbody");
        for (var h = 0; h < tbodies.length; h++) {
		    var even = false;
            var trs = tbodies[h].getElementsByTagName("tr");
            for (var i = 0; i < trs.length; i++) {
                if(YAHOO.util.Dom.hasClass(trs[i], "oddColor")) { YAHOO.util.Dom.removeClass(trs[i], "oddColor") };
                if(YAHOO.util.Dom.hasClass(trs[i], "evenColor")) { YAHOO.util.Dom.removeClass(trs[i], "evenColor") };
                if(even) {
                     YAHOO.util.Dom.addClass(trs[i], "evenColor");
                }else{
                    YAHOO.util.Dom.addClass(trs[i], "oddColor");
                }
                even =  ! even;
            }
        }
    }
} // end Tawala.Tables.Stripe


Tawala.Block = new function() {
	var blockContainer;
	var blockHeading;
	var blockContent;
	
    this.init = function() {
        var collapsibleSections = YAHOO.util.Dom.getElementsByClassName("collapsible", "div");
        
		// Check the page for sections marked collapsible
        for(var i=0; i < collapsibleSections.length; i++){
            blockHeading = YAHOO.util.Dom.getElementsByClassName("sectionHeading", "", collapsibleSections[i])[0];
            blockHeading.style.paddingLeft= ".8em";
            blockHeading.setAttribute("title", "Click title bar to hide or view");
			blockHeading.style.cursor = "pointer";
            YAHOO.util.Dom.addClass(blockHeading, "arrowDown");
            YAHOO.util.Event.addListener(blockHeading, "click", Tawala.Block.toggleVisible, Tawala.Block);
			blockContainer = blockHeading.parentNode;
	        blockContent = YAHOO.util.Dom.getElementsByClassName("sectionContent", "div", blockContainer)[0];
			
			// If a  section container is marked with class 'closed' we initially hide that section
			if(YAHOO.util.Dom.hasClass(blockContainer, "closed")){
				YAHOO.util.Dom.removeClass(blockContainer, "closed");
				hideBlock();
			}
        }
    }

    this.toggleVisible = function(e){
        if(e.srcElement){
            blockHeading = e.srcElement;
        }else{
            blockHeading = e.target;
        }

		blockContainer = blockHeading.parentNode;
        blockContent = YAHOO.util.Dom.getElementsByClassName("sectionContent", "div", blockContainer)[0];
        if(YAHOO.util.Dom.hasClass(blockContent, "closed")) {
			showBlock();
        }else{
			hideBlock();
		}
    }
	
	showBlock = function() {
		YAHOO.util.Dom.removeClass(blockContent, "closed");		
		toggleArrow();
	}
	
	hideBlock = function() {
		YAHOO.util.Dom.addClass(blockContent, "closed");
		toggleArrow();
	}
	
    toggleArrow = function() {
        if(YAHOO.util.Dom.hasClass(blockHeading, "arrowDown")){
            YAHOO.util.Dom.replaceClass(blockHeading, "arrowDown", "arrowRight");
        }else{
            YAHOO.util.Dom.replaceClass(blockHeading, "arrowRight", "arrowDown");
        }
    }
}

// end of Tawala.Block


Tawala.XML = function(){
	return {
		newDocument: function(rootTagName, namespaceURL) {
		    if (!rootTagName) rootTagName = "";
		    if (!namespaceURL) namespaceURL = "";

		    if (document.implementation && document.implementation.createDocument) {
		        // This is the W3C standard way to do it
		        return document.implementation.createDocument(namespaceURL, rootTagName, null);
		    } else { // This is the IE way to do it
		        // Create an empty document as an ActiveX object
		        // If there is no root element, this is all we have to do
		        var doc = new ActiveXObject("MSXML2.DOMDocument");

		        // If there is a root tag, initialize the document
		        if (rootTagName) {
		            // Look for a namespace prefix
		            var prefix = "";
		            var tagname = rootTagName;
		            var p = rootTagName.indexOf(':');
		            if (p != -1) {
		                prefix = rootTagName.substring(0, p);
		                tagname = rootTagName.substring(p+1);
		            }

		            // If we have a namespace, we must have a namespace prefix
		            // If we don't have a namespace, we discard any prefix
		            if (namespaceURL) {
		                if (!prefix) prefix = "a0"; // What Firefox uses
		            }
		            else prefix = "";

		            // Create the root element (with optional namespace) as a
		            // string of text
		            var text = "<" + (prefix?(prefix+":"):"") + tagname +
		                (namespaceURL
		                 ?(" xmlns:" + prefix + '="' + namespaceURL +'"')
		                 :"") +
		                "/>";
		            // And parse that text into the empty document
		            doc.loadXML(text);
		        }
		        return doc;
		    }
		},

		load: function(url) {
		    // Create a new document with the previously defined function
			try{
			    var xmldoc = this.newDocument();
			    xmldoc.async = false;  // We want to load synchronously
			    xmldoc.load(url);      // Load and parse
			    return xmldoc;         // Return the document				
			}catch(e){
//				alert("XML Load failed:\n" + e);				
			}
		}
	}
}()

Tawala.Feed = {};

Tawala.Feed.Atom = function(feedXML) { 
	if(feedXML){
		this.feed = feedXML
		this.parse();
	}
}

Tawala.Feed.Atom.prototype = { 
	container: "",
	feed: "",
	
	//Feed elements
	//required
	id: "",
	title: "",
	updated: "",
	//recommended
	author: "",
	link: {},
	//optional
	category: [],
	contributor: "",
	generator: "",
	icon: "",
	logo: "",
	rights: "",
	subtitle: "",
	
	entries: [],
	
	parse: function(){
		this.title = this.feed.getElementsByTagName("title")[0].firstChild.nodeValue;

		if(this.feed.getElementsByTagName("subtitle")[0].childNodes.length != 0) {
			this.subtitle = this.feed.getElementsByTagName("subtitle")[0].firstChild.data;
		}

		if(this.feed.getElementsByTagName("link")){
			this.link = this.parseLinks();
		}

		var entriesXML = this.feed.getElementsByTagName("entry");
		for(var i = 0; i < entriesXML.length; i++){
			var entry = new Tawala.Feed.Atom.entry(entriesXML[i]);
			this.entries.push(entry);
		}
	},
	
	parseLinks: function() {
		// if there's a link with the alternate attribute return that otherwise return the first link
		var links = this.feed.getElementsByTagName("link");
		var link = "";
		for(var i = 0; i < links.length; i++) {
			if(links[i].getAttribute("rel") == "alternate") {
				return (links[i].getAttribute("href"));
			}
		}
		return (links[0].getAttribute("href"));
	},
	
	renderHeading: function(containerId) {
		if (containerId) {
			this.container = containerId;
		}
		var feedHeading = document.createElement("div");
		var ft = document.createElement("h2");
		var fst = document.createElement("div");
		var ftLink = document.createElement("a");
		ftLink.href = this.link;
		$D.addClass(ftLink, "title");
		ftLink.innerHTML = this.title;
		ft.appendChild(ftLink);
		if(this.subtitle){
			fst.innerHTML = this.subtitle;
			$D.addClass(fst, "subtitle");
		}
		feedHeading.appendChild(ft);
		feedHeading.appendChild(fst);
		this.container.appendChild(feedHeading);
	},
	
	renderEntries: function(containerId, numEntries) {
		var c = 0;
		
		if (containerId) {
			this.container = containerId;
		}
		var entryList = document.createElement("ul");
		if (numEntries && numEntries > 0 && numEntries < this.entries.length) {
			c = numEntries;			
		} else {
			c = this.entries.length;
		}

		for (var i = 0; i < c; i++) {
			var entryHTML = this.entries[i].render();
			entryList.appendChild(entryHTML);
		}
		
		this.container.appendChild(entryList);
	},
	
	getTitle: function() {
		return(this.title);
	},
	
	getSubtitle: function() {
		return(this.subtitle);
	},
	
	getLink: function() {
		return(this.link);
	}
}

Tawala.Feed.Atom.entry = function(entryXML) {
	if(entryXML) {
		this.parse(entryXML);
	}
}

Tawala.Feed.Atom.entry.prototype = { 
	//Entry elements
	//required
	id: "",
	title: "",
	updated: "",
	//recommended
	author: "",
	content: "",
	link: [],
	summary: "",
	//optional
	category: [],
	contributor: "",
	published: "",
	rights: "",
	
	parse: function(entry) {					
		this.title = entry.getElementsByTagName("title")[0].firstChild.nodeValue;
		this.id = entry.getElementsByTagName("id")[0].firstChild.nodeValue;
		this.author = entry.getElementsByTagName("author")[0].getElementsByTagName('name')[0].firstChild.nodeValue;
		this.category = entry.getElementsByTagName("category")[0].getAttribute('label');
		this.published = entry.getElementsByTagName("published")[0].firstChild.nodeValue;
		this.updated = entry.getElementsByTagName("updated")[0].firstChild.nodeValue;
		if(entry.getElementsByTagName("summary").length > 0){
			this.summary = entry.getElementsByTagName("summary")[0].firstChild.nodeValue;
		}
		if(entry.getElementsByTagName("content")){
			this.content = entry.getElementsByTagName("content")[0].firstChild.nodeValue;
		}
	},
	
	render: function() {
		// article title
		var item = document.createElement("li");
		var itemTitle = document.createElement("h3");
		var itemLink = document.createElement("a");
		itemLink.href = this.id;
		itemLink.innerHTML = this.title;
		itemTitle.appendChild(itemLink);
		item.appendChild(itemTitle);
		
		// article info
		var desc = document.createElement("div");
		desc.className = "description";

		var pdate = this.parseDate(this.updated);
		desc.innerHTML = Tawala.constant.months[pdate.getMonth()] + " " + pdate.getDate() + ", " + pdate.getFullYear() + 
			" at " + pdate.getHours() + ":" + pdate.getMinutes() + " " +
			" by " + this.author;
		item.appendChild(desc);
		
		// article contents
		var itemContent = document.createElement("div");
		itemContent.innerHTML = this.content;
		item.appendChild(itemContent);
/*
		var imore = document.createElement("div");
		imore.className = "readMore";
		imore.innerHTML = "[<a href='" + this.id + "'>Read More / Add Comment</a>]";
		item.appendChild(imore);
*/		
		return(item);
	},
	
	// Parse the blog date/time to the standard date/time format
	parseDate: function(dString) {
		var d = new Date();
		d.setYear(Number(dString.slice(0,4)));
		d.setMonth(Number(dString.slice(5,7)) - 1);
		d.setDate( Number(dString.slice(8,10)));

		d.setHours( Number(dString.slice(11,13)),  Number(dString.slice(14,16)),  Number(dString.slice(17,19)))
		return (d);
	}
			}

/* ---------------------------------------------------------
 * Global utility functions
 */

/*
 * this function is needed to work around
 * a bug in IE related to element attributes
 */
function hasClass(obj) {
var result = false;
    if (obj.getAttributeNode("class") != null) {
       result = obj.getAttributeNode("class").value;
    }
    return result;
}

/*
 * Return the inner text of an element
 */
function getInnerText(el) {
	if (typeof el == "string") return el;
	if (typeof el == "undefined") { return el };
	if (el.innerText) return el.innerText;
	var str = "";

	var cs = el.childNodes;
	var l = cs.length;
	for (var i = 0; i < l; i++) {
		switch (cs[i].nodeType) {
			case 1: //ELEMENT_NODE
				str += getInnerText(cs[i]);
				break;
			case 3:	//TEXT_NODE
				str += cs[i].nodeValue;
				break;
		}
	}
	return str;
}

/*
 * Test whether a string is a member of an array
 */
function isMember(str, arr){
    for(var i = 0; i < arr.length; i++) {
        if (str == arr[i]) { return (true) }
    }
    return(false);
}

/*
 *  Given an element get the parent of type "TagName"
 */
function getParent(el, pTagName) {
	if (el == null) return null;
	else if (el.nodeType == 1 && el.tagName.toLowerCase() == pTagName.toLowerCase())	// Gecko bug, supposed to be uppercase
		return el;
	else
		return getParent(el.parentNode, pTagName);
}

/*
 * Create an HTML element
 */
function createHtmlElement ( elemName, attribs, txt ){
	if ( typeof document.createElement == 'undefined' ) return;
	var elem = document.createElement( elemName );
	if ( typeof attribs != 'undefined' ) {
		for ( var i in attribs ) {
			switch ( true ) {
				case ( i == 'class' ) : elem.className = attribs[i]; break;
				default : elem.setAttribute( i, attribs[i] );
			}
		}
	}

    if( txt && txt != '') {
        elem.appendChild(document.createTextNode( txt ));
    }
	return elem;
}

/*
 * Swap two nodes in the document tree
 */
function swapNodes(node1Id, node2Id){
    var node1 = new Array();
    var node2 = new Array();
    var placeHolder = document.createElement(document.getElementById(node1Id).tagName);
    placeHolder.id = "placeHolder";
    node1["parent"] = document.getElementById(node1Id).parentNode;
    node2["parent"] = document.getElementById(node2Id).parentNode;

    node1["element"] = node1["parent"].replaceChild(placeHolder, document.getElementById(node1Id));
    node2["element"] = node2["parent"].replaceChild(node1["element"], document.getElementById(node2Id));
    node1["parent"].replaceChild(node2["element"], placeHolder);
    return false;
}

function LTrim( value ) {
	var re = /\s*((\S+\s*)*)/;
	return value.replace(re, "$1");
}

function RTrim( value ) {
	var re = /((\s*\S+)*)\s*/;
	return value.replace(re, "$1");
}

function trim( value ) {
	return LTrim(RTrim(value));
}

function getEventTarget(evt) {
    var e = evt || window.event;
    if(e.target) {
        target = e.target;
    }else{
        if(e.srcElement) target = e.srcElement;
    }
    return target;
}

function decodeHtml(t) {
	t = t.replace(/&quot;/g,"\"");
	t = t.replace(/&lt;/g,"<");
	t = t.replace(/&gt;/g,">");
	return(t);
}
			
function getScrollXY() {
	var scrOfX = 0, scrOfY = 0;
	if( typeof( window.pageYOffset ) == 'number' ) {
		//Netscape compliant
		scrOfY = window.pageYOffset;
		scrOfX = window.pageXOffset;
	} else if( document.body && ( document.body.scrollLeft || document.body.scrollTop ) ) {
		//DOM compliant
		scrOfY = document.body.scrollTop;
		scrOfX = document.body.scrollLeft;
	} else if( document.documentElement && ( document.documentElement.scrollLeft || document.documentElement.scrollTop ) ) {
		//IE6 standards compliant mode
		scrOfY = document.documentElement.scrollTop;
		scrOfX = document.documentElement.scrollLeft;
	}
	return [ scrOfX, scrOfY ];
}

function setPageTitle(title) {
	var title = document.createTextNode(title);
	document.getElementById("pageTitle").appendChild(title);
}

/*
	* Sugar Arrays (c) Creative Commons 2006
	* http://creativecommons.org/licenses/by-sa/2.5/
	* Author: Dustin Diaz | http://www.dustindiaz.com
	* Reference: http://www.dustindiaz.com/basement/sugar-arrays.html
*/
Function.prototype.method = function (name, fn) {
	this.prototype[name] = fn;
	return this;
};

if ( !Array.prototype.forEach ) {
	Array.
		method(
			'forEach',
			function(fn, thisObj) {
				var scope = thisObj || window;
				for ( var i=0, j=this.length; i < j; ++i ) {
					fn.call(scope, this[i], i, this);
				}
			}
		).
		method(
			'every',
			function(fn, thisObj) {
				var scope = thisObj || window;
				for ( var i=0, j=this.length; i < j; ++i ) {
					if ( !fn.call(scope, this[i], i, this) ) {
						return false;
					}
				}
				return true;
			}
		).
		method(
			'some',
			function(fn, thisObj) {
			    var scope = thisObj || window;
				for ( var i=0, j=this.length; i < j; ++i ) {
			        if ( fn.call(scope, this[i], i, this) ) {
			            return true;
			        }
			    }
			    return false;
			}
		).
		method(
			'map',
			function(fn, thisObj) {
			    var scope = thisObj || window;
			    var a = [];
			    for ( var i=0, j=this.length; i < j; ++i ) {
			        a.push(fn.call(scope, this[i], i, this));
			    }
			    return a;
			}
		).
		method(
			'filter',
			function(fn, thisObj) {
			    var scope = thisObj || window;
			    var a = [];
			    for ( var i=0, j=this.length; i < j; ++i ) {
			        if ( !fn.call(scope, this[i], i, this) ) {
			            continue;
			        }
			        a.push(this[i]);
			    }
			    return a;
			}
		).
		method(
			'indexOf',
			function(el, start) {
			    var start = start || 0;
			    for ( var i=start, j=this.length; i < j; ++i ) {
			        if ( this[i] === el ) {
			            return i;
			        }
			    }
			    return -1;
			}
		).
		method(
			'lastIndexOf',
			function(el, start) {
			    var start = start || this.length;
			    if ( start >= this.length ) {
			        start = this.length;
			    }
			    if ( start < 0 ) {
			         start = this.length + start;
			    }
			    for ( var i=start; i >= 0; --i ) {
			        if ( this[i] === el ) {
			            return i;
			        }
			    }
			    return -1;
			}
		);
}

/* ----------------------------------------------------------------------------
 * Add events to the page
 */
YAHOO.util.Event.addListener(window, "load", Tawala.Tables.init);
YAHOO.util.Event.addListener(window, "load", Tawala.Block.init);
