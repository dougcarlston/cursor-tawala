<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

			<div id="tutorial">
			
				<div class="section">
					<h3>Step-By-Step Procedure To create a simple Project with Tawala</h3>
	                <p>
	                    To familiarize you with the basic components of Tawala Designer we'll show you how to
	                    create a simple input form that you will post online for your invited users to fill out.
	                </p>
					<br />
					
	                <h3>Create a Form</h3>
	                <ol>
	                    <li>Start the <b>Tawala Project Designer</b> from the All Programs / Tawala folder in the Windows Start menu.</li>
	                    <li>In the Project Explorer toolbar on the left, click on the "New Form" icon, the one farthest to the left.
	
	                    	<div class="note">You will see in the <i>Items Palette</i> tools that are available to 
	                    		you as components for building your Form, just to the right of the <i>Project 
	                    		Explorer Palette</i>. Also note that in	the <i>Project Explorer</i> on the left, 
	                    		"<b>Form 1</b>" has been added under the Forms node and a new Window titled 
	                    		"<b>Form 1</b>" has appeared in the central work area.</div>
	                 	</li>
	                </ol>
                </div>
				<div class="section">
                  <h3>Add a Text Item to the Form</h3>
                  <ol>
                      <li>Click on the <b>Text</b> icon in the <i>Items Palette</i>, drag it over the <i>Form Window</i> and drop it.

                      	<div class="note">Notice that a text item appears with the label "T1" on the left and an editable
                           text area on the right.</div>
                           
                      </li>
                      <li>Begin typing to replace the highlighted default text. Type in "My first Tawala Form"</li>
                  </ol>
                  <div class="image">
                      <img src="/images/tutorial1/tut1-figure1.gif" />
                      <div>Figure 1 - Tawala Project Designer after adding Text Item to the Form</div>
                  </div>
    			</div>
    			<div class="section">
                  <h3>Add a Fill in the Blank question</h3>
                  <ol>
                      <li>Next, drag a <b>Fill in the Blank</b> question from the <i>Palette</i> and drop it in the <i>Form Window</i> beneath the text item.
                      
                      	<div class="note">Notice that a text item appears with the label "Q1" on the left, identifying this
                          as Question 1 in your Form. Also notice the "blank line" that appears after the
                          default question text. In Tawala, you create blanks for Fill in the Blank
                          questions by typing in underscore ( _ ) characters.</div>
                          
                      </li>
    
                      <li>Begin typing to replace the highlighted default text. Type in "<code>Type something
                          here:</code>" Leave the "blank line" in place.</li>
                  </ol>
                  </div>
                  <div class="section">
                  <h3>Add a Multiple Choice question</h3>
                  <ol>
                      <li>Drag a <b>Multiple Choice</b> question from the <i>Palette</i> and drop it in the Form
                          Window beneath the previous question.</li>
                      <li>Type to replace the default text: "What is your favorite color?"</li>
                      <li>Place the text cursor next the first choice, labeled "a)", and type in "<code>Red</code>".</li>
                      <li>Press the Enter key to create the next choice, and type in "<code>Green</code>".</li>
                      <li>Press the Enter key again and type in "<code>Blue</code>".</li>
                  </ol>
                  <div class="image">
                      <img src="/images/tutorial1/tut1-figure2.gif" />
                      <div>Figure 2 - Completed Form</div>
                  </div>
    			</div>
    			<div class="section">
                  <h3>Create a Document</h3>
                  <ol>
                      <li>In the <i>Project Explorer toolbar</i>, click on the "New Document" icon (the third 
                      	from the left). A Document Window entitled "<b>Document 1</b>" will appear in the central workspace 
                      	on top of "<b>Form 1</b>."</li>
                      <li>In the Document Window (the large one on the right), type in "<code>You entered:</code>"
                          followed by a space.</li>
                      <li>Along the righthand side of the application you will see a <i>Fields Palette</i>. Click on "<b>Q1:a</b>", which 
                      	should appear right below "<b>Form 1</b>" in the <i>Palette</i>. Drag it to the <i>Document Window</i> to the 
                      	right of the text you just typed.
                      	
                      	<div class="note">Notice that the Field appears in the Document surrounded by double angle
                          brackets (&lt;&lt;&nbsp;&gt;&gt;) to distinguish it from normal text.</div>
                      
                      </li>
                      <li>Place the text cursor to the right of the <i>Field</i> you just inserted, and type the
                          following: "<code>For favorite color you selected:</code>" followed by a space.</li>
                      <li>In the Fields Palette, click on "Q1:a", drag it and drop it in the Document Window
                          to the right of the text you just typed.</li>
                  </ol>
                  <div class="image">
                      <img src="/images/tutorial1/tut1-figure3.gif" />
                      <div>Figure 3 - Document</div>
                  </div>
    			</div>
    			<div class="section">
                  <h3>Add a Process</h3>
                  <ol>
                      <li>In the <i>Project Explorer toolbar</i> click on the "New Process" icon, which is second from the 
                      	left.  Another windows, entitled "<b>Process 1</b>", will appear in the central workspace.</li>
                      
                      <li>We want to tell Tawala to execute this process after Form 1 is received. To do this, 
                      	select "<b>Process 1</b>" from the list in the Project Explorer, and drag it up to and drop it on 
                      	"<b>Form 1</b>" just above. You will see a copy of "<b>Process 1</b>" appear, indented and attached to 
                      	"<b>Form 1</b>", in the <i>Project Explorer</i>
    
                      	<div class="note">This creates a "connection" between the Form and the Process, which
                          means that "Process 1" will automatically run whenever one of your users
                          complete "Form 1" online.</div>
                      
                      </li>
                      
                      <li>Select "<b>Process 1</b>" in the <i>Project Explorer</i>. Note that when a Process Window is highlighted 
                      	in the central workspace, the <i>Items Palette</i> is replaced by a <i>Statements Palette</i> containing 
                      	commands that can be dragged and dropped into a Process.</li>
                      
                      <li>Click on the <b>Show</b> statement button in the <i>Statements palette</i>.
                      
                      	<div class="note">Notice that the <i>Statement Details</i> panel appears at the top of the 
                      	<i>Process Window</i> on the right.</div>
                      </li>
                      
                      <li>In the <i>Statement Details</i> panel, select "<b>Document 1</b>" from the list (since 
                      	there is only one Document, it will actually already be selected).</li>
                      
                      <li>Press the <i>Add</i> button at the bottom of the <i>Statement Details</i> panel. This will insert
                          the <i>Show</i> statement into your Process List.</li>
                  </ol>
                  <div class="image">
                      <img src="/images/tutorial1/tut1-figure4.gif" />
                      <div>Figure 4 - Process</div>
                  </div>
    			</div>
    			<div class="section">
                  <h3>Deploy and test the Form</h3>
                  <ol>
                      <li>You have now completed building your first Project. To try it out, press the 
                      	<i>Deploy Project</i> button on the Main toolbar (the fourth icon from the left), or select 
                      	the <i>Deploy Project</i> item from the <i>File</i> menu.</li>
                      	
                      <li>You will be prompted to save the Project (unless you have previously saved it
                          already). Save it as "My First Project".</li>
                      <li>After a moment the <i>Deployed Project</i> dialog will appear. It contains a link to the
                          Form. Click on the link and the Form you just created will appear in your
                          browser.</li>
                      <li>Answer the questions in the Form and press the <i>Submit</i> button to see the results.
                          You may press the Back button in your browser to try other responses and verify
                          that the Process works correctly.</li>
                      <li>When you're done testing, close the browser and press the <i>OK</i> button in the
                          <i>Deployed Project</i> dialog to dismiss it.</li>
                  </ol>
                 </div>
                 <div class="section">
                  <h3>Invite your users with the Invitation Manager</h3>
                  <ol>
                      <li>Click on the <i>Invitation Manager</i> button in the toolbar, or select <i>Invitation
                          Manager</i> from the <i>Tools</i> menu.

                      <div class="note">The <b>Invitation Manager</b> can also be launched from the <b>Tawala</b> folder
                          in the <i>Windows Start / All Programs</i> menu.</div>
                      
                      </li>

                      <li>In the <i>Deployed Projects</i> pane on the left, select the "<b>Form 1</b>" under "My First
                          Project".</li>
                      
                      <li>Click the <u>Click here to add a new Invitation</u> link.</li>
                      <li>Type "<code>Test my form online</code>" in the <i>Subject</i> box.</li>
                      <li>Type any additional text you like into the main body text window.

                      	<div class="note">Be careful not to delete or overwrite any portion of the URL link in this
                          window. This is the link that will allow your users to go to your deployed
                          online quiz.</div>
                      
                      </li>

                      <li>Click the "<b>Send Invitation</b>" button at the lower right.

                      	<div class="note">This will create a message using your own email client application. You may
                          select an address or addresses from your email contacts list or address
                          book, and send the invitation. You may return to the <b>Invitation Manager</b> and
                          click "Send Invitation" as many times as you like to generate additional
                          messages.</div>
                      
                      </li>

                      <li>After selecting your recipients, send the message(s) via your email application.</li>
                  </ol>
                  <div class="image">
                      <img src="/images/tutorial1/tut1-figure5.gif" />
                      <div>Figure 5 - Invitation Manager</div>
                  </div>
				</div>
			</div>