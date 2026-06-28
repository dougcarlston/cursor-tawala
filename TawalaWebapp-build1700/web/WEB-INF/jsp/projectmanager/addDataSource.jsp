<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<style>

ul#fieldsContainer {
	border: 1px solid #aaaaaa; 
	min-height: 4em; 
	padding: .2em 1em 1em 1em; 
	margin-bottom: 1em;
	width: 80%;
}
ul#fieldsContainer li {
  margin: 2px;
  padding: .2em;
  text-align: left;
}
ul#fieldsContainer li input {
  width: 60%;
}

ul#fieldsContainer li.selected {
  cursor: move;
}

ul#fieldsContainer li.selected {
  background-color: #D1E6EC;
  border:1px solid #7EA6B2;
}

</style>
<script>
	setPageTitle("Add Data Source");
	
	function onKeyUp(el) {
		if(el.value.length > 0) {
			makeExistingInputSelected(el);
			addNewField();
		}
	}
	
	function makeExistingInputSelected(el) {
		el.onkeyup = new function() {};
		el.parentNode.setAttribute('class', 'selected');
		//--- To satisfy IE's quirkiness..
		el.parentNode.setAttribute('className', 'selected');

		var deleteIcon = document.createElement('a');
		deleteIcon.setAttribute('href', "javascript:deleteSection('" + el.parentNode.id + "')");
		deleteIcon.setAttribute('style', 'float: right;');
		deleteIcon.style.cssText = 'float: right;';
		
		var image = document.createElement('img');
		image.setAttribute('src', '/images/silk/cross.gif');
		image.setAttribute('width', '16');		
		image.setAttribute('height', '16');	
		image.setAttribute('alt', 'Delete this field');
		deleteIcon.appendChild(image);

		el.parentNode.insertBefore(deleteIcon, el);
		
		new DDList(el.parentNode.id);
	}
	
	function addNewField() {
		var container = document.getElementById("fieldsContainer");
		
		var sectionId = 'field' + nextFieldId;
		var inputFieldId = 'inputField' + nextFieldId;
		nextFieldId = nextFieldId + 1;
		
		var newItem = document.createElement('li');
		newItem.setAttribute('id', sectionId);
		newItem.setAttribute('class', 'unselected');
		
		var input = document.createElement('input');
		input.setAttribute('type', 'text');
		input.setAttribute('name', 'field');
		input.setAttribute('id', inputFieldId );
		input.setAttribute('onKeyUp', 'onKeyUp(this)');
		
		newItem.appendChild(input);
		container.appendChild(newItem);
		
		input = document.getElementById(inputFieldId);
		//--- A bit of browser detection. There might be a better way of doing this, esp. setAttribute.
		if(input.addEventListener) {
			input.setAttribute('onKeyUp', 'onKeyUp(this)');
		} else if(input.attachEvent) {
			input.onkeyup = function() { onKeyUp(window.event.srcElement) };
		} 
		
	}
	
	function deleteSection(sectionId) {
		var el = document.getElementById(sectionId);
		el.parentNode.removeChild(el);
	}
	
	var nextFieldId = ${fn:length(formBean.fieldNames)} + 1;

	(function() {
	
	var Dom = YAHOO.util.Dom;
	var Event = YAHOO.util.Event;
	var DDM = YAHOO.util.DragDropMgr;
	
	DDList = function(id, sGroup, config) {
	
	    if (id) {
	        this.init(id, sGroup, config);
	        this.initFrame();
	        this.logger = this.logger || YAHOO;
	    }
	
	    var el = this.getDragEl();
	    Dom.setStyle(el, "opacity", 0.67);
	
	    this.setPadding(-4);
	    this.goingUp = false;
	    this.lastY = 0;
	};
	
	YAHOO.extend(DDList, YAHOO.util.DDProxy, {
	    clickValidator: function(e) {
	        var target = Event.getTarget(e);
	        if(! this.isValidHandleChild(target)) {
	        	return false;
	        }
	        if(target.name && target.name == 'field') {
	        	return false;
	        }
	        return this.id == this.handleElId || 
	                        this.DDM.handleWasClicked(target, this.id);
    	},
	
	    startDrag: function(x, y) {
	        this.logger.log(this.id + " startDrag");
	
	        var dragEl = this.getDragEl();
	        var clickEl = this.getEl();
	        Dom.setStyle(clickEl, "visibility", "hidden");
	
	        dragEl.innerHTML = clickEl.innerHTML;
	
	
	        Dom.setStyle(dragEl, "color", Dom.getStyle(clickEl, "color"));
	        Dom.setStyle(dragEl, "backgroundColor", Dom.getStyle(clickEl, "backgroundColor"));
	        Dom.setStyle(dragEl, "border", "2px solid gray");
	    },
	
	    endDrag: function(e) {
	
	        var srcEl = this.getEl();
	        var proxy = this.getDragEl();
	        Dom.setStyle(proxy, "visibility", "visible");
	
	        // animate the proxy element to the src element's location
	        var a = new YAHOO.util.Motion( 
	            proxy, { 
	                points: { 
	                    to: Dom.getXY(srcEl)
	                }
	            }, 
	            0.3, 
	            YAHOO.util.Easing.easeOut 
	        )
	        var proxyid = proxy.id;
	        var id = this.id;
	        a.onComplete.subscribe(function() {
	                Dom.setStyle(proxyid, "visibility", "hidden");
	                Dom.setStyle(id, "visibility", "");
	            });
	        a.animate();
	
	
	    },
	
	    onDragDrop: function(e, id) {
	    },
	
	    onDrag: function(e, id) {
	
	        // figure out which direction we are moving
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
	        var destEl;
	
	        if ("string" == typeof id) {
	            // POINT mode
	            destEl = Dom.get(id);
	        } else { 
	            // INTERSECT mode
	            destEl= YAHOO.util.DDM.getBestMatch(id).getEl();
	        }
	        var p = destEl.parentNode;
	        
	        var destId = destEl.id;
	
	        if (this.goingUp) {
	            p.insertBefore(srcEl, destEl);
	        } else {
	            p.insertBefore(srcEl, destEl.nextSibling);
	        }
	
	        DDM.refreshCache();
	    },
	
	    onDragEnter: function(e, id) {
	    },
	
	    onDragOut: function(e, id) {
	    },
	
	    toString: function() {
	        return "DDList " + this.id;
	    }
	
	});
	
	})();

	DDApp = function() {
	    var Dom = YAHOO.util.Dom;
	    var DDM = YAHOO.util.DragDropMgr;
	    return {
	        init: function() {
			<c:forEach var="field" items="${formBean.fieldNames}" varStatus="status">
				new DDList('field${status.index}');
			</c:forEach>
	
	            DDM.mode = 0;
	        }
	    };
	} ();

