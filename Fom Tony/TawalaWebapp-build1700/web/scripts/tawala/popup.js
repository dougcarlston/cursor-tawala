/*
 * requires prototype.js and Yahoo UI library
*/
Tawala.Popup = new function() {};

Tawala.Popup.Confirm = new function() {
    var isOpen = false;
    var target;
    var targetRegion;
    var popup;
    var popupRegion;
    var action;
    var hiddenFields;
    var formAction;

    this.init = function() {
        var elems = YAHOO.util.Dom.getElementsByClassName("confirm");

        for(var i in elems){
        	if(elems[i].nodeType == 1 && elems[i].tagName.toLowerCase() == "form"){
        		elems[i].setAttribute("onclick", "return false;");
            	YAHOO.util.Event.addListener(elems[i], "click", Tawala.Popup.Confirm.showPopup);
            }
        }
    }

    this.showPopup = function(e) {
        if(isOpen){hidePopup();}
        target = getEventTarget(e);
        targetRegion = YAHOO.util.Region.getRegion(target);
        var xy = YAHOO.util.Dom.getXY(target);
        var tH = target.offsetHeight;
        var tW = target.offsetWidth;

        cTarget = YAHOO.util.Dom.findElement(e, "form");

        var hiddenFields = getHiddenFields(cTarget);
        if(hiddenFields["action"]) {
            action = hiddenFields["action"];
        }else{
            return false;
        }
        
        var popupId = "" + action.toLowerCase() + "Confirm";
        popup = document.getElementById(popupId);
		
        YAHOO.util.Dom.setStyle(popup, "display", "block");
		popup.style.zindex = 10;
        popupForm = popup.getElementsByTagName("form")[0];
				
        if(cTarget.getAttribute("action") != null) {
        	popupForm.setAttribute("action", cTarget.getAttribute("action"));
        }

        removeHiddenFields(popup.getElementsByTagName("form")[0]);
        addHiddenFields(popupForm, hiddenFields);

        var dim = YAHOO.util.Dom.getDimensions(popup);
        pH = dim.height;
        pW = dim.width;

        var newXY = new Array((xy[0] - (pW / 2)), (xy[1]) - tH);

		var scrollXY = getScrollXY();
				
		var windowXY = new Array();;
		windowXY[0] = YAHOO.util.Dom.getViewportWidth() + scrollXY[0];
		windowXY[1] = YAHOO.util.Dom.getViewportHeight() + scrollXY[1];
		
		if((newXY[0] + pW) >= windowXY[0]) newXY[0] = windowXY[0] - pW;
		if((newXY[1] + pH) >= windowXY[1]) newXY[1] = windowXY[1] - pH;
		if(newXY[0] <= scrollXY[0]) newXY[0] = scrollXY[0];
		if(newXY[1] <= scrollXY[1]) newXY[1] = scrollXY[1];
 		
        YAHOO.util.Dom.setXY(popup, newXY);
        YAHOO.util.Dom.setStyle(popup, "visibility", "visible");
        popupRegion = YAHOO.util.Region.getRegion(popup);

        YAHOO.util.Event.addListener(document, "mousemove", Tawala.Popup.Confirm.mouseMove);
        isOpen = true;
        initButtonActions(e);
        $E.stopEvent(e);
        return false;
    }

    initButtonActions = function(e) {
        popupForm = popup.getElementsByTagName("form")[0];
        popupElements = popupForm.elements;

        for( var i = 0; i < popupElements.length; i++){

            if(popupElements[i] &&
                   popupElements[i].nodeType == 1 &&
                   popupElements[i].tagName.toLowerCase() == "input" ) {
            	if(popupElements[i].type.toLowerCase() == "submit") {
                	YAHOO.util.Event.addListener(popupElements[i], "click", Tawala.Popup.Confirm.submit);
                }
                if(popupElements[i].value.toLowerCase() == "cancel") {
                    YAHOO.util.Event.addListener(popupElements[i], "click", Tawala.Popup.Confirm.cancel);
                }
           }
        }
    }

    this.submit = function(e) {
        if(isOpen) {
            YAHOO.util.Event.removeListener(target, "click", Tawala.Popup.Confirm.submit);
            hidePopup();
            return true;
        }
    }

    this.cancel = function() {
        if(isOpen){
            hidePopup();
            return false;
        }
    }

    getHiddenFields = function(formId){
        var found = YAHOO.util.Dom.getElementsByAttribute("type", "hidden", "input", formId);
        var hidden = new Object();
        for(var i = 0; i < found.length; i++){
            hidden[found[i].name] = found[i].value;
        }
        return hidden;
    }

    addHiddenFields = function(form, hf) {
        for( var f in hf){
            newInput = createHtmlElement("input", {"type":"hidden", "name":f, "value":hf[f]});
            form.appendChild(newInput);
        }
    }

    removeHiddenFields = function(form) {
        var found = YAHOO.util.Dom.getElementsByAttribute("type", "hidden", "input", form);
        for(i=0; i < found.length; i++){
            form.removeChild(found[i]);
        }
    }

    hidePopup = function(e) {
        isOpen = false;
        YAHOO.util.Dom.setStyle(popup, "visibility", "hidden");
        YAHOO.util.Dom.setStyle(popup, "display", "none");
        YAHOO.util.Event.removeListener(document, "mousemove", Tawala.Popup.Confirm.mouseMove);
    }

    this.mouseMove = function(e) {
        if(isOpen){
        	scrollXY = getScrollXY();
            mX = e.pageX?e.pageX : e.clientX + scrollXY[0];
            mY = e.pageY?e.pageY : e.clientY + scrollXY[1];
            var mousePt = new YAHOO.util.Point(mX, mY);
            if(!targetRegion.contains(mousePt) && !popupRegion.contains(mousePt)) {
                hidePopup();
            }
        }
    }

	findScroll = function(obj) {
		var so;
		if(obj.scrollTop != 0 || obj.scrollLeft != 0) {
			// return null if obj is top level HTML obj
			// otherwise return the object
			if(obj.tagName && obj.tagName.toLowerCase() != "html") { 
				return obj; 
			}else{
				return null;
			}
		}else{
			if(obj.parentNode){
				so = findScroll(obj.parentNode);		
			}
		}
		return so;
	}

};

