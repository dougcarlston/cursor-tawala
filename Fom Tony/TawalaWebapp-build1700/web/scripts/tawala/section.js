Tawala.Section = new function(){
}
/*
Tawala.Section.Controls = new function(){
	this.init = function() {
		var controls = YAHOO.util.Dom.getElementsByClassName("controls");
		for( var i = 0; i < controls.length; i++) {
			if(controls[i].nodeType == 1 && YAHOO.util.Dom.hasClass(controls[i], "hide")){
				controls[i].style.display = "none";
			}
		}

		var sections = YAHOO.util.Dom.getElementsByClassName("section");
		for( i = 0; i < sections.length; i++) {
			sections[i].setAttribute("onmouseover", "Tawala.Section.mouseIn(this)");
			sections[i].setAttribute("onmouseout", "Tawala.Section.mouseOut(this)");
		}

	}
		
	this.mouseIn = function(section){
		var controls = YAHOO.util.Dom.getElementsByClassName("controls", "", section);
		if(controls.length > 0 && controls[0].nodeType == 1 && YAHOO.util.Dom.hasClass(controls[0], "hide")) {
			for( var i = 0; i < controls.length; i++) {
				controls[0].style.display = "block"; 
			}
		}
	}

	this.mouseOut = function(section){
		var controls = YAHOO.util.Dom.getElementsByClassName("controls", "", section);
		if(controls.length > 0 && controls[0].nodeType == 1 && YAHOO.util.Dom.hasClass(controls[0], "hide")) {
			for( var i = 0; i < controls.length; i++) {
				controls[0].style.display = "none";
			}
		}
	}
*/

Tawala.Section.Collapsible = new function() {
    this.init = function() {
        var collapsibleSections = $D.getElementsByClassName("collapsible");

        for(var i=0; i < collapsibleSections.length; i++){
            var h5Tag = $D.getElementsByClassName("sectionHeading", "", collapsibleSections[i]);
                       
            h5Tag[0].setAttribute("title", "Click heading to toggle view");
            h5Tag[0].style.paddingLeft= ".8em";
			h5Tag[0].style.cursor = "pointer";
            $D.addClass(h5Tag[0], "arrowDown");
            $E.addListener(h5Tag[0], "click", Tawala.Block.toggleVisible);
        }
    }

    this.toggleVisible = function(e){
        if(e.srcElement){
            var elem = e.srcElement;
        }else{
            var elem = e.target;
        }
        var target = YUD.getElementsByClassName("sectionContent", "div", elem.parentNode);
        toggleArrow(elem);
        if(target[0].style.display == "none") {
            target[0].style.display = "block";
        }else{
            target[0].style.display = "none";
        }
    }
	
    toggleArrow = function(elem) {
        if($D.hasClass(elem, "arrowDown")){
            $D.replaceClass(elem, "arrowDown", "arrowRight");
        }else{
            $D.replaceClass(elem, "arrowRight", "arrowDown");
        }
    }
}	

// YAHOO.util.Event.addListener(window, "load", Tawala.Section.Controls.init);
$E.addListener(window, "load", Tawala.Section.Collapsible.init);
