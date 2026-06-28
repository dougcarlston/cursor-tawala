/*
 * Example Projects page scripts
 */
Tawala.ExampleProjects = {};

Tawala.ExampleProjects.category = function(id) {
	if($(id)){
		this.init(id);
	}
}

Tawala.ExampleProjects.category.prototype = {
	name: "",
	categoryId: {},
	link: {},
	section: {},
	selected: false,
	type: "category",
	
	init: function(id) {
		this.name = $(id).name;
		this.link = $(id);
		this.section = $(this.link.name);
		if($D.hasClass(this.link,"selected")){
			this.selected = true;							
		}else{
			this.selected = false;
		}
	},

	hide: function(){
		if(this.name != "all"){			
			$(this.section).style.display = "none";
		}
	},

	show: function(){
		if(this.name != "all"){
			$(this.section).style.display = "block";
		}
	}
}

Tawala.ExampleProjects.categoryMenu = function(id){
	this.clickedOn = null;
	this.categoryMenuId = id;
	this.init();
}

Tawala.ExampleProjects.categoryMenu.prototype = {
	categoryMenuId: "",
	categoryList: {},
	currentCategoryId: null,
	nextCategoryId: null,
	
	init: function() {
		// Get ids of all <a> elements in menu and populate categoryList
		var ul = $(this.categoryMenuId);
		var findLiMethod = function(e){ if(e.tagName.toLowerCase() == "li"){return true;}else{return false;}}
		var cl = [];
		
		var catLi = $D.getChildrenBy(ul, findLiMethod);
		var findLinksMethod = function(e){ if(e.tagName.toLowerCase() == "a"){return true;}else{return false;}}
		var catA;
		for(var i =0; i < catLi.length; i++){
			catA = $D.getChildrenBy(catLi[i], findLinksMethod);
			if(catA.length > 0){			
				this.categoryList[catA[0].name] = new Tawala.ExampleProjects.category(catA[0]);
				if(this.categoryList[catA[0].name].selected == true) {
					this.currentCategoryId = this.categoryList[catA[0].name].name;
				}
				$E.addListener(catA[0], "click", this.click, this, true);
			}
		}
	},
	
	click: function(e){
		$E.stopEvent(e);
		
		this.clickedOn = $E.getTarget(e, false).name;
		if(this.clickedOn == "all"){
			this.showAll();
		}else{
			this.showSelected();
		}
		this.updateMenu();
	},
	
	setId: function(id) {
		this.categoryMenuId = id;
	},
	
	updateMenu: function() {
		for(var i in this.categoryList) {
			if(this.categoryList[i].selected == true){
				this.menuSelect(this.categoryList[i]);
			}else{
				this.menuDeselect(this.categoryList[i]);
			}
		}
	},
	
	menuSelect: function(el){
		if(!$D.hasClass(el.link, "selected")) {
			$D.addClass(el.link, "selected");
		}
	},
	
	menuDeselect: function(el){
		if($D.hasClass(el.link, "selected")) {
			$D.removeClass(el.link, "selected");
		}
	},

	showSelected: function(){		
		for(var i in this.categoryList){
			if(this.categoryList[i].type == "category"){
				if(this.categoryList[i].name == this.clickedOn){
					next = this.categoryList[i];
					this.categoryList[i].show();
					this.categoryList[i].selected = true;
					this.currentCategoryId = this.categoryList[i].name;
				}else{
					current = this.categoryList[i];
					this.categoryList[i].hide();
					this.categoryList[i].selected = false;
				}
			}
		}		
		this.animateOpen();
	},
	
	showAll: function(){
		var current = "";
		var next = "";
		
		for(var i in this.categoryList){
			if(this.categoryList[i].type == "category"){
				this.categoryList[i].show();						
				this.categoryList[i].selected = false;
			}
		}
		this.categoryList["all"].selected = true;
		this.animateOpen();
	},			

	animateClosed: function(){
		$("categoryContainer").style.overflow = 'hidden';
		var attributes = {
			opacity: { from: 1, to: 0 }
			};
	
		var myAnim = new YAHOO.util.Anim("categoryContainer", attributes);
		myAnim.animate();

	},

	animateOpen: function(){
		/*
		var ew = $D.getStyle($("categoryContainer"), "width");
		var attributes2 = {
				width: { from: 0, to: 100, unit: "%" }
			};
		var myAnim2 = new YAHOO.util.Anim("categoryContainer", attributes2);
		myAnim2.animate();
		*/
		
		myAnim = new YAHOO.util.ColorAnim("categoryContainer", { backgroundColor: { from: '#FFFF88', to: '#FFFFFF' } }, 1);
		myAnim.animate();		
	}

}

function setupMenus() {
	var categoryMenus = [];

	var lists = $D.getElementsByClassName("menuList");

	for(var i =0; i < lists.length; i++){
		var cat = new Tawala.ExampleProjects.categoryMenu(lists[i].id);
		categoryMenus.push(cat);
	}
}

$E.addListener(window, "load", setupMenus);
