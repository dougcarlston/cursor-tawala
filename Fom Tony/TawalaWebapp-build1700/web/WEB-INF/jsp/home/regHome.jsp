<%@ taglib prefix="tiles" uri="http://jakarta.apache.org/struts/tags-tiles" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fmt" uri="http://java.sun.com/jsp/jstl/fmt" %>
<%@ taglib prefix="spring" uri="http://www.springframework.org/tags" %>

<tiles:importAttribute />

<div class="yui-gc">

	<div class="yui-u first">
		<div class="contentBox">
			<div id="news" class="news section hover">
				<div class="controls hide">
					<a class="action" href="/news">Read More...</a>
				</div>
				<div class="contentHeading">Tawala News</div>
				<div class="newsItem">
					<div class="title">Pumpkins and traffic don't mix</div>
					<div class="info">10/23/2006 - Posted By: tonyf</div>
					<p>
					You can always tell it's October in Sonoma county by the backup on the freeway from Petaluma to Cotati.
					For some reason everyone feels they need to slow down to look at the pumpkin patch at the north end
					of Petaluma. They do have a great corn maze but there's really not that much to look at. A few days after 
					Halloween (once they plow it all under) traffic magically returns to normal.
					</p>
					<br />
					<p>
					Well, we do have a few things to see in this new release (hopefully, without slowing down!).
					The past week or two we've been trying to get the formating in Forms completed. We've made good progress
					but there are still a couple of more things to finish up. We've also been working adding some server-side 
					caching to try to make performance of projects better.
					</p>
					<br />
					<p>
					This next release should add the final touches to the formating features in Forms. A slew of bugs
					will be dealt with and hopefully will clear up some irritating problems. Finally, the new site design
					is just about done. Barring any last minute problems we'll be releasing it as well in the next build.
					In addition to giving the site a new clean look we've rearranged thing a bit to make finding and developing
					projects a little easier. At least we think so. I'm sure you'll let us know!
					</p>
				</div>		
			</div>
			
			<div id="changes" class="news section hover">
				<div class="controls hide">
					<a class="action" href="/whatsnew">Read More...</a>
				</div>
				<div class="contentHeading">Recent Changes</div>
		
				<div class="newsItem">
				     <div class="title">Updates in build #96</div>
				     <div class="info">10/23/2006 - Posted By: tonyf</div>
				     <div>
				        <div class="subTitle">New Features and other changes:</div>
				        <div class="subProject">Project Designer (build 96)</div>
				        <ul>
				            <li><b>Fields in MCQs</b> - Fields can now be inserted into the question text and/or choice text of MCQs.</li>
				            <li><b>Tables in Text Items</b> - Tables can now be inserted into Text Items in Forms.</li>
				            <li><b>FIB Item formatting</b> - FIB Items in Forms now support the following formatting capabilities:
				                <ul>
				                    <li>Bold
				                    <li>Italic
				                    <li>Underline
				                    <li>Font selection
				                    <li>Font size
				                    <li>Font color
				                </ul>
				            </li>
				            <li><b>Tabs in Text Items</b> - Tab characters may now be used for simple layout of text and blanks within a Text Item. The Tab positions may be set via the Format / Tabs... menu item.</li>
				        </ul>
				
				        <div class="subProject">Known issues:</div>
				        <ul>
				            <li>Inserting a column in a table in a Text Item sometimes causes the empty space at the bottom of the Text Item to grow inordinately. We don't currently have a fix for this, but typing any character in the Text Item removes the excess space.
				            <li>You might encounter an exception (crash) when opening a Project created prior to Build 94, if an MCQ contains either an ampersand (&amp;) or a left angle bracket (&lt;) in its text. This is an rare and isolated issue, most easily resolved by manually editing the Project file's XML using a text editor. If you have an older Project that throws an exception upon opening, please send it to the developers for repair.
				        </ul>
				
				        <div class="subProject">Bug fixes:</div>
				        <ul>
				            <li><b>Mantis Issue 320:</b> MCQ has smaller Font / Different Font than other field</li>
				            <li><b>Mantis Issue 220:</b> Placing a field into a text item causes error</li>
				            <li><b>Mantis Issue 308:</b> Tab positions not maintained in Documents</li>
				            <li><b>Mantis Issue 326:</b> Attempting to Open TOUR OF TAWALA v.1 crashes Designer</li>
				            <li><b>Mantis Issue 327:</b> Reference in Text Item to Field in subsequent Form causes BAD save</li>
				            <li><b>Mantis Issue 315:</b> "where" clause does not give expected result</li>
				        </ul>
				     </div>
				</div>

		</div>

		<!--
				<div class="newsItem">
					<div class="title">Update Template</div>
					<div class="info">6/4/2006 - Posted By: JDF</div>
					<div class="subTitle">New Features and other changes:</div>
					<div class="subProject">Project Designer</div>
					<ul>
						<li>
							<b>Item</b> - Description
						</li>
						<li>
							<b>Item</b> - Description
						</li>
					</ul>
					<div class="subProject">Web site:</div>
					<ul>
						<li>
							<b>Item</b> - Description
						</li>
						<li>
							<b>Item</b> - Description
						</li>
					</ul>
					<div class="subTitle">Bug fixes:</div>
					<ul>
						<li>
							<b>Item</b> - Description
						</li>
					</ul>
				</div>
		-->
		</div>
	</div> <!-- end yui-u first -->

	<div class="yui-u">
		<div class="contentBox">
			<tiles:useAttribute id="rightBlockList" name="rightBlockList" classname="java.util.List" ignore="true"/>
			<c:forEach var="block" items="${rightBlockList}" varStatus="status">
				<tiles:insert name="${block}" />
			</c:forEach>
		</div>
	</div> <!-- end yui-u -->
</div> <!-- end yui-gc -->

		