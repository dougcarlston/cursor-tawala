<%@ page contentType="text/html"%>
<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core"%>
<%@ taglib prefix="fn" uri="http://java.sun.com/jsp/jstl/functions"%>

<div id="tawalaNews" class="newsFeed"></div>
<!--
<div class="newsItem">
	<div class="date">APRIL 15, 2008</div>
	<div><span class="title">Tax Day</span><span class="info">posted by TonyF</span></div>
	<div class="text">
		<p>
			It's that time of year once again to make sure the government gets it's fair share of what you make. Here's 
			hoping you're getting a refund!
		</p>
		<p>
			Not too many changes for Tawala this time around. We did simplify the customization process, cutting it down
			to three steps from five. There are a couple of changes in the categorizer function which we hope will
			make it easier to move items from the source to the destination tables. Of course there are also a few 
			bug fixes for good measure.
		</p>
		
		<h6>Website changes:</h6>
		<ul>
			<li>Revised categorizer to show tables side-by-side and scroll if more than 20 rows.</li>
			<li>Simplified customization process.</li>
			<li>Caching project runtime to improve performance.</li>
			<li>Changes to the Excel data import: empty rows are skipped, empty columns are properly recognized.</li>
		</ul>

		<h6>Project Designer Fixed</h6>
		<ul>
			<li>Fixed: Copying and Pasting a Document that contains an invitation results in Exception Error</li>
			<li>Fixed: Copying and Pasting a Form with a Hyperlink in a Text Item causes Exception Error</li>
			<li>Fixed: Show URL within an if causes a Bad Save</li>
			<li>Fixed: Bad Characters allowed in Show URL cause error on save then Exception Error</li>
		</ul>
	</div>
</div>

<div class="newsItem">
	<div class="date">MARCH 24, 2008</div>
	<div><span class="title">March Madness</span><span class="info">posted by TonyF</span></div>
	<div class="text">
		<p>
		After a flurry of activity and changes on the website in the recent past the designer application takes center-stage once again. We have a bunch 
		of new features and functions that should make more sophisticated projects even easier to make. Here's the lowdown:
		</p>

		<h6>New Features</h6>
		<ul>
			<li><b>Dynamic MCQ</b> - There is now an option that allows the designer to specify that the displayed choices be provided by from a Field in a stored Form.</li>
		    <li><b>SET Form response Fields</b> - It is now possible to assign a value (via the SET statement) to a FIB Blank or an MCQ of a Form submission in the current session.</li>
		    <li><b>MCQ is blank / is not blank comparison operators</b> - It is now possible to check the status of an MCQ for is blank or is not blank in conditions.</li>
		    <li><b>Invitations in Text Items</b> - Designers can now insert Invitation links to Forms into Text Items in Forms.</li>
		    <li><b>Link to External URL</b> - Designers may now insert hyperlinks to any URL via the Insert / Hyperlink... menu item.</li>
		    <li><b>Show URL statement</b> - A URL tab has been added to the Statement Details Panel for the SHOW statement. The Designer can use the SHOW URL statement to cause a Tawala Process to display any web page.</li>
		    <li><b>Invitations to non-Starting Points</b> - Designers can now insert an Invitation to any Form for the current Project, not just Starting Points, via the Insert / Invitation... menu item.</li>
		    <li><b>Expressions in Hyperlinks</b> - The URL portion of a hyperlink may now be constructed as an expression consisting of both literal text and fields (MCQs, Blanks, Variables or Hidden fields).</li>
		    <li><b>Expressions in Dynamic MCQ</b> - Designers can now use any combination of Fields and literal text for the Display Text and Value fields of a Dynamic MCQ.</li>
		</ul>
	
		<h6>New Functions</h6>
		<ul>
			<li><b>Response Totals</b> - This new Function shows the total counts of all responses to multiple-choice questions in table form.</li>
			<li><b>Categorizer</b> - Provides an interactive (drag-and-drop) method for grouping stored records into categories.</li>
		</ul>
	</div>
</div>

<div class="newsItem">
	<div class="date">JANUARY 30, 2008</div>
	<div><span class="title">Happy New year</span><span class="info">posted by TonyF</span></div>
	<div class="text">
		<p>
			I hope everyone's new year is off to a good start. In this release we've done some work on the project details pages that we hope will
			make it easier to use. We've also added backup and restore functionality making it easier to protect important data collected
			in your projects.
		</p>
		<p>
			On the designer side there are no new features but some very nice performance enhancements. This should make working on
			larger projects much easier. We are also working on a major overhaul of the designer application that is looking really good.
			It will take another month or two until it's ready but it should be a big step forward in designing projects.
		</p>
		<p>Here are the details of what's changed:</p>
		<p><b>Web site changes:</b></p>
		<ul>
			<li>Added Backup and Restore capabilities to projects</li>
			<li>Added a Project Actions section to the Project Details page</li>
			<li>Project actions now come up in a dialog instead of a separate page</li>
			<li>Added three new themes: Soup's On, Tennis and Orange Swirl</li>
		</ul>

		<p><b>Changes in Tawala Designer:</b></p>
		<ul>
			<li>Performance improvements in refreshing the fields palette when changes are made in a project</li>
		</ul>
	</div>
</div>

