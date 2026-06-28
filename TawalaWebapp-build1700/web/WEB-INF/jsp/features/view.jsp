<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

			<h2>Features</h2>
            <div id="manual">
                    <p>
                    <b>The Tawala project</b> is still in the early stages of development and things are changing very
                    quickly. The lists below will give you an idea of where we are, where we're heading and what
                    things might be coming in the future. This list is going to be changing often with features
                    appearing, disappearing and sliding around between the lists.
                    </p>
                    <p>
                    If you have any thoughts about the features listed below (features that you think should be
                    listed below!) you can post comments in our
                    <a href="http://www.tawala.com/forum/forums/show/4.page">Feature Requests</a> forum.
                    </p>
                <div class="section">
                    <h3>Currently Implemented Features</h3>
                    <p>
                        These are features that are in the current build of the product. Clicking the items below
                        will bring more detail about that feature.
                    </p>

                    <h4><a href="/manual#General Features">General Features</a></h4>
                    <ul>
	                    <li><a href="/manual#GeneralFeatures">Main Window with Project Pane</a></li>
	                    <li><a href="/manual#New Project">New Project</a></li>
	                    <li><a href="/manual#Open Project">Open Project</a></li>
	                    <li><a href="/manual#Add New">Add New {Document, Form, Process}</a></li>
	                    <li><a href="/manual#Save">Save</a></li>
	                    <li><a href="/manual#Save As">Save As</a></li>
	                    <li><a href="/manual#Deploy Project">Deploy Project</a></li>
	                    <li><a href="/manual#Exit">Exit</a></li>
		                <li><a href="/manual#Project Themes">Project Themes</a></li>
					</ul>
                    <h4><a href="/manual#Forms">Forms</a></h4>
                    <ul>
	                    <li><a href="/manual#Text">Text</a></li>
	                    <li><a href="/manual#DisplayFields">Add Display Fields to Text items</a></li>
	                    <li><a href="/manual#Fill in the Blank">Fill in the Blank Questions</a></li>
	                    <li><a href="/manual#Multiple Choice">Multiple Choice Questions</a></li>
	                    <li><a href="/manual#Skip">Skip Instructions</a></li>
	                    <li><a href="/manual#Show">Show Form command</a></li>
	                    <li><a href="/manual#Labels">Text &amp; Question Labels Editable</a></li>
	                    <li><a href="/manual#PageBreak">Page Breaks</a></li>
	                    <li><a href="/manual#Required">Required Answers are handled</a></li>
	                    <li><a href="/manual#Starting Points">Starting Points</a></li>
					</ul>
                    <h4><a href="/manual#Processes">Processes</a></h4>
                    <ul>
	                    <li><a href="/manual#If-Simple">If Statement (Simple)</a></li>
	                    <li><a href="/manual#If-Advanced">If Statement (Advanced)</a></li>
	                    <li><a href="/manual#Show">Show Statement</a></li>
	                    <li><a href="/manual#Send">Send Statement</a></li>
	                    <li><a href="/manual#Send Email">Send Email Statement</a></li>
	                    <li><a href="/manual#Send Document">Send Document Statement</a></li>
		                <li><a href="/manual#Send Invitation">Send Invitation Statement</a></li>
	                    <li><a href="/manual#Send Invitation">Send Invitation Statement</a></li>
	                    <li><a href="/manual#Set">Set statement</a></li>
	                    <li><a href="/manual#Arithmetic">Arithmetic Statements (Add, Subtract, Multiply, Divide)</a></li>
	                    <li><a href="/manual#Append">Append command</a></li>
	                    <li><a href="/manual#Get">Get Statement</a></li>
	                    <li><a href="/manual#ForEach">ForEach Statement</a></li>
        		        <li><a href="/manual#ForEach Record">ForEach Record Statement</a></li>
		                <li><a href="/manual#ForEach Question">ForEach Question Statement</a></li>
					</ul>
                    <h4><a href="/manual#Documents">Documents</a></h4>
                    <ul>
	                    <li><a href="/manual#Fields">Fields in documents</a></li>
	                    <li><a href="/manual#Default Form">Default Form Acknowledgement</a></li>
	                    <li><a href="/manual#VirtualDocuments">Virtual documents</a></li>
					</ul>
                    <h4><a href="/manual#Invitation Manager">Invitation Manager</a></h4>
                    <ul>
	                    <li><a href="/manual#Invitation Manager">Public Invitations</a></li>
					</ul>
                    <h4><a href="/manual#Project Manager">Project Manager</a></h4>

                </div>

                <div class="section">
                    <h3>New Features in Alpha Release 5</h3>
                    <p>
                        These are features that have been added to the current release of the product.
                    </p>
                    <h4>General Features</h4>
                    <ul>
						<li>Contents of drop-down lists are now refreshed when the Insertion Pointer is repositioned in the Process window.
						<li>Forms in a Project may now be designated as Starting Points for the project</li>
						<li>The icon for Starting Point Forms in the Project Explorer has been changed to a green flag for better visibility						
						<li>The Designer may now select from a list of themes that will change the appearance of a project						
						<li>Insertion marker enhancements:
							<ul>
								<li>The insertion marker to the left the Process window can be moved by clicking and dragging it</li>
								<li>The marker remains visible as you drag it</li>
								<li>As you drag the insertion point it is indicated by a horizontal line in Process window</li>
								<li>Upon release, the marker snaps to the nearest insertion point</li>
								<li>The Process window scrolls if you drag the marker past the bottom or top of the visible Process lines</li>
							</ul>
							</li>
					</ul>
					
                    <h4>Forms</h4>
                    <ul>
						<li>Any form in a Project may now be previewed in a Web browser						
                    </ul>

                    <h4>Processes</h4>
                    <ul>
						<li>New Variables can be created in a Process by entering a new Variable name in the ADD, SUBTRACT, MULTIPLY or DIVIDE statement details</li>
						<li>SEND Invitation statement has been implemented. Public invitations may be sent to any Form in the same Project</li>
						<li>A SHOW Document statement followed by a SHOW Form statement will display the document with the Form immediately after it on the same page</li>
						<li>Multiple choice questions may be compared in Simple IF statements (e.g., If Q1 equals record:Q1)</li>
						<li>Set and Arithmetic statements can modify or create new Variable fields in fetched records</li>
						<li>The SET and Arithmetic statements, may now be used used inside a For Each statement to create new fields in a Record</li>
						<li>For Each Multiple Choice Question allows comparisons such as If (selection) equals Record:(selection)</li>
					</ul>

                    <h4>Invitation Manager</h4>
                    <ul>
	                    <li>Invitation Manager now lists only Forms designated as Starting Points</li>
					</ul>

                    <h4>Project Manager</h4>
                    <ul>
						<li>The Project Manager has been redesigned with a separate details page for each project</li>			
						<li>Sizes of the project and its data are displayed.</li>						
						<li>You can now purge the all the data for a project with one click.</li>
						<li>Clicking a project name in the list on the left side of the page brings you to the project details page.</li>
						<li>Clicking on the table headings in the Project Manager will sort the items in the table.</li>
					</ul>
                </div>

                <div class="section">
                    <h3>Features Coming In Alpha Release 6</h3>
                    <p>
                        These are features that we are hoping to get into Alpha Release 6 of the product.
                    </p>

                    <h4>General Features</h4>
                    <ul>
	                    <li>Undo/Redo</li>
	                    <li>Cannot deploy Project if any Process contains invalid statements</li>
	                    <li>Option to store projects on the server</li>
	                    <li>Designer my create a custom Style Sheet for the project</li>
	                    <li>More user-friendly text for server errors</li>
	                    <li>Collapse / Expand Process view</li>
					</ul>
					
                    <h4>Forms</h4>
                    <ul>
	                    <li>Set/change values of Form properties</li>
	                    <li>Designate an image to appear at the top of the Form</li>
	                    <li>Number formatting</li>
	                    <li>Default text in Form Items selected as a whole block for replacement.</li>                    
                    </ul>

                    <h4>Processes</h4>
                    <ul>
	                    <li>Text formatting in Text and Question items</li>
	                    <li>Upon statement</li>
	                    <li>Next statement</li>
	                    <li>Print statement</li>
	                    <li>Sort Invitation</li>
	                    <li>Fields in body text of Send statement</li>
	                    <li>UI for adding and removing parentheses to and from the conditions box</li>
	                    <li>Don't highlight "non-editable" lines in Process</li>
	                    <li>Specify range of Multiple Choice questions in ForEach statement</li>
					</ul>
					
                    <h4>Documents</h4>
                    <ul>
	                    <li>Formatting of text within a Document</li>
	                    <li>Live hyperlinks within a Document</li>
	                    <li>Display images within a Document</li>
						<li>Include table formatting within a Document</li>
						<li>Fields in Documents can only be selected as a unit for deletion or replacement.</li>
					</ul>

                    <h4>Invitation Manager</h4>
                    <ul>
	                    <li>Send private Invitations</li>
					</ul>

                    <h4>Website</h4>
                    <ul>
                    	<li>Browse Library of projects</li>
                    	<li>Download project from Library</li>
                    	<li>Submit a project for publication in the Library</li>
	                    <li>Show number of responses to deployed Projects</li>
                    </ul>

                </div>

                <div class="section">
                    <h3>Features Coming In Alpha Release 7</h3>
                    <p>
                        These are features that we are hoping to get into Alpha Release 7 of the product.
                    </p>

                    <h4>General Features</h4>
                    <ul>
	                    <li>Print/Print Preview of all Designer app views</li>
					</ul>
					
                    <h4>Forms</h4>
                    <ul>
	                    <li>A multi-line text box can be added to the Form for free-form text entry.</li>	                    
						<li>Designer can insert a live hyperlink in a form</li>
                    </ul>

                    <h4>Processes</h4>
                    <ul>
	                    <li>Send a private invitation from a Process</li>
	                    <li>Text is maintained between tabs in Send details</li>
					</ul>

                    <h4>Invitation Manager</h4>
                    <ul>
	                    <li>Attach a document or hyperlink to email / invitation</li>
					</ul>

                    <h4>Website</h4>
                    <ul>
                    	<li>Rate projects in the Library</li>
	                    <li>List of most popular Projects displayed on the web page</li>
	                    <li>List of featured projects</li>
                    </ul>

                </div>
		</div>
