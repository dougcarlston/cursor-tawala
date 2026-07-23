/*
 * Global variables
 */
var Dom = YAHOO.util.Dom;
var Event = YAHOO.util.Event;
var DDM = YAHOO.util.DragDropMgr;
// var $ = Dom.get;

//--- Taken from http://www.webcheatsheet.com/javascript/disable_enter_key.php
function stopRKey(evt) {
  var evt = (evt) ? evt : ((event) ? event : null);
  var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
  if ((evt.keyCode == 13) && (node.type=="text"))  {return false;}
}

document.onkeypress = stopRKey;

/*********************************************
 * Tawala project code
 */

var Tawala = new function(){ 
};

Tawala.fixTemplates = function(){
	var a = document.getElementsByTagName("link");
 	for(var i=0; i < a.length; i++) {
		if(a[i].getAttribute("rel").indexOf("style") != -1 && a[i].getAttribute("title")) {
	    	if(a[i].getAttribute("title") == "template") {				
				a[i].disabled = true;
				a[i].disabled = false;
			}
		}
	}
};

/*
 * Fix to make sure tables don't extend beyond the width of the container
 */
Tawala.fixTableWidth = function(){
	var cw = Dom.get("tawalaProjectContainer").clientWidth;
	var comp = Dom.getElementsByClassName("component");
	for (var i in comp) {		
		if (comp[i].tagName.toLowerCase() == "table" && !Dom.hasClass(comp[i],"noFixWidth") ) { 
			if (comp[i].clientWidth > cw &&  !Dom.hasClass(getParent(comp[i], "div"), "tawalaDataTable") ) {
				getParent(comp[i], "div").style.width = "99%";
				getParent(comp[i], "div").style.overflow = "scroll";
			} else {
				getParent(comp[i], "div").style.overflow = "visible";
			}
		}
	}
};

Tawala.Tables = new function(){
	
    this.init = function() {
    	// Set the window name so we can reference it later if needed
    	window.name = "TawalaProjectWindow";
    	
        // Find all tables with class sortable, stripe and rule
        if (!document.getElementsByTagName) return;
        tables = document.getElementsByTagName("table");
        for (var ti=0;ti<tables.length;ti++) {
            thisTable = tables[ti];
            // find sortable
            if (Dom.hasClass(thisTable, "sortable") &&
            	(thisTable.getElementsByTagName("tbody")[0].getElementsByTagName("tr").length > 0)) {
                Tawala.Tables.Sort.makeSortable(thisTable);
            }
            // find stripe
            if (Dom.hasClass(thisTable, "stripe")) {
                Tawala.Tables.Stripe.addStripes(thisTable);
            }
            // find rule
            if (Dom.hasClass(thisTable, "ruler")) {
                Tawala.Tables.rule(thisTable);
            }
            // find limitHeight
            if (Dom.hasClass(thisTable, "limitHeight")) {
                Tawala.Tables.limitHeight(thisTable);
            }
        }
    };

    this.rule = function(table) {
        var trs = table.getElementsByTagName('tr');
		for(var j = 0; j < trs.length; j++) {
		    if(trs[j].parentNode.nodeName.toLowerCase()=='tbody') {

			    var controls = Dom.getElementsByClassName("controls", "", trs[j]);
	
		    	if(controls.length > 0 && Dom.hasClass(controls[0], "hide")){
	 			    trs[j].onmouseover = function(){
							controls =  Dom.getElementsByClassName("controls", "", this);
	 			    		controls[0].style.display = "block";
	 			    		Dom.addClass(this, "ruled");
	 			    		return false;
	 			    	};
					trs[j].onmouseout = function(){
							controls =  Dom.getElementsByClassName("controls", "", this);
							controls[0].style.display = "none";
	 			    		Dom.removeClass(this, "ruled");
	 			    		return false;
						};
				}else{
	 			    trs[j].onmouseover=function(){Dom.addClass(this, "ruled");return false};
	 				trs[j].onmouseout=function(){Dom.removeClass(this, "ruled");return false};
				}
		    }
		    
		}	
    };

	this.limitHeight = function(table) {
		var maxRows = 20;
		var container = table.parentNode;
		if(table.rows.length > maxRows) {
			container.style.overflow = "scroll";
			var tb = table.getElementsByTagName("tbody");
			var trHeight = tb[0].rows[0].offsetHeight + 1;
			var newHeight = maxRows  * trHeight;
			container.style.height = newHeight.toString() + "px";
		}else{
			container.style.height = "auto";			
		}
	};   
    
};

