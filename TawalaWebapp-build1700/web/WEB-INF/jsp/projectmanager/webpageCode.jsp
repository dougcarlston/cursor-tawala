<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

<script>
	setPageTitle("${project.name}");
</script>
<script type="text/javascript">

<!-- <% // Partially modelled based on Google's Calendar %>
function changeIFrameSource() {
  var form = $('embedParameterForm');

  var width = form.elements['width'].value;
  var height = form.elements['height'].value;
  var border = form.elements['border'].checked;
  var  url = form.elements['url'].value;

  var props = [
    'style="' +  (border ? 'border:solid 1px #777' : 'border-width:0') + '"',
    'width="' + width + '"',
    'height="' + height + '"',
    'frameborder="0"'
  ];

  // Show & render the html code
  var code = '<iframe src="' + url + '" ' + props.join(' ') + '></iframe>';
  $('code-source').value = code;
  $('embeddedProjectDiv').style.display = 'block';
  $('embeddedProject').innerHTML = code;
}

//-->
</script>

<p><a href="/projectmanager/projectdetail?projectName=${project.name}">[return to project details]</a></p>

<div class="section" >
	<h3>Incorporate this project into a web page</h3>
	<p>
	We provide two ways to access your project from another website. The first options provides a link to the project which will open
	in a new window. The second option allows you to add the project as an element on a web page.
	</p>
	<br />
	<h5>Option 1: Link to this project from another web page</h5>
	<p>
		Cut and paste the link below to access this project from another webpage. (You may want to change the 
		description in the link):
	</p>
	<br />
	<c:forEach var="urlEntry" items="${project.entryPointURLs}" >
		<c:set var="form" value="${urlEntry.key}"/>
		<c:set var="url" value="${urlEntry.value}"/>
		&lt;a href="<c:out value="${url}" />" target="_blank" &gt;<c:out value="${project.name}"/>&lt;/a&gt;<br/>
	</c:forEach>
</div>

<div class="section" >
	<h5>Option 2: Integrate this project in another web page</h5>
	<p>
		Use the settings below to change the attributes of the how the project will be displayed. 
		(Make sure that the web site you're integrating this into supports &lt;iframe&gt;s.)
	</p>
	<br />
	<form action="" onsubmit="try{changeIFrameSource()}finally{return false}" id="embedParameterForm">
		<table class="edit">
			<col style="width: 140px;" />
			<col style="width: 60px;" />
			<col />
			<tr>
				<td class="label">Project Starting Point: </td>
				<td class="left" colspan="2">
					<select onchange="changeIFrameSource()" name="url" onkeydown="if(event.keyCode==13)changeIFrameSource()">
						<c:forEach var="urlEntry" items="${project.entryPointURLs}" >
							<c:set var="form" value="${urlEntry.key}"/>
							<c:set var="url" value="${urlEntry.value}"/>
							<option value="<c:out value="${url}" />"><c:out value="${form.name}"/></option>
						</c:forEach>
					</select>
				</td>
			</tr>
			<tr><td></td><td></td><td></td></tr>
			<tr>
				<td class="label">IFrame Size</td>
				<td class="label">
					Width: 
				</td>
				<td class="left">
					<input type="text" onchange="changeIFrameSource()" id="width" name="width" value="700" size="5"> pixels
				</td>
			</tr>
			<tr>
				<td></td>			
				<td class="label">
					Height: 
				</td>
				<td class="left">
					<input type="text" onchange="changeIFrameSource()" id="height" name="height" value="400" size="5"> pixels
				</td>
			</tr>
			<tr>
				<td></td>
				<td class="label">
					Border: 
				</td>
				<td class="left">
					<input type="checkbox" onchange="changeIFrameSource()" id="border" name="border">
				</td>
			</tr>
			<tr>
				<td></td>
				<td colspan="2"><button onclick="changeIFrameSource()">Update</button></td>
			</tr>
		</table>
	</form>

	<br />
	<p>
		Copy and paste the following HTML into your page:
	<br />
	<textarea rows="6" cols="90" name="code-source" id="code-source"></textarea>
	</p>
	<br /><br />
	<div style="display: none" id="embeddedProjectDiv">
		And your project should look like this: <br /><br />
		<div id="embeddedProject" style="overflow: scroll; padding: 0.3em;"></div>
	</div>

</div>