<div class="newsItem">
	<div class="date">DECEMBER 19, 2007</div>
	<div><span class="title">A Quick Update</span><span class="info">posted by TonyF</span></div>
	<div class="text">
		<p>
			Before we're completely engulfed by the holidays we wanted to get out a quick update of Tawala. Here's what's new:
		</p>
		<p><b>Web site changes:</b></p>
		<ul>
			<li>Added two new themes: Purple Haze and Red Rays</li>
		</ul>

		<p><b>Changes in Tawala Designer:</b></p>
		<ul>
			<li>Undo/Redo in processes is now working</li>
		</ul>
		<p><b>Customizable Web-app changes</b></p>
		<ul>
			<li>Poll or Survey app has been updated to add user-by-user report</li>
		</ul>
		<p>
			We have lot's more in the works that will have to wait until after the holidays. Be sure to check back in the
			new year.
		</p>
		<p>
			From the Tawala Team we hope everyone has a happy holiday season and a wonderful New Years!
		</p>
	</div>
</div>

<div class="newsItem">
	<div class="date">DECEMBER 3, 2007</div>
	<div><span class="title">Tawala Updates</span><span class="info">posted by TonyF</span></div>
	<div class="text">
		<p>
			We have more changes dealing with styles this month. These are mostly UI changes in the designer. We've also
			added a couple of new Themes to use in your projects. In addition we've improved the data import process
			in the Project Manager on the web site. The biggest change there is that you can now import data from
			an Excel spreadsheet. Here's the details on the changes:
		</p>
		<p><b>Web site changes:</b></p>
		<ul>
			<li>Data can be imported from Excel files</li>
			<li>Added new themes: Lime and Baseball</li>
			<li>Improved data import:
				<ul>
					<li>During the import if the first row is headers the columns are automatically assigned.</li>
					<li>During the import first row can be skipped and existing records can be deleted.</li>
				</ul>
			</li>
		</ul>

		<p><b>Changes in Tawala Designer:</b></p>
		<ul>
			<li>The Fill in the Blank Styles dialog has been modified so that style selection requires no scrolling</li>
			<li>Improvements to the Form Item Styles dialog</li>
			<li>Preview updates when Page Header is changed</li>
			<li>Applying Style to selected items now acts only on selected items in the active Form window</li>
		</ul>
		<p><b>Customizable Web-app changes</b></p>
		<p>
		Customizable Web apps updated to respect styles in themes.
		</p>
	</div>
</div>

<div class="newsItem">
	<div class="date">NOVEMBER 5, 2007</div>
	<div><span class="title">Tawala Updates</span><span class="info">posted by TonyF</span></div>
	<div class="text">
		<p>
			As usual we have lots of changes both on the Tawala website as well as the downloadable Tawala Designer application
			Most of the changes center improving the look of your projects when they are deployed. 
			Here's the list of what's changed:
		</p>

		<p><b>Tawala Designer Changes</b></p>
		<ul>
			<li>New Form Preview tab - You can now easily preview forms in the designer by clicking on the Preview tab at the top of the editing window.</li>
			<li>Fill-in-the blank question styles - You can now choose from the following styles:
				<ul>
					<li>Labels on top (this is the new default style)</li>
					<li>Left-aligned labels</li>
					<li>Right-aligned labels</li>
					<li>Full-justified (question items will line up on the left and right sides)</li>
					<li>Free-form - you control the formatting (this is how the designer always worked in past versions)</li>
				</ul>
			</li>
			<li>Multiple-choice question styles - You can now choose from the following styles:
				<ul>
					<li>Vertical format</li>
					<li>Horizontal format</li>
					<li>Two-column format</li>
				</ul>
			</li>
			<li>Page header - appears at the top of each page of your project.</li>
			<li>You can insert an image in a page header</li>
			<li>Headings item in forms - You can now add a heading item in a form to separate sections of a form. These can either be a "main" heading or "sub-heading"</li>
			<li>Theme styles now take precedence unless over-ridden in the project.</li>
			<li>Limited font list to "web-safe" fonts</li>
			<li>Added new themes. More coming soon!</li>
		</ul>
		
		<p><b>Web-site Changes</b></p>

		<ul>
		    <li>Projects can be taken off-line.</li>
		    <li>Added the ability to filter inactive projects in Project Manager list.</li>
		    <li>Project theme can be changed from Project Manager.</li>
		    <li>Form data can be exported in Excel.</li>
		    <li>Registered user can define a shared data source.</li>
		    <li>Data can be imported into shared data source.</li>
		</ul>

		<p><b>Customizable Application Changes</b></p>

		<ul>
		    <li>Updated versions of:
				<ul>
			        <li>Poll or Survey</li>
			        <li>Online Exam Builder</li>
				</ul>
			</li>
			<li>Updated demos for most customizable apps</li>
		</ul>
	</div>
</div>	