Tawala.Tables.Sort = new function(){
	var headingRow;
	var columnIndex;

	this.cookie_table_idx = 0;
	
    this.makeSortable = function(table) {
		if(table.getElementsByTagName("thead").length == 0) return;
        theadRow =  table.getElementsByTagName("thead")[0].getElementsByTagName("tr");
        if(theadRow.length > 0) {
            headingRow = theadRow[0];
        }

        if (!headingRow) return;

        for (var i = 0; i < headingRow.cells.length; i++) {
            var cell = headingRow.cells[i];
            var txt = getInnerText(cell);

            cell.innerHTML = txt + '<span class="sortarrow"></span>';
			cell.title = "Click to sort on this column";
			Event.addListener(cell, "click", Tawala.Tables.Sort.resortTable, cell, true);
			cell.style.cursor = "pointer";
        }
        
        if(Dom.hasClass(table, "saveSortOrder")) {
        	this.saveSortOrder(table);
        }        
    };
    
	this.saveSortOrder = function(table) {
		if(!table.id) {
			this.cookie_table_idx = this.cookie_table_idx + 1;
			table.cookieId = getDocName() + this.cookie_table_idx;
		} else {
			table.cookieId = getDocName() + table.id;
		}
	        
		table.saveSortOrder = true;	

		var tableCookie = YAHOO.util.Cookie.get(table.cookieId); 

		if(tableCookie) {
			var hRowCells = table.getElementsByTagName("thead")[0].getElementsByTagName("tr")[0].cells;
			var col = YAHOO.util.Cookie.getSub(table.cookieId, "Column");
			var dir = YAHOO.util.Cookie.getSub(table.cookieId, "Order");
			
			this.resortTable(null, hRowCells[col], dir)			
		}
	};

    this.resortTable = function(e, obj, sortOrder) {
		var ARROW;
        var span;

        for (var ci=0;ci<obj.childNodes.length;ci++) {
            if (obj.childNodes[ci].tagName && obj.childNodes[ci].tagName.toLowerCase() == 'span') span = obj.childNodes[ci];
        }
        
        var td = obj;
        var table = getParent(td,'table');

		// Workaround for Safari cellIndex bug
		// should just be: td.cellIndex
		columnIndex = -1; 
		for (var i = 0; i < td.parentNode.cells.length; i++) { 
			if (td === td.parentNode.cells[i]) { 
				columnIndex = i; 
			} 
		}


        // Work out a type for the column
        if (table.rows.length <= 1) return;
        tbodies = table.getElementsByTagName("tbody");
        if(tbodies && tbodies.length > 0) {
            for(var tb = 0; tb < tbodies.length; tb++) {
                tbody = tbodies[tb];
                var itm = trim(getInnerText(tbody.rows[0].cells[columnIndex]));
                var sortfn = _sort_caseinsensitive;

                if (itm.match(/^\d{1,2}[\/-]\d{1,2}[\/-]\d{2,4}$/)) sortfn = _sort_date;
                if (itm.match(/^\d\d[\/-]\d\d[\/-]\d\d$/)) sortfn = _sort_date;
                if (itm.match(/^[?$]/)) sortfn = _sort_currency;
                if (itm.match(/^[\d\.]+$/)) sortfn = _sort_numeric;

				// Tawala specific sort parameters
                if(tbody.rows[0].cells[columnIndex].getElementsByTagName("IMG").length > 0){
                	var images = tbody.rows[0].cells[columnIndex].getElementsByTagName("IMG");
                	if(images[0].alt.toLowerCase() == "published"){
                		sortfn = _sort_image;
                	}else{
                		sortfn = _sort_rating;
                	}
                }

                SORT_ORDER_INDEX = columnIndex;
				
                var newRows = new Array();
                for (j = 0; j < tbody.rows.length; j++) { 
					newRows[j] = tbody.rows[j]; 
				}
    
                newRows.sort(sortfn);
				
    			if (!td.getAttribute("sortdir") && columnIndex == 0 || sortOrder == 0){
                	sortOrder = 0;
                    ARROW = '&nbsp;&nbsp;&uarr;';
                    td.setAttribute('sortdir','up');
				}else{
	                if (td.getAttribute("sortdir") == 'up' || sortOrder == 1) {
	                	sortOrder = 1;
	                    ARROW = '&nbsp;&nbsp;&darr;';
	                    newRows.reverse();
	                    td.setAttribute('sortdir','down');
	                } else {
	                	sortOrder = 0;
	                    ARROW = '&nbsp;&nbsp;&uarr;';
	                    td.setAttribute('sortdir','up');
	                }
				}
				
				// Remove sortdir attribute on non-sorted columns
		        for (var i = 0; i < headingRow.cells.length; i++) {
					if(headingRow.cells[i] !== td){
						headingRow.cells[i].removeAttribute('sortdir');
					}
				}
				
                // We appendChild rows that already exist to the tbody, so it moves them rather than creating new ones
                // don't do sortbottom rows
                for (i = 0; i < newRows.length; i++) { 
					if (!newRows[i].className || (newRows[i].className && (newRows[i].className.indexOf('sortbottom') == -1))) {
						tbody.appendChild(newRows[i]);
					}
				}
                // do sortbottom rows only
                for (i = 0; i < newRows.length; i++) { 
					if (newRows[i].className && (newRows[i].className.indexOf('sortbottom') != -1)) {
						tbody.appendChild(newRows[i]);
					}
				}

               	if(YAHOO.util.Dom.hasClass(table, "stripe")) {
               		Tawala.Tables.Stripe.addStripes(table);
               	}
    
                // Delete any other arrows there may be showing
                var allspans = table.getElementsByTagName("span");
                for (var ci = 0; ci < allspans.length; ci++) {
                    if (allspans[ci].className == 'sortarrow') {
                    	allspans[ci].innerHTML = '&nbsp;&nbsp;&nbsp;';
                    }
                }

                if(span) {span.innerHTML = ARROW;}
            }
        }

        // Set cookies with sort column cellIndex and sort order. To be used to remember sort order on page reload
        YAHOO.util.Cookie.remove(table.cookieId) 
        
        YAHOO.util.Cookie.setSub(table.cookieId,"Column", obj.cellIndex);
        YAHOO.util.Cookie.setSub(table.cookieId, "Order", sortOrder ? sortOrder : 0);
        
		return false;
    }

    /*
     * Column Sorting functions
     */
    _sort_date = function (a,b) {
        // y2k notes: two digit years less than 50 are treated as 20XX, greater than 50 are treated as 19XX
        dt1 = _parseDate(getInnerText(a.cells[SORT_ORDER_INDEX]));
        dt2 = _parseDate(getInnerText(b.cells[SORT_ORDER_INDEX]));
        if (dt1==dt2) return 0;
        if (dt2>dt1) return -1;
        return 1;
    };

    _parseDate = function (date) {
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
    };

    _sort_currency = function (a,b) {
        aa = getInnerText(a.cells[SORT_ORDER_INDEX]).replace(/[^0-9.]/g,'');
        bb = getInnerText(b.cells[SORT_ORDER_INDEX]).replace(/[^0-9.]/g,'');
        return parseFloat(bb) - parseFloat(aa);
    };

    _sort_numeric = function (a,b) {
        aa = parseFloat(getInnerText(a.cells[SORT_ORDER_INDEX]));
        if (isNaN(aa)) aa = 0;
        bb = parseFloat(getInnerText(b.cells[SORT_ORDER_INDEX]));
        if (isNaN(bb)) bb = 0;
        return aa-bb;
    };

	// value is in the alt atribute of the rating image
    _sort_rating = function (a,b) {
    	if(! a.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0] ||
	    	! b.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0]){return 0;}
        aa = parseFloat(a.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0].alt);
        bb = parseFloat(b.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0].alt);
        if (isNaN(aa)) aa = 0;
        if (isNaN(bb)) bb = 0;
        return bb-aa;
    };

	_sort_image = function (a,b){
		var aa = 0;
		var bb = 0;
    	if(a.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0]) aa = 1;
	    if(b.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0]) bb = 1;
		return aa-bb;
	};
	
    _sort_caseinsensitive = function (a,b) {
        aa = getInnerText(a.cells[SORT_ORDER_INDEX]).toLowerCase();
        bb = getInnerText(b.cells[SORT_ORDER_INDEX]).toLowerCase();
        if (aa==bb) return 0;
        if (aa<bb) return -1;
        return 1;
    };

    _sort_default = function (a,b) {
        aa = getInnerText(a.cells[SORT_ORDER_INDEX]);
        bb = getInnerText(b.cells[SORT_ORDER_INDEX]);
        if (aa==bb) return 0;
        if (aa<bb) return -1;
        return 1;
    };
}; // end Tawala.Tables.Sort


/*
 * Table stripe function modified from A List Apart article
 */
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
    };

    this.addStripes = function(table) {
        if (!table) { return; }

        var tbodies = table.getElementsByTagName("tbody");
        for (var h = 0; h < tbodies.length; h++) {
		    var even = false;
            var trs = tbodies[h].getElementsByTagName("tr");
            for (var i = 0; i < trs.length; i++) {
                if(Dom.hasClass(trs[i], "odd")) { Dom.removeClass(trs[i], "odd") };
                if(Dom.hasClass(trs[i], "even")) { Dom.removeClass(trs[i], "even") };
                if(even) {
                	Dom.addClass(trs[i], "even");
                }else{
                	Dom.addClass(trs[i], "odd");
                }
                even =  ! even;
            }
        }
    };
}; // end Tawala.Tables.Stripe


/* ******************************
 * Categorizer Function
 */
 
Tawala.Categorizers = new Object();
 
function setupCategorizer(){
	var catList, i;
	catList = Dom.getElementsByClassName("categorizer", "div");
	
	for(i = 0; i < catList.length; i++ ) {
		var categorizerElement = catList[i];
		Tawala.Categorizers[categorizerElement.id] = new Tawala.Categorizer(categorizerElement, i);
	}
};

Tawala.Categorizer = function(catEl, groupId) {
	this.categorizerContainer = catEl;
	this.groupId = groupId;

	this.init();
};

