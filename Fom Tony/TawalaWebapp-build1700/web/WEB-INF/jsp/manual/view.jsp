<%@ page contentType="text/html" %>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions" %>

            <div id="manual">

                <div class="section">
                    <h3 id="General Features">General Features</h3>
                    <p>
                    The Tawala Project Designer is where you create the Forms that will be presented to your Users
                    to elicit information from them, or that you may use for your own purposes to work with the
                    information your Users have submitted.
                    </p>
                    <p>
                    The Project Designer implements a "Multiple Document Interface" or MDI which allows
                    viewing of multiple Forms, Processes or Documents simultaneously. It has an
                    auto-update feature which will automatically check to make sure you are running the
                    most recent version of the application. Also, projects created with earlier versions
                    of the Project Designer will be automatically updated when they are loaded.
                    </p>
                    <p>
					The main Project Designer Window is shown below. It contains the Project Explorer, a 
					"tree" control that allows you to navigate among all your Project components, and the 
					main workspace where the components you are currently constructing are viewed, as 
					well as the Tawala menus and toolbar buttons.
					</p>
                    <div class="image">
                        <img src="/images/manual/project-designer.gif" alt=""/>
                        <div>The Tawala Project Designer</div>
                    </div>
                    <p>
                    The following features are accessible from the File menu. Most are also available via the Tawala toolbar.
                    </p>

                    <h5 id="New Project">New Project</h5>
                    <p>
                    Creates a new blank Project.  If you currently have a Project open, it will be closed
                    (you will be prompted to save it first).  The new Project will have a default name of Untitled.
                    When you save the Project you will have the opportunity to change its name.
                    </p>

                    <h5 id="Open Project">Open Project</h5>
                    <p>
                    Displays a dialog which allows you to select a Project to open.  Select a Project and click
                    the Open button to open it.  If a Project was already open, it will be closed (you will be prompted to save it first).
                    </p>

                    <h5 id="Add New">Add New</h5>
                    <p>
					a cascading menu that allows you to create a new component (Form, Process or Document) to
					your Project. The new component is placed in the appropriate folder and given a default
					name (e.g. "Form 1"). You may rename any component by clicking the current name and typing
					in a new one. Components may also be added via the "tool strip" at the top of the Project
					Explorer or from the context menu of a root component folder (node) in the Project Explorer.
                    </p>

                    <h5 id="Save">Save</h5>
                    <p>
                    Saves the current Project to file.
                    </p>

                    <h5 id="Save As">Save As</h5> 
                    <p>
                    Allows you to save a copy of the current Project with a new name.
                    </p>

                    <h5 id="Deploy Project">Deploy Project</h5>
                    <p>
                    Uploads the current Project to the Tawala Host Server, making your Forms accessible to your
                    Users. Use the Invitation Manager to send invitations to your Users. The Project Manager allows you to manage
                    User data in your deployed Projects.
                    </p>

                    <h5 id="Exit">Exit</h5>
                    <p>
                    Quits the application, prompting you to save your Project first if it has been changed.
                    </p>
                    <p>
                    The following features are accessible from the <i>Format</i> menu.
					</p>
					<h5 id="Project Themes">Project Themes</h5> 
					<p>
					Allows you to choose from a list of styles to change the look of your project when it is deployed.
                    </p>
                    <div class="image">
                        <img src="/images/manual/toolbar.gif" alt=""/>
                        <div>The Tawala Toolbar</div>
                    </div>
                    <div class="js-back">
                        <a class="js-back" href="javascript:history.go(-1)"  onMouseOver="self.status=document.referrer;return true"><img src="/images/gray-arrow-left.gif" /> Back</a>
                    </div>
                </div>
                
                <div class="section">
                     <h3 id="Forms">Forms</h3>
                    <p>
                    In Tawala a "Form" refers to the component that contains the questions you present to your Users, along with
                    the means for them to respond (blanks to fill in, or multiple choices from which to select) and any
                    explanatory text to assist them in completing the Form. The Form is what is what your User sees and
                    interacts with, appearing as a "Web page" (or pages) in a browser window on his or her computer.
                    The Form is presented to your User as a single page with a Submit button at the bottom of it, which
                    the User clicks when he has finished responding to the questions.
                    </p>
                    <p>
                    You create your Forms in the Tawala Form Designer. In the Form Designer you can define all the questions
                    you want to pose to your User, and set up the means of responding. Tawala provides two basic types
                    of Questions: Fill in the Blank and Multiple Choice.
                    </p>

                    <div class="image">
                        <img src="/images/manual/form-designer.gif" alt="" />
                        <div>The Tawala Form Designer </div>
                    </div>
                    <h5 id="Edit">Edit Text and Question Labels</h5>
                    <p>
					You can edit the names of text and questions to make them more meaningful. To do this,
					just double-click on the existing label and type the name you would like to appear.
                    </p>

                    <h5 id="Text">Text</h5>
                    <p>
                    You add text by dragging the Text icon from the Items Palette to the Form Window. The 
                    default text is initially selected, so you can type in your own text to replace 
                    it. Text blocks are identified by a default label in the column left of text (e.g., "T1").
                    </p>
                    <div class="image">
                        <img src="/images/manual/text-item.gif" alt="" />
                        <div>Text Item</div>
                    </div>
                    <h5 id="DisplayFields">Display Fields</h5>
                    <p>
					A Display Field allows you to display the results of a fill-in-the-blank, multiple-choice question or a 
					previously set variable in a text block. To use this feature just click inside of a text 
					block where you want the field to be and select the field you want to include from the 
					dropdown menu below your text. Click the up arrow icon next to the menu to add the field. 
					A marker for that field will be placed in the text block. This marker will be replaced 
					with the contents of the field when the Form is displayed.
					</p>
                    
                    <h5 id="Fill in the Blank">Fill in the Blank Questions</h5>
                    <p>
                    A Fill in the Blank question consists of text plus one or more blanks (underlines) of varying size. Blanks
                    may be interspersed within the text or occur at the end. You create blanks simply by typing underscore
                    characters ( _ ) into your question. Each blank has a default label (e.g. "Q1:a"), but may replace that
                    with an Alternate Label (e.g. "First Name"). Alternate Labels make it easier for you to identify and
                    use the blanks in your Form when you create Processes and Documents. You may also mark a question as
                    "Required" by checking the "Response Required" check box.
                    </p>

                    <div class="image">
                        <img src="/images/manual/fib.gif" alt="" />
                        <div>Fill In The Blank question in edit mode</div>
                    </div>

                    <h5 id="Multiple Choice">Multiple Choice Questions </h5>
                    <p>
                    A Multiple Choice question consists of a question and one or more choices. You may allow your Users to
                    select only one choice from the list you define, or to select more than one choice. In the former case
                    a round "radio button" will appear before each choice on the online Form; in the latter case a square
                    check box will appear before each choice. You add additional choices by pressing the Enter key at the
                    end of your question or of a choice.
                    Each question has default label (e.g. "Q2"), and the choices are identified by the question label plus
                    the choice label (e.g., "Q2:a"). You may also mark a question as "Required" by checking
                    the "Require at least one" check box.
                    </p>

                    <div class="image">
                        <img src="/images/manual/mc-radio.gif" alt="" />
                        <div>Multiple Choice question that allows a User to select only one choice</div>
                    </div>
                    <div class="image">
                        <img src="/images/manual/mc-checkbox.gif" alt="" />
                        <div>Multiple Choice question that allows a User to select more than one choice</div>
                    </div>

                    <h5 id="PageBreak">Page Breaks</h5>
                    <p>
					A page break allows you to split questions in a Form onto separate pages. This provides 
					a way to logically split up items on a Form. Putting a page break between questions in a 
					Form will cause the questions above it to be displayed on a separate page with a submit 
					button. Once the questions are answered and the submit button pressed the items after 
					the page break will be displayed on their own page. A page break does not affect the 
					processing of a Form in any way.
					</p>

                    <h5 id="SkipBlock">Skip Blocks</h5>
					<p>
					A Skip Instructions are a kind of process within a Form. They allow you to skip either 
					forward or backwards over items in a Form depending on the criteria you specified. This 
					allows for skipping questions based on previous answers or looping back to previous 
					questions to change answers.
					</p>
                    <div class="image">
                        <img src="/images/manual/skip.gif" alt="" />
                        <div>Skip Block</div>
                    </div>
					<p>
					To setup a Skip Instruction you drag it to the Form just like any other item. Once in the 
					Form you can click on the "Edit" link in the Skip Instruction to add or modify 
					instructions. When you do this the Item expands to display the Skip Instructions 
					statements, and the palette on the left changes to offer you the available statement 
					types. You can use any combination of these commands to implement the logic you desire. 
					When you are done editing the skip block instructions just click on any item in the 
					Form to collapse the Skip Instructions and return to the regular Form Items Palette.
                    </p>

                    <div class="image">
                        <img src="/images/manual/skip-edit.gif" alt="" />
                        <div>Editing Skip Block instructions</div>
                    </div>
                    
                    <h5 id="Starting Points">Starting Points</h5>
                    <p>
                    A Starting Point is an entry into the project. The first Form that you create in a project 
                    is always designated as a starting point. You may toggle the starting point attribute of 
                    any form in a project. A project must have at least one starting point in order to function.
                    </p>
                    <p>
                    To set a Form as a starting point select the Form name in the Project Explorer and
                    either select "Starting Point" from the "Edit" menu or right click and select
                    "Starting Point" from the context menu.
                    </p>
                    <div class="js-back">
                        <a class="js-back" href="javascript:history.go(-1)"  onMouseOver="self.status=document.referrer;return true"><img src="/images/gray-arrow-left.gif" /> Back</a>
                    </div>
                </div>
                <br /><br />

                <div class="section">
                    <h3 id="Processes">Processes</h3>
                    <p>
                    A Tawala Process is a collection of instructions that you build to describe "what to do" 
                    with information that has been collected from a Form. For a Process to be used, it must 
                    be connected to a Form. The connection of a Process to a Form is done in the Project 
                    Explorer by dragging a Process and dropping it on a Form, or by clicking in the 
                    Info Bar at the top of a Process window. A Process attached to a Form will execute 
                    automatically after the User has completed and submitted the Form.
                    </p>

                    <div class="image">
                        <img src="/images/manual/connected-process.gif" alt="" />
                        <div>Form with a connected Process</div>
                    </div>
                    <p>
                    However, it is not necessary for a Form to have a connected Process in order to make 
                    that Form available to your Users. A Form without a connected Process will collect and 
                    store all the responses your Users submit to the questions on the Form. This basic 
                    behavior will be appropriate for many Forms, such as surveys.
                    </p>
                    <p>
                    You create your Processes in the Tawala Process Designer by selecting statement or 
                    command from the Statements Palette, constructing its exact logic in the Statement 
                    Details Panel, and adding it to the Process Window by clicking the Add button. To 
                    minimize the possibility of syntax and logic errors, Process statements may not be 
                    edited directly in the Process Window.
                    </p>
                    <div class="image">
                        <img src="/images/manual/process-designer.gif" alt="" />
                        <div>The Tawala Process Designer</div>
                    </div>

                    <h5 id="IfSimple">If Statement (Simple)</h5>
                    <p>
                    The If statement allows you to define a condition and one or more actions to perform if the condition is
                    true. The Otherwise option permits you to define one or more actions if the condition is untrue.
                    </p>

                    <div class="image">
                        <img src="/images/manual/if-simple.gif" alt="" />
                        <div>Statement details Panel for the Simple If statement</div>
                    </div>

                    <h5 id="IfAdvanced">If Statement (Advanced)</h5>
                    <p>
					Just like the simple If statement outlined above the Advanced If lets you define 
					conditions and actions to perform. The difference is that the advanced version allows 
					multiple conditions to be specified that are tied together using logical AND and OR 
					operators. This allows you to create a much broader range of conditions.
					</p>
                    <div class="image">
                        <img src="/images/manual/if-advanced.gif" alt="" />
                        <div>Statement details Panel for the Advanced If statement</div>
                    </div>
					<p>
					To use the Advanced If click on the "Advanced" tab and click the "Set Conditions" button. 
					You will be presented with the Expression Builder dialog. In the top part of the 
					dialog box  you can create comparisons just like the simple if. Once the comparison 
					is created click the down arrow in the middle of the dialog to move it down to the 
					Conditions box. As you add more comparisons you can use the AND and OR operators to 
					determine how they will be interpreted.
					</p>
                    <div class="image">
                        <img src="/images/manual/expression-builder.gif" alt="" />
                        <div>Expression Builder dialog</div>
                    </div>

					<h5 id="Show">Show Statement</h5>
					<p>
					There are two variations of the Show statement: Show Document and Show Form. You
					select the type you wish via the tabs at the top of the Statement Details Panel.
					</p>

                    <h5 id="Show">Show Document</h5>
                    <p>
                    Use the Show Document statement to display a Web page containing a Document. You may 
                    show more than one Document on a page by placing multiple Show commands in your 
                    Process. Each Show statement appends the contents of the selected Document to the 
                    end of the previous one for display to your users.
                    </p>
                    <p>
                    If a Show Document statement is followed by a Show Form statement the Form will be 
                    appended to the Document and they will both be shown on the same web page.
                    </p>
                    <div class="image">
                        <img src="/images/manual/show-document.gif" alt="" />
                        <div>Show Document statements in the Process Window</div>
                    </div>

                    <h5 id="ShowForm">Show Form</h5>
					<p>
					The Show Form statement allows you to leave the Form you are currently in and present a 
					different Form to the user (within the same project). Execution of a Show Form 
					statement causes a Process to stop execution at the point where the Show Form statement 
					is encountered and to immediately display the indicated Form to the user. 
					</p>

                    <div class="image">
                        <img src="/images/manual/show-form.gif" alt="" />
                        <div>Show Form statements in the Process Window</div>
                    </div>

                    <h5 id="Send">Send</h5>
                    <p>
                    Send statement There are two types of Send statement in the current build: Send Email and
                    Send Document. You select the type of message you wish to send via the tabs at the top of the
                    Statement Details Panel.   The Send statement automatically generates and sends an email message
                    to one  recipient, and you may cc another recipient. The recipients’ addresses may be literal
                    text entered by you, or they can come from blanks filled in by your Users.
                    </p>


                    <h5 id="Send Email">Send Email</h5>
                    <p>
                    In the basic Send Email statement, you enter the text for your message directly into the  Body text box.
                    </p>

                    <div class="image">
                        <img src="/images/manual/send-email.gif" alt="" />
                        <div>Send Email statement </div>
                    </div>

                    <h5 id="Send Document">Send Document</h5>
                    <p>
                    The Send Document statement will use the contents of a Document for the Body text.
                    </p>

                    <div class="image">
                        <img src="/images/manual/send-document.gif" alt="" />
                        <div>Send Document statement</div>
                    </div>

                    <h5 id="Send Invitation">Send Invitation</h5>
                    <p>
                    The Send Invitation Statement allows you to send an invitation to a project from within a
                    Process. It is similar to the Send Email statement but adds a link to the project in the
                    body of the message.
					</p>
                    <div class="image">
                        <img src="/images/manual/send-invitation.gif" alt="" />
                        <div>Send Invitation Statement</div>
                    </div>

                    <h5 id="Set">Set Statement</h5>
                    <p>
                    The Set statement establishes the value of a variable. The variable can be assigned an  arithmetic value
                    or a text string (Expression), which itself can be a constant value, a variable, or a combination of
                    these. In constructing an expression, you may do any or all of  the following:
                    </p>
                    <ul>
                    	<li>Type any arithmetic or string values directly into the Expression box.</li>
	                    <li>Type any arithmetic operators (+, -, * /) directly into the Expression box.</li>
	                    <li>Select a Field (response from a Form question or another Process variable from the Field List, then
    	                    click the  arrow button on the right to move it into the Expression box.</li>
                    </ul>

                    <div class="image">
                        <img src="/images/manual/set.gif" alt="" />
                        <div>Set statement used to establish an arithmetic value</div>
                    </div>
                    <div class="image">
                        <img src="/images/manual/set-field.gif" alt="" />
                        <div>Set statement using a Field in an arithmetic expression</div>
                    </div>
                    <div class="image">
                        <img src="/images/manual/set-concatenate.gif" alt="" />
                        <div>Set statement used to concatenate two text Fields</div>
                    </div>

					<h5 id="Append">Append Statement</h5>
					<p>
					The append statement allows you to add the contents of one Document to another. To 
					use it just drag the Append icon to the form window. You will then be asked to choose 
					two Document names. The first Document will be appended to the second one.
					</p>
					<p>
					When choosing the second Document you have the option of typing in a name. If the name 
					you enter does not belong to an existing document then a "Virtual" Document will be 
					created. Virtual Documents can be used just like any other Document but they do not 
					show up in the Project Explorer. A Virtual Document only lasts as long as a project 
					is being run. Once the project session is over the Document and it's contents will 
					be lost.
					</p>

                    <div class="image">
                        <img src="/images/manual/append.gif" alt="" />
                        <div>Append statement</div>
                    </div>

					<h5 id="Get">Get Statement</h5>
					<p>
					The Get statement is used to retrieve data collected from responses to Forms in your 
					Project. All the responses of all of the Users of your project are saved on the Tawala 
					server (unless you explicitly delete them with the Project Manager) and may be retrieved.
					</p>
					<p>
					When you add a Get statement to a Process you will be asked to supply a "record list" 
					name and the name of the Form from which to retrieve the data. All the data collected 
					for the selected Form will be available in the record list and can then be accessed 
					via a "ForEach" statement.
					</p>

                    <div class="image">
                        <img src="/images/manual/get.gif" alt="" />
                        <div>Get statement</div>
                    </div>

					<h5 id="ForEach">ForEach Statement</h5>
					<p>
					The ForEach statements allow you to iterate through data retrieved using the Get statement. There
					are two types of ForEach statements: ForEach Record and ForEach Question.
					</p>
					<h5 id="ForEach Record">ForEach Record Statement</h5>
					<p>
					The ForEach Record statement allows you to iterate through the data returned from a Get statement. 
					If you think of the data returned by the Get statement as a series of rows, or records, 
					the ForEach statement allows you to take each row and work with the data contained 
					in it. The data in a row includes one User?s responses to the Form plus any values 
					created by variables in the Process connected to that Form.
					</p>
					<p>
					To setup the ForEach Record you will be presented with two drop-down menus. In the first you 
					supply a "record" variable name you'll use to reference the data within the ForEach 
					block or "loop". You can either choose an existing variable name from the list or 
					enter a new one. From the second drop-down menu you select the ?record list? defined 
					in a previous Get statement that contains the records you?re interested in. With 
					those two elements defined, you can reference one record at a time from the record 
					set within the ForEach block.
					</p>

                    <div class="image">
                        <img src="/images/manual/foreach-record.gif" alt="" />
                        <div>ForEach Record statement</div>
                    </div>

					<h5 id="ForEach Question">ForEach Question Statement</h5>
					<p>
					The ForEach Question statement is used inside a ForEach Record statement and allows you to iterate 
					through questions contained in a record.					
					</p>
					<p>
					 To setup the ForEach Question statement just choose the type of questions you want to iterate over
					 from the drop-down menu and click "Add". <i>At this time only Multiple Choice questions are supported</i>
					</p>

                    <div class="image">
                        <img src="/images/manual/foreach-question.gif" alt="" />
                        <div>ForEach Question statement</div>
                    </div>

                    <h5 id="Arithmetic">Arithmetic Statements</h5>
                    <p>
                    Arithmetic statements are used to modify the value of a variable by addition, subtraction,
                    multiplication or division. The variable must have been previously established with a Set
                    statement. The modifying value (the one being added, subtracted, etc.) can be either a
                    constant numeric value (a number) or another Field.
                    </p>


                    <h5 id="Add">Add</h5>

                    <div class="image">
                        <img src="/images/manual/add.gif" alt="" />
                        <div>Add statement</div>
                    </div>
                    <br />

                    <h5 id="Subtract">Subtract</h5>

                    <div class="image">
                        <img src="/images/manual/subtract.gif" alt="" />
                        <div>Subtract Statement</div>
                    </div>
                    <br />

                    <h5 id="Multiply">Multiply</h5>

                    <div class="image">
                        <img src="/images/manual/multiply.gif" alt="" />
                        <div>Multiply Statement</div>
                    </div>
                    <br />

                    <h5 id="Divide">Divide</h5>

                    <div class="image">
                        <img src="/images/manual/divide.gif" alt="" />
                        <div>Divide Statement</div>
                    </div>
                    <div class="js-back">
                        <a class="js-back" href="javascript:history.go(-1)"  onMouseOver="self.status=document.referrer;return true"><img src="/images/gray-arrow-left.gif" /> Back</a>
                    </div>
                </div>

                <div class="section">
                    <h3 id="Documents">Documents</h3>
                    <p>
                    You can create Documents to display or send to Users (or yourself). The Show statement 
                    is used to display a Document on the screen to your Users. The Document will be 
                    displayed as a Web page and is constructed in the Process you define to run immediately 
                    when your User completes a Form. The Send statement can be used to send a Document 
                    to a User as the body of an email message.
                    </p>

                    <h5 id="Fields">Fields</h5>
                    <p>
                    Documents can contain Fields, which are placeholders for information from Forms and 
                    Processes. Fields are replaced by actual User responses to Forms or by variables created 
                    with the Set statement in the Process. Replacement is done at the time the Document is 
                    constructed in the Process. This allows you to design personalized or customized 
                    Documents to present to your Users.
                    </p>
                    <p>
                    A Field is inserted in a document by dragging it from the Fields Palette and dropping it in  the
                    Document, or by double-clicking on the Field in the Palette.
                    </p>

                    <div class="image">
                        <img src="/images/manual/document-designer.gif" alt="" />
                        <div>Document Menu bar and Toolbar</div>
                    </div>

                    <h5 id="Default Form">Default Form Acknowledgement</h5>
                    <p>
                    Provides automatic "Thank you page" to User when User submits Form
                    Designers will be able to replace default page in future Alpha release
                    </p>

                    <h5 id="VirtualDocuments">Virtual Documents</h5>
                    <p>
					In addition to regular Documents it is possible to create Virtual Documents. These 
					Documents do not show up in the Document Designer window. They are created using the 
					Append command in the Process Designer. Regular Documents as well as other Virtual 
					Documents can be appended to a Virtual Document. The Virtual Document can be used 
					within a process just like a regular Document. The main difference being that once 
					the process is finished the Virtual Document will cease to exist.
					</p>

                </div>

                <div class="section">
                    <h3 id="Invitation Manager">Invitation Manager</h3>
                    <p>
                    The Tawala Invitation Manager allows you to create, manage and send custom email  invitations to your Users,
                    letting them know that a Form is available for them to fill out. (Forms are referred to as Starting
                    Points in the Invitation Manager.) The Invitation Manager presents you with a list of all your deployed
                    Projects.  Listed under each Project are the Starting Points to which you may invite a User. Under each
                    of these you may create and edit one or more invitations. The invitations are saved when you exit the
                    application. When you create a new invitation a link to the Starting Point is automatically inserted into
                    the message as a URL link to the Starting Point you specify for the invitation. When your  Users receive the
                    email and click on the link the Form for that Starting Point will open in a browser window. Be careful not
                    to delete or modify this link as you create your invitation message.
                    </p>

                    <div class="image">
                        <img src="/images/manual/invitation-manager.gif" alt="" />
                        <div>The Invitation Manager</div>
                    </div>
                    <p>
                    When you are satisfied with your message, click the Send Invitation button to open it in a new email
                    message window of you own email client application (e.g., Outlook Express).  From there you may enter
                    addresses from your own address book and send the message on to your User(s). (Please be sure to use
                    common email etiquette to avoid creating "Spam" and to protect your user's privacy.) The Invitation
                    Manager application can be run by itself, or it can be invoked from the Project Designer.
                    </p>
                    <div class="js-back">
                        <a class="js-back" href="javascript:history.go(-1)"  onMouseOver="self.status=document.referrer;return true"><img src="/images/gray-arrow-left.gif" /> Back</a>
                    </div>
                </div>

                <div class="section">
                    <h3 id="Project Manager">Project Manager</h3>
                    <p>
                    You use the Tawala Project Manager to manage your deployed Projects. In the current  version, the Project
                    Manager allows you to do the following:
                    </p>
                    <ul>
                    	<li>View response data from your Users</li>
                    	<li>View Summary of response data to multiple choice questions</li>
	                    <li>Erase stored User information</li>
                    </ul>
                    <p>
                    The Project Manager can be invoked from the toolbar or the Tools menu of the Project Designer, or it
                    can accessed from the Tawala.com Web site. This will bring you to the project list page. Here you can
                    see all the projects you have deployed along with the amount of space they are using. You can also delete
                    or purge all the data for a project from this page.
                    </p>

                    <div class="image">
                        <img src="/images/manual/projectmanager-list.gif" alt="" />
                        <div>Project Manager List page</div>
                    </div>
                    <p>
                    Clicking on a project name either in the Project Manager list or the project list in the 
                    left column of the screen will bring up the project details page. Here you can see all the
                    information related to a particular project. In addition to being able to delete a project 
                    and purge all project data you can also purge data for individual forms. You can also choose 
                    to view the data or a summary of the data.
                    </p>

                    <div class="image">
                        <img src="/images/manual/projectmanager-details.gif" alt="" />
                        <div>Project Details page</div>
                    </div>
                    <div class="js-back">
                        <a class="js-back" href="javascript:history.go(-1)"  onMouseOver="self.status=document.referrer;return true"><img src="/images/gray-arrow-left.gif" /> Back</a>
                    </div>
                </div>
            </div>
