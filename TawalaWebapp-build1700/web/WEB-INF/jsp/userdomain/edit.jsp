
<jsp:directive.page import="com.tawala.web.userdomain.EditUserDomainController"/><%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="form" uri="http://www.springframework.org/tags/form" %>

<style>

/*ul { height: 200px; overflow:auto } */
ul#selectedProjects, 
ul#vettedProjects {
	border: 1px solid #aaaaaa; 
	min-height: 4em; 
	padding: .2em 1em 1em 1em; 
	margin-bottom: 1em;
}
ul#selectedProjects li, ul#vettedProjects li {
  margin: 2px;
  padding: .2em;
  cursor: move;
  text-align: left;
}
	ul#selectedProjects li.selected,
	ul#selectedProjects li.unselected {
	  background-color: #D1E6EC;
	  border:1px solid #7EA6B2;
	}
	
	ul#vettedProjects li.selected, 
	ul#vettedProjects li.unselected {
	  background-color: #D8D4E2;
	  border:1px solid #6B4C86;
	}

.block input, .block textarea {
	padding: .2em;
	width: 95%;
/*	border: 1px solid #666688; */
}

</style>
<div class="section">
	<h2 class="sectionHeading">Edit User Domain</h2>
	<div class="sectionContent">
		<c:if test="${form.updateSuccessful}">
		<h3>This user domain has been updated!</h3>
		</c:if>
		
		<form:form commandName="form" method="post" id="editUserDomainForm" onsubmit="return assembleProjectList();">
		
		<form:hidden path="projectIds" id="projectIdsField"/>
		<c:if test="${form.updateSuccessful}">
			<input type="hidden" name="<%= EditUserDomainController.DOMAIN_ID_PARAMETER %>" value="${form.userDomain.id}"/>
		</c:if>
		
		<div class="block">
			<div class="title">Name:</div> 
			<form:input path="userDomain.name" size="80" maxlength="60" title="This is the name that will be part of the URL for the landing page"/>
		</div>
		<div class="block">
			<div class="title">Display Name:</div> 
			<form:input path="userDomain.displayName" size="80" maxlength="60" title="The display name is what will be seen by on the landing page"/>
		</div>
		<div class="block">
			<div class="title">Title:</div> 
			<form:input path="userDomain.title" size="80" maxlength="120" title="This is the main heading on the page"/>
		</div>
		<div class="block">
			<div class="title">Subtitle:</div> 
			<form:input path="userDomain.subtitle" size="80" maxlength="120" title="This is the sub-heading under the main heading"/><br />
		</div>
		<div class="block">
			<div class="title">Featured Solutions Caption:</div> 
			<form:input path="userDomain.featuredSolutionsCaption" size="80" maxlength="120" title="This is the caption just above the 4 featured solution icons"/><br />
		</div>
		<div class="block">
			<div class="title">Description Caption:</div> 
			<form:input path="userDomain.descriptionCaption" size="80" maxlength="120" title="This is the title of the description just above the featured projects list"/><br />
		</div>
		<div class="block">
			<div class="title">Descripton:</div> 
			<form:textarea path="userDomain.description" cols="80" rows="6" title="This is the text of the description just above the featured projects list"/><br />
		</div>
		<div class="block">
			<ul id="selectedProjects" title="These are the projects that will be shown on the page. They will appear in the order listed and the first four will be the featured icons">
				<li id="headerItem1"><b>Drag the projects to be displayed in this area. They will appear in the order listed</b></li>
			<c:forEach var="project" items="${form.userDomain.featuredProjects}">
				<li class="selected" id="project${project.id}"><c:out value="${project.name}"/></li>
			</c:forEach>
			
			</ul>
		</div>
		<div class="block">
			<ul id="vettedProjects" >
				<li id="headerItem2"><b>All System Projects (including under construction)</b></li>
			<c:forEach var="project" items="${allprojects}">
				<li class="unselected" id="project${project.id}"><c:out value="${project.name}"/></li>
			</c:forEach>
			</ul>
		</div>
		<div class="buttons">
			<button type="submit" value="Submit">SUBMIT</button>
			<a href="${urls.listUserDomains}" alt="CANCEL">CANCEL</a>
		</div>
		</form:form>
	</div>
</div>
<script language="javascript">
function assembleProjectList() {
	var selectedList = document.getElementById("selectedProjects");
	var children = selectedList.getElementsByTagName('li');

	var projectIds = new Array();

	var i;
	
	for(i=0; i<children.length;i++) {
		var id = children[i].id;
		if(id.substr(0,7) == 'project') {
			id = id.substr(7);
			projectIds.push(id);
		} else {
			continue;
		}
	}
	
	if(projectIds.length < 4) {
		alert("You selected only " + projectIds.length + " projects. Select at least 4 of them in order to create a valid domain.");
		return false; 
	} else {
		document.getElementById('projectIdsField').value = projectIds.join(',');
		return true;
	}
		
}

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

        if (this.goingUp && destId.match('headerItem') == null) {
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
			new DDList("headerItem1");
			new DDList("headerItem2");
		<c:forEach var="project" items="${form.userDomain.featuredProjects}">
			new DDList("project${project.id}");
		</c:forEach>
		<c:forEach var="project" items="${allprojects}">
	        new DDList("project${project.id}");
        </c:forEach>

            DDM.mode = 0;
        }
    };
} ();

YAHOO.util.Event.addListener(window, "load", DDApp.init);
</script>