Tawala.Categorizer.prototype = {
	group: "",
	groupId: "",
	categorizerContainer: "",
	categoryContainers: [],
	sourceTable: {},
	destinationTable: {},
	button: "",
	results: [],
	
    init: function() {
		var tableList, j;
		this.group = "categorizer" + this.groupId;
		
		tableList = this.categorizerContainer.getElementsByTagName("table");
		for(j = 0; j < tableList.length; j++ ) {
			if(Dom.hasClass(tableList[j], "draggable")){
				this.makeDraggable(tableList[j]);
				if(Dom.hasClass(tableList[j], "sourceTable")){
					this.sourceTable = tableList[j];
				}else if (Dom.hasClass(tableList[j], "destinationTable")){
					this.destinationTable = tableList[j];
					this.categoryContainers = tableList[j].getElementsByTagName("tbody");
				}
			}
		}

		this.assignEvents();
		this.setupTooltips();		
    },
	
	clearSourceFilter: function(filterInputElement) {
		filterInputElement.value = "";
		this.filter(this.sourceTable, "");
	},
	
	clearDestinationFilter: function(filterInputElement) {
		filterInputElement.value = "";
		this.filter(this.destinationTable, "");
	},
	
	filterSource: function(filterValue) {
		this.filter(this.sourceTable, filterValue);
	},
	
	filterDestination: function(filterValue) {
		this.filter(this.destinationTable, filterValue);
	},

	filter : function(table, filterValue) {
		filterValue = filterValue.toLowerCase();
		var tbList = table.getElementsByTagName("tbody");
		for(var i = 0; i < tbList.length; i++){
			rowList	= tbList[i].getElementsByTagName("tr");			
			for(var j = 0; j < rowList.length; j++){
				var row = rowList[j];
				if(Dom.hasClass(row, "datarow")) {
					var cells = row.getElementsByTagName("td");
					var found = false;
					for(var k=0;k<cells.length;k++) {
						if(cells[k].innerHTML.toLowerCase().indexOf(filterValue) >=0) {
							found = true;
							break;
						}
					}
					if(found) {
						row.style.display = '';
					} else {
						row.style.display = 'none';
					}
				}
			}
		}

	},
	makeDraggable: function(table) {
		var i, j, tbList, thList, rowList;

		tbList = table.getElementsByTagName("tbody");
		thList = table.getElementsByTagName("thead");

		for(i = 0; i < tbList.length; i++){
			new YAHOO.util.DDTarget(tbList[i], this.group);

			rowList	= tbList[i].getElementsByTagName("tr");			
			for(j = 0; j < rowList.length; j++){				
				if(!Dom.hasClass(rowList[j], "heading")) {
					new Tawala.Categorizer.DDList(rowList[j], this.group);
					rowList[j].style.cursor = "move";
				}else{
					new YAHOO.util.DDTarget(rowList[j], this.group);
				}
			}
		}

		for( i = 0; i < thList.length; i++){
			new YAHOO.util.DDTarget(thList[i], this.group);
		}		
	},
	
	setupTooltips: function() {
		//--- TODO: disabled by Sergei. Not functional because the right .js file is not included.
		return;
		
		var stb, dtb, tbrows, rowIds, i, j, ttScr, tds, getIds;
		
		getIds = function(tbs) {			
			for(i = 0; i < tbs.length; i++){
				tbrows = tbs[i].getElementsByTagName("tr");
				for (j = 0; j < tbrows.length; j++) {
					if(tbrows[j].className != "heading") {
						tds = tbrows[j].getElementsByTagName("td");
						tbrows[j].title = tds[1].innerHTML + " " + tds[2].innerHTML;
						rowIds.push( tbrows[j].id );
					}
				}
			}
		}

		rowIds = [];
		stb = this.sourceTable.getElementsByTagName("tbody");
		getIds(stb);
		dtb =  this.destinationTable.getElementsByTagName("tbody");
		getIds(dtb);
		
		ttSrc = new YAHOO.widget.Tooltip("ttSrc", { 
			context:rowIds
		});
		
	},
	
	assignEvents: function() {
		this.button = Dom.getElementsByClassName("categorizerSubmit", "button", this.categorizerContainer)[0];
		Event.on( this.button, "click", this.postData, this, true);
	},

	collectChanges: function() {
		var i, j, rows, newCat, srcTbody, srcRows, srcCat;
		this.results = [];

		for(i = 0; i < this.categoryContainers.length; i++) {
			newCat = new Tawala.Categorizer.Category();
			rows = this.categoryContainers[i].getElementsByTagName("tr");

			for(j = 0; j < rows.length; j++){
				if(Dom.hasClass(rows[j], "heading")) {
					newCat.setId(this.getId(rows[j]));
				}else{	
					newCat.addItem(this.getId(rows[j]));
				}
			}	
			this.results.push(newCat);
		}

		// get source table data
		srcCat = new Tawala.Categorizer.Category();
		srcTbody = this.sourceTable.getElementsByTagName("tbody");

		for(i = 0; i < srcTbody.length; i++) {
			srcRows	= srcTbody[i].getElementsByTagName("tr");			
			for(j = 0; j < srcRows.length; j++) {
				if(Dom.hasClass(srcRows[j], "heading")) {
					srcCat.setId(this.getId(srcRows[j]));
				}else{	
					srcCat.addItem(this.getId(srcRows[j]));
				}
			}
			this.results.push(srcCat);
		}
	},

	getId: function(row) {
		var c, i;
		c = row.childNodes;
		for( i = 0; i < c.length; i++) {
			if(Dom.hasClass(c[i], "id")) {
				return c[i].firstChild.nodeValue;
			}
		}
	},

	postData: function() {
		var i, j;
		
		this.collectChanges();

		var form = document.createElement('form');
		form.setAttribute("action", this.button.getAttribute("formname"));
		form.setAttribute("method", "post");
		
		document.body.appendChild(form);

		for(i in this.results){
			var category = this.results[i];
			var field = document.createElement('input');
			field.setAttribute('type', 'hidden');
			field.setAttribute('name', 'data');
			field.setAttribute('value', "categoryId" + category.getId());
			form.appendChild(field);

			for(j in category.getItems()) {
				var itemId = category.getItems()[j];
				var field = document.createElement('input');
				field.setAttribute('type', 'hidden');
				field.setAttribute('name', 'data');
				field.setAttribute('value', "sourceId" + itemId);
				form.appendChild(field);
			}
		}
		
		form.submit();
	}
};

Tawala.Categorizer.Category = function() {
	this.id = "";
	this.items = [];
};

Tawala.Categorizer.Category.prototype = {
	id: "",
	items: [],
	
	setId: function(id) {
		this.id = id;
	},
	
	getId: function() {
		return this.id;
	},
	
	addItem: function(item) {
		this.items.push(item);
	},
	
	getItems: function() {
		return this.items;
	}
}

/*
 * custom drag and drop implementation for catergorizer
 */
Tawala.Categorizer.DDList = function(id, sGroup, config) {	
	YAHOO.util.DDM.mode = YAHOO.util.DDM.POINT;
    Tawala.Categorizer.DDList.superclass.constructor.call(this, id, sGroup, config);

    this.logger = this.logger || YAHOO;
    var el = this.getDragEl();
    Dom.setStyle(el, "opacity", 0.67); // The proxy is slightly transparent

    this.goingUp = false;
    this.lastY = 0;
};

