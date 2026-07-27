<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>
<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="tawala" uri="http://www.tawala.com/tags" %>

<script>
	setPageTitle("Sample Data: ${project.name} - v${version.versionNumber}");
</script>

<div class="section">
	<div id="formList">
		<table class="list ruler">
			<col style="width: 180px;" />
			<col  />
			<col style="width: 100px;" />
			<thead>
				<tr>
					<th class="left">Form Name</th>
					<th class="left">Status</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
		<c:forEach var="form" items="${version.project.forms}">
			<c:set var="formData" value="${submissions[form]}" />
			<tr>
				<td class="left">${form.name}:  </td>
				<td class="left">
					<c:choose>
						<c:when test="${empty formData}">No data recorded</c:when>
						<c:otherwise>
							<c:forEach var="submission" items="${formData}" varStatus="status">
								Submission #${status.count}<br />
							</c:forEach>
						</c:otherwise>
					</c:choose>
				</td>
				<td>
					<c:if test="${form.startingPoint}">
						<div id="addDataSelect">
							<a href="#" onclick="addSampleData('<tawala:linkToSampleCollectingProject 
									project="${userProject}" form="${form}" />');">
								[add data]
							</a>
						</div>
					</c:if>
				</td>
			</tr>
		</c:forEach>
			</tbody>
		</table>
	</div>
</div>

<div class="section">
	<div id="addDataControls" style="display: none; border-top: 1px solid #cccccc;">
		<br />
		<div class="controls">
			<button type="button" onclick="doneAddingData();">
				Done
			</button>
		</div>
	</div>
	<div id="iContainer" style="display: none; padding-top: 2.4em;">
	</div>
</div>

<script type="text/javascript">
    var containerName = "iContainer";
    var iframeName = "newIframe";

    function addSampleData(projectUrl) {
        if(!document.getElementById(iframeName)) {
            var container = document.getElementById(containerName);
            addIframe(iframeName, containerName, projectUrl);
            container.style.display = "block";
            document.getElementById("formList").style.display = "none";
            document.getElementById("addDataControls").style.display = "block";
        }
    }
    function removeIframe(iframeId) {
        var myIframe = document.getElementById(iframeId);
        myIframe.parentNode.removeChild(myIframe);
    }

    function addIframe(iframeName, targetElement, contentSrc){
        var newIframe = document.createElement("iframe");
        newIframe.setAttribute("id", iframeName);
        newIframe.setAttribute("width", "99%");
        newIframe.setAttribute("height", "400");
        newIframe.setAttribute("src", contentSrc);
        var container = document.getElementById(targetElement);
        container.appendChild(newIframe);
    }

    function doneAddingData(){
        var update = document.createElement("div");
        update.style.textAlign = "center";
        update.style.fontSize = "1.4em";
        update.style.color = "#cc4400";
        var textNode = document.createTextNode("Updating...");
		update.appendChild(textNode);
        var myIframe = document.getElementById("newIframe");
        myIframe.parentNode.insertBefore(update, myIframe);

        removeIframe("newIframe");
        document.getElementById("addDataControls").style.display = "none";
        window.location.reload();
    }
</script>
		