YAHOO.util.Dom.getElementsByAttribute = function(atr, val, tag, root) {
	var method = function(el) {
		var re = new RegExp('(?:^|\\s+)' + val + '(?:\\s+|$)');
		if ( el.getAttribute(atr) && re.test(el.getAttribute(atr)) ) {
			return true;
		}
		return false;
	};
	return this.getElementsBy(method, tag, root);
};

// functions stolen from Prototype and converted to YUI for conveinience
YAHOO.util.Dom.findElement = function(event, tagName) {
	var element = YAHOO.util.Event.getTarget(event);
	while (element.parentNode && (!element.tagName ||
		(element.tagName.toUpperCase() != tagName.toUpperCase())))
	element = element.parentNode;
	return element;
}

YAHOO.util.Dom.getDimensions = function(element) {
	element = $(element);
	if (YAHOO.util.Dom.getStyle(element, 'display') != 'none')
		return {width: element.offsetWidth, height: element.offsetHeight};
	
	// All *Width and *Height properties give 0 on elements with display none,
	// so enable the element temporarily
	var els = element.style;
	var originalVisibility = els.visibility;
	var originalPosition = els.position;
	els.visibility = 'hidden';
	els.position = 'absolute';
	els.display = '';
	var originalWidth = element.clientWidth;
	var originalHeight = element.clientHeight;
	els.display = 'none';
	els.position = originalPosition;
	els.visibility = originalVisibility;
	return {width: originalWidth, height: originalHeight};
}

// Add listeners
YAHOO.util.Event.addListener(window, "load", Tawala.Popup.Confirm.init);