YAHOO.extend(Tawala.Categorizer.DDList, YAHOO.util.DDProxy, {
    startDrag: function(x, y) {
        this.logger.log(this.id + " startDrag");

        // make the proxy look like the source element
        var dragEl = this.getDragEl();
        var clickEl = this.getEl();
        Dom.setStyle(clickEl, "visibility", "hidden");
		
//        dragEl.innerHTML = clickEl.innerHTML;
        dragEl.innerHTML = this.getRowText();

        Dom.setStyle(dragEl, "color", Dom.getStyle(clickEl, "color"));
        Dom.setStyle(dragEl, "backgroundColor", Dom.getStyle(clickEl, "backgroundColor"));
        Dom.setStyle(dragEl, "border", "2px solid gray");
//        Dom.setStyle(dragEl, "width", "200px");
    },

	// Get the all text from the selected row except for the ID
	getRowText: function(){
		var tds, i, text = "";
		tds = this.getEl().childNodes;
		for( i = 0; i < tds.length; i++) {
			if( !Dom.hasClass(tds[i], "id") ){
				text += tds[i].innerHTML + "&nbsp;&nbsp;";
			}
		}
		return text;
	},
	
    endDrag: function(e) {

        var srcEl = this.getEl();
        var proxy = this.getDragEl();

        // Show the proxy element and animate it to the src element's location
        Dom.setStyle(proxy, "visibility", "");
        var a = new YAHOO.util.Motion( 
            proxy, { 
                points: { 
                    to: Dom.getXY(srcEl)
                }
            }, 
            0.2, 
            YAHOO.util.Easing.easeOut 
        )
        var proxyid = proxy.id;
        var thisid = this.id;

        // Hide the proxy and show the source element when finished with the animation
        a.onComplete.subscribe(function() {
                Dom.setStyle(proxyid, "visibility", "hidden");
                Dom.setStyle(thisid, "visibility", "");
            });
        a.animate();
    },

    onDragDrop: function(e, id) {

        // If there is one drop interaction, the li was dropped either on the list,
        // or it was dropped on the current location of the source element.
        if (DDM.interactionInfo.drop.length === 1) {

            // The position of the cursor at the time of the drop (YAHOO.util.Point)
            var pt = DDM.interactionInfo.point; 

            // The region occupied by the source element at the time of the drop
            var region = DDM.interactionInfo.sourceRegion; 

            // Check to see if we are over the source element's location.  We will
            // append to the bottom of the list once we are sure it was a drop in
            // the negative space (the area of the list without any list items)
            if (!region.intersect(pt)) {
                var destEl = Dom.get(id);
                var destDD = DDM.getDDById(id);
				
				if(destEl.nodeName.toLowerCase() == "thead") {
					var tb = destEl.parentNode.getElementsByTagName("tbody");
					destEl = tb[0];
				}

                destEl.appendChild(this.getEl());
                destDD.isEmpty = false;
                DDM.refreshCache();
            }
        }
    },

    onDrag: function(e) {

        // Keep track of the direction of the drag for use during onDragOver
        var y = Event.getPageY(e);

        if (y < this.lastY) {
            this.goingUp = true;
        } else if (y > this.lastY) {
            this.goingUp = false;
        }

        this.lastY = y;
    },

    onDragOver: function(e, id) {
    
        var srcEl = this.getEl();
        var destEl = Dom.get(id);
		
		if(destEl.nodeName.toLowerCase() == "thead") {
			var tb = destEl.parentNode.getElementsByTagName("tbody");
			destEl = tb[0];
		}

        if (destEl.nodeName.toLowerCase() == "tr") {
            var orig_p = srcEl.parentNode;
            var p = destEl.parentNode;
			if(Dom.hasClass(destEl, "heading")){
               	p.insertBefore(srcEl, destEl.nextSibling); // insert below				
			}else{
	            if (this.goingUp) {
	                p.insertBefore(srcEl, destEl); // insert above
	            } else {
	               	p.insertBefore(srcEl, destEl.nextSibling); // insert below
	            }
			}
            DDM.refreshCache();
        }
    }
});
// End categorizer function


/*
 * YUI datatable functions for Tawala
 * 
 * Looks for the class "tawalaDataTable" on a div element that contains a table.
 * Creates a datatable from existing markup.
 * You can also add the "paginate" class if you want to turn on pagination.
 *
 */
Tawala.DataTables = function() {
	var containerList = [], 
		dataTableList = [];

	return {
		init: function() {
			var dt, i;
	
			var sortChange = function() {
				/* TODO: This code accesses items in the YUI code that probably shouldn't be accessed. 
				 * Change to use the getState function when we update the YUI library to >= 2.7 
				 */
				dt.sortedByKey = this._configs.sortedBy.value.key.substring(6);
				if(this._configs.sortedBy.value.dir == "yui-dt-asc"){
					dt.sortedByDir = 0;
				} else if (this._configs.sortedBy.value.dir == "yui-dt-desc"){
					dt.sortedByDir = 1;
				}				
			}
			
			this.containerList = YAHOO.util.Dom.getElementsByClassName("tawalaDataTable", "div");
			
			for( i = 0; i < this.containerList.length; i++ ) { 
				dt = dataTableList[i] = new Tawala.DataTable();
				dt.container = this.containerList[i];
				dt.paginate = YAHOO.util.Dom.hasClass(dt.container, "paginate");
				dt.enablePrint = YAHOO.util.Dom.hasClass(dt.container, "enablePrint");
				dt.enableExport = YAHOO.util.Dom.hasClass(dt.container, "enableExport");
				dt.exportTemplateId = dt.container.getAttribute('exportTemplateId');
				// Never force 97% container width (empty “blank box” right of last
				// column). Columns stay resizeable via heading-border drag.
				dt.fixTableWidth = false;
				dt.fixTableHeight = YAHOO.util.Dom.hasClass(dt.container, "dtFixTableHeight");
				dt.presetColumnWidth = YAHOO.util.Dom.hasClass(dt.container, "presetColumnWidth");
				dt.sourceElement = dt.container.getElementsByTagName("table")[0];
				dt.sourceString = dt.container.getElementsByTagName("table")[0].innerHTML;
				dt.columnHeadings = dt.sourceElement.getElementsByTagName("thead")[0].getElementsByTagName("th");
    
				dt.dataSource = new YAHOO.util.DataSource(dt.sourceElement);				
				dt.dataSource.responseType = YAHOO.util.DataSource.TYPE_HTMLTABLE;
				dt.createColumnDefs();
				dt.createDataSourceSchema();
				dt.createDataTable();
//				dt.onEventSortColumn();
				dt.dataTable.subscribe("sortedByChange", sortChange);				
			}
			
		}
	}
}();

