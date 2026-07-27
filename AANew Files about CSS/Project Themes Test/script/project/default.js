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
var Dom = YAHOO.util.Dom;
var Event = YAHOO.util.Event;
var $ = Dom.get;

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
}

Tawala.Tables = new function(){

    this.init = function() {
        // Find all tables with class sortable, stripe and rule
        if (!document.getElementsByTagName) return;
        tables = document.getElementsByTagName("table");
        for (ti=0;ti<tables.length;ti++) {
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
        }
    }

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
    }
    
};

Tawala.Tables.Sort = new function(){
	var headingRow;
	var columnIndex;
	
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

		// Sort table on first column
		this.resortTable(null, headingRow.cells[0], 1)
    }

    this.resortTable = function(e, obj, sortOrder) {
		var ARROW;
        var span;
		
        for (var ci=0;ci<obj.childNodes.length;ci++) {
            if (obj.childNodes[ci].tagName && obj.childNodes[ci].tagName.toLowerCase() == 'span') span = obj.childNodes[ci];
        }

        var td = obj;

		// Workaround for Safari cellIndex bug
		// should just be: td.cellIndex
		columnIndex = -1; 
		for (var i = 0; i < td.parentNode.cells.length; i++) { 
			if (td === td.parentNode.cells[i]) { 
				columnIndex = i; 
			} 
		}

        var table = getParent(td,'table');

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
				
    			if (!td.getAttribute("sortdir") && columnIndex == 0){
                    ARROW = '&nbsp;&nbsp;&uarr;';
                    td.setAttribute('sortdir','up');
				}else{
	                if (td.getAttribute("sortdir") == 'up') {
	                    ARROW = '&nbsp;&nbsp;&darr;';
	                    newRows.reverse();
	                    td.setAttribute('sortdir','down');
	                } else {
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
    }

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
    }

    _sort_currency = function (a,b) {
        aa = getInnerText(a.cells[SORT_ORDER_INDEX]).replace(/[^0-9.]/g,'');
        bb = getInnerText(b.cells[SORT_ORDER_INDEX]).replace(/[^0-9.]/g,'');
        return parseFloat(bb) - parseFloat(aa);
    }

    _sort_numeric = function (a,b) {
        aa = parseFloat(getInnerText(a.cells[SORT_ORDER_INDEX]));
        if (isNaN(aa)) aa = 0;
        bb = parseFloat(getInnerText(b.cells[SORT_ORDER_INDEX]));
        if (isNaN(bb)) bb = 0;
        return aa-bb;
    }

	// value is in the alt atribute of the rating image
    _sort_rating = function (a,b) {
    	if(! a.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0] ||
	    	! b.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0]){return 0;}
        aa = parseFloat(a.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0].alt);
        bb = parseFloat(b.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0].alt);
        if (isNaN(aa)) aa = 0;
        if (isNaN(bb)) bb = 0;
        return bb-aa;
    }

	_sort_image = function (a,b){
		var aa = 0;
		var bb = 0;
    	if(a.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0]) aa = 1;
	    if(b.cells[SORT_ORDER_INDEX].getElementsByTagName("IMG")[0]) bb = 1;
		return aa-bb;
	}
	
    _sort_caseinsensitive = function (a,b) {
        aa = getInnerText(a.cells[SORT_ORDER_INDEX]).toLowerCase();
        bb = getInnerText(b.cells[SORT_ORDER_INDEX]).toLowerCase();
        if (aa==bb) return 0;
        if (aa<bb) return -1;
        return 1;
    }

    _sort_default = function (a,b) {
        aa = getInnerText(a.cells[SORT_ORDER_INDEX]);
        bb = getInnerText(b.cells[SORT_ORDER_INDEX]);
        if (aa==bb) return 0;
        if (aa<bb) return -1;
        return 1;
    }
} // end Tawala.Tables.Sort


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
    }

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
    }
} // end Tawala.Tables.Stripe


/********************************
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
}

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

/*************************************
 * Add events to the page
 */
Event.addListener(window, "load", Tawala.Tables.init);
Event.addListener(window, "load", Tawala.fixTemplates);