YAHOO.util.Event.addListener(window, "load", DDApp.init);
</script>

<div class="sectionContent">

<form:form method="POST" commandName="formBean" id="addDataSourceForm">
	<div class="title">Data Source Name: <form:errors path="dataSourceName" cssClass="error"/></div>
	<form:input path="dataSourceName" cssStyle="width: 50%;" /> <br />
	<br />
	<div class="title">Fields<sup>*</sup>:</div> <form:errors path="fieldNames" cssClass="error"/>
	<ul id="fieldsContainer">
		<c:forEach var="field" items="${formBean.fieldNames}" varStatus="status">
		<li id="field${status.index}" class="selected">
			<a href="javascript:deleteSection('field${status.index}')" style="float: right;"><img src="/images/silk/cross.gif" width="16" height="16" alt="Delete this field"/></a>
			<input type="text" name="field" value="<c:out value="${field}" />"/>
			<form:errors path="fieldNames[${status.index}]" cssClass="error"/>
		</li>
		</c:forEach>
		<li id="field${fn:length(formBean.fieldNames)}" class="unselected">
		<input type="text" name="field" onkeyup="onKeyUp(this)" value="" />
		</li>
	</ul>
	<br />
	<input type="submit" value="Create Data Source" />
</form:form>

<br />
<sup>*</sup> A field name should start with a letter or an underscore, cannot contain the colon character or start with two underscores.


</div>