Tawala.DataTable = function() {
	var escapeHTML = function (str) {                                       
        return  (                                                               
            str.replace(/<br *\/?>/g,'\n').
            	replace(/&nbsp;/g,' ').
            	replace(/&amp;/g, '&').
            	replace(/&/g,'&amp;').                                         
                replace(/>/g,'&gt;').                                           
                replace(/</g,'&lt;').                                           
                replace(/"/g,'&quot;')
		);                                                                     
    };

    var stripHTML = function(str) {
    	return str.replace(/(<([^>]+)>)/ig,"");
    } 
    
	return {
		container: "",
		sourceElement: "",
		sourceString: "",
		columnHeadings: [],
		columnDefs: [],
		dataSource: {},
		dataTable: {},
		paginate: false,
		enablePrint: false,
		enableExport: false,
		exportTemplateId: "",
		tableWidth: "97%",
		tableHeight: "400px",
		defaultColumnWidth: 120,
		rowsPerPage: 100,
		printLinkText: "Print This List",
		exportLinkText: "Export to Excel",
		sortedByKey: "",
		sortedByDir: "",
		
		createColumnDefs: function(columnHeadings) {
			if(this.columnHeadings.length == 0) { return };
			for(i = 0; i < this.columnHeadings.length; i++) {
				this.columnDefs[i] = { "key": "column" + i, 
									"label": this.columnHeadings[i].innerHTML,
									"resizeable": true,
									"sortable": true
								}
			
				if(this.columnHeadings[i].innerHTML == "__Created__" || this.columnHeadings[i].innerHTML == "__Updated__") {
					this.columnDefs[i].hidden = true;
				}
				
				if(this.presetColumnWidth) {
					this.columnDefs[i].width = this.defaultColumnWidth;
				}
			}
		},
		
		createDataSourceSchema: function() {
			if(this.columnHeadings.length == 0) { return };
		
			this.dataSource.responseSchema = {};
			this.dataSource.responseSchema.fields = [];
			for(i = 0; i < this.columnHeadings.length; i++) {
				this.dataSource.responseSchema.fields[i] = { "key": "column" + i };
			}
		},
		
		createDataTable: function() {			
			var dataTableOptions = {	
					scrollable: true
				};

			if(this.fixTableWidth) {
		       dataTableOptions.width = this.tableWidth; // width as a string value
			}
			
			if(this.fixTableHeight) {
			    dataTableOptions.height = this.tableHeight // height as a string value
			}	

			if(this.paginate) {
            	dataTableOptions.paginator = new YAHOO.widget.Paginator({ rowsPerPage: this.rowsPerPage });
			}
			
			this.dataTable = new YAHOO.widget.DataTable( this.container, this.columnDefs, 
															this.dataSource, dataTableOptions, {renderLoopSize: 100});
			
			// Content-fit: clear YUI’s stretch widths so columns hug text (CSS caps at ~6in).
			if(!this.fixTableWidth) {
				this.fitTableToContent();
			}

			if(this.enablePrint || this.enableExport) {
				this.addPrintAndExportControls();
			}			
		},

		fitTableToContent: function() {
			try {
				if(!this.container) { return; }
				YAHOO.util.Dom.setStyle(this.container, "width", "auto");
				YAHOO.util.Dom.setStyle(this.container, "maxWidth", "");
				var tables = this.container.getElementsByTagName("table");
				for(var ti = 0; ti < tables.length; ti++) {
					YAHOO.util.Dom.setStyle(tables[ti], "width", "auto");
				}
			} catch(ignore) {}
		},
		
		addPrintAndExportControls: function() {			
			var dtControls = document.createElement("div");
			dtControls.style.width = "99%";
			dtControls.style.lineHeight = "2.2em";
			
			var hintControls = document.createElement("div");
			hintControls.style.textAlign = "left";
			hintControls.style.cssFloat = "left";
			hintControls.style.styleFloat = "left";
			hintControls.style.width = "68%";
			hintControls.style.color = "#888888";
			hintControls.style.fontSize = "90%";
			hintControls.innerHTML = "NOTE: Drag column heading border to resize column. Click any column heading to sort.";
			
			var pControls = document.createElement("div");
			pControls.style.textAlign = "right";
			pControls.style.cssFloat = "right";
			pControls.style.styleFloat = "right";
			pControls.style.width = "30%";
			
			var pControlsLink = document.createElement("a");
			pControlsLink.href = "#";
			pControlsLink.innerHTML = this.printLinkText;
			pControlsLink.style.marginLeft= "12px";
			YAHOO.util.Event.on(pControlsLink, "click", this.printTable, this, true);

			var exportControlsLink = document.createElement("a");
			exportControlsLink.href = "#";
			exportControlsLink.innerHTML = this.exportLinkText;
			exportControlsLink.style.marginLeft= "12px";
			YAHOO.util.Event.on(exportControlsLink, "click", this.exportTableToExcel, this, true);

			var br = document.createElement("br");
			br.style.clear = "both";
			
			if(this.enablePrint) {
				pControls.appendChild(pControlsLink);
			}
			if(this.enableExport && this.enablePrint) {
//				pControls.appendChild(document.createElement("br"));
			}
			if(this.enableExport) {
				pControls.appendChild(exportControlsLink);
			}
			
			dtControls.appendChild(hintControls);
			dtControls.appendChild(pControls);
			dtControls.appendChild(br);
			this.container.parentNode.insertBefore(dtControls, this.container);			
		},
		
		printTable: function(e) {
			YAHOO.util.Event.stopEvent(e);
			var pdw = window.open("", "printDataWindow", "width=850,height=600,resizable=1,scrollbars,location=0");
			pdw.document.open();
			pdw.document.write("<html><head><title>Tawala</title>");
			pdw.document.write("<style> table { border-collapse: collapse; border-spacing: 0;}");
			pdw.document.write("table td { padding: 2px 4px; border: 1px solid #eeeeee } table th { text-align: left; }</style>");
			pdw.document.write("</head><body>");
			pdw.document.write("<table id='reportTable'>");
			
			//pdw.document.write(this.sourceString);
			pdw.document.write("<thead>\n<tr>\n");
			var headerRow = this.dataSource.liveData.tHead.rows[0];
			var headers = headerRow.cells;
			var nonDisplay = 0;
			for (var i = 0; i < headers.length; i++) {
				if (stripHTML(headers[i].innerHTML) == "__Created__" || stripHTML(headers[i].innerHTML) == "__Updated__") {
					nonDisplay++;
					continue;
					}
				pdw.document.write("<th>" + stripHTML(headers[i].innerHTML) + "</th>");
				}
			pdw.document.write("</tr>\n</thead>\n<tbody>\n");

			var dataRows = this.dataSource.liveData.getElementsByTagName('tbody')[0].rows;
			for (var i = 0; i < dataRows.length; i++) {
				pdw.document.write("<tr>");
				var dataRow = dataRows[i];
				for (var j = 0; j < dataRow.cells.length - nonDisplay; j++) {
					pdw.document.write("<td>" + stripHTML(dataRow.cells[j].innerHTML) + "</td>");
					}
				pdw.document.write("</tr>\n");
				}
			pdw.document.write("</tbody>");

			pdw.document.write("</table></body>");
			pdw.document.close();

			var tableEl = pdw.document.getElementById("reportTable");
			Tawala.Tables.Sort.makeSortable(tableEl);
			var theadRow =  tableEl.getElementsByTagName("thead")[0].getElementsByTagName("tr");
    		if(theadRow.length > 0) {
        		var headingRow = theadRow[0];
    		}
			
			if (this.sortedByKey) {
				Tawala.Tables.Sort.resortTable(null, headingRow.cells[this.sortedByKey], this.sortedByDir);
			}

			pdw.print();
			return false;
		},
		
		exportTableToExcel: function(e) {
			YAHOO.util.Event.stopEvent(e);
			//-- TODO: Need to see if the window is already open and close it or remove the previous HTML.
			var exportWindow = window.open("", "exportDataWindow", "width=600,height=300,resizable=0,scrollbars,location=0");
			var doc = exportWindow.document; 
			doc.open();
			doc.write("<html><head><title>Tawala</title>");
			doc.write("</head><body>");
			doc.write("<h2 style='border-bottom: 1px solid #000000;'>Exporting Data to Excel Spreadsheet</h2>")

			doc.write("<h4>Preparing data for the spreadsheet.</h4>");
			doc.write("<p>This could take up to a few minutes depending on the size of your league. Please wait...</p>");
			
			doc.write('<form action="/create-excel-spreadsheet" id="exportToExcelForm" target="TawalaProjectWindow" method="POST">\n');
			if(this.exportTemplateId && this.exportTemplateId != '') {
				doc.write('<input name="template" value="' + this.exportTemplateId + '" type="hidden" />\n');
			}
			
			var export_data = "";
			var headerRow = this.dataSource.liveData.tHead.rows[0];
			var headers = headerRow.cells;
//			doc.write('<input name="columns" value="' + headers.length + '" type="hidden" />\n');
			export_data += '<input name="columns" value="' + headers.length + '" type="hidden" />\n';
			for ( var i = 0; i < headers.length; i++) {
				var headerCellValue = stripHTML(headers[i].innerHTML);
				if(headerCellValue == '') {
					continue;
				}
//				doc.write('<input name="h' + i + '" value="' + headerCellValue + '" type="hidden" />\n');
				export_data += '<input name="h' + i + '" value="' + headerCellValue + '" type="hidden" />\n';
			}

			var dataRows = this.dataSource.liveData.getElementsByTagName('tbody')[0].rows;
//			doc.write('<input name="rows" value="' + dataRows.length + '" type="hidden" />\n');
			export_data += '<input name="rows" value="' + dataRows.length + '" type="hidden" />\n';
			for ( var i = 0; i < dataRows.length; i++) {
				var dataRow = dataRows[i];
				for ( var j = 0; j < dataRow.cells.length; j++) {
					var cellValue = stripHTML(dataRow.cells[j].innerHTML);
					if(cellValue == '') {
						continue;
					}
//					doc.write('<input name="d' + i + '_' + j + '" value="' + cellValue + '" type="hidden" />\n');
					export_data += '<input name="d' + i + '_' + j + '" value="' + cellValue + '" type="hidden" />\n';
				}
			}
			doc.write(export_data);
			
			doc.write('</form>');
			
			doc.write("<h4>Processing complete!</h4>");
			doc.write("<p>Downloading of the spreadsheet should start automatically in a few moments.</p>");
			doc.write("<p>This window will close in about 10 seconds or you can click the link below to close it manually.</p>");
			doc.write('<a style="float:right;" onclick="window.close();" href="">Close Window</a>');
			doc.write("</body>");
			doc.close();

			var form = doc.getElementById('exportToExcelForm');
			form.submit();

			exportWindow.setTimeout(function(){exportWindow.close()}, 10000);
			
			return false;
		}

	}
}


/* *******************************
 * Support functions
 */

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
};