<div class="newsItem">
	<div class="date">SEPTEMBER 19, 2007</div>
	<div><span class="title">What's New at Tawala</span><span class="info">posted by DougC</span></div>
	<div class="text">
		<p>
			For those using Apps straight from the Web site
		</p>
		<ul>
			<li><b>New customization process</b> gives you many new features, including:
				<ul>
					<li>Real-time Preview - lets you see your forms as you design them</li>
					<li>Themes - so you can change colors and fonts to match your need</li>
					<li>Multiple ways to save your App, including automatic emailing of links to you for storage</li>
				</ul>
			</li>
			<li><b>Deletion of individual records from any app from the Project Manager</b> is now possible - lets you eliminate unintended users, etc.</li>
		</ul>
		<p>
		For those building their own Web apps with Tawala Designer
		</p>
		<ul>
			<li><b>Preview Tab for Forms</b> - New tab at the top of each form shows you exactly how the Form Web page will look! Skips and Page Breaks are ignored so you can check out your entire form.</li>
			<li><b>Styles</b> - These are slowly being added and currently only affect Forms.  Using Styles you can choose to apply a particular look to all items of a particular type in Forms. Currently you can use:
				<ul>
					<li>Fill in the blanks - you can now chose from:
						<ul>
							<li>Left-aligned labels</li>
							<li>Right-aligned labels</li>
							<li>Label on top</li>
							<li>Free-form FIB's (default)</li>
						</ul>
					</li>
					<li>Multiple choice questions (partly implemented)
						<ul>
							<li>Vertical list (default)</li>
							<li>Horizontal list of choices</li>
							<li>Two columns</li>
						</ul>
					</li>
					<li>And coming soon - Headings!</li>
				</ul>
			</li>
			<li><b>Styles - Coming soon:</b>
			 	<ul>
					<li>Styles covering Text boxes and Documents</li>
					<li>Local styles - you'll be able to apply a style to a single item</li>
					<li>Tables - now you can insert up to 200 rows. Old limitation of 20 has been removed.</li>
					<li>Print and Print Preview - now work for Forms</li>
				</ul>
			</li>
		</ul>
	</div>
</div>

<div class="newsItem">
	<div class="date">AUGUST 20, 2007</div>
	<div><span class="title">Tawala News</span><span class="info">posted by TonyF</span></div>
	<div class="text">
		<p>
		From the your side of the screen it can seem like not much is happening 
		with Tawala but on this side we're moving as fast as ever. You will see 
		some obvious changes with this update such as the "Keep me logged in" feature 
		which saves you from entering your name and password each time you come 
		to the site. Then there's other little things like being able to create 
		tables with up to 200 rows instead of 20 in the Designer application. 
		Of course there are lots of bug fixes that we got in this time as well. 
		What we're most excited about are some things that are coming soon.
		</p>
		<p>
		One of the strengths of Tawala is the customizable projects available 
		on our home page. We provide easy solutions for things like taking a 
		quick poll, putting together a sign up sheet or organizing a potluck dinner. 
		The customization process was pretty good but we thought it could be better. 
		We're putting the final touches on a brand new customization process that 
		should make the process even easier. It provides simple steps to walk you 
		through the process as well as dynamic previews that update as you enter 
		the information. We'll be rolling this out in the next couple of weeks. 
		We just need to do some final testing and update the customizable projects 
		and we'll be ready to roll it out. I think you'll like this change.
		</p>
		<p>
		We're also taking a long hard look at project styles and how they're applied.
		We're going to revamp our themes and change the way that styles 
		are applied to the text and questions in a project. If you download the new 
		Designer application you may notice a new "Styles" palette under the "Format" 
		menu. This isn't hooked up quite yet and is still subject to change but it 
		gives you an idea of the direction we're heading. Oh, and we're also adding 
		a new preview tab in the Designer that will allow you more easily see what 
		your project will look like as you're creating it.
		</p>
		<p>
		All this is coming soon so be sure to check back here for more exciting Tawala developments!
		</p>
	</div>	                                    	
</div>

