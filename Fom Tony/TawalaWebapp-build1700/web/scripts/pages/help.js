Tawala.Page = {};
Tawala.Page.Help = {};
Tawala.Page.Help.manualTOC = (function() {
		var currentChapter, treeContainer;

		var treeContents = [
				{label:"1 Tawala Basics", id:"chapter1", itemType:"chapter",
					children:[
						{label:"- What do I need to make Tawala work?", id:"section1", itemType:"subSection", link:"#c1s1"},
						{label:"- What do you mean by 'information'?", id:"section2", itemType:"subSection", link:"#c1s2"},
						{label:"- What do you mean by 'structure'?", id:"section3", itemType:"subSection", link:"#c1s3"},
						{label:"- So I probably need to be pretty smart and technology-savvy to do this, right?", id:"section4", itemType:"section", link:"#c1s4"},
						{label:"- I don't have time to learn a new product and I need answers now!", id:"section5", itemType:"subSection", link:"#c1s5"},
						{label:"- What if I need more than the customizables and templates do?", id:"section6", itemType:"subSection", link:"#c1s6"},
						{label:"- What's this 'registration' business?", id:"section7", itemType:"subSection", link:"#c1s7"},
						{label:"- How are  you going to make money? Selling my e-mail address?", id:"section8", itemType:"subSection", link:"#c1s8"},
						{label:"- What about privacy? I can't have my employee (customer) survey fall into the wrong hands?", id:"section9", itemType:"subSection", link:"#c1s9"}
					]
				},
				{label:"2 My First Project", id:"chapter2", itemType:"chapter",
					children:[
						{label:"- Basic Building Block", id:"section1", itemType:"subSection", link:"#c2s1"},
						{label:"- Where do I start?", id:"section2", itemType:"subSection", link:"#c2s2"},
						{label:"- What do I put in my Form?", id:"section3", itemType:"subSection", link:"#c2s3"},
						{label:"- Placing a Text Box", id:"section4", itemType:"subSection", link:"#c2s4"},
						{label:"- Placing a Multiple Choice Question", id:"section5", itemType:"subSection", link:"#c2s5"},
						{label:"- Checking your work and finishing up", id:"section6", itemType:"subSection", link:"#c2s6"},
						{label:"- A word about naming", id:"section7", itemType:"subSection", link:"#c2s7"},
						{label:"- Are we done yet?", id:"section8", itemType:"subSection", link:"#c2s8"},
						{label:"- Deploying this project", id:"section9", itemType:"subSection", link:"#c2s9"},
						{label:"- Where are the answers to my project?", id:"section10", itemType:"subSection", link:"#c2s10"},
						{label:"- Accessing the results", id:"section11", itemType:"subSection", link:"#c2s11"},
						{label:"- Making your project public", id:"section12", itemType:"subSection", link:"#c2s12"},
						{label:"- Invitation Manager", id:"section13", itemType:"subSection", link:"#c2s13"},
					]
				},
				{label:"3 My Tawala", id:"chapter3", itemType:"chapter",
					children:[
						{label:"- What is 'My Tawala'", id:"section1", itemType:"subSection", link:"#c3s1"},
						{label:"- I only do customizables. Why can't I have access to My Tawala?", id:"section2", itemType:"subSection", link:"#c3s2"},
						{label:"- When I deploy my project, I already have 'control' over it. Why do I need My Tawala?", id:"section3", itemType:"subSection", link:"#c3s3"},
						{label:"- How do I access My Tawala?", id:"section4", itemType:"subSection", link:"#c3s4"},
						{label:"- What is 'My Account' all about?", id:"section5", itemType:"subSection", link:"#c3s5"},
						{label:"- Why aren't all my projects listed on the My Tawala page?", id:"section6", itemType:"subSection", link:"#c3s6"},
						{label:"- Is there a history of my projects?", id:"section7", itemType:"subSection", link:"#c3s7"},
						{label:"- How do I know how many people have responded to my project?", id:"section8", itemType:"subSection", link:"#c3s8"},
						{label:"- Where is my data?", id:"section9", itemType:"subSection", link:"#c3s9"},
						{label:"- Can I get a summary of the data?", id:"section10", itemType:"subSection", link:"#c3s10"},
						{label:"- How do I back-up my data?", id:"section11", itemType:"subSection", link:"#c3s11"},
						{label:"- How do I restore my back-up?", id:"section12", itemType:"subSection", link:"#c3s12"},
						{label:"- Can I add data without filling out each Form in the project several times?", id:"section13", itemType:"subSection", link:"#c3s13"},
						{label:"- How do I delete my test data when I'm satisfied with how my project works?", id:"section14", itemType:"subSection", link:"#c3s14"},
						{label:"- I have several Forms. Can I delete the data from all of them at one time?", id:"section15", itemType:"subSection", link:"#c3s15"},
						{label:"- I'm done with this project. How can I delete it?", id:"section16", itemType:"subSection", link:"#c3s16"},
						{label:"- My computer crashed and I lost my project files. Can I download the file I deployed to the Tawala server?", id:"section17", itemType:"subSection", link:"#c3s17"},
						{label:"- This project really helped me out and I think it migh help others in my situation. Can I put it in the User-Contributed Library?", id:"section18", itemType:"subSection", link:"#c3s18"},
						{label:"- I put my project in the Library, but now I've improved it. Can I replace the old version with this better one?", id:"section18", itemType:"subSection", link:"#c3s19"}
					]
				},
				{label:"4 Customizables", id:"chapter4", itemType:"chapter",
					children:[
						{label:"- What's so special about Customizables? Isn't all software customizable?", id:"section1", itemType:"subSection", link:"#c4s1"},
						{label:"- What do I need?", id:"section1", itemType:"subSection", link:"#c4s2"},
						{label:"- Do I need to register to use the Customizables?", id:"section1", itemType:"subSection", link:"#c4s3"},
						{label:"- If I'm an anonymous user, how will I be able to see the responses to my project?", id:"section1", itemType:"subSection", link:"#c4s4"},
						{label:"- How do I know which Customizable to use?", id:"section1", itemType:"subSection", link:"#c4s5"},
						{label:"- What can I expect when I try one?", id:"section1", itemType:"subSection", link:"#c4s6"},
						{label:"- What if I decide to change it after I have saved it?", id:"section1", itemType:"subSection", link:"#c4s7"},
						{label:"- The Appearance feature is slick. But what do I do now? I can't type on the form?", id:"section1", itemType:"subSection", link:"#c4s8"},
						{label:"- I got a 'You haven't finished the editing process' alert. Can I ignore it", id:"section1", itemType:"subSection", link:"#c4s9"},
						{label:"- I did a test drive and I want to make changes. What do I do?", id:"section1", itemType:"subSection", link:"#c4s10"},
						{label:"- Help! As an anonymous user, when I e-mailed the hyperlinks to my project to myself, it worked fine. Then I wanted to e-mail invitations to view my project and I got this question about POP3! What's with that?", id:"section1", itemType:"subSection", link:"#c4s11"},
						{label:"- The Customizables seem to do everything I need. Why should I become a registered Tawala user?", id:"section1", itemType:"subSection", link:"#c4s12"},
						{label:"- I have a common problem that none of your current Customizables address. Will you be adding more projects?", id:"section1", itemType:"subSection", link:"#c4s13"}
					]
				},
				{label:"5 Templates", id:"chapter5", itemType:"chapter",
					children:[
						{label:"- The difference between Customizables and Templates", id:"section1", itemType:"subSection", link:"#c5s1"},
						{label:"- Accessing the Template menu", id:"section2", itemType:"subSection", link:"#c5s2"},
						{label:"- Basic Templates", id:"section3", itemType:"subSection", link:"#c5s3"},
						{label:"- Activities Templates", id:"section4", itemType:"subSection", link:"#c5s4"},
						{label:"- Meetings and Gatherings Templates", id:"section5", itemType:"subSection", link:"#c5s5"},
						{label:"- Polls and Surveys Templates", id:"section6", itemType:"subSection", link:"#c5s6"}
					]				
				},
				{label:"6 Form Items", id:"chapter6", itemType:"chapter",
					children:[
						{label:"- Why would I create questions and then skip them?", id:"section1", itemType:"subSection", link:"#S1"},
						{label:"- How do I enter a Skip Item?", id:"section2", itemType:"subSection", link:"#S2"},
						{label:"- How do I 'instruct' a Skip Item?", id:"section3", itemType:"subSection", link:"#S3"},
						{label:"- Why would I want to Skip backward?", id:"section4", itemType:"subSection", link:"#S4"},
						{label:"- I noticed a Set Statement when the Statements Palette was available. Why would I ever use that?", id:"section5", itemType:"subSection", link:"#S5"},
						{label:"- SA word about conditionals", id:"section6", itemType:"subSection", link:"#S6"}
					]
				},
				{label:"7 Process Statements", id:"chapter7", itemType:"chapter",
					children:[
						{label:"- If Statement", id:"section1", itemType:"subSection", link:"#c7s1"},
						{label:"- Show Statement", id:"section2", itemType:"subSection", link:"#c7s2"},
						{label:"- Send Statement", id:"section3", itemType:"subSection", link:"#c7s3"},
						{label:"- Append Statement", id:"section4", itemType:"subSection", link:"#c7s4"},
						{label:"- Get / ForEach Statements", id:"section5", itemType:"subSection", link:"#c7s5"},
						{label:"- Delete Statement", id:"section6", itemType:"subSection", link:"#c7s6"},
						{label:"- Set Statement", id:"section7", itemType:"subSection", link:"#c7s7"},
						{label:"- Comment Statement", id:"section8", itemType:"subSection", link:"#c7s8"}
					]				
				},
				
				{label:"8 Sessions", id:"chapter8", itemType:"chapter",
					children:[
					{label:"- Session explained", id:"section1", itemType:"subSection", link:"#c8s1"},
					{label:"- The relationship between sessions and active elements", id:"section2", itemType:"subSection", link:"#c8s2"},
					{label:"- A better way", id:"section3", itemType:"subSection", link:"#c8s3"},
					{label:"- Using a Pre-Process", id:"section4", itemType:"subSection", link:"#c8s4"},
					{label:"- Retrieving stored records for use in the current session", id:"section5", itemType:"subSection", link:"#c8s5"}
					]
				},

				{label:"9 Skip Item", id:"chapter9", itemType:"chapter"},
				{label:"10 FIBs", id:"chapter10", itemType:"chapter"},
				{label:"11 MCQs", id:"chapter11", itemType:"chapter"},
				{label:"12 Headings", id:"chapter12", itemType:"chapter"},
				{label:"13 Text", id:"chapter13", itemType:"chapter"},
				{label:"14 Break", id:"chapter14", itemType:"chapter"},

				{label:"15 IF", id:"chapter15", itemType:"chapter"},
				{label:"16 Show", id:"chapter16", itemType:"chapter"},
				{label:"17 Send", id:"chapter17", itemType:"chapter"},
				{label:"18 Append", id:"chapter18", itemType:"chapter"},
				{label:"19 Get/ForEach", id:"chapter19", itemType:"chapter"},
				{label:"20 Delete", id:"chapter20", itemType:"chapter"},
				{label:"21 Set", id:"chapter21", itemType:"chapter"},

				{label: "22 Scoring a Test", id:"chapter22", itemType:"chapter"},

				{label:"Glossary", id:"glossary", itemType:"chapter"}
		];

		function buildTree() {
			var tree;
			
			if(treeContents[0].itemType == "section") {
				for (var i = 0; i < treeContents.length; i++) {
					var sectionTitle = document.createElement("h5");
					sectionTitle.innerHTML = treeContents[i].label;
					treeContainer.appendChild(sectionTitle);

					tree = new YAHOO.widget.TreeView(createNewTreeContainer());
					buildBranch(treeContents[i].children, tree.getRoot());
				}
			}else{
				tree = new YAHOO.widget.TreeView(createNewTreeContainer());
				buildBranch(treeContents, tree.getRoot());
			}

			tree.subscribe("labelClick", labelClicked);			
			tree.subscribe("expand", expandedHandler);			
			tree.draw();
		}
		
		function createNewTreeContainer(){
			var newtree;
			
			newTree = document.createElement("div");
			newTree.className = "treeView";
			treeContainer.appendChild(newTree);
			return(newTree);
		}
		
		function buildBranch(els, parent) {
			for (var i = 0; i < els.length; i++) {
				var tmpNode = new YAHOO.widget.MenuNode(els[i], parent, false);
				if(currentChapter == null) {
					loadChapter(tmpNode.data.id);
					currentChapter = tmpNode.data.id;
				}
				
				if(els[i].children) {
					buildBranch(els[i].children, tmpNode);
				}
			}
		}
		
		function expandedHandler(node){
//			alert(node.data.id + " was expanded");
		}
		
		function labelClicked(node) {
			var parentNode, parentId;

			parentNode = node.getAncestor();
			
			if(parentNode == "RootNode") {
				parentId = node.getAncestor();
			}else{
				parentId= node.getAncestor().data.id;
			}

			// Check if chapter already loaded
			if(node.data.itemType == "chapter") {
				if(currentChapter != node.data.id) {
					loadChapter(node.data.id);
				}
				currentChapter = node.data.id;
				tree.collapseAll();
			}else if(node.data.itemType == "subSection") {
				if(currentChapter != parentNode.data.id) {
					loadChapter(parentNode.data.id)
				}
				document.location.href = node.data.link;
			}
		}
		
		function loadChapter(chapterId) {
			var targetDiv, contentAddr;
			
			contentAddr = "/help-content/" + chapterId + ".html";			
			targetDiv = $("helpContent");
	        var callback = {
	                success: function(o) {
						targetDiv.innerHTML = o.responseText;
	                },
	        	    failure: function(o) { alert('Error getting ' + chapterId +  ': ' + o.status + ' -- ' + o.statusText); return null}
	        	};

	        var transaction = YAHOO.util.Connect.asyncRequest('GET', contentAddr, callback, null);			
		}
		
		return {
			treeInit: function() {
				currentChapter = null;				
				treeContainer = $("helpMenu");
				
				buildTree();				
			}
		}
})();

YAHOO.util.Event.onDOMReady(Tawala.Page.Help.manualTOC.treeInit);