function getEventTarget(evt) {
    var e = evt || window.event;
    if(e.target) {
        target = e.target;
    }else{
        if(e.srcElement) target = e.srcElement;
    }
    return target;
};

/*
 *  Given an element get the parent of type "TagName"
 */
function getParent(el, pTagName) {
	if (el == null) {
		return null;
	}else if (el.nodeType == 1 && el.tagName.toLowerCase() == pTagName.toLowerCase()){		
		return el;
	}else{
		return getParent(el.parentNode, pTagName);
	}
};

function LTrim( value ) {
	var re = /\s*((\S+\s*)*)/;
	return value.replace(re, "$1");
};

function RTrim( value ) {
	var re = /((\s*\S+)*)\s*/;
	return value.replace(re, "$1");
};

function trim( value ) {
	return LTrim(RTrim(value));
};

function getDocName() {
	var regex = /.+\/(.+)\??/;
	var results = regex.exec(window.location.href);
	if(results) return results[1].replace(/[^A-Za-z0-9]+/g, '');
};


function updateTimeoutCountdown() {
     //Convert both today's date and the target date into milliseconds.                           
     var Todays_Date = new Date().getTime();                                 
     var Target_Date = Tawala.timeoutDialog.expirationTime;                  
     
     //Find their difference, and convert that into seconds.                  
     var Time_Left = Math.round((Target_Date - Todays_Date) / 1000);
     
     if(Time_Left < 0) {
     	//--- Nothing much we cand do at this point.
     	Tawala.timeoutDialog.hide();
     	return;
     }
     
     var minutes = Math.floor(Time_Left / 60);
     Time_Left %= 60;
     var seconds = Time_Left;
                
	 var mps = 's'; sps = 's';  //ps is short for plural suffix.
     if(minutes == 1) mps ='';
     if(seconds == 1) sps ='';
     
     var timeLeftText = "";
     if(minutes > 0) {
     	timeLeftText += minutes + ' minute' + mps  + ' and ';
     }
     timeLeftText += seconds + ' second' + sps + " left.";
     
     var text = "Your session will expire soon!<br /><br/>" + timeLeftText ;
           
     document.getElementById('timeoutDialogText').innerHTML = text;
           
     YAHOO.lang.later(300, null, updateTimeoutCountdown, null, false);
};

function initSessionExpirationTimer(timeoutInMinutes) {
	YAHOO.lang.later((timeoutInMinutes - 5) * 60 * 1000, null, showTimeoutDialog, null, false);
};

function showTimeoutDialog() {
	Tawala.timeoutDialog.expirationTime = new Date().getTime() + ((5 * 60 - 5) * 1000 /* -5 seconds is some extra time to account for network latency, etc. */);
	updateTimeoutCountdown();
	
	Tawala.timeoutDialog.show();
};

function initTimeoutDialog() {
	var processServerResponse = function(o) {
		var serverResponse = YAHOO.lang.JSON.parse(o.responseText);
		if(! serverResponse.expired) {
			initSessionExpirationTimer(sessionExpirationTimeout);
		}
	};
	
	var handleSubmit = function() {
		this.hide();
		this.submit();
	};

	Tawala.timeoutDialog = new YAHOO.widget.Dialog("timeoutDialogDiv", 
			 { width: "300px",
			   fixedcenter: true,
			   visible: false,
			   draggable: false,
			   close: false,
			   constraintoviewport: true,
			   buttons: [ { text: "Continue my Session", handler: handleSubmit, isDefault: true } ]
			 } );
	Tawala.timeoutDialog.callback = { success: processServerResponse };
	Tawala.timeoutDialog.render();
	
	initSessionExpirationTimer(sessionExpirationTimeout);
};

/*************************************
 * Handling of page exits in case where the back button navigation should be discouraged.
 */

var confirmNavigationAwayFromPage = false;

window.onbeforeunload = function () {
	if(confirmNavigationAwayFromPage == true) {
		return "You might have unsaved information on the form. To save any possible unsaved information, stay on the page and click submit.";
	}
};     

function onSubmit(form) {
	var i, inputs, input;
	
	if(! Tawala.validation.validateForm(form)) {
		return false;
	}
	
	confirmNavigationAwayFromPage = false;

	inputs = form.getElementsByTagName("input");
	for( i=0; i < inputs.length; i++) {
	        input = inputs[i];
	        if(input.type.toLowerCase() == 'submit') {
	                input.disabled = true;
	        }
	}
	
	YAHOO.lang.later(5000, null, initWaitOnSubmitPanel, null, false);
	return true;
};

function initWaitOnSubmitPanel() {
	YAHOO.namespace("tawala.onsubmit");
	
    var content = document.getElementById("wait.panel");
    
    content.innerHTML = "";

    if (!YAHOO.tawala.onsubmit.wait) {
        YAHOO.tawala.onsubmit.wait = 
                new YAHOO.widget.Panel("wait",  
                                               { width: "240px", 
                                                  fixedcenter: true, 
                                                  close: false, 
                                                  draggable: false, 
                                                  zindex:4,
                                                  modal: true,
                                                  visible: false
                                                } 
                                            );

        YAHOO.tawala.onsubmit.wait.setHeader("Processing, please wait...");
        YAHOO.tawala.onsubmit.wait.setBody("<img src=\"/images/submit-progress.gif\"/>");
        YAHOO.tawala.onsubmit.wait.render(document.body);
    }
	YAHOO.tawala.onsubmit.wait.show();
};

/*************************************
 * Add events to the page
 */
//Event.addListener(window, "load", Tawala.Tables.init);
//Event.addListener(window, "load", Tawala.fixTemplates);

//Event.addListener(window, "load", Tawala.fixTableWidth);
Event.addListener(window, "resize", Tawala.fixTableWidth);

Event.onDOMReady(Tawala.Tables.init);
Event.onDOMReady(Tawala.fixTemplates);
Event.onDOMReady(Tawala.fixTableWidth);

Event.onDOMReady(setupCategorizer);
Event.onDOMReady(Tawala.DataTables.init);


/***************************************
 * Validators
 * Assumption - there is only one form.
 */
Tawala.validation = {};
Tawala.validation.validatorsByIds = {};
Tawala.validation.fieldsToValidate = [];
Tawala.validation.SUCCESS = {"success": true};  