<div class="newsItem">
	<div class="date">
		AUGUST 1, 2007
	</div>
	<div>
		<span class="title">What's New at Tawala</span><span class="info">posted
			by DougC</span>
	</div>
	<div class="text">

		<h5>
			<b>For those using Customized Web Apps</b>
		</h5>
		<ul>
			<li>
				<b>The multiquestion Poll</b>
				<ul>
					<li>
						Lets you create a poll of virtually any length, comprising a
						combination of "fill-in-the-blank" and "multiple choice"
						questions.
					</li>
					<li>
						You can view summary data for any or all questions, see the
						detailed responses of each user, and see correlated data for the
						first ten questions.
					</li>
				</ul>
			</li>
			<li>
				<b>Sign-up Sheet</b>
				<ul>
					<li>
						Lets you choose whether or not to display everyone's contact info
						on the Sign-up Sheet
					</li>
				</ul>
			</li>

			<li>
				<b>New feature in tables</b>
				<ul>
					<li>
						All the tables in Single Question Poll, Potluck, Signup Sheet, and
						Get Together can be sorted by clicking on the heading in any
						column.
					</li>
				</ul>
			</li>
			<li>
				<b>Delays</b>
				<ul>
					<li>
						The Automated Email List Builder customizable Web app should be
						available within the next two weeks. Student Gradebook has been
						put on hold for the time being.
					</li>
				</ul>
			</li>
		</ul>

		<h5>
			For those building their own Web apps with Tawala Designer
		</h5>
		<ul>
			<li>
				<b>New approach to saving Fields and Variables</b>
				<ul>
					<li>
						When Tawala saves a user's Form entries, it now saves only the
						Fields of the Form. Variables are discarded at the end of a user
						session.
					</li>
					<li>
						Fields are usually the responses elicited from users through
						Forms. However, you can create "Hidden Fields" on a Form that
						users will never see, in order to save other values. You can place
						values in these Hidden Fields with the SET command in Processes.
					</li>
				</ul>
			</li>
			<li>
				<b>Using Functions within Forms</b>
				<ul>
					<li>
						It is now possible to use display Functions in Text Items of
						Forms. This makes it possible to display information on the Form
						where you want it and may make it possible to display information
						to the user without attaching a Process to the Form.
					</li>
					<li>
						For example, you can use the Function Response Bar Graph
						(previously called the Choice Summary Table) in a Form used for a
						poll to show the user a graph of all the responses to date.
					</li>
				</ul>
			</li>
			<li>
				<b>Pre-populating a Form with a previously saved record</b>
				<ul>
					<li>
						Under the SHOW statement in a Process is an option to Show a
						Stored Record (which you can specify with a "Where" extension to
						the option).
					</li>
					<li>
						When selected, the chosen Form appears with all Fields already
						filled in with the selected record. The User may replace any of
						the contents of any field, editing the existing record (or, at the
						Designer's option creating a new one).
					</li>
				</ul>
			</li>
			<li>
				<b>Deleting multiple versions within Project Manager</b>
				<ul>
					<li>
						Tawala creates a new version of a project each time it is
						deployed. Occasionally, a Designer wants to clear out most
						versions of the project, keeping only a few.
					</li>
					<li>
						Changes in the Project Manager now permit multiple versions to be
						deleted simultaneously.
					</li>
				</ul>
			</li>
		</ul>

		<h5>
			Coming Soon! 
		</h5>
		<p>Here are some major changes that you can expect in the next couple of months:</p>
		<ul>
			<li>
				<b>New improved customization process. Some of the features:</b>
				<ul>
					<li>
						Change the overall appearance of your Web app using Themes.
					</li>
					<li>
						Preview your finished Web application as you assemble it.
					</li>
					<li>
						Multiple options for saving the links to your projects so that you
						don't lose track of them.
					</li>
				</ul>
			</li>
			<li>
				<b>Styles for Forms</b>
				<ul>
					<li>
						Change fonts, sizes, formats across a Form or all Forms
						simultaneously.
					</li>
				</ul>
			</li>
			<li>
				<b>Sharing Data between Web apps</b>
				<ul>
					<li>
						Create databases in one Web application that can be used in all
						your other Tawala Web applications.
					</li>
				</ul>
			</li>
		</ul>
	</div>
</div>


<div class="newsItem">
	<div class="date">
		MAY 10, 2007
	</div>
	<div>
		<span class="title">What's New at Tawala</span><span class="info">posted
			by DougC</span>
	</div>
	<div class="text">

		<h5>
			For those using Customized Web Apps:
		</h5>
		<ul>
			<li>
				For
				<i>Teachers</i> and others - the
				<b>Online Exam Builder</b> lets you build a test that's any mixture
				of multiple-choice and fill-in-the-blank questions, then grades it
				automatically. You can see a summary of your student's efforts and a
				question-by-question analysis of the test to show you where the
				class had the most trouble.
				<ul>
					<li>
						And
						<i>coming soon</i> -
						<b>Student Gradebook</b> will keep a record of all your students'
						scores over the school year, by class, with student, class and
						overall averages. When you run Online Exam Builder, Gradebook gets
						updated automatically, so you're always on top of what's going on.
					</li>
				</ul>
			</li>

			<li>
				For
				<i>Organizers</i> of all stripes - the
				<b>Automated Email List Builder</b> lets you build an email list for
				your group or organization by using social networking. As long as
				you have a few emails to start with, the group builds its own list
				virally. Includes anti-spam required opt-out provisions, plus the
				ability for members to update their own data.
			</li>
		</ul>

		<h5>
			For those building their own Web Apps with Tawala Designer:
		</h5>
		<ul>
			<li>
				<b>Private Invitations</b> - Now any invitation link can be marked
				"private" by a Designer. Any information, such as the User's email
				address, can be encrypted into the invitation URL so that you know
				exactly who has responded to the link and can direct them
				appropriately within your Web app. Use this powerful feature to give
				different group members different levels of access within your
				application or to make sure, for example, that each group member is
				permitted only a single vote in an election.
			</li>
			<li>
				<b>Delete</b> - Our first new Process command in many months, Delete
				allows the Designer to purge one, some, or all of the records from a
				particular form from within the application itself.
			</li>
			<li>
				<b>Functions</b> - We are just beginning to add functions to the
				Designers. The first functions are display functions for use in
				Documents. They can vastly shorten your Processes. Here are a couple
				of currently available functions and some of the things you can do
				with them:
				<ul>

					<li>
						<b>Field Table</b> - Insert this function into a document and it
						will display a table showing the values of as many fields and
						variables associated with a Form as you like, one Form per row.
					</li>
					<li>
						<b>Record Count</b> - Displays the number of responses to any
						Form.
					</li>
					<li>
						<b>Choice Summary Table</b> - Displays a table showing each
						response in a multiple choice question, followed by the number of
						responses to each and the percentage of the total represented by
						that number. This is particularly useful for surveys and polls.
					</li>
				</ul>
			</li>
			<li>
				<b>Pre-populated Forms</b> - It is now possible to display a Form
				with data already in the Fields. Right-clicking on a Form in the
				Project Explorer, pops up the context menu. The bottom item is
				"Pre-populate With Last Entry." This will make it easy to return a
				User to a Form in edit mode if he wishes to change one of his
				responses upon review (Note: you will still have to delete the
				earlier record if you have left the Form, but it still makes it
				easier for the User).
				<ul>
					<li>
						<i>Coming Soon</i> - You will soon be able to pre-populate a Form
						with a previously saved record and edit or delete that record
						directly.
					</li>
				</ul>
			</li>
			<li>
				<b>Improvements in the Send Command</b> - Send has been modified to
				allow substantially increased customization:
				<ul>
					<li>
						A Designer can now specify the From address using either literal
						text or a variable.
					</li>
					<li>
						A Designer can also use variables in the Subject line of an email.
					</li>
					<li>
						Designers can specify a Personal Name, in addition to the email
						address provided in the From field, which makes it easier for
						recipient to know who the email actually came from.
					</li>
				</ul>
			</li>
		</ul>
	</div>
</div>

<div class="newsItem">
	<div class="date">
		APRIL 11, 2007
	</div>
	<div>
		<span class="title">Under the Covers</span><span class="info">posted
			by TonyF</span>
	</div>
	<div class="text">
		<p>
			Don't be fooled but what looks like a lack of changes in Tawala.
			There's been lots going on. Much of it is in the Tawala Designer
			application and the presentation of projects.
		</p>
		<p>
			The big addition this time around has been the introduction of
			functions in the Designer. After looking at a number of Tawala
			projects that had been created we discovered that if we created
			functions to handle common things like counting records or displaying
			a table containing fields from responses it greatly simplified the
			process code. This means it's easier to get the results you want with
			less coding. Currently functions can only be added to documents.
			You'll find them under the "Insert" menu in the menu bar. In a short
			time we'll have them available in processes as well.
		</p>
		<p>
			Another change you might notice is the addition of "landing pages"
			targeted at specific groups such as schools, clubs and non-profit
			groups. We're trying this out as a way to gage what people are most
			interested in using Tawala for. That way we can better tailor
			projects and features to user needs.
		</p>
		<p>
			We're also added things like video demos of the projects featured on
			our home page as well as improving the projects themselves. So we
			have been (and continue to be) quite busy. If you have any thoughts
			you'd like to share about what we're doing you can post a message in
			our forums (
			<a href="http://www.tawala.com/forums">http://www.tawala.com/forums</a>)
			or drop us a note at
			<a href="mailto:info@tawala.com">info@tawala.com</a>
		</p>
	</div>
</div>

<div class="newsItem">
	<div class="date">
		JANUARY 26, 2007
	</div>
	<div>
		<span class="title">New Year, new Tawala</span><span class="info">posted
			by TonyF</span>
	</div>
	<div class="text">
		<p>
			It's been a while since we last updated this news page but we
			certainly haven't been idle. We actually did sneek in an updated
			version of the Designer app (build 106) without much fanfare. This
			realse brings the Designer to build 108 which includes the ability to
			add images to forms as well as a number of bug fixes.
		</p>
		<p>
			The big change this time (as I'm sure you've already noticed) is to
			the web site. We've changed the focus of the home page to highlight
			four pre-made projects that provide easy solutions to common
			problems. We've also move to a multi-teired approach to exposing
			Tawala. Instead of hitting new users with everthing at once we're
			trying to ease them in by allowing people to "Take Tawala to the next
			level" and expose additional features as they feel ready for them.
		</p>
		<p>
			It's a lot of change but so far we're pretty happy with how
			everything is working. And there's more to come. We've got some big
			plans for the Designer. We've come up with some ideas that will make
			creating projects much easier and more fun. More details on that in
			the coming weeks.
		</p>
	</div>
</div>

<div class="newsItem">
	<div class="date">
		DECEMBER 8, 2006
	</div>
	<div>
		<span class="title">Winter in the west</span><span class="info">posted
			by TonyF</span>
	</div>
	<div class="text">
		<p>
			It's offical. Winter is here in northern California. The rain has
			started and it's been unusually cold so far so I'm thinking we may
			even see a little snow again this year if it keeps going like this.
			It never lasts long but it's fun to see. Me and snow don't get along
			all that well so I'm happy it doesn't last. I do so enjoy seeing
			people ruining perfectly good skis trying to go down the local hills
			when we get a little dusting of white. People are funny sometimes.
		</p>
		<p>
			Another week of cleanup in the designer and the website. We have been
			doing lots of meeting and talking the past couple of weeks and there
			will be some interesting changes coming to Tawala. Our goal is to try
			and make it even easier for anyone to get starting using and
			customizing web applications. No programming required. To that end we
			the developers are going to put down our compilers for a couple of
			days and use our own creation to make some compelling projects. I
			believe we've come up with some really good ideas. You'll be seeing
			the fruits of our labors soon on the website so stay tuned...
		</p>
	</div>
</div>

<div class="newsItem">
	<div class="date">
		NOVEMBER 17, 2006
	</div>
	<div>
		<span class="title">Spiffing up the place</span><span class="info">posted
			by TonyF</span>
	</div>
	<div class="text">
		<p>
			We've spent the past week doing some cleaning up. We've been going
			through the website and Designer application making sure everything
			is ship shape. We changed much of the text on the home page to give
			people coming to the site a clearer idea of what Tawala is. Let us
			know what you think. Doug has also polished up some of his projects
			and added a new Due Diligence project that looks pretty slick. You
			can check them out in the demo links on the home page. They're also
			available for downloading in the Library.
		</p>
		<p>
			It's all about the Designer this week. We haven't added any new
			features but you find some nice improvements and fixes in the
			interface of the Designer application. Most are little things like
			changing where the cursor is when you're editing but they make
			creating projects more enjoyable.
		</p>
		<p>
			We're still going to be tackling bugs in the coming weeks. There will
			be a couple of new enhancements coming on the webisite and I'm sure
			we'll probably sneak a new feature or two in the Designer as well.
		</p>
	</div>
</div>

<div id="news" class="news">
	<div class="newsItem">
		<div class="date">
			NOVEMBER 9, 2006
		</div>
		<div>
			<span class="title">The kid in me</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				OK, I admit it. I'm rather excited about the pending release of new
				game systems from Nintendo and Sony (I'm rather partial to Nintendo
				for some reason). I take this as a good sign that the kid in me is
				still alive and kicking even though I recently turned 50. One thing
				that will be interesting is that these new systems will sport web
				browsers so we may have to do some Tawala testing on them. That
				gives the adult in me a good reason to justify their purchase!
			</p>
			<p>
				This week sees that addition of formating in multiple-choice
				questions and text alignment and indents in text items. This mostly
				concludes the formating features in the Designer. We will be doing
				some work to spiff up the templates in the near future. Also of note
				this week is an update of the home page. Tawala while being a
				powerful tool and fun to use is rather difficult to describe. We've
				gone round and round with the wording on the home page and think
				we've found something that we like. For now...
			</p>
			<p>
				In the near future we're going to take some time to do some cleanup
				and bug fixes so you may not see many major new features added. We
				do have some big plans for additions and changes to Tawala and as
				soon as the schedule for those gets nailed down we'll let you know.
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			NOVEMBER 3, 2006
		</div>
		<div>
			<span class="title">May we introduce...</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				It's taken a while but I think it was worth the wait. We're finally
				done with the web site redesign. Besides a fresh new look we tried
				to make areas like the Library and Project Manager easier to use. I
				hope you'll agree. Please post your thoughts in the forums.
			</p>
			<p>
				Besides a new web design we bring you build #98 of the Designer.
				This version adds a number of fixes that should speed up the
				development of projects. A number of bugs dealing with images have
				been addressed as well. On the web side there is a new feature in
				the Project Manager that allows you to embed projects into other web
				pages. It also makes the link to the project easily accessible if
				that's all you need.
			</p>
			<p>
				For the next couple of builds we plan on trying to make a
				significant dent in the bug list. The formating controls for
				multiple-choice questions we also be added shortly which should
				complete that part of the Tawala feature list. Until next time...
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			OCTOBER 23, 2006
		</div>
		<div>
			<span class="title">Pumpkins and traffic don't mix</span><span
				class="info">posted by TonyF</span>
		</div>
		<div class="text">
			<p>
				You can always tell it's October in Sonoma county by the backup on
				the freeway from Petaluma to Cotati. For some reason everyone feels
				they need to slow down to look at the pumpkin patch at the north end
				of Petaluma. They do have a great corn maze but there's really not
				that much to look at. A few days after Halloween (once they plow it
				all under) traffic magically returns to normal.
			</p>
			<p>
				Well, we do have a few things to see in this new release (hopefully,
				without slowing down!). The past week or two we've been trying to
				get the formating in Forms completed. We've made good progress but
				there are still a couple of more things to finish up. We've also
				been working adding some server-side caching to try to make
				performance of projects better.
			</p>
			<p>
				This next release should add the final touches to the formating
				features in Forms. A slew of bugs will be dealt with and hopefully
				will clear up some irritating problems. Finally, the new site design
				is just about done. Barring any last minute problems we'll be
				releasing it as well in the next build. In addition to giving the
				site a new clean look we've rearranged thing a bit to make finding
				and developing projects a little easier. At least we think so. I'm
				sure you'll let us know!
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			OCTOBER 13, 2006
		</div>
		<div>
			<span class="title">Superstitious</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				Friday the 13th. Kind of an ominous day to be releasing a new build
				but what the heck. What Friday the 13th would be complete without
				some words of warning:
			</p>

			<div class="note important">
				It is strongly recommended that you use Windows "Add or Remove
				Programs" to uninstall any previous Build of Tawala before you
				install Build 93.
			</div>
			<p>
				There some changes between builds 91 and 93 that will cause the
				application to crash if you don't do a clean install. We don't plan
				on making this a regular event. Just a one time fix.
			</p>
			<p>
				This build is all about formating Forms. The formating controls that
				you've come to know and love in documents are now available in
				fill-in-the-blank questions in Forms. Formating for Multiple-choice
				questions and the addition of tables should be coming in the next
				build. So go forth and format those Forms!
			</p>
			<p>
				As mentioned above some more work on the formating of Forms is
				coming in the next build. Also, the site redesign is coming along
				nicely. Sorry there's nothing to see yet. It's kind of an all or
				nothing deal. I think you'll like the changes when they arrive in
				the coming weeks.
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			SEPTEMBER 28, 2006
		</div>
		<div>
			<span class="title">Falling Leaves</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				Well, it's starting to look like autumn here in Northern California
				(at least where I am in Sonoma County). Leaves are starting to pile
				up in the backyard. One more thing to deal with. I hope we get a few
				more hot days. I'm not ready to give up Summer quite yet.
			</p>
			<p>
				We've added some nice enhancements this week. The biggest news is
				that you can now modify previously stored repsonses. So you can now
				create projects where users can update existing data. There have
				also been some improvements in the Get and If statement UI's. On the
				web side of things you won't see any visible changes but behind the
				scenes some changes have been made to improve performance,
				especially in the Project Manager.
			</p>
			<p>
				We're currently working on adding formating capabilities to Forms so
				you should see the fruits of those labors start showing up in the
				next couple of builds. Also in the works is a complete redesign of
				the Tawala website. It's going to take a few weeks to get that
				altogether but I think you'll like the changes.
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			SEPTEMBER 20, 2006
		</div>
		<div>
			<span class="title">It's been a while</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				Sometimes things just don't go the way you plan. We've been hoping
				(and still hope to) release a new build of Tawala each week. Well,
				for the past two weeks that didn't quite work out. It took longer
				than we hoped to iron out some problems. But we're back with a new
				version and some new functionality.
			</p>
			<p>
				On the Designer side we're happy to announce the return of Print and
				Print Preview for processes. That functionallity for Forms and
				Documents will be coming soon. Also new this week is the ability to
				use multiple Forms in Get statements as well as using variables in
				Set statements. The Get statement has a new UI for setting
				conditions that we think is a big improvement.
			</p>
			<p>
				On the web site the Project Manager now sports a new project version
				feature. Every time you deploy a project a new version is created in
				the Project Manager in case you need to go back to an earlier
				design. When you deploy a project from the Designer that project
				becomes the currently active version. You can also change which
				version is active in the project manager. We're hoping this feature
				will make life a little easier when developing projects.
			</p>
			<p>
				Coming up in the next few builds you should see the addition for
				formating of text items in Forms. Also on the schedule is the
				ability to overwrite data in a previously stored record. So stay
				tuned...
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			AUGUST 30, 2006
		</div>
		<div>
			<span class="title">Import Business</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				It was a seemingly slow week for new features but a number of bugs
				did get fixed. This week we are going to start a round of market
				validation for Tawala to make sure we're on the right track. We're
				also getting started on a re-design of the web site which should
				freshen things up quite a bit.
			</p>
			<p>
				This week we bring you the ability to import data into your project.
				Go to the project details page in the Project Manager. Down by the
				list of Forms for the project you'll find a new icon for importing
				data. It's fairly basic at the moment. Data has to be in
				comma-delimited format (CSV). We figured we'd start with this and
				see how people use the feature. Let us know what you think!
			</p>
			<p>
				Coming in the next Tawala build you should find some more
				improvements to the Get statement as well as the ability to
				over-write fill-in-the-blank responses. And of course more bug will
				be put to rest.
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			AUGUST 24, 2006
		</div>
		<div>
			<span class="title">To Tawala and Beyond</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				I'm a little bummed this week that they decided to demote Pluto from
				a real planet to a dwarf planet. I spent all that time years ago
				learning the names of all the planets and now they've gone and
				changed it on me. At least it's the last in the list and not
				somewhere in the middle.
			</p>
			<p>
				Well, the developers retreat was pretty productive and enjoyed by
				all. We had some quality time to discuss a number of upcoming
				features to Tawala. We also managed to get a few new features in the
				designer as well as fix a slew of bugs.
			</p>
			<p>
				New this week: The If statement UI has been completely redesigned.
				The Expression builder dialog as well as the simple and advanced
				tabs have given way to what we hope is a much simpler interface for
				creating If statements. Also included this week is the ability to
				determine if a variable in a Set statment should be treated as a
				text or numeric value. In addition Tawala is now running on a new
				server. It's a faster, dedicated machine which should alleviate the
				problems we've been running into recently. We appologize for the
				trouble the old server caused.
			</p>
			<p>
				Coming in the next Tawala build:
			</p>
			<ul>
				<li>
					Import data into projects via the Project Manager
				</li>
				<li>
					Select multiple forms in the Get statement
				</li>
			</ul>
			<p>
				And of course there will be a more bugs fixed as well. Until next
				time...
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			AUGUST 8, 2006
		</div>
		<div>
			<span class="title">Witty Title Goes Here...</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				Sometimes I think the hardest thing about writing these little
				messages is coming up with the title. As you can see I've drawn a
				blank this week. Writing has never been my forte (as you can
				probably tell!) but I struggle along.
			</p>
			<p>
				It's been an interesting week as usual. We fixed a few more bugs
				including the inability to delete projects in the Library when using
				IE (thanks to Babita for pointing that one out!). We also started
				talking about adding version control for projects in the Project
				Manager. Not quite sure how that's going to look yet but it should
				be a nice feature.
			</p>
			<p>
				New this week: As promised you can now add comments in a process.
				You can also add sample data to a project in the Library to make the
				test-drive experience even better. And for those days when you can't
				seem to remember your name (or in this case password) you can now
				reset a forgotten password on the site. The new Expression Builder
				UI didn't quite make it in this week but will be in the next
				release.
			</p>
			<p>
				Coming soon to Tawala:
			</p>
			<ul>
				<li>
					New UI in Expression Builder dialog (for real!)
				</li>
				<li>
					Many improvements to the Get statement
				</li>
				<li>
					Ability to choose numeric or text format for data in a process
					variable
				</li>
			</ul>
			<p>
				Things may slow down a little in the next week as the Tawala
				developers are gathering for a little retreat. We'll have time to
				talk about all the great feedback we've been getting in the forums
				and hash out some details and new features that are better done in
				person. Hopefully, we'll have lot's of interesting tidbits to talk
				about next release.
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			AUGUST 2, 2006
		</div>
		<div>
			<span class="title">The Week That Was</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				The Tawala team has been keeping busy this week. Lot's of bugs have
				been fixed. You can see the list on the
				<a href="/whatsnew">What's New</a> page.
			</p>
			<p>
				And we have some new things for you to check out. Yes, you can now
				reset documents for re-use. You'll find a new check box in the Show
				Document statement that will allow you to do this. We also bid
				good-bye to the Send Invitation command since we can now include
				invitations in documents. There are a few minor additions on the
				website as well such as a count of how many times a Library project
				has been test-driven and confirmation dialogs when deleting an item
				in the Library.
			</p>
			<p>
				Here are some of the features on deck:
			</p>
			<ul>
				<li>
					New UI in Expression Builder dialog
				</li>
				<li>
					Add a text comment in a process (commenting out a process command
					will be coming soon)
				</li>
				<li>
					Record sample data to be used with project published in the Library
				</li>
				<li>
					Password recovery
				</li>
			</ul>
			<p>
				In addition to all that we have a good amount of behind the scenes
				work going on. We're working on improving the performance of Tawala
				and moving some of our services to new servers. If we do our jobs
				well you shouldn't notice a thing (except maybe a little more spring
				in the step of your projects). If we don't, well we don't want to
				think about that...
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			JULY 25, 2006
		</div>
		<div>
			<span class="title">Images and Heat</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				It's been hot out here in sunny CA with the temperatures hovering
				around the 100 mark all week (and passing it a couple of times!) but
				we're still plugging along here at Tawala.
			</p>
			<p>
				The big news this week is images in Form text items. We even got the
				images in Form previews working. This should go a long way to
				spicing up those projects. Also included in this weeks update is the
				ability to add Invitation fields in Documents. This also brings the
				demise of the Send Invitation statement as it's not really necessary
				now. If you're using this in any projects they will need to be
				updated. The ability to reset Virtual Documents got bumped this week
				but should be showing up in the next build.
			</p>
			<p>
				Here's what's coming up in the next week or two:
			</p>
			<ul>
				<li>
					Reseting Documents for re-use (finally!)
				</li>
				<li>
					Eliminate the Send Invitation statement
				</li>
				<li>
					Display the number of times a project has been test-driven
				</li>
				<li>
					Add delete confirmation dialogs in the Library
				</li>
			</ul>
			<p>
				We're also going to push hard on catching up with bug fixes this
				week. They've been piling up a little as we've been concentrating on
				adding new features. Also, the addition of our new Alpha testers
				quite a few new bugs have been brought to light. So this is the week
				to squash some of those critters.
			</p>
		</div>
	</div>

	<div class="newsItem">
		<div class="date">
			JULY 18, 2006
		</div>
		<div>
			<span class="title">Tawala Update</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<p>
				There's been lot's going on behind the scenes at Tawala lately, just
				nothing that's quite ready for the public yet. One thing that's
				changing is how often we release updates to the product.
			</p>
			<p>
				For many months we've been doing weekly development builds of the
				Tawala Designer application. In the past we only updated the version
				on the website every few months when we felt there were enough new
				features. This is changing though. We will be releasing updates to
				the Tawala Designer much more frequently. Maybe not every week
				(though that might happen if we feel the program is solid enough)
				but there will be no more than 3 weeks between releases.
			</p>
			<p>
				Here are some new capabilities we'll be adding soon:
			</p>
			<ul>
				<li>
					Add images to a text item in a Form (images in Form preview to
					follow shortly)
				</li>
				<li>
					Insert a public invitation link in a Document
				</li>
				<li>
					Reset a Virtual Documents for re-use in a Form
				</li>
			</ul>
			<p>
				We'll also be doing some behind the scenes work like moving the
				Tawala website to a new server and starting to do some performance
				testing on projects.
			</p>
			<p>
				So, things are happening here at Tawala. We'll also be reading the
				discussion groups as well so if you have suggestions for features or
				don't like the way something works that's the place to let your
				voice be heard.
			</p>
		</div>
	</div>
	<div class="newsItem">
		<div class="date">
			JUNE 27, 2006
		</div>
		<div>
			<span class="title">We're Hiring!</span><span class="info">posted
				by TonyF</span>
		</div>
		<div class="text">
			<span class="strikeThrough">See the job postings.</span> 7/18 - These
			job openings have been filled.
		</div>
	</div>

</div>
-->
  