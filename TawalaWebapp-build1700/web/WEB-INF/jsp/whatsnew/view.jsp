<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

		<div id="changes" class="news">
			<div class="newsItem">
			    <div class="date">JANUARY 26, 2007</div>
			    <div><span class="title">Updates in build #108</span><span class="info">posted by TonyF</span></div>
			    <div class="text">
			        <div class="subTitle">Project Designer</div>
                    <ul>
				    	<li><b>Images in Documents</b> - You may now insert images into Documents by clicking Image... on the Insert menu.</li>
						<li><b>GET WHERE enhanced</b> - The ability to compare fields in the current encounter to those in previous encounters is now implemented in both the designer app and the server app.</li>						
						<li><b>GET selects single Form by default</b> - The default Form selection in the GET statement details panel is now a single Form rather than "All Forms".</li>
                    </ul>
                <div class="subTitle">Web site</div>
                    <ul>
						<li>Reduced size of project-related URLs - the random id is 20 characters, /project is reduced to /p.</li>
						<li>Multiple levels of users and home page redesign - please refer to this document provided by Doug and Steve P.</li>
						<li>SEND command modification to accomodate users without email address - "no-reply@tawala.com" will be used.</li>
                    </ul>

                <div class="subTitle">Bug fixes</div>
                    <ul>
                        <li><b>Mantis Issue 295:</b> Unable to Copy and Paste Form</li>
                        <li><b>Mantis Issue 411:</b> Large Processes can slow Designer</li>
                        <li><b>Mantis Issue 416:</b> Insert Invitation dialog shows wrong Starting Point when modifying invitation</li>
                        <li><b>Mantis Issue 418:</b> Removing a Form referenced by an Invitation causes Bad Save</li>
                        <li><b>Mantis Issue 396:</b> Record Fields appear in Fields Palette for Documents</li>
                        <li><b>Mantis Issue 296:</b> Unable to Copy and Paste Document</li>
                        <li><b>Mantis Issue 423:</b> Deploy hangs if exception is thrown on the server</li>
                        <li><b>Mantis Issue 417:</b> Cannot paste process with certain statements</li>
                        <li><b>Bug:</b> SET of a field within FOR EACH loop when records are selected from multiple forms doesn't save data</li>
                        <li><b>Bug:</b> Change in Forms dropdown in GET Statement Details panel modified statement in Process even if Modify button not pressed</li>
                        <li><b>Bug:</b> Record Set node appears in Fields Palette when Document or Form is active window</li>
                        <li><b>Bug:</b> Record Set and Forms boxes in GET Statement Details panel not resizing when panel is resized</li>
                    </ul>
                </div>
           </div>

			<div class="newsItem">
			    <div class="date">DECEMBER 8, 2006</div>
			    <div><span class="title">Updates in build #102</span><span class="info">posted by TonyF</span></div>
			    <div class="text">
			        <div class="subTitle">Project Designer</div>
                    <ul>
                        <li><b>Confirm deletion of components</b> - Designer is now prompted to confirm before a Form, Process or Document is deleted.</li>
                        <li><b>For Each Question tab</b> has been removed from the Statement Details panel because the functionality is not yet implemented.</li>
                    </ul>

                <div class="subTitle">Web site</div>
                    <ul>
                        <li>Test Drive Latest Version button on Library Project Detail page doesn't go through the intermediate page.</li>
                        <li>Modified Library Search screen (limited category tree expansion, fixed several bugs).</li>
                        <li>Fixed problem with overlapping large fonts.</li>
                        <li>Last updated date in Project Library shows correct date.</li>
                        <li>Ability to delete project versions</li>
                    </ul>

                <div class="subTitle">Bug fixes</div>
                    <ul>
                        <li><b>Mantis Issue 309:</b> Setting negative variable value causes bad save if checkbox not used</li>
                        <li><b>Mantis Issue 313:</b> DELETE button does not work on images in TEXT</li>
                        <li><b>Mantis Issue 319:</b> No Choices in Palette for Record:MCQ</li>
                        <li><b>Mantis Issue 322:</b> Unable to enter field in left box of SET statement via Double Click</li>
                        <li><b>Mantis Issue 328:</b> Cannot replace selected Field in Document</li>
                        <li><b>Mantis Issue 355:</b> Field Palette Node should expand when the first field is added to a form...</li>
                        <li><b>Mantis Issue 366:</b> MCQ Responses can be added as fields without Qualifying Form information &lt;&lt;a&gt;&gt;</li>
                        <li><b>Mantis Issue 367:</b> MCQ Responses remain after project is closed</li>
                        <li><b>Mantis Issue 369:</b> PREVIEW FORM does not show recent changes in Text Box</li>
                        <li><b>Mantis Issue 375:</b> EDIT > DELETE does not work on images</li>
                        <li><b>Mantis Issue 379:</b> Alternate Label Text Box not enabled at all positions of a blank after carriage returns</li>
                        <li><b>Mantis Issue 383:</b> New blanks in FIB not recognized without saving and reopening Project</li>
                        <li><b>Mantis Issue 289:</b> Value in far right text box of get statement (ConditionGroup) is retained</li>
                        <li><b>Mantis Issue 290:</b> Any Get Statement where "" is not defined will keep the application from deploying.</li>
                        <li><b>Mantis Issue 291:</b> Double clicking on IF in SKIP statement can add fields to TEXT box</li>
                        <li><b>Mantis Issue 297:</b> SKIP label does not update immediately</li>
                        <li><b>Mantis Issue 329:</b> When Designer opens all Documents are briefly flashed on the screen</li>
                        <li><b>Mantis Issue 353:</b> Attempting to Paste an image causes image path to be pasted in blank container then a crash</li>
                        <li><b>Mantis Issue 356:</b> Clicking once on Text Item after working in a Document or Process does not bring Text Item into focus</li>
                        <li><b>Mantis Issue 358:</b> Cursor invisible in MCQ</li>
                        <li><b>Mantis Issue 365:</b> MCQ adding choiced in wrong position under certain conditions</li>
                        <li><b>Mantis Issue 377:</b> Designer Beeps when adding statement via ENTER button</li>
                        <li><b>Mantis Issue 380:</b> Forms not offset in main MDI window</li>
                        <li><b>Mantis Issue 381:</b> Alternate label for blank not retained upon save</li>
                        <li><b>Mantis Issue 383:</b> 1 New blanks in FIB not recognized without saving and reopening Project</li>
                    </ul>
                </div>
           </div>

			<div class="newsItem">
			    <div class="date">NOVEMBER 17, 2006</div>
			    <div><span class="title">Updates in build #100</span><span class="info">posted by TonyF</span></div>
			    <div class="text">
			        <div class="subTitle">Project Designer</div>
                    <ul>
                        <li><b>Navigation in Statement Details Panel</b> - Several improvements were made to enhance navigation when building new statements in the Statement Details Panel:
                            <ul>
                                <li>For statements whose first UI control is a text box, focus is initially set to that text box so that you can begin editing without having to click in the first box.
                                <li>Inserting a Field in the Field Box (left hand text box) automatically moves focus and selection to the Expression Box (right hand text box) in any of the following:
                                    <ul>
                                        <li>A SET statement</li>
                                        <li>A condition in an IF statement</li>
                                        <li>A "where" condition in a GET statement</li>
                                    </ul>
                                </li>
                                <li>When you add a condition to an IF or a GET statement, focus and selection are automatically moved to the Field Box (left hand text box) of the new condition</li>
                                <li>Inserting a Field in the To: Box of a SEND statement automatically moves focus to the Cc: Box.</li>
                                <li>The tab order of boxes has been set to allow tabbing from one box to the next in left-to-right, top-to-bottom order.</li>
                                <li>Pressing the Enter key causes a statement to be added or modified (just like the Add/Modify button).</li>
                            </ul>
                        </li>
                
                        <li><b>Alignment Options Removed From Tabs dialog</b> - The Tabs dialog has been simplified, eliminating the center, right and decimal tab options.</li>
                
                        <li><b>Vertical Sizing of Statement Details Panel Optimized</b> - The statement details panel is now just tall enough to hold the contents of each statement, with no extra space. In addition, the default statement details panel text has been reduced to a single line. Finally, dragging the bottom edge of the Process window no longer changes the position of the top edge of the window.</li>
                    </ul>
                    <div class="subTitle">Bug fixes</div>
                    <ul>
                        <li><b>Mantis Issue 86:</b> A text box that extends beyond the BOTTOM of a Form window will not shift text to keep the cursor in sight.
                        <li><b>Mantis issue 299:</b> Ctrl-A does not work in Form Items.
                        <li><b>Mantis issue 310:</b> Saving a project with an image inserted into the text field causes a bad save.
                        <li><b>Mantis Issue 363:</b> Extremely slow response with FIBs in large projects.
                    </ul>
                </div>
           </div>

			<div class="newsItem">
			    <div class="date">NOVEMBER 9, 2006</div>
			    <div><span class="title">Updates in build #99</span><span class="info">posted by TonyF</span></div>
			    <div class="text">
			        <div class="subTitle">Project Designer</div>
			        <ul>
			            <li><b>MCQ Item formatting</b> - MCQ Items in Forms now support the following formatting capabilities:
			                <ul>
			                    <li>Bold</li>
			                    <li>Italic</li>
			                    <li>Underline</li>
			                    <li>Font selection</li>
			                    <li>Font size</li>
			                    <li>Font color</li>
			                </ul>
			            </li>
			
			            <li><b>Alignment and Indent in Text Items</b> - Text Items in Forms now support the following formatting capabilities:
			                <ul>
			                    <li>Left, Centered, Right and Justified alignment</li>
			                    <li>Indent</li>
			                </ul>
			            </li>
			        </ul>
			
			        <div class="subTitle">Website</div>
			        <ul>
			            <li>Home page redesign</li>
			        </ul>
			
			        <div class="subTitle">Bug fixes:</div>
			        <ul>
			            <li><b>Mantis Issue 119:</b> Deleting the last item in a form jumps the selection to the first item.</li>
			            <li><b>Mantis Issue 267:</b> Too Much Many Text Highlighted</li>
			            <li><b>Mantis Issue 278:</b> CLICK AND HOLD out of Text Box into grey area to remove all text.</li>
			            <li><b>Mantis Issue 305:</b> Fields are editable when you reopen a Document</li>
			            <li><b>Mantis Issue 306:</b> Fields are editable when you reopen a Form</li>
			            <li><b>Mantis Issue 336:</b> Context menu acts on entire Form Item rather than selected text</li>
			            <li><b>Mantis Issue 337:</b> Field Palette should maintain it's position regardless of insertion point within a process</li>
			            <li><b>Mantis Issue 351:</b> Field disappears from Text Item if you add it via double-click</li>
			        </ul>
			    </div>
			</div>

			<div class="newsItem">
				<div class="date">NOVEMBER 3, 2006</div>
				<div><span class="title">Updates in build #98</span><span class="info">posted by TonyF</span></div>
				<div class="text">
			        <div class="subTitle">Project Designer</div>
			        <ul>
                        <li><b>Fixed a bug in Build 97</b> where projects with more than one image could not be opened and resaved.</li>
                        <li><b>Performance enhancements</b> - This weeks work focused on performance improvements when saving and deploying Projects, and addressing some issues with our handling of images:
                            <ul>
                                <li><b>Speed improvements when saving</b> - Much of the slowness that designers have encountered when saving or deploying Projects has been aleviated.</li>
                                <li><b>Improvements in image storage and loading:</b>
                                    <ul>
                                        <li>Unnecessary duplication of image data in Project files with subequent saves has been eliminated.</li>
                                        <li>Changing of image sizes when reopening Forms has been eliminated.</li>
                                        <li>Unreferenced image data are now removed from the Project XML.</li>
                                        <li>Images now appear at their proper, full size when inserted into Text Items.</li>
                                        <li>Images load faster for display when opening a Form window.</li>
                                        <li>8-bits-per-pixel images now load and display properly.</li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                    </ul>

			        <div class="subTitle">Web Site</div>
			        <ul>
                        <li>Website redesign</li>
                        <li>Added feature in Project Manager to enable either linking to projects or embedding a project inside another web page.</li>
                        <li>More work on caching and server performance.</li>
			        <ul>

			        <div class="subTitle">Bug Fixes</div>
			        <ul>
                        <li><b>Mantis Issue 311:</b> Appoximately 18 seconds to save each 28k Jpeg in a form</li>
                        <li><b>Mantis Issue 334:</b> JPEG image gets bigger when Form is reopened</li>
                        <li><b>Mantis Issue 335:</b> JPEG image gets "duplicated" in XML after reopening Form</li>
                        <li><b>Mantis Issue 330:</b> Image Data Remains in XML after Image is removed</li>
                        <li><b>Mantis Issue 347:</b> Slow saves when Forms windows are open</li>
                        <li><b>Mantis Issue 346:</b> Opening a saved project with a GIF image - image is ALL WHITE</li>
                        <li><b>Mantis Issue 345:</b> Opening a saved project with a GIF image - image is Compressed and Dark</li>
			        </ul>
               </div>
           </div>

			<div class="newsItem">
				<div class="date">OCTOBER 23, 2006</div>
				<div><span class="title">Updates in build #96</span><span class="info">posted by TonyF</span></div>
				<div class="text">
			        <div class="subTitle">Project Designer</div>
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
			
			        <div class="subTitle">Known issues:</div>
			        <ul>
			            <li>Inserting a column in a table in a Text Item sometimes causes the empty space at the bottom of the Text Item to grow inordinately. We don't currently have a fix for this, but typing any character in the Text Item removes the excess space.
			            <li>You might encounter an exception (crash) when opening a Project created prior to Build 94, if an MCQ contains either an ampersand (&amp;) or a left angle bracket (&lt;) in its text. This is an rare and isolated issue, most easily resolved by manually editing the Project file's XML using a text editor. If you have an older Project that throws an exception upon opening, please send it to the developers for repair.
			        </ul>
			
			        <div class="subTitle">Bug fixes:</div>
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

			<div class="newsItem">
				<div class="date">OCTOBER 13, 2006</div>
				<div><span class="title">Updates in build #93</span><span class="info">posted by TonyF</span></div>
				<div class="text">
			          <div class="subTitle">Project Designer</div>
			          <ul>
			              <li><b>Tabs in FIBs</b> - Tab characters may now be used for simple layout of text and blanks within a FIB. As with tabs in Documents, they are converted to a table in HTML for display in the browser.</li>
			              <li><b>Fields in FIBs</b> - Fields may now be inserted into the text of FIB items. They behave the same as Fields in Text Items.</li>
			              <li><b>Text Item formatting</b> - Text Items in Forms now support the following additional formatting capabilities:
			                  <ul>
			                      <li>Font face</li>
			                      <li>Font size</li>
			                      <li>Font color</li>
			                  </ul>
			              </li>
			              <li><b>Carriage returns in FIB</b> Items are now preserved when the deployed Form is displayed.
			                  <ul>
			                      <li><i>Note that there is currently an issue displaying "FIBs" properly in the browser. This is a server-side problem that should be fixed soon.</i></li>
			                  </ul>
			              </li>
			              <li><b>Multiple contiguous space characters</b> in FIB Items and Text Items are now preserved when the deployed Form is displayed.</li>
			              <li><b>Text Item formatting</b> - Text Items in Forms now support the following formatting capabilities:
			                  <ul>
			                      <li>Bold</li>
			                      <li>Italic</li>
			                      <li>Underline</li>
			                  </ul>
			              </li>
			              <li><b>Default font size</b> is now being set universally for all content in Documents, Text Items and FIB Items. This should put an end to "mysterious" changes of fonts to Arial 12 in table cells or when adding to existing text. Our default font is Arial 10.</li>
			              <li><b>Deploying Projects</b> - A modal dialog and wait cursor are now displayed to indicate that a deployment is in process and to prevent multiple clicks on the Deploy button.</li>
			              <li><b>Correct spacing at end of Documents and Text Items</b> - The extraneous new line that the TX control was adding when Documents and Text Items were reopened has been eliminated. Any carriage returns entered by the designer, and only those carriage returns are preserved.</li>
			              <li><b>Text Item and FIB Item default font size</b> was changed from 9 point to 10 point (to match default for Documents)</li>
			          </ul>


			          <div class="subTitle">Bug fixes:</div>
			          <ul>
			              <li><b>Mantis Issue 240</b> - ...create an invitational field into Document then save or deploy cause error</li>
			              <li><b>Mantis Issue 264</b> - Deleting Set Statement Causes BAD Save</li>
			              <li><b>Mantis Issue 292</b> - SET command cannot change records on server</li>
			              <li><b>Mantis Issue 293</b> - SET command cannot add variable to server</li>
			              <li><b>Mantis Issue 298</b> - Formatting a Blank Space in a Text box causes bad save.</li>
			              <li><b>Mantis Issue 300</b> - Attempting to Copy a Text Item containing an image causes crash</li>
			              <li><b>Mantis Issue 196</b> - Text Box in Larger Font / Different Font than other fields</li>
			              <li><b>Mantis Issue 269</b> - Deploy button holds keystrokes in buffer</li>
			              <li><b>Mantis Issue 274</b> - "no-reply@tawala.com" consistently treated as bulk mail</li>
			              <li><b>Mantis Issue 279</b> - Extra space ... appears at the bottom of ... text boxes</li>
			              <li><b>Mantis Issue 294</b> - FIB formatting not preserved when deployed</li>
			          </ul>

			          <div class="subTitle">Known issues:</div>
			          <ul>
			              <li>Updating Build 91 to Build 92 or Build 93 without first manually uninstalling Build 91 can lead to some problems. You will see the installer rerun automatically every time you open Project files created with Build 91. You may possibly encounter exceptions when opening older files.
						  <br />
			              <div class="note important">It is strongly recommended that you use Windows "Add or Remove Programs" to uninstall any previous Build of Tawala before you install Build 93.</div>
			              <br />
			               This is a one-time issue; manually uninstalling should not be required in the future.</li>
			              <li>Shortcut keys for Bold (Ctrl-B), Italic (Ctrl-I) and Underline (Ctrl-U) do not currently work in Text Items. We're working on that one.</li>
			          </ul>

			          <div class="subTitle">Please Note:</div>
			          <ul>
			              <li>Implementing the new FIB formatting behaviors entailed a major architectural modification, switching to the TX text control as the base for FIB Items. As always, major changes in the code could introduce bugs, so please report any anomalous behavior.</li>
			              <li>The XML format version for Project files has been bumped up to version 1.4.</li>
			          </ul>
			    </div>
			</div>

			<div class="newsItem">
				<div class="date">SEPTEMBER 28, 2006</div>
				<div><span class="title">Updates in build #91</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
			        <ul>
			             <li><b>GET statement UI</b> - Improvements have been made to the Statement Details Panel for the GET statement:
			                 <ul>
			                     <li>The AND / OR radio buttons have been replaced by a drop-down list with ALL and ANY as the choices.
			                     <li>Minus button has been added to the first "where" condition. Pressing it will do accomplish one of the following behaviors:
			                         <ul>
			                             <li>If there is more than one condition, the first line will be deleted from the list</li>
			                             <li>If there is only one condition, its controls will be cleared</li>
			                         </ul>
			                     </li>
			                     <li>Controls resize and reposition properly when the Process window is resized</li>
			                     <li>Added conditions will no longer "disappear" behind the Statement Details Panel if you resize the Process window</li>
			                 </ul>
			             </li>

			             <li><b>IF statement UI</b> - Improvements have been made to the Statement Details Panel for the IF statement:
			                 <ul>
			                     <li>The AND / OR radio buttons have been replaced by a drop-down list with ALL and ANY as the choices.</li>
			                     <li>Minus button has been added to the first IF condition. Pressing it will do accomplish one of the following behaviors:
			                         <ul>
			                             <li>If there is more than one condition, the first line will be deleted from the list</li>
			                             <li>If there is only one condition, its controls will be cleared</li>
			                         </ul>
			                     </li>
			                 </ul>
			             </li>
			             <li><b>Overwrite Users' stored FIB responses</b> - It is now possible to modify FIB responses in fetched records in a FOR EACH loop.</li>
			             <li><b>Fields Palette</b> - The Fields Palette is now updated to display the appropriate Record Fields upon the following events:
			                 <ul>
			                     <li>Adding a new FOR EACH statement to a Process</li>
			                     <li>Connecting a Process to or or disconnecting a Process from a Form</li>
			                     <li>Deleting a SET statement from a Process</li>
			                 </ul>
			             </li>

			             <li><b>Record-and-form-qualfied variable references</b> - Two issues noted in Build 90 have been addressed and now work properly:
			                 <ul>
			                     <li>IF statements that contain record-and-form-qualfied variable references in an expression (e.g. IF Score equals Record:Form 1:Score)</li>
			                     <li>SET statements that contain record-and-form-qualfied variable references in an expression (e.g. SET Score to Record:Form 1:Score)</li>
			                 </ul>
			             </li>

			             <li><b>Images</b> - The size limit of 400 x 400 pixels has been removed when inserting images into Text Items.</li>
			        </ul>
					<div class="subTitle">Website</div>
					<ul>
						<li>No visible changes this week but there have been some back-end changes to help performance escpecially in the Project Manager.</li>
					</ul>

			        <div class="subTitle">Bug fixes:</div>
			        <ul>
			            <li><b>Mantis Issue 127</b> - Line spacing in SEND Documents different from the same sent via SHOW Document</li>
			            <li><b>Mantis Issue 153</b> - SEND does not work when nested within a FOR EACH</li>
			            <li><b>Mantis Issue 166</b> - Extra Field created on website.</li>
			            <li><b>Mantis Issue 205</b> - Record List name cannot be reused</li>
			            <li><b>Mantis Issue 221</b> - Choices don't refresh in IF MCQ=... instructions</li>
			            <li><b>Mantis Issue 253</b> - Library categories ... are all double spaced with spaces breaking all of the tree lines.</li>
			            <li><b>Mantis Issue 265</b> - Moving Table Borders in and out and closing docuement can mangle tables.</li>
			            <li><b>Mantis Issue 271</b> - Add Project Version" Navigation menu is double spaced in IE not Firefox</li>
			            <li><b>Mantis Issue 281</b> - If you reference a MCQ field from a different form in a SKIP command...bad save...</li>
			            <li><b>Mantis Issue 282</b> - $ Dollar Sign throw error on website</li>
			            <li><b>Mantis Issue 283</b> - MCQ choices not available in Fields Palette to modify IF statement</li>
			            <li><b>Mantis Issue 285</b> - Refreshing Project Details page via browser can effectively erase new responses</li>
			            <li><b>Mantis Issue 286</b> - Cannot edit SEND statements in saved Projects</li>
			            <li><b>Mantis Issue 287</b> - Categories tree in Project Library has extra lines when viewed with IE</li>
			            <li>Undocumented issue where resizing the Process window when the GET Statement Details panel was visible could cause added conditions to "disappear" and be inaccessible.</li>
			        </ul>

			        <div class="subTitle">Please Note:</div>
			        <ul>
			            <li>Using large images can cause Project files to get really big and cause slowdowns, and possible timeouts, when working with, saving or deploying. We are continuing to learn how we can improve image handling in Tawala, and your feedback on the types, sizes and number of images you want to use in your Projects will help us determine how best to proceed.</li>
			            <li>Major architectural improvements have been made in both the Project Designer and the server code, to better accomodate Record-qualified and Form-qualified Field references wherever they occur in Projects. As always, major changes in the code could introduce bugs, so please report any anomalous behavior.</li>
			        </ul>
			    </div>
			</div>

			<div class="newsItem">
				<div class="date">SEPTEMBER 20, 2006</div>
				<div><span class="title">Updates in build #90</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Web Site</div>
			        <ul>
			            <li><b>Project Version Deployment</b> - By default the latest version of the project
			                is deployed. Project Versions Page (accessible from Project Details page)
			                allows to deploy a different version.</li>
			            <li><b>Project Versioning</b> - Every time a project is deployed to the server a new
			                version is created. Projects can be viewed and download by following links
			                from the Project Details Page.</li>
			        </ul>
			
			        <div class="subTitle">Project Designer</div>
			        <ul>
			            <li><b>Variables in SET statements</b> are now maintained as references to the
			                Variables (objects) themselves, rather than by name. While there is no
			                visible change to designers, this fixes some issues (reported and un-reported).
			                It entailed a sizeable refactoring of the code and testers should verify that
			                SET statements and Variables are working as expected.</li>
			            <li><b>Multiple Forms in GET Statements</b> - The GET statement now lets you get a
			                list of records from one or more Forms. The "forms" dropdown box presents a
			                list of Forms, each of which has a checkbox in front of it. Check the Forms
			                you want to get records from, and uncheck the Forms you wish to exclude. (Note:
			                This feature will have limited utility until we add the "where" clause to the
			                GET statement).</li>
			            <li><b>Print and Print Preview</b> have been reinstated for Processes. Those commands
			                do not yet work with Forms and Documents, so the menu items are disabled for
			                those components.</li>
			            <li><b>Enhanced debugging of "Bad" projects</b> - The "BAD" project files that are
			                generated when Tawala encounters a problem while saving or deploying a Project
			                now contains additional information to help engineers track down bugs.</li>
			            <li><b>Where Clause in GET Statements</b> - The GET statement now lets you specify zero
			                or more conditions that determine which records it fetches. The conditions behave
			                just like those previously implemented in the IF statement.</li>
			            <li><b>Images</b> - The image code underwent a major refactoring to reduce the size of
			                Project files (and thereby alleviate timeout issues when deploying files). Images
			                are now handled as references the the Project image list, and raw image data has
			                been elimintated from the Text Item definitions in the XML.</li>
			        </ul>
			
			        <div class="subTitle">Bug fixes:</div>
			        <ul>
			            <li><b>Mantis Issue 272:</b> Deploying a project with an image(s) can cause a timeout</li>
			            <li><b>Mantis Issue 264:</b> Deleting Set Statement Causes BAD Save</li>
			            <li><b>Mantis Issue 276:</b> Fields changed to literal strings when modifying IF statement</li>
			            <li><b>Mantis Issue 277:</b> Operator in If details reverts to default when field changed and
			                yet allowable operators haven't changed.</li>
			        </ul>
			
			        <div class="subTitle">Known Issues:</div>
			        <ul>
			            <li>Changes that were made during implementation of the GET statement's where clause are
			                temporarily preventing use of certain constructs inside FOR EACH statements. The constructs
			                that are known not to work are:
			                <ul>
			                    <li>IF statements that contain record-and-form-qualfied variable references in an
			                        expression (e.g. IF Score equals Record:Form 1:Score)</li>
			                    <li>SET statements that contain record-and-form-qualfied variable references in an
			                        expression (e.g. SET Score to Record:Form 1:Score)</li>
			                </ul>
			            </li>
			            <li>At this time there are no known issues due to the refactoring of the image code, but
			                please keep your eyes open for bugs that may have been introduced.</li>
			        </ul>
			    </div>
			</div>

			<div class="newsItem">
				<div class="date">AUGUST 30, 2006</div>
				<div><span class="title">Updates in build #88</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Web Site</div>
					<ul>
             	       <li><b>Data Import</b> - Data in CSV format can be imported into individual forms from the Project Details page.</li>
                    </ul>

                    <div class="subTitle">Bug fixes:</div>
                    <ul>
                	    <li><b>Mantis Issue 201:</b> Error saving FIB with two blanks seperated by a space</li>
                   		<li><b>Mantis Issue 206:</b> Bad Save after hitting ENTER on FIB</li>
	                    <li><b>Mantis Issue 209:</b> Problems deleting variables in Text Box</li>
    	                <li><b>Mantis Issue 218:</b> Hitting Enter after the blank in an FIB can cause bad save.</li>
						<li><b>Mantis Issue 236:</b> Using 'Delete' key I am not able to delete the invitational field that I created in Document.</li>
						<li><b>Mantis Issue 255:</b> Using a tab in a document can mangle formatting</li>
						<li><b>Mantis Issue 256:</b> Double Clicking to insert a field into a document and using a Tab will erase any previous text</li>
						<li><b>Mantis Issue 268:</b> Can't Delete or Purge projects in Project Manager when browser window is scrolled down</li>
						<li><b>Mantis Issue 270:</b> "Add Project Version" menu is repetitive</li>
					</ul>
				</div>
			</div>
			<div class="newsItem">
				<div class="date">AUGUST 24, 2006</div>
				<div><span class="title">Updates in build #87</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
                        <li><b>New IF UI</b> - The "Simple" and "Advanced" tabs in the IF statement Details Panel have
                            been replaced with a single panel. Multiple conditions may be created simply by adding them
                            in the Panel, and without the Expression Builder dialog (which has been retired).</li>
                        <li><b>Variables as Arithmetic or Text</b> - The SET statement now includes a check box to
                            designate whether the expression assigned to a Variable should be treated as text or as
                            an Arithmetic expression. The check box is enabled only if the expression contains an
                            arithmetic operator (+, -, * or /).</li>
                        <li><b>Horizontal scroll bar in Process</b> - If statements in the Process are too long to be
                            entirely visible, a horizontal scroll bar appears at the bottom of the Process window.</li>
                        <li><b>Default text in Form Text Items is now immediately editable</b> (you can just start typing
                            without clicking in the edit area and reselecting the text).</li>
                        <li><b>Resizing images disabled</b> - We have turned off the ability to resize images that
                            have been insrerted into Text Items in Forms using the Insert/Image... menu item. We
                            intend to support resizing of images in a future story.</li>
                        <li><b>Print / Print Preview</b> - These have been temporarily removed from the menu. They
                            weren't working properly, which generated bug reports. Implementation is deferred until
                            after converting the Form Item controls to use TX text controls.</li>
					</ul>
					<div class="subTitle">Bug Fixes:</div>
					<ul>
                        <li><b>Mantis Issue 29:</b> Hyphenated text treated as arithmetic expression</li>
                        <li><b>Mantis Issue 160:</b> If you put a # sign in the project file name you can effect the
                            navigation on ...projectmanager/view page</li>
                        <li><b>Mantis Issue 177:</b> Certain words will not work in search "Will"</li>
                        <li><b>Mantis Issue 194:</b> Missing space between variables and operators results in bad save</li>
                        <li><b>Mantis Issue 199:</b> Arithmetic expressions cause "Saved Project Invalid" message</li>
                        <li><b>Mantis Issue 203:</b> Slash or Dash in Variable Name causes bad save -FFERUCH</li>
                        <li><b>Mantis Issue 232:</b> Cut, Copy, Paste in Text Items does not work on selected text</li>
                        <li><b>Mantis Issue 233:</b> "...drag the field from the palette into the document it seems to treat
                            is as text."</li>
                        <li><b>Mantis Issue 242:</b> Paste Button not available from within Text field</li>
                        <li><b>Mantis Issue 243:</b> When using "Copy" Button Ctrl V does not work.</li>
                        <li><b>Mantis Issue 244:</b> Paste Button pastes creates new Text Block instead of pasting text.</li>
                        <li><b>Mantis Issue 245:</b> The text in 2 different Text Items can be selected at the same time.</li>
                        <li><b>Mantis Issue 248:</b> Unable to delete a Form, Process, or Document without opening it.</li>
                        <li><b>Mantis Issue 250:</b> Removing OTHERWISE results in bad project</li>
                        <li><b>Mantis Issue 251:</b> Renaming a Form that contains a field referenced by a Document causes
                            crash</li>
                        <li><b>Mantis Issue 254:</b> Form qualified Field name in Document not updated upon Form rename</li>
                        <li><b>Mantis Issue 258:</b> Record fields should not be available in Documents and Forms</li>
                        <li><b>Undocumented bug:</b> in Build 86 that caused an exception to be thrown when opening a Project
                            with a SET statement in Skip Instructions</li>
				    </ul>
				</div>
			</div>

			<div class="newsItem">
				<div class="date">AUGUST 8, 2006</div>
				<div><span class="title">Updates in build #85</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
						<li><b>Comments in Processes</b> - You may now add text comments in Processes.</li>
						<li><b>Default data set name</b> - A default name is suggested for the record set returned 
							by the GET statement. The designer may change the suggested name if desired.</li>					
					</ul>
					<div class="subTitle">Web Site</div>
					<ul>
						<li><b>Password recovery</b> - If a designer forgets their password they may now reset it.</li>
						<li><b>Add sample data to library project</b> - A designer may now add sample data to a project published in the library.</li>
					</ul>
					<div class="subTitle">Bug Fixes:</div>
					<ul>
						<li><b>Mantis Issue 148:</b> Old Variable names not available for current use</li>
						<li><b>Undocumented issue:</b> Modify button not enabled when you select an existing GET statement</li>
       				</ul>
				</div>
			</div>

			<div class="newsItem">
				<div class="date">AUGUST 2, 2006</div>
				<div><span class="title">Updates in build #83</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
						<li><b>Reset Document</b> - You can now reset a Document that has been appended to its original contents (or in the case of a Virtual Document, clear its contents entirely). This is done by checking the appropriate check box in a SHOW Document statement or a SEND statement.
						<li><b>SEND Invitation eliminated</b> - The SEND Invitation statement has been eliminated from the Project Designer UI. It is effectively replaced by the ability to insert Invitation Fields into a Documents and SEND the Document. Note that SEND Invitation statements in older Projects will continue to work, but they cannot be edited and new ones cannot be created.
						<li><b>Dropping Fields in Documents and Text Items</b> - Fields dragged from the Fields Palette are now inserted where dropped in Documents and Text Items, rather than at the text cursor position. Note that there are still some end cases where we continue to do battle with the TX Text Control.
						<li><b>Removed Skip Instructions statements</b> - The unimplemented UPON statement and the arithmetic statements have been removed from the Statements Palette for Skip Instructions. This should have been done when they were removed for Processes.
						<li><b>Edit Invitation Fields</b> - When an Invitation Fields is double-clicked for editing the dialog caption now reads "Edit Invitation".
						<li><b>Performance improvements</b> - Issues with slow and/or erratic updates in Processes have been greatly alleviated. Validation of Process lines and refreshing of the Fields Palette have been deferred to execute when the application goes idle, rather than during designer interactions with the UI.
						<li><b>FOR EACH Question statement</b> has been temporarily disabled in the Process UI until we can get it working again... hopefully in the next couple of iterations.
						<li><b>Duplicate Names</b> - Following requests from our designers, the restrictions on names for Project elements have been decreased. We were disallowing use of duplicate names to avoid potential confusion. Now you may use the same name for a Form, Process, Document, and/or Form Item or Process Variable.
					</ul>
					<div class="subTitle">Web Site</div>
					<ul>
						<li>Category changes should be logged reversible events.
						<li>Count and display the number of times a project was test driven
						<li>Add/Delete confirmation screens everywhere in the library
					</ul>
					<div class="subTitle">Bug Fixes:</div>
					<ul>
						<li>Mantis Issue 116: Field dropped on Document is not inserted at drop location
						<li>Mantis Issue 125: Upper Case dropped when project opened via Double Click or My Recent Documents
						<li>Mantis Issue 186: Document Control - Inserting field via Drag and Drop is not accurate
						<li>Mantis Issue 195: Performance too slow with larger projects
						<li>Mantis Issue 208: Using the COPY Button within a TEXT box causes crash
						<li>Mantis Issue 210: Cut or Delete Buttons delete last item on form regardless of focus of cursor
						<li>Mantis Issue 215: CUT / PASTE of entire Text block causes crash.
						<li>Mantis Issue 216: Arithmetic Operators still available in SKIP commands
						<li>Mantis Issue 224: Expression Builder Dialog is editable by the user and may causes the unhandled exception...
						<li>Mantis Issue 226: Insert &gt; Image... is disabled if the Text item is not the last item in the Form
						<li>Mantis Issue 227: Invitation links not working in emails
						<li>Mantis Issue 228: Can't insert image into Text area under certain conditions
						<li>Mantis Issue 229: Extra characters in Display Text appear when editing Invitation Fields
						<li>Mantis Issue 231: Performance / Refresh too slow on projects with large processes
						<li>Mantis Issue 188: My Project Delete Cancel confirm winks out creating cat and mouse condition
						<li>Mantis Issue 204: Text Box is indented Left when deployed
       				</ul>
				</div>
			</div>


			<div class="newsItem">
				<div class="date">JULY 25, 2006</div>
				<div><span class="title">Updates in Tawala build #82</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
						<li><b>Invitation Fields in Documents</b> - Invitation Fields are now inserted into 
							Documents from the Insert menu. In the Insert Invitation dialog, select the Project 
							and Form that you want to link to in your Document, and enter the text for the link. 
							An Invitation Field may be edited by double-clicking and changing its properties in 
							the dialog. Note: Invitation links are currently working in Documents displayed via a 
							Show Document statement, but do not work in emails when the Document is used as the 
							Body text. This will be fixed in the next Build.</li>
						<li><b>Images in Form Previews</b> - Images now appear in previewed Forms.</li>
						<li><b>Support for 8-bit images</b> - Added support for insertion of 8-bit indexed images 
							(e.g. GIF files) into Text items.</li>
	
						<li><b>Graphics in Text Items</b> - (added in build #81) You may now insert GIF, JPG and PNG images into text items. 
							Click Image... on the Insert menu, then select an image. Images are inserted inline at the 
							current cursor position, and are limited to 400 x 400 pixels. Note: At present, only 
							full-color (24-bit JPG and PNG) files are handled properly. Text items containing images 
							from GIF files will cause an error when the Project file is saved.</li>
					</ul>
					
					<div class="subTitle"> Web:</div>
					<ul>
						<li>Adjusted the position of the confirmation dialogs in the Project Manager to make selection easier</li>
					</ul>
	
					<div class="subTitle"> Bug fixes:</div>
					<ul>
						<li>Fixed server app bug which caused Fields in Text Items to not display.</li>
						<li>Fixed bug in which Variable Fields in Text Items from older (pre- build 79) Projects displayed as "&lt;&lt;Form::Field&gt;&gt;" instead of, for example, "&lt;&lt;Variable 1&gt;&gt;".</li>
						<li>Fixed a bug in which the fields palette contained "leftover" record items from a previously open project.</li>
					</ul>
				</div>
			</div>
			
			<div class="newsItem">
				<div class="date">JULY 13, 2006</div>
				<div><span class="title">Updates in Tawala build #80</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
						<li><b>Fields in Text Items</b> - The following functionality is now implemented for Fields in Text Items in Forms:
							<ul>
								<li>Double-clicking a Field in the Fields Palette causes it to be inserted at the text cursor postion in the Text Item.</li>
								<li>Selecting a Field in the Fields Palette and pressing the Enter key causes it to be inserted at the text cursor postion in the Text Item.</li>
								<li>Fields are now automatically updated to reflect changes (insert, remove, rename) in referenced items.</li>
								<li>Fields are selectable only as units. They cannot be edited or partially deleted.</li>
								<li>Note: Text Items with Fields are currently not appearing properly in the server when deployed. This bug is being addressed and should be fixed in the next few days.</li>
							</ul>
						<li><b>Record Variables</b> - Record variables (variables set in a record inside a FOR EACH block, e.g., SET record:Score to 100) are now associated with the Form, rather than with the Process. They are kept in separate lists from "regular" Process variables and will appear only under Record nodes in Fields Palette when the current insertion point is inside a FOR EACH block.</li>
					</ul>
					<div class="subTitle"> Bug fixes:</div>
					<ul>
						<li><b>Mantis Issue 119:</b> Deleting the last item in a form jumps the selection to the first item</li>
						<li><b>Mantis Issue 165:</b> Variable created in For Each Loop is not avaible in Drop Down list.</li>
					</ul>
				</div>
			</div>


			<div class="newsItem">
				<div class="date">JULY 4, 2006</div>
				<div><span class="title">Updates in Tawala build #77</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
						<li><strong>Fields Palette interactive with all statements</strong> - Fields may be inserted into applicable statement elements from the Fields Palette. New elements implemented in this iteration are:
							<ul>
								<li>Variable text box in SET statement</li>
								<li>Expression text box in SET statement</li>
								<li>To: text box in SEND Email statement</li>
								<li>To: text box in SEND Invitation statement</li>
							</ul>
						</li>
						<li><strong>MCQ Choices in Fields Palette</strong> - Choices are now available in the Fields palette, including Choices for record-qualfied MCQs.
							<ul>
								<li>Choices can be inserted only into the Expression (right-hand) box of an IF statement details panel.</li>
								<li>When one MCQ replaces another in the Field (left-hand) text box, the Expression text box is cleared if it no longer contails a valid choice.</li>
							</ul>
						</li>
						<li><strong>SEND Email</strong> - The old SEND Email statement has been removed. The new default SEND Email sends a Document as Body Text.
							<ul>
								<li>Note: Previous SEND Email statements in older Project files will still appear in the Process and will work properly when deployed. They can be deleted, but not edited.</li>
							</ul>
						</li>
						<li><strong>Widened application window</strong> to accommodate extra real estate occupied by Fields Palette.</li>
					</ul>
					<div class="subTitle">Web site:</div>
					<ul>
						<li><strong>Migrated users and Project Manager</strong> - Users and project in the Project Manager now use the PostgreSQL database</li>
						<li><strong>New home pages</strong> - There are new home pages designer for users new to the sites and users that have been here before.</li>
						<li><strong>Terms &amp; Conditions and Privacy Policy</strong> - Terms &amp; Conditions and Privacy Policy pages have been added. Links are in the footer</li>
						<li><strong>FAQ Added</strong> - An FAQ page has been added along with a link in the main menu bar</li>
						<li>New home page(s)</li>
						<li>Updated message text on various emails/screens related to user registration</li>
						<li>Modified registration workflow (users become registered upon email verification)</li>			
					</ul>
					<div class="subTitle"> Bug fixes:</div>
					<ul>
						<li>Fixed bug in which a supposedly left-aligned table cell actually inherited its alignment from the cell preceding it.</li>
						<li>Fixed a Javascript bug that was causing problems in IE.</li>
						<li>Fixed bug #175 (user gets deleted if a project is deleted)</li>
						<li>Fixed bug #174 (unable to delete project from the library)</li>
						<li>Fixed a bug where old home page appears on unsuccessful login.</li>
						<li>Fixed a bug where an error screen was presented when a project was deleted from Project Manager</li>
						<li>Fixed a bug where a category renaming would fail.</li>
					</ul>
				</div>
			</div><!-- end news item -->

			<div class="newsItem">
				<div class="date">JUNE 20, 2006</div>
				<div><span class="title">Updates in Tawala build #76</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
				    <i><strong>Note:</strong> Build 76 replaces Buid 75 (6/16/06) to address a bug that caused Process windows to refresh extremely slowly in large Projects.</i>
				    <br /><br />
					<ul>
						<li><strong>Fields Palette interactive with IF statement</strong> - Fields may be inserted into both the left-hand (Field) box and the right-hand (Expression) box of IF statements using the Fields Palette.
							<br />
							<ul>
					        	<li>There are three ways to insert Fields:
					        		<ul>
							        	<li>Drag a Field from the Palette and drop it in the box.</li>
										<li>"Activate" a box by clicking in it, then double-click a Field in the Palette. Note that the active box is indicated by a light green background.</li>
				    		          	<li>"Activate" a box, then select a Field in the Palette and press the Enter key.
				    	       		</ul>
						        </li>
								<li>Note that Record Fields (Fields associated with a Records fetched using the GET statement) appear in the Palette when the statement Insertion Arrow is positioned inside a FOR EACH Record block. Record Fields are not available outside a FOR EACH Record block.</li>
	        				  	<li>Please be aware of the following issues with this new funtionality, which will be fixed in the next build:
	        				  		<ul>
				                		<li>MCQ Choices are not yet available in the Fields Palette. To create a statement that evaluates a user's selection (e.g., "If Form1:Q1 equals a"), you must type the letter 'a' in the right-hand box.</li>
	            			    		<li>The (selection) "Proxy Field" for MCQs is not avaialable in the Fields Palette. (The Proxy Field is used only inside FOR EACH Question statements. That functionality is currently broken and we are working on it.)</li>
	       			    			</ul>
	       			    		</li>
	       					</ul>
	       				</li>
						<li><strong>Form Qualified Fields in Processes</strong> - Blanks and MCItems now show up as form-qualified fields (E.g., Form1:Q1:a) in the Process window and are handled properly in deployed Projects.</li>
						<li><strong>Fields in "foreign" Forms</strong> - A Process may now reference Fields from Forms other than the one to which it is connected.</li>
					</ul>
	
					<div class="subTitle">Web site:</div>
					<ul>
						<li><strong>Library persistance migrated to PostgrSQL</strong> - The backend database for the library has been changed from DB4o to PostgreSQL</li>
						<li><strong>Cleaned up links</strong> - Went through the page buttons and links in both the Library and Project Manager to make sure they had sensible targets</li>
					</ul>
					<div class="subTitle"> Bug fixes:</div>
					<ul>
					    <li><strong>Mantis Issue 126:</strong> Collapsing a Form in the Tree causes the Form to open.</li>
					    <li><strong>Mantis Issue 128:</strong> After Deleting a project from the Project Manager Delete Button remains... causing 500 error</li>
						<li><strong>Mantis Issue 151:</strong> Variable shows up twice in Text Field DropDown</li>
						<li><strong>Mantis Issue 157:</strong> Theme not applied to "Thanks" page after submit</li>
						<li><strong>Mantis Issue 159:</strong> Trash Icons have different verbage </li>
						<li>Made Fields palette invisible during Project open to eliminate delay caused by multitude of palette redraws.</li>
					</ul>
				</div>
			</div><!-- end news item -->
			
			<div class="newsItem">
				<div class="date">JUNE 12, 2006</div>
				<div><span class="title">Updates in Tawala Build #74</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
						<li><strong>Clean-up unused variables</strong> - Process Variables that are not referenced in SET statements or Arithmetic statements in the Project are now removed from field lists.</li>
						<li><strong>Reduced Statements set</strong> - Unimplemented statement types and the Arithmetic statements have been removed from the Statements Palette for Processes. Arithmetic statements in old Projects will still be read in and displayed, and they will still work, but they cannot be modified. You should now use the SET statement to effect the behavior previously done with Arithmetic statements.</li>
						<li><strong>Fields Palette updates</strong> - Improvements have been made to the Fields Palette to insure that it remains synchronized with various changes in the Project.</li>
						<li><strong>Single installation</strong> - The same installations of the Project Designer and Invitation Manager applications are used to communicate with both the development server and the production server.</li>
						<li><strong>Project file backups</strong> - The designer's old Project files in the default Project folder are automatically copied to a backup folder during installation.</li>
					</ul>
	
					<div class="subTitle">Web site:</div>
	
					<ul>
						<li><strong>Published Projects indicated in deployed list</strong> - On the Project Manager list page projects that have been published to the library will have an icon indicating their status. Hovering over the icon will show more details.</li>
						<li><strong>View bug reports on Web site</strong> - We have changed the Mantis bug tracking system to allow anonymous viewers. There is also a listing on the home page (when logged in) of the latest bugs. Clicking on an item in the list will bring up Mantis in a separate window.</li>
					</ul>
	                <br />                              
					<div class="subTitle">Bug fixes:</div>
					<ul>
						<li>Mantis Issue 149 - Field Palette does not clear after selecting NEW PROJECT</li>
						<li>Mantis Issue 150 - Field Palette Does not show new variables</li>
						<li>Mantis Issue 152 - Deleting Nested IF statements can corrupt .tawala file</li>
					</ul>
				</div>
			</div><!-- end news item -->

			<div class="newsItem">
				<div class="date">JUNE 4, 2006</div>
				<div><span class="title">Updates in Tawala Build #73</span><span class="info">posted by TonyF</span></div>
				<div class="text">
					<div class="subTitle">Project Designer</div>
					<ul>
						<li> <strong>"Global" Fields Palette</strong> -  The Fields Palette is now always available on the right side of the Project Designer UI when editing Forms, Processes or Documents. Currently it is interactively functional only with Documents, but the list is always kept current for reference purposes.</li>
						<li> <strong>Hide / Show Fields Palette</strong> - The Fields Palette may be hidden or shown via the <em>Fields Palette</em> item in the <em>View</em> menu.</li>
					</ul>
	
					<div class="subTitle">Web site:</div>
	
					<ul>
						<li> <strong>Improvements to Edit Categories Screen</strong> - Several bug fixes, improved error handling, faster first screen load.</li>
						<li> <strong>Ability to Test Drive a Project</strong> - On the Project Details page there is a Run button next to each project version.</li>
					</ul>
	                                              
					<div class="subTitle">Bug fixes:</div>
					<ul>
						<li> Mantis Issue 142 - Attempting to format a field in the Documents Window causes Crash when deployed</li>
					</ul>
				</div>
			</div><!-- end news item -->
		
		</div>
        