Tawala.validation.validateForm = function(form) {
	var validationFailures = 0;
	var errorMessageDisplayed = false;
	for ( var fieldNumber = 0; fieldNumber < Tawala.validation.fieldsToValidate.length; fieldNumber++) {
		var id = Tawala.validation.fieldsToValidate[fieldNumber];
		var input = document.getElementById(id);

		var validators = Tawala.validation.validatorsByIds[id];
		if(! validators) {
			continue;
		}
		for ( var i = 0; i < validators.length; i++) {
			var validator = validators[i].validator;
			var parameters = validators[i].parameters;
			
			var value;
			if(input && YAHOO.lang.isValue(input.value)) {
				value = input.value;
			} else {
				value = null;
			}
			var result = validator(value, parameters);
			if(typeof result.newValue == 'string') {
				input.value = result.newValue;
			}
			if(!result.success) {
				++validationFailures;
				if(! errorMessageDisplayed ) {
					if(!document.getElementById("formErrorMsg")) {
						errorDiv = document.createElement("div");
						errorDiv.id = "formErrorMsg";
						errorDiv.className = "text error";
						formDiv = document.getElementById("tawalaProjectForm");
						formDiv.insertBefore(errorDiv, formDiv.firstChild);
					}else{
						errorDiv = document.getElementById("formErrorMsg");
					}
					
					errorDiv.innerHTML = "There were errors detected in the form. Fields with errors are outlined in red.<br /> Please correct them before proceeding.";
					
					Tawala.validation.displayValidationMessage(input, result.errorMessage);
					errorMessageDisplayed = true;
				} else {
					Tawala.validation.indicateAsInvalid(input);
				}
			}
		}
		
	}
	return validationFailures == 0;
};

Tawala.validation.register = function(id, validator, parameters) {
	var previousList = Tawala.validation.validatorsByIds[id];
	if(!previousList) {
		previousList = [];
		Tawala.validation.validatorsByIds[id] = previousList;
		Tawala.validation.fieldsToValidate.push(id);
	}
	previousList.push({"validator":  validator, "parameters": parameters});
};

Tawala.validation.validate = function(input) {
	var id = input.id;
	var validators = Tawala.validation.validatorsByIds[id];
	if(! validators) {
		return;
	}
	for ( var i = 0; i < validators.length; i++) {
		var validator = validators[i].validator;
		var parameters = validators[i].parameters;
		
		var result = validator(input.value, parameters);
		if(typeof result.newValue == 'string') {
			input.value = result.newValue;
		}
		if(result.success) {
			Tawala.validation.removeValidationMessage(input);
		} else {
			Tawala.validation.displayValidationMessage(input, result.errorMessage);
			return;
		}
	}
};

Tawala.validation.indicateAsInvalid = function(element) {
	YAHOO.util.Dom.addClass(element, "validateError");
};

Tawala.validation.removeInvalidIndicator = function(element) {
	YAHOO.util.Dom.removeClass(element, "validateError");
};

Tawala.validation.hideValidationMessage = function() {
	var validateMsg = document.getElementById('validateMsg');
	if(validateMsg) {
		validateMsg.style.display = "none";
	}
};
Tawala.validation.removeValidationMessage = function(input) {
	Tawala.validation.hideValidationMessage();
	Tawala.validation.removeInvalidIndicator(input);
};

Tawala.validation.displayValidationMessage = function(target, msgText) {
	Tawala.validation.indicateAsInvalid(target);
	
	if(! msgText) {
		return;
	}

	var validateMsg, validateMsgContent, msgHeight, targetHeight, targetWidth, topPosition, leftPosition;

	if(!document.getElementById('validateMsg')) {
		validateMsg = document.createElement('div');
		validateMsg.id = 'validateMsg';
		validateMsgContent = document.createElement('div');
		validateMsgContent.id = 'validateMsgContent';
		document.body.appendChild(validateMsg);
		validateMsg.appendChild(validateMsgContent);
	} else {
		validateMsg = document.getElementById('validateMsg');
		validateMsgContent = document.getElementById('validateMsgContent');
	}

	validateMsgContent.innerHTML = msgText;
	validateMsg.style.display = 'block';
	msgHeight = validateMsg.offsetHeight;
	//target.focus();

	var elH = target.offsetHeight;
	var elW = target.offsetWidth;
	var elX = YAHOO.util.Dom.getX(target);
	var elY = YAHOO.util.Dom.getY(target);
	var msgH = validateMsg.offsetHeight;

	validateMsg.style.top = (elY - ((msgH - elH) / 2)) + "px"; 
	validateMsg.style.left = (elX + elW) + "px";

	YAHOO.util.Event.on(validateMsg, "click", Tawala.validation.removeValidationMessage);
};

Tawala.validation.integerValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return Tawala.validation.SUCCESS;
	}
	
    var reg = /^-?[\d]+$/;
    
    var failure = {"success": false, "errorMessage": parameters.errorMessage}; 
    if(!reg.test(value)) {
    	return failure;
    }
    
	if(typeof parameters.lowerLimit == 'number') {
		if(value < parameters.lowerLimit) {
			return failure;
		}
	}

	if(typeof parameters.upperLimit == 'number') {
		if(value > parameters.upperLimit) {
			return failure;
		}
	}

	return Tawala.validation.SUCCESS;
};

Tawala.validation.validUSStateAbbreviations = {
		'AL': true,
		'AK': true,
		'AS': true,
		'AZ': true,
		'AR': true,
		'CA': true,
		'CO': true,
		'CT': true,
		'DE': true,
		'DC': true,
		'FM': true,
		'FL': true,
		'GA': true,
		'GU': true,
		'HI': true,
		'ID': true,
		'IL': true,
		'IN': true,
		'IA': true,
		'KS': true,
		'KY': true,
		'LA': true,
		'ME': true,
		'MH': true,
		'MD': true,
		'MA': true,
		'MI': true,
		'MN': true,
		'MS': true,
		'MO': true,
		'MT': true,
		'NE': true,
		'NV': true,
		'NH': true,
		'NJ': true,
		'NM': true,
		'NY': true,
		'NC': true,
		'ND': true,
		'MP': true,
		'OH': true,
		'OK': true,
		'OR': true,
		'PW': true,
		'PA': true,
		'PR': true,
		'RI': true,
		'SC': true,
		'SD': true,
		'TN': true,
		'TX': true,
		'UT': true,
		'VT': true,
		'VI': true,
		'VA': true,
		'WA': true,
		'WV': true,
		'WI': true,
		'WY': true,
		'AE': true,
		'AA': true,
		'AP': true
};

Tawala.validation.USStateValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return Tawala.validation.SUCCESS;
	}
	value = value.toUpperCase();
	
	var result = {'newValue': value};
	if(Tawala.validation.validUSStateAbbreviations[value]) {
		result.success = true;
	} else {
		result.success = false;
		result.errorMessage =  parameters.errorMessage;
	}
	return result;
};

Tawala.validation.emailValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return Tawala.validation.SUCCESS;
	}
	
	var newValue = value.replace(/^\s*/, "").replace(/\s*$/, "");
	
	var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
	
	result = {'newValue' : newValue };
	
	if(reg.test(newValue) && newValue.indexOf('..') < 0) {
		result.success = true;
		return result;
	} else {
		result.success = false;
		result.errorMessage = parameters.errorMessage;
		return result;
	}
};

Tawala.validation.dollarAmountValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return Tawala.validation.SUCCESS;
	}
	
	value = value.replace(/ /g, '');
	value = value.replace(/\$/g, '');
	value = value.replace(/,/g, '');
	
	var result = {'newValue': value};

	var reg = /^(\d*)(\.\d?\d?)?$/;
	if(reg.test(value)) {
		result.newValue = parseFloat(value).toFixed(2);
		result.success = true;
	} else {
		result.success = false;
		result.errorMessage =  parameters.errorMessage;
	}
	return result;
}; 

