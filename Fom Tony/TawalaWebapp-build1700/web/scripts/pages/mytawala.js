function init() {

	var panelConfig = {	width:"600px",
						height: "450px", 
						fixedcenter: true, 
						constraintoviewport: true, 
						visible:false, 
						close: false,
						modal: true,
//						effect:{effect:YAHOO.widget.ContainerEffect.FADE,duration:1},
						draggable: false}
	
	var panel1 = new YAHOO.widget.Dialog("myOverlay", panelConfig);
																					
	panel1.render();

	var panel2 = new YAHOO.widget.Dialog("myOverlay2", panelConfig);						
	panel2.render();

	var closeHandler = function(e, o){
		o.hide();
	}

	YAHOO.util.Event.on('myOverlay', 'click', closeHandler, panel1);
	YAHOO.util.Dom.setStyle('myOverlay', 'cursor', 'pointer');

	YAHOO.util.Event.on('myOverlay2', 'click', closeHandler, panel2);
	YAHOO.util.Dom.setStyle('myOverlay2', 'cursor', 'pointer');

	YAHOO.util.Event.addListener("myTawalaLink", "click", panel1.show, panel1, true);
	YAHOO.util.Event.addListener("projectDetailsLink", "click", panel2.show, panel2, true);			
}

YAHOO.util.Event.addListener(window, "load", init);
