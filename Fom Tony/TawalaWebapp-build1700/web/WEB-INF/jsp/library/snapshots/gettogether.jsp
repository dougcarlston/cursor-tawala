<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

	<div id="snapshots" class="section collapsible">
		<h3 class="sectionHeading">Snapshots</h3>
		<div class="sectionContent">
			<p style="font-size: .9em; padding-bottom: .5em;">Click the thumbnails below for a larger image</p>
			<div id="snap1" style="margin-top: 1em;">
				<div style="float: left; width: 510px;">
					<img style="border: 1px solid #dddddd;"src="/images/snapshots/gettogether-setup-thumb.gif" alt="" />
				</div>
				<div style="font-size: .9em; color: #666666; margin-left: 1em; float: left; width: 135px;">
					This is the  setup screen for the Get Together project.
				</div>
			</div>
			<div id="snap2" class="" style="clear: both; padding-top: 1em;">
				<div style="font-size: .9em; color: #666666; margin-right: 1em; float: left; width: 135px;">
					Here's an example of what the Get Together project might look like after setup.
				</div>
				<div style="float: right; width: 510px;">
					<img style="border: 1px solid #dddddd;"src="/images/snapshots/gettogether-mainscreen-thumb.gif" alt="" />
				</div>
			</div>
			<div id="snap3" style="clear: both; padding-top: 1em;">
				<div style="float: left; width: 510px;">
					<img style="border: 1px solid #dddddd;"src="/images/snapshots/gettogether-report-thumb.gif" alt="" />
				</div>
				<div style="font-size: .9em; color: #666666; margin-left: 1em; float: left; width: 135px;">
					Here's the report screen for the Get Together project.
				</div>
			</div>
			<br style="clear:both;" />
			
<%--
			<table style="border-collapse: collapse;">
				<col style="width: 310px;" />
				<col style="width: 310px;" />
				<col />
				<tbody>
					<tr>
						<td id="snap1" style="vertical-align: top; text-align: left; cursor: pointer;">
							<img style="border: 1px solid #dddddd;"src="/images/snapshots/gettogether-snapshot1-thumb.gif" alt="" />
						</td>
						<td id="snap2" style="vertical-align: top; text-align: left; cursor: pointer;">
							<img style="border: 1px solid #dddddd;" src="/images/snapshots/gettogether-snapshot1-thumb.gif" alt="" />
						</td>
						<td><br /></td>
					</tr>
					<tr>
						<td style="text-align: left;">	
							<span style="font-size: .9em; color: #888888;">
								Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin diam. In 
								tincidunt dui vel enim. Donec eleifend. Suspendisse id lorem. Curabitur 
								pretium, tellus at sagittis ornare, nulla turpis pharetra ligula, pulvinar 
								dignissim orci nulla sed enim.
							</span>
						</td>
						<td style="text-align: left;">
							<span style="font-size: .9em; color: #888888;">
								Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin diam. In 
								tincidunt dui vel enim. Donec eleifend. Suspendisse id lorem. Curabitur 
								pretium, tellus at sagittis ornare, nulla turpis pharetra ligula, pulvinar 
								dignissim orci nulla sed enim.
							</span>
						</td>
						<td><br /></td>
					</tr>
				</tbody>
			</table>
--%>
		</div>
	</div>

	<div id="snapshot1" style="visibility:hidden; cursor: pointer;">
		<div class="hd"><div class="tl"></div><span></span><div class="tr"></div></div>
		<div class="bd" style="background-color: #dddddd;">
			<img src="/images/snapshots/gettogether-setup.gif" />
			<p>
				The setup screen for the Get Together project.
			</p>
		</div>
		<div class="ft">Click image to close window</div>
	</div>
	
	<div id="snapshot2" style="visibility:hidden; cursor: pointer;">
		<div class="hd"><div class="tl"></div><span></span><div class="tr"></div></div>
		<div class="bd" style="background-color: #dddddd;">
			<img src="/images/snapshots/gettogether-mainscreen.gif" />
			<p>
				An example of the main screen of the Get Together project after setup.
			</p>
		</div>
		<div class="ft">Click image to close window</div>
	</div>

	<div id="snapshot3" style="visibility:hidden; cursor: pointer;">
		<div class="hd"><div class="tl"></div><span></span><div class="tr"></div></div>
		<div class="bd" style="background-color: #dddddd;">
			<img src="/images/snapshots/gettogether-report.gif" />
			<p>
				An example of the report screen of the Get Together project after setup.
			</p>
		</div>
		<div class="ft">Click image to close window</div>
	</div>
		
<script>
	var snapshot1;
	var snapshot2;
	var snapshot3;
	
	function snapshotInit(){
		// Build overlays based on markup, initially hidden, fixed to the center of the viewport, and 300px wide
		snapshot1 = new YAHOO.widget.Panel("snapshot1", 
					{ fixedcenter:true, 
					  	visible:false,
						draggable: false,
						effect:{effect:YAHOO.widget.ContainerEffect.FADE, duration:.25},
						underlay:"none",
						width: "640px",
						modal: true } );
						
		snapshot1.body.style.textAlign = "center";
		snapshot1.footer.style.height = "2em";
		snapshot1.render();
		
		YAHOO.util.Event.addListener("snap1", "click", 
										snapshot1.show, 
										snapshot1, true);
	
		YAHOO.util.Event.addListener("snapshot1", "click", 
										snapshot1.hide, 
										snapshot1, true);
	
		snapshot2 = new YAHOO.widget.Panel("snapshot2", 
					{ fixedcenter:true, 
						visible:false, 
						draggable: false,
						effect:{effect:YAHOO.widget.ContainerEffect.FADE, duration:.25},
						underlay:"none",
						width: "640px",
						modal: true } );
						
		snapshot2.body.style.textAlign = "center";
		snapshot2.footer.style.height = "2em";
		snapshot2.render();
		
		YAHOO.util.Event.addListener("snap2", "click", 
										snapshot2.show, 
										snapshot2, true);
	
		YAHOO.util.Event.addListener("snapshot2", "click", 
										snapshot2.hide, 
										snapshot2, true);

		snapshot3 = new YAHOO.widget.Panel("snapshot3", 
					{ fixedcenter:true, 
						visible:false, 
						draggable: false,
						effect:{effect:YAHOO.widget.ContainerEffect.FADE, duration:.25},
						underlay:"none",
						width: "640px",
						modal: true } );
						
		snapshot3.body.style.textAlign = "center";
		snapshot3.footer.style.height = "2em";
		snapshot3.render();
		
		YAHOO.util.Event.addListener("snap3", "click", 
										snapshot3.show, 
										snapshot3, true);
	
		YAHOO.util.Event.addListener("snapshot3", "click", 
										snapshot3.hide, 
										snapshot3, true);
	}

	YAHOO.util.Event.addListener(window, "load", snapshotInit);
</script>