Tawala.validation.ZIPCodeValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return Tawala.validation.SUCCESS;
	}
	
	value = value.replace(/ /g, '');
	
	var result = {'newValue': value};

	var reg = /^(\d\d\d\d\d)(-\d\d\d\d)?$/;
	if(reg.test(value)) {
		result.success = true;
	} else {
		result.success = false;
		result.errorMessage =  parameters.errorMessage;
	}
	return result;
}; 

Tawala.validation.phoneNumberValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return Tawala.validation.SUCCESS;
	}
	
	value = value.replace(/ /g, '');
	value = value.replace(/[\)\(\-\.]/g, '');
	
	var result = {'newValue': value};

	var reg = /^1?([2-9]\d{9})(x\d+)?$/;
	var matchResult = reg.exec(value); 
	if(matchResult != null) {
		var mainNumber = matchResult[1];
		var extension = typeof(matchResult[2]) == 'string' ? matchResult[2] : "";
		value = "(" + mainNumber.substring(0, 3) + ") " + mainNumber.substring(3, 6) + "-" + mainNumber.substring(6) + extension;
		
		result.newValue = value;
		result.success = true;
	} else {
		result.success = false;
		result.errorMessage =  parameters.errorMessage;
	}
	return result;
}; 
 
Tawala.validation.ProperValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return Tawala.validation.SUCCESS;
	}

	var delimiters = {' ': true, '.': true, '-': true};
	var newValue = value.toLowerCase();
	newValue = newValue.split('');
    var capitalizeNext = true;
    for (var i = 0; i < newValue.length; i++) {
    	var ch = newValue[i];
    	if (delimiters[ch]) {
    		capitalizeNext = true;
    	} else if (capitalizeNext) {
    		newValue[i] = ch.toUpperCase();
    		capitalizeNext = false;
    	}
    }

    newValue = newValue.join('');
	
	var result = {'newValue': newValue};
	result.success = true;

	return result;
}; 
 	
	
Tawala.validation.isBlank = function(value) {
	var reg = /^\s*$/;
	return reg.test(value);
};

Tawala.validation.nonEmptyFieldValidation = function(value, parameters) {
	if(Tawala.validation.isBlank(value)) {
		return {"success": false, "errorMessage": "This field needs to be filled out."};
	} else {
		return Tawala.validation.SUCCESS;
	}
};

Tawala.validation.nonEmptyMCQValidation = function(value, parameters) {
	var fieldName = parameters.fieldName;
	var containerId = parameters.containerId;
	
	var form = document.getElementById("tawalaProjectForm");
	var inputs = form.getElementsByTagName("input");
	for ( var i = 0; i < inputs.length; i++) {
		var input = inputs[i];
		if(input.name == fieldName && input.checked) {
			return Tawala.validation.SUCCESS;
		}
	}
	
	//--- Set up radio buttons to remove error message if clicked.
	var onClickHandler = function() { 
		var mcContainer = document.getElementById(containerId);
		if(mcContainer) {
			Tawala.validation.removeInvalidIndicator(mcContainer);
		}
	};
	for ( var i = 0; i < inputs.length; i++) {
		var input = inputs[i];
		if(input.name == fieldName) {
			input.onclick = onClickHandler;
		}
	}
	
	return {"success": false};
};

Tawala.validation.dateValidation = function(value, parameters) {
    var reg = /^(0?[1-9]|1[012])[- \/.](0?[1-9]|[12][0-9]|3[01])[- \/.](19|20)?\d\d$/;
    if(reg.test(value)) {
		return {"success": true};
	} else {
		return {"success": false, "errorMessage": "Please enter a valid date (mm/dd/yyyy)"};
	}
};

Tawala.validation.numericValidation = function(value, parameters) {
    var reg = /^[\d]+/;
    if(reg.test(value)) {
		return {"success": true};
	} else {
		return {"success": false, "errorMessage": "Please enter numbers only"};
	}
};
	
Tawala.validation.alphaValidation = function(value, parameters) {
    var reg = /^[A-Za-z]+/;
    if(reg.test(value)) {
		return {"success": true};
	} else {
		return {"success": false, "errorMessage": "Please enter letters only"};
	}
};

Tawala.validation.alphaNumericValidation = function(value, parameters) {
    var reg = /^[A-Za-z0-9]+/;
    if(reg.test(value)) {
		return {"success": true};
	} else {
		return {"success": false, "errorMessage": "Please enter letters and numbers only"};
	}
};

/* 
 * Convert text begining with http:// or https:// to active links
 */
Tawala.convertTextToLinks = function() {
	var element, urlpattern, a;
	
	element = document.getElementsByTagName("body")[0];
    urlpattern= /\bhttps?:\/\/[^\s<>"`{}|\^\[\]\\]+/g;
    Tawala.findTextExceptInLinks(element, urlpattern, 
		function(node, match) {
	        node.splitText(match.index+match[0].length);
	        a= document.createElement('a');
	        a.target= "_blank";
	        a.href= match[0];
	        a.appendChild(node.splitText(match.index));
	        node.parentNode.insertBefore(a, node.nextSibling);
	    }
	);
};

// Find text in descendents of an element, in reverse document order
// pattern must be a regexp with global flag
//
Tawala.findTextExceptInLinks = function(element, pattern, callback) {
	var childi, child, tag, matches, match, i;
	
    for (childi= element.childNodes.length; childi-->0;) {
        child= element.childNodes[childi];
        if (child.nodeType===1) {
        	tag = child.tagName.toLowerCase();
            if (tag !=='a' && tag !== 'input' && tag !== 'textarea' )
                Tawala.findTextExceptInLinks(child, pattern, callback);
        } else if (child.nodeType===3) {
            matches= [];
            match;
            while (match= pattern.exec(child.data))
                matches.push(match);
            for (i= matches.length; i-->0;)
                callback.call(window, child, matches[i]);
        }
    }
};

YAHOO.util.Event.onDOMReady(Tawala.convertTextToLinks);

// Turn on TinyMCE for textareas
tinyMCE.init({
	mode : "textareas",
    theme : "advanced",
	plugins: "paste",

 	// these two lines for absolute urls
 	remove_script_host : false,
	convert_urls : false,

    // Theme options - button# indicated the row# only
//  theme_advanced_buttons1 : "newdocument,|,undo,redo,|,bold,italic,underline,strikethrough,|,forecolor,fontsizeselect,|,justifyleft,justifycenter,justifyright,|,outdent,indent,|,link,unlink,|,pasteword",
    theme_advanced_buttons1 : "bold,italic,underline,|,justifyleft,justifycenter,justifyright,justifyfull,|,fontselect,fontsizeselect,forecolor,|,link,unlink",
	theme_advanced_buttons2 : "",
	theme_advanced_buttons3 : "",
    theme_advanced_toolbar_location : "top",
    theme_advanced_toolbar_align : "left",
    theme_advanced_statusbar_location : "none",
    theme_advanced_resizing : true,
    theme_advanced_resizing_min_height : 50,
	theme_advanced_resize_horizontal : false,
	content_css : "/css/tinymce/custom_content.css",

	convert_newlines_to_brs : false,
	remove_linebreaks : true,
	
    force_br_newlines : true,
    force_p_newlines : false,
    forced_root_block : '', // Needed for 3.x
	
	setup : function(ed) {
		ed.onBeforeSetContent.add(function(ed, o) {
			// Add a <br> at the end of lines that only have a \n
			var temp = o.content.split("\n"), i;
			
			for( i = 0; i < temp.length; i++ ) {
				if( !temp[i].match(/(\<br *\/? *\>|\<\/ *p *\>) *?/)) {
					temp[i] += "<br />";
				}
			}
			o.content = temp.join("\n");
		});
	}
